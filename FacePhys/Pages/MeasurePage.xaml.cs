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
	private TimersTimer? _carouselTimer; // 直接使用 Timer

	private CameraWorkflowManager _workflowManager;
	private SKBitmap? _skBitmap;

	public ObservableCollection<string> _carouselViewImagePaths =
	[
		"Resources/Images/waveform.png",
		"Resources/Images/waveform.png",
		"Resources/Images/waveform.png",
	];

	private readonly string[] _dynamicImagePaths =
	[
		"Resources/Images/transparent.png",
		"Resources/Images/dynamic1.png",
		"Resources/Images/dynamic2.png",
		"Resources/Images/dynamic3.png",
	];

	public MeasurePage()
	{
		BindingContext = this;
		InitializeComponent();

		SetupCarousel();
		SetupTimer();

		_workflowManager = new(new CameraService(), new DetectService(), new NetworkService());
		_workflowManager.BitmapUpdated += UpdateCanvas;
		_workflowManager.LogUpdated += UpdateLog;

		_workflowManager.StartCountDown += OnStartingCountDown;
	}

	private async void OnStartingCountDown()
	{
		dynamicImage.IsVisible = true;
		updateImage(3);
		await Task.Delay(1000);
		updateImage(2);
		await Task.Delay(1000);
		updateImage(1);
		await Task.Delay(1000);
		dynamicImage.IsVisible = false;

		void updateImage(int index)
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				dynamicImage.Source = ImageSource.FromFile(_dynamicImagePaths[index]);
			});
		}
	}

	private void SetupCarousel()
	{
		carouselView.ItemsSource = _carouselViewImagePaths;
		carouselView.Loop = true;
	}

	private void SetupTimer()
	{
		_carouselTimer = new TimersTimer(300); // 每0.3秒触发一次
		_carouselTimer.Elapsed += OnTimedEvent;
		_carouselTimer.AutoReset = true;
		_carouselTimer.Enabled = true;
	}

	private void OnTimedEvent(object? source, ElapsedEventArgs e)
	{
		int nextPosition = (carouselView.Position + 1) % _carouselViewImagePaths.Count;
		MainThread.BeginInvokeOnMainThread(() =>
		{
			carouselView.Position = nextPosition;
		});
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_carouselTimer?.Start();
		_ = _workflowManager.OpenCamera();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_carouselTimer?.Stop();
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
			logLabel.Text += $"\n{message}";
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

	private async void OnDetectFaceClicked(object sender, EventArgs e)
	{
		if (_workflowManager.CameraState == WorkflowStateEnum.Off)
		{
			await _workflowManager.OpenCamera();
			_workflowManager.StartDetectingWorkflow();
		}
		else if (_workflowManager.CameraState == WorkflowStateEnum.Idle)
		{
			_workflowManager.StartDetectingWorkflow();
		}
	}

	private void OnDetectFaceCanceled(object sender, EventArgs e)
	{
		// TODO: 添加取消功能
	}
}
