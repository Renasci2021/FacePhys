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
		
		LoadHealthMetricValues();
	}

	private async void LoadHealthMetricValues()
	{
		// heartRateMetric, bloodPressureMetric, bloodOxygenMetric, respiratoryRateMetric
		List<HealthMetric?> healthMetrics = await _userViewModel.LoadHealthMetrics();

		var heartRateMetric = healthMetrics?[0];
		if (heartRateMetric != null)
		{
			HeartRateValue.Text = heartRateMetric.HeartRate.ToString();
		}
		else
		{
			HeartRateValue.Text = "N/A"; // 如果心率数据不可用，显示 "N/A" 或其他适当的占位符
		}

		var bloodPressureMetric = healthMetrics?[1];
		if (bloodPressureMetric != null)
		{
			BloodPressureDiastolicValue.Text = bloodPressureMetric.BloodPressure.Item1.ToString();
			BloodPressureSystolicValue.Text = bloodPressureMetric.BloodPressure.Item2.ToString();
		}
		else
		{
			BloodPressureDiastolicValue.Text = "N/A";
			BloodPressureSystolicValue.Text = "N/A";
		}

		var bloodOxygenMetric = healthMetrics?[2];
		if (bloodOxygenMetric != null)
		{
			BloodOxygenValue.Text = bloodOxygenMetric.BloodOxygen.ToString();
		}
		else
		{
			BloodOxygenValue.Text = "N/A";
		}

		var respiratoryRateMetric = healthMetrics?[3];
		if (respiratoryRateMetric != null)
		{
			RespiratoryRateValue.Text = respiratoryRateMetric.RespiratoryRate.ToString();
		}
		else
		{
			RespiratoryRateValue.Text = "N/A";
		}

	}

}