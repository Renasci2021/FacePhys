using System.Drawing;
using FacePhys.Services;
using FacePhys.Utils;
using SkiaSharp;

namespace FacePhys.Managers;

public class CameraWorkflowManager
{
    private CameraService _cameraService;
    private DetectService _detectService;
    private int _frameCount = -1;
    private SKBitmap? _skBitmap;
    private int _detectStatus = -1;

    private bool cameraReady = false;

    public event Action<SKBitmap?>? BitmapUpdated;
    public event Action<string>? LogUpdated;

    public CameraWorkflowManager(CameraService cameraService, DetectService detectService)
    {
        _cameraService = cameraService;
        _detectService = detectService;
        _cameraService.FrameCaptured += OnFrameCaptured;
    }

    /// <summary>
    /// 打开摄像头，实时显示摄像头画面
    /// </summary>
    public async void OpenCamera()
    {
        var result = await _cameraService.CheckCameraPermissionAsync();
        if (result)
        {
            cameraReady = true;
            _cameraService.StartCamera();
            LogUpdated?.Invoke("Camera started.");
        }
        else
        {
            LogUpdated?.Invoke("Camera permission is required to take a photo.");
        }
    }

    private void OnFrameCaptured(object? sender, byte[] imageData)
    {
        // 一开始的时候不能调用，需要记录状态
        if (!cameraReady)
        {
            return;
        }




        ProcessImage(imageData);
    }

    private void ProcessImage(byte[] imageData)
    {
        _skBitmap = imageData.DecodeImage().CropBitmapToSquare().RotateBitmap(-90);
        BitmapUpdated?.Invoke(_skBitmap);

        if (_detectStatus == -1)
        {
            _detectStatus = 0;
        }
        else if (_detectStatus == 0)
        {
            var detectResult = _detectService.Detect(imageData);
            if (detectResult != null && detectResult.Boxes.Count > 0)
            {
                LogUpdated?.Invoke($"\n{detectResult.Boxes.Count} faces detected.");
            }
            _detectStatus = 1;
        }
    }

}
