using Android.Graphics;
using FacePhys.Models;
using FacePhys.Services;
using FacePhys.ViewModels;
namespace FacePhys.Pages;

public partial class HomePage : ContentPage
{
	private UserViewModel _userViewModel;
	
	public HomePage()
	{
		InitializeComponent();
		_userViewModel = App.UserViewModel;
		LoadLatestHealthMetricValues();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
	}

	private async void LoadLatestHealthMetricValues()
	{
		HeartRate heartRate = await _userViewModel.GetLatestHealthMetricAsync<HeartRate>();
		BloodOxygen bloodOxygen = await _userViewModel.GetLatestHealthMetricAsync<BloodOxygen>();
		BloodPressure bloodPressure = await _userViewModel.GetLatestHealthMetricAsync<BloodPressure>();
		RespiratoryRate respiratoryRate = await _userViewModel.GetLatestHealthMetricAsync<RespiratoryRate>();

		HeartRateValue.Text = heartRate.BeatsPerMinute.ToString();
		BloodPressureDiastolicValue.Text = bloodPressure.Diastolic.ToString();
		BloodPressureSystolicValue.Text = bloodPressure.Systolic.ToString();
		BloodOxygenValue.Text = bloodOxygen.OxygenLevel.ToString();
		RespiratoryRateValue.Text = respiratoryRate.BreathsPerMinute.ToString();
	}
}