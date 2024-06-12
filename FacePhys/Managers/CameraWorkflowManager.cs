using FacePhys.Services;
using FacePhys.Utils;
using SkiaSharp;
using UltraFaceDotNet;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Maui.Controls;
using Android.Hardware.Camera2;
using Android.Util;
using System.Timers;
using SystemTimer = System.Timers.Timer;
using System.Diagnostics;
using Android.Hardware.Camera2.Params;
using FacePhys.Models;
using FacePhys.ViewModels;
namespace FacePhys.Managers;

public enum WorkflowStateEnum
{
    Off,
    Idle,
    NextToDetect,
    Detecting,
    NextToRecord,
    Recording,
}

public class CameraWorkflowManager
{
    private readonly HealthMetricsViewModel _healthMetricsViewModel;
    private CameraService _cameraService;
    private DetectService _detectService;
    private NetworkService _networkService;

    private WorkflowStateEnum _workflowState = WorkflowStateEnum.Off;

    private SKBitmap? _skBitmap;
    private FaceInfo _faceInfo = new();
    private Stopwatch _stopwatch = new();

    public float? heartRate { get; set; }

    private int _detectTryCount = 0;

    private int _uploadCount = 0;
    private int _uploadMaxCount = 300;

    public WorkflowStateEnum CameraState => _workflowState;

    public event Action<SKBitmap?>? BitmapUpdated;
    public event Action<string>? LogUpdated;
    public event Action? StartCountDown;

    public CameraWorkflowManager(CameraService cameraService, DetectService detectService, NetworkService networkService)
    {
        _healthMetricsViewModel = App.HealthMetricsViewModel;
        _cameraService = cameraService;
        _detectService = detectService;
        _networkService = networkService;
        _cameraService.FrameCaptured += OnFrameCaptured;
    }

    public async Task OpenCamera()
    {
        var result = await _cameraService.CheckCameraPermissionAsync();
        if (result)
        {
            _cameraService.StartCamera();
            _workflowState = WorkflowStateEnum.Idle;
            LogUpdated?.Invoke("Camera started.");
        }
        else
        {
            LogUpdated?.Invoke("Camera permission is required to take a photo.");
        }
    }

    public void StartDetectingWorkflow()
    {
        _workflowState = WorkflowStateEnum.NextToDetect;
        _uploadCount = 0;
        _detectTryCount = 0;
    }

    public void StopDetectingWorkflow()
    {
        _workflowState = WorkflowStateEnum.Off;
        _cameraService.StopCamera();
    }

    private async void OnFrameCaptured(object? sender, byte[] imageData)
    {
        _skBitmap = PrepareBitmap(imageData, -90);
        BitmapUpdated?.Invoke(_skBitmap);

        switch (_workflowState)
        {
            case WorkflowStateEnum.NextToDetect:
                _workflowState = WorkflowStateEnum.Detecting;
                LogUpdated?.Invoke($"检测人脸，第 {_detectTryCount + 1} 次");
                var result = await Task.Run(() => DetectFace(_skBitmap));
                if (result == null)
                {
                    _detectTryCount++;
                    if (_detectTryCount > 10)   // * 每间隔 100 毫秒检测一次，连续 10 次未检测到人脸则提示
                    {
                        LogUpdated?.Invoke("未检测到人脸，请保证镜头中有人脸");
                        _detectTryCount = 0;
                        _workflowState = WorkflowStateEnum.Idle;
                    }
                    else
                    {
                        await Task.Delay(100);
                        _workflowState = WorkflowStateEnum.NextToDetect;
                    }
                }
                else
                {
                    _detectTryCount = 0;
                    _workflowState = WorkflowStateEnum.Idle;
                    StartCountDown?.Invoke();
                    LogUpdated?.Invoke("人脸检测成功，即将开始测量");
                    await Task.Delay(3000);
                    _workflowState = WorkflowStateEnum.NextToRecord;
                    _faceInfo = result;
                }
                break;
            case WorkflowStateEnum.NextToRecord:
                LogUpdated?.Invoke("开始测量");
                _networkService.StartImageUpload();
                _workflowState = WorkflowStateEnum.Recording;
                _stopwatch = Stopwatch.StartNew();
                break;
            case WorkflowStateEnum.Recording:
                if (_uploadCount % 50 == 0)
                {
                    LogUpdated?.Invoke($"正在上传第 {_uploadCount} 张图片");
                }

                if (_uploadCount >= _uploadMaxCount)
                {
                    _workflowState = WorkflowStateEnum.Idle;
                    _stopwatch.Stop();
                    float fps = _uploadCount / (float)_stopwatch.ElapsedMilliseconds * 1000;
                    heartRate = await _networkService.EndImageUpload(fps);
                    // 弹窗显示心率
                    if (heartRate != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("检测成功！", $"您的心率为 {heartRate} 次/分钟", "确定");
                        HeartRate heartRateMetric = new()
                        {
                            UserId = App.UserViewModel.User!.Id,
                            //BeatsPerMinute = beatsPerMinute1,
                            BeatsPerMinute = heartRate,
                        };
                        _healthMetricsViewModel.AddHealthMetric(heartRateMetric);

                        //await App.Current.MainPage.DisplayAlert("Success", $"{heartRate}", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("检测失败！", "请检测您的网络连接", "确定");
                    }

                    _uploadCount = 0;
                    return;
                }

                _uploadCount++;
                await Task.Run(async () =>
                {
                    var croppedBitmap = _skBitmap.CropBitmap(_faceInfo);
                    var resizedBitmap = croppedBitmap.ResizeToSize(8);
                    var bytes = resizedBitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray();
                    _networkService.UploadImage(bytes,_uploadCount);
                });
                break;
            default:
                break;
        }
    }

    private SKBitmap PrepareBitmap(byte[] imageData, int rotation = 0)
    {
        return imageData.DecodeImage().CropBitmapToSquare().RotateBitmap(rotation);
    }

    private FaceInfo? DetectFace(SKBitmap skBitmap)
    {
        using var skData = skBitmap.Encode(SKEncodedImageFormat.Png, 100);
        var detectResult = _detectService.Detect(skData.ToArray());
        if (detectResult != null && detectResult.Boxes.Count > 0)
        {
            if(detectResult.Boxes.Count > 1)
            {
                LogUpdated?.Invoke($"出现多个检测目标，请保证镜头中只有一人");
                return null;
            }
            return detectResult.Boxes.First();
        }
        else
        {
            LogUpdated?.Invoke($"未检测到人脸，请保证镜头中有人脸");
            return null;
        }
    }
}
