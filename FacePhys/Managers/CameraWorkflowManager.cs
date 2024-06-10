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
namespace FacePhys.Managers;

public enum CameraState
{
    Off,
    Idle,
    NextToDetect,
    DetectingFace,
    Recording,
    Uploading,
    Error
}

public class CameraWorkflowManager
{
    // 摄像头状态
    public CameraState _cameraState = CameraState.Off;
    private Stopwatch stopwatch;
    private SystemTimer _recordingTimer;
    // 记录录制时间
    private int _elapsedSeconds=-1;
    // 定义 OnImageUpdate 事件,事件处理程序需要接受一个 int 参数
    public event Action<int> OnImageUpdate;
    private CameraService _cameraService;
    private DetectService _detectService;

    // private bool cameraReady = false;

    private SKBitmap? skBitmap;

    //public bool DetectAtNextFrame { get; set; } = false;

    public event Action<SKBitmap?>? BitmapUpdated;
    public event Action<string>? LogUpdated;

    private FaceInfo faceInfo;

    // 上传图片计数
    private int uploadCount = 0;
    private int uploadMaxCount = 300;
    // 上传图片的地址
    private string uploadUrl = "http://183.173.184.25:8000/uploader/";
    private HttpClient client = new HttpClient();
    public CameraWorkflowManager(CameraService cameraService, DetectService detectService)
    {
        _recordingTimer = new SystemTimer(); 
        _recordingTimer.Elapsed += OnRecordingTimerElapsed;
        _recordingTimer.Interval = 1000;// 每秒触发一次
        _recordingTimer.AutoReset = true;

        _cameraService = cameraService;
        _detectService = detectService;
        _cameraService.FrameCaptured += OnFrameCaptured;
    }

    /// <summary>
    /// 打开摄像头，实时显示摄像头画面
    /// </summary>
    public async Task OpenCamera()
    {
        var result = await _cameraService.CheckCameraPermissionAsync();
        if (result)
        {
            //cameraReady = true;

            _cameraState = CameraState.Idle;
            _cameraService.StartCamera();
            LogUpdated?.Invoke("Camera started.");
        }
        else
        {
            LogUpdated?.Invoke("Camera permission is required to take a photo.");
        }
    }

    private async void OnFrameCaptured(object? sender, byte[] imageData)
    {
        // if (!cameraReady)
        // {
        //     return;
        // }
        skBitmap = PrepareBitmap(imageData, -90);
        BitmapUpdated?.Invoke(skBitmap);

        switch(_cameraState)
        {
            case CameraState.Off:
                break;
            case CameraState.Idle:
                break;
            case CameraState.NextToDetect:
                _cameraState = CameraState.Idle;
                var result = await Task.Run(() => DetectFace(skBitmap));
                if (result != null)
                {
                    faceInfo = result;
                    _cameraState = CameraState.Recording;
                }
                else
                {
                    _cameraState = CameraState.Error;
                }
                break;
            case CameraState.DetectingFace:
                break;
            case CameraState.Recording:
                _cameraState = CameraState.Uploading;
                // 录制视频
                _elapsedSeconds = 0;
                _recordingTimer.Start();
                await Task.Delay(3000);
                _recordingTimer.Stop();
                break;
            case CameraState.Uploading:
                if(uploadCount == 0)
                {
                    
                    // 打印进入
                    // Application.Current.MainPage.DisplayAlert("Success", "即将开始传输", "OK");
                    // 开始上传
                    StartImageTransfer(this, EventArgs.Empty);
                }
                if(uploadCount < uploadMaxCount)
                {
                    var croppedBitmap = skBitmap.CropBitmap(faceInfo);
                    var resizedBitmap = croppedBitmap.ResizeToSize(8);

                    // 逐帧上传
                    uploadCount++;
                    //StartImageTransfer(this, EventArgs.Empty);

                    UploadImage(this, EventArgs.Empty,resizedBitmap);
                }
                else
                {
                    // 结束上传
                    EndImageTransfer(this, EventArgs.Empty);
                    _cameraState = CameraState.Idle;
                    uploadCount = 0;
                }
                break;
            case CameraState.Error:
                _cameraState = CameraState.Idle;
                LogUpdated?.Invoke("未检测到人脸，请正面朝向手机屏幕、检查光照等环境条件");
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
            LogUpdated?.Invoke($"已成功捕捉到检测目标，倒计时结束后即将开始测量，请保证目标稳定直至检测结束");
            return detectResult.Boxes.First();
        }
        else
        {
            LogUpdated?.Invoke($"No face detected.");
            return null;
        }
    }

    private async void StartImageTransfer(object sender, EventArgs e)
    {
        try
        {
            var response = await client.GetAsync(uploadUrl+"start_image_transfer");
            response.EnsureSuccessStatusCode();
            stopwatch = Stopwatch.StartNew();
        }
        catch (Exception ex)
        {
            LogUpdated?.Invoke("传输开始失败！" + ex.Message);
        }
    }

    private async void UploadImage(object sender, EventArgs e, SKBitmap result)
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent(uploadCount.ToString()), "index" }
        };

        using (var data = result.Encode(SKEncodedImageFormat.Png, 100))
        {
            byte[] imageBytes = data.ToArray();  // 将 SKData 转换为字节数组
            // 添加 PNG 图片数据到表单数据中
            var imageContent = new ByteArrayContent(imageBytes);
			imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
			content.Add(imageContent, "image", "image.png");
        }
        try
        {
            var response = await client.PostAsync( uploadUrl+"upload_image", content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            LogUpdated?.Invoke("图片上传失败！" + ex.Message);
        }
    }


	private async void EndImageTransfer(object sender, EventArgs e)
	{
		try
		{
            var content = new MultipartFormDataContent();
            stopwatch.Stop();
            var fps = uploadMaxCount/(stopwatch.ElapsedMilliseconds/1000);
            content.Add(new StringContent(fps.ToString()), "fps");
			var response = await client.PostAsync(uploadUrl+"end_image_transfer",content);
			response.EnsureSuccessStatusCode();
            LogUpdated?.Invoke("传输结束成功！");
        }
        catch (Exception ex)
        {
            LogUpdated?.Invoke("传输结束失败：" + ex.Message);
        }
    }

    private void OnRecordingTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _elapsedSeconds++; // 增加录制时间
        if(_elapsedSeconds>1){

        }
        OnImageUpdate?.Invoke(_elapsedSeconds); // 触发图片更新事件，并传递当前秒数
    }

}
