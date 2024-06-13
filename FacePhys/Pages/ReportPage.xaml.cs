using FacePhys.ViewModels;
using FacePhys.Models;
//using FacePhys.Converters;
using FacePhys.Selectors;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace FacePhys.Pages;

public partial class ReportPage : ContentPage
{
	private UserViewModel _userViewModel;
	private HealthMetricsViewModel _healthMetricsViewModel;

	// public ObservableCollection<HeartRate> HeartRateMetrics;
	// public ObservableCollection<BloodOxygen> BloodOxygenMetrics;
	// public ObservableCollection<BloodPressure> BloodPressureMetrics;
	// public ObservableCollection<RespiratoryRate> RespiratoryRateMetrics;

	public ReportPage()
	{
		try{
			InitializeComponent();

			// 订阅消息
			MessagingCenter.Subscribe<AddHealthMetricPage>(this, "RefreshHealthMetrics", async (sender) =>
			{
				await RefreshHealthMetrics();
			});
			_userViewModel = App.UserViewModel;
			_healthMetricsViewModel = App.HealthMetricsViewModel;
			//LoadHealthMetricValues();
			BindingContext = App.HealthMetricsViewModel;
		}
		catch (Exception ex)
		{
			DisplayAlert("Initialization Error", ex.Message, "OK");
		}
	}

	private async Task RefreshHealthMetrics()
	{
		// 刷新页面数据的逻辑
		await _healthMetricsViewModel.LoadHealthMetricsAsync();

		// 确保通知 UI 更新
    	OnPropertyChanged(nameof(_healthMetricsViewModel.HeartRateMetrics));
		OnPropertyChanged(nameof(_healthMetricsViewModel.BloodOxygenMetrics));
		OnPropertyChanged(nameof(_healthMetricsViewModel.BloodPressureMetrics));
		OnPropertyChanged(nameof(_healthMetricsViewModel.RespiratoryRateMetrics));
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		MessagingCenter.Unsubscribe<AddHealthMetricPage>(this, "RefreshHealthMetrics");
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		try
		{
			await RefreshHealthMetrics();
			
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", ex.Message, "OK");
		}
		
	}

	// private async void LoadHealthMetricValues()
	// {
	// 	BloodOxygenMetrics = new ObservableCollection<BloodOxygen>(_healthMetricsViewModel.BloodOxygenMetrics);
	// 	BloodPressureMetrics = new ObservableCollection<BloodPressure>(_healthMetricsViewModel.BloodPressureMetrics);
	// 	RespiratoryRateMetrics = new ObservableCollection<RespiratoryRate>(_healthMetricsViewModel.RespiratoryRateMetrics);
	// 	HeartRateMetrics = new ObservableCollection<HeartRate>(_healthMetricsViewModel.HeartRateMetrics);

	// 	BloodPressureMetricsCollectionView.ItemsSource = BloodPressureMetrics;
	// 	HeartRateMetricsCollectionView.ItemsSource = HeartRateMetrics;
	// 	BloodOxygenMetricsCollectionView.ItemsSource = BloodOxygenMetrics;
	// 	RespiratoryRateMetricsCollectionView.ItemsSource = RespiratoryRateMetrics;

	// }

	private async void OnAddHealthMetricButtonClicked(object sender, EventArgs e)
	{
		try
		{
			await Navigation.PushAsync(new AddHealthMetricPage(_healthMetricsViewModel));
			//Application.Current.MainPage = new AddHealthMetricPage(_healthMetricsViewModel);
		}
		catch (Exception ex)
		{
			await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
		}
	}
}