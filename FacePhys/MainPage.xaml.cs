using SkiaSharp;
using SkiaSharp.Views.Maui;
using FacePhys.Services;
using FacePhys.Managers;

namespace FacePhys;

public partial class MainPage : ContentPage
{
    private CameraWorkflowManager _workflowManager;
    private SKBitmap? _skBitmap;

    public MainPage()
    {
        InitializeComponent();
        _workflowManager = new CameraWorkflowManager(new CameraService(), new DetectService());
        _workflowManager.BitmapUpdated += UpdateCanvas;
        _workflowManager.LogUpdated += UpdateLog;
        // _workflowManager.OpenCamera();
    }

    private void UpdateCanvas(SKBitmap? skBitmap)
    {
        canvasView.InvalidateSurface();
        _skBitmap = skBitmap;
    }

    private void UpdateLog(string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            logLabel.Text += message;
        });
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
    }

    private async void OnButtonClicked(object sender, EventArgs e)
    {
        _workflowManager.OpenCamera();
    }
}
