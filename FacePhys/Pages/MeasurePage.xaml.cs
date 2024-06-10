using SkiaSharp;
using SkiaSharp.Views.Maui;
using FacePhys.Services;
using FacePhys.Managers;
using System.Collections.ObjectModel;
using System.Timers;
using TimersTimer = System.Timers.Timer;

namespace FacePhys.Pages;

public partial class MeasurePage : ContentPage
{
	private TimersTimer? carouselTimer; // 直接使用 Timer

	public ObservableCollection<string> carouselViewImagePaths =
	[
		"Resources/Images/waveform.png",
		"Resources/Images/waveform.png",
		"Resources/Images/waveform.png",
	];

	private string[] dynamicImagePaths =
	[
		"Resources/Images/dynamic1.png",
		"Resources/Images/dynamic2.png",
		"Resources/Images/dynamic3.png",
		"Resources/Images/transparent.png",
	];

	private CameraWorkflowManager workflowManager;
	private SKBitmap? skBitmap;

	public MeasurePage()
	{
		InitializeComponent();
		SetupCarousel();
		SetupTimer();

		BindingContext = this;

		workflowManager = new CameraWorkflowManager(new CameraService(), new DetectService());
		workflowManager.BitmapUpdated += UpdateCanvas;
		workflowManager.LogUpdated += UpdateLog;
		workflowManager.OnImageUpdate += UpdateImage;

		//dynamicImage11.Source = ImageSource.FromFile(dynamicImagePaths[2]);
	}

	private void UpdateImage(int elapsedSeconds)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			Console.WriteLine($"Updating image for second: {elapsedSeconds}");
			dynamicImage.Source = elapsedSeconds switch
			{
				1 => ImageSource.FromFile(dynamicImagePaths[2]),
				2 => ImageSource.FromFile(dynamicImagePaths[1]),
				3 => ImageSource.FromFile(dynamicImagePaths[0]),
				_ => ImageSource.FromFile(dynamicImagePaths[3]),
			};
		});
	}

	private void SetupCarousel()
	{
		carouselView.ItemsSource = carouselViewImagePaths;
		carouselView.Loop = true;
	}

	private void SetupTimer()
	{
		carouselTimer = new TimersTimer(300); // 每0.3秒触发一次
		carouselTimer.Elapsed += OnTimedEvent;
		carouselTimer.AutoReset = true;
		carouselTimer.Enabled = true;
	}

	private void OnTimedEvent(object? source, ElapsedEventArgs e)
	{
		int nextPosition = (carouselView.Position + 1) % carouselViewImagePaths.Count;
		MainThread.BeginInvokeOnMainThread(() =>
		{
			carouselView.Position = nextPosition;
		});
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		carouselTimer?.Start();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		carouselTimer?.Stop();
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

	// private void OnButtonClicked(object sender, EventArgs e)
	// {
	//     workflowManager.OpenCamera();
	// }

	private async void OnDetectFaceClicked(object sender, EventArgs e)
	{
		if (workflowManager._cameraState == CameraState.Off)
		{
			await workflowManager.OpenCamera();
		}
		workflowManager._cameraState = CameraState.NextToDetect;
	}

	private void OnDetectFaceCanceled(object sender, EventArgs e)
	{
		workflowManager._cameraState = CameraState.Idle;

		// TODO: 添加取消功能
	}

}
