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

	public ObservableCollection<HeartRate> HeartRateMetrics;
	public ObservableCollection<BloodOxygen> BloodOxygenMetrics;
	public ObservableCollection<BloodPressure> BloodPressureMetrics;
	public ObservableCollection<RespiratoryRate> RespiratoryRateMetrics;

	public ReportPage()
	{
		InitializeComponent();
		_userViewModel = App.UserViewModel;
		LoadHealthMetricValues();
		BindingContext = this;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		
	}

	private async void LoadHealthMetricValues()
	{
		List<HeartRate> heartRates = await _userViewModel.GetHealthMetricsAsync<HeartRate>();
		List<BloodOxygen> bloodOxygens = await _userViewModel.GetHealthMetricsAsync<BloodOxygen>();
		List<BloodPressure> bloodPressures = await _userViewModel.GetHealthMetricsAsync<BloodPressure>();
		List<RespiratoryRate> respiratoryRates = await _userViewModel.GetHealthMetricsAsync<RespiratoryRate>();

		HeartRateMetrics = new ObservableCollection<HeartRate>(heartRates);
		BloodOxygenMetrics = new ObservableCollection<BloodOxygen>(bloodOxygens);
		BloodPressureMetrics = new ObservableCollection<BloodPressure>(bloodPressures);
		RespiratoryRateMetrics = new ObservableCollection<RespiratoryRate>(respiratoryRates);
        
		BloodPressureMetricsCollectionView.ItemsSource = BloodPressureMetrics;
		HeartRateMetricsCollectionView.ItemsSource = HeartRateMetrics;
		BloodOxygenMetricsCollectionView.ItemsSource = BloodOxygenMetrics;
		RespiratoryRateMetricsCollectionView.ItemsSource = RespiratoryRateMetrics;
	}
}