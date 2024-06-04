using FacePhys.Services;
using FacePhys.Utils;
using SkiaSharp;
using UltraFaceDotNet;

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

    private async void OnFrameCaptured(object? sender, byte[] imageData)
    {
        if (!cameraReady)
        {
            return;
        }

        // ! 旋转角度根据实际情况调整
        skBitmap = PrepareBitmap(imageData, -90);
        BitmapUpdated?.Invoke(skBitmap);

        if (DetectAtNextFrame)
        {
            DetectAtNextFrame = false;
            var faceInfo = await Task.Run(() => DetectFace(skBitmap));
            if (faceInfo != null)
            {
                var croppedBitmap = skBitmap.CropBitmap(faceInfo);
                var resizedBitmap = croppedBitmap.ResizeToSize(8);

                // TODO: 传递给后端
            }
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
            LogUpdated?.Invoke($"{detectResult.Boxes.Count} faces detected.");
            return detectResult.Boxes.First();
        }
        else
        {
            LogUpdated?.Invoke($"No face detected.");
            return null;
        }
    }
}
