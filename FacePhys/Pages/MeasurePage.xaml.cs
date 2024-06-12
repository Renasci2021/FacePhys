using SkiaSharp;
using SkiaSharp.Views.Maui;
using FacePhys.Services;
using FacePhys.Managers;
using System.Collections.ObjectModel;
using System.Timers;
using TimersTimer = System.Timers.Timer;
using FacePhys.Models;

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
		_workflowManager.StopDetectingWorkflow();
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

		// if (_skBitmap != null)
		// {
		// 	canvas.DrawBitmap(_skBitmap, e.Info.Rect);
		// }


		// 获取画布的尺寸
		var width = e.Info.Width-40;
		var height = e.Info.Height-40;
		var radius = Math.Min(width, height) / 2 -25;
		var centerX = width / 2 + 20;
		var centerY = height / 2 + 20;

		// 清除画布
		canvas.Clear(SKColors.Transparent);

		// 绘制灰色的矩形，作为背景
		var paint = new SKPaint
		{
			Style = SKPaintStyle.Fill,
			Color = SKColor.Parse("#F5F5F5"),
			IsAntialias = true
		};
		canvas.DrawRect(0, 0, width+40, height+40, paint);

		// 绘制绿色的圆周，不填充
		paint.Style = SKPaintStyle.Stroke;
		paint.Color = SKColor.Parse("#7CC587");
		paint.StrokeWidth = 15; // 设置线条宽度
		radius = radius + 30;
		canvas.DrawCircle(centerX, centerY, radius, paint);
		radius = radius- 30;
		
		// 创建一个圆形路径
		using (var path = new SKPath())
		{
			path.AddCircle(centerX, centerY, radius);

			// 裁剪画布以保留圆形区域
			canvas.ClipPath(path);

			// 如果 _skBitmap 存在，在圆形区域内绘制 _skBitmap
			if (_skBitmap != null)
			{
				// 计算缩放因子和目标矩形
				float scale = Math.Min((float)radius * 2 / _skBitmap.Width, (float)radius * 2 / _skBitmap.Height);
				float bitmapWidth = scale * _skBitmap.Width;
				float bitmapHeight = scale * _skBitmap.Height;
				var destRect = new SKRect(
					centerX - bitmapWidth / 2,
					centerY - bitmapHeight / 2,
					centerX + bitmapWidth / 2,
					centerY + bitmapHeight / 2
				);

				// 绘制位图
				canvas.DrawBitmap(_skBitmap, destRect);
			}
		}

		canvas.Restore();
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
		_workflowManager.StopDetectingWorkflow();
	}
}
