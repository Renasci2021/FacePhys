using SkiaSharp;
using SkiaSharp.Views.Maui;
using FacePhys.Services;
using FacePhys.Managers;

namespace FacePhys;

public partial class MainPage : ContentPage
{
    private CameraWorkflowManager workflowManager;
    private SKBitmap? skBitmap;

    public MainPage()
    {
        InitializeComponent();
        workflowManager = new CameraWorkflowManager(new CameraService(), new DetectService());
        workflowManager.BitmapUpdated += UpdateCanvas;
        workflowManager.LogUpdated += UpdateLog;
    }

    private void UpdateCanvas(SKBitmap? skBitmap)
    {
        canvasView.InvalidateSurface();
        this.skBitmap = skBitmap;
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
        if (skBitmap != null)
        {
            canvas.DrawBitmap(skBitmap, e.Info.Rect);
        }
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        workflowManager.OpenCamera();
    }

    private void OnDetectFaceClicked(object sender, EventArgs e)
    {
        workflowManager.DetectAtNextFrame = true;
    }
}
