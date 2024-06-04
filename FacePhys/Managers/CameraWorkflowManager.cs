using System.Drawing;
using Android.Graphics;
using FacePhys.Services;
using FacePhys.Utils;
using SkiaSharp;

namespace FacePhys.Managers;

public class CameraWorkflowManager
{
    private CameraService _cameraService;
    private DetectService _detectService;

    private bool cameraReady = false;

    private SKBitmap? skBitmap;

    public bool DetectAtNextFrame { get; set; } = false;

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
        if (!cameraReady)
        {
            return;
        }

        skBitmap = imageData.DecodeImage().CropBitmapToSquare().RotateBitmap(-90);
        BitmapUpdated?.Invoke(skBitmap);

        if (DetectAtNextFrame)
        {
            DetectAtNextFrame = false;
            using var skData = skBitmap.Encode(SKEncodedImageFormat.Png, 100);
            var detectResult = _detectService.Detect(skData.ToArray());
            if (detectResult != null && detectResult.Boxes.Count > 0)
            {
                LogUpdated?.Invoke($"{detectResult.Boxes.Count} faces detected.");
            }
            else
            {
                LogUpdated?.Invoke($"No face detected.");
            }
        }
    }
}
