using FacePhys.Services;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Drawing;

namespace FacePhys;

public partial class MainPage : ContentPage
{
    CameraService _cameraService = new();
    DetectService _detectService = new();

    bool _taking = false;

    DateTime _startTime;
    int _frameCount = -1;

    byte[] _imageData = [];
    SKBitmap? _skBitmap;

    Rectangle? _faceRect;

    public MainPage()
    {
        InitializeComponent();
        _cameraService.FrameCaptured += OnFrameCaptured;
    }

    /// <summary>
    /// Called when a frame is captured by the camera service
    /// </summary>
    /// <param name="sender">The camera service</param>
    /// <param name="e">The image data</param>
    private void OnFrameCaptured(object sender, byte[] e)
    {
        if (!_taking)
        {
            return;
        }

        if (_frameCount == -1)
        {
            _startTime = DateTime.Now;
            _frameCount = 0;
        }
        else
        {
            _frameCount++;
        }

        _imageData = e;

        // * 经过一系列处理，获得能用于显示的位图
        _skBitmap = DecodeImage(_imageData);
        // _skBitmap = CropBitmapToSquare(_skBitmap);
        // _skBitmap = RotateBitmap(_skBitmap, -90);

        // * Detect face
        var detectResult = _detectService.Detect(_imageData);

        if (detectResult == null)
        {
        }
        else if (detectResult.Boxes.Count == 0)
        {
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                logLabel.Text += $"\n{detectResult.Boxes.Count} faces detected.";
            });
        }

        // * 通知视图刷新
        canvasView.InvalidateSurface();

        // MainThread.BeginInvokeOnMainThread(() =>
        // {
        //     logLabel.Text += $"\nBytes received at {DateTime.Now.Second}";
        // });
    }

    SKBitmap DecodeImage(byte[] data)
    {
        using var stream = new SKMemoryStream(data);
        return SKBitmap.Decode(stream);
    }

    SKBitmap CropBitmapToSquare(SKBitmap origin)
    {
        int size = Math.Min(origin.Width, origin.Height);
        int left = (origin.Width - size) / 2;
        int top = (origin.Height - size) / 2;

        var rect = new SKRectI(left, top, left + size, top + size);
        var cropped = new SKBitmap(size, size);
        origin.ExtractSubset(cropped, rect);

        return cropped;
    }

    SKBitmap RotateBitmap(SKBitmap bitmap, float degrees)
    {
        var matrix = SKMatrix.CreateRotationDegrees(degrees, bitmap.Width / 2, bitmap.Height / 2);

        var rotatedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
        using (var canvas = new SKCanvas(rotatedBitmap))
        {
            canvas.Clear();
            canvas.SetMatrix(matrix);
            canvas.DrawBitmap(bitmap, 0, 0);
        }

        return rotatedBitmap;
    }

    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var surface = e.Surface;
        var canvas = surface.Canvas;

        canvas.Clear();

        if (_skBitmap != null)
        {
            canvas.DrawBitmap(_skBitmap, e.Info.Rect);
        }
        else
        {
            logLabel.Text += "\nNo bitmap to draw";
        }
    }

    private void StartCapture()
    {
        _cameraService.StartCamera();
    }

    private async void OnButtonClicked(object sender, EventArgs e)
    {
        if (_taking)
        {
            _cameraService.StopCamera();
            _taking = false;
        }
        else
        {
            var result = await _cameraService.CheckCameraPermissionAsync();
            if (!result)
            {
                await DisplayAlert("Permission Required", "Camera permission is required to take a photo", "OK");
            }
            _cameraService.StartCamera();
            _taking = true;
        }
    }
}
