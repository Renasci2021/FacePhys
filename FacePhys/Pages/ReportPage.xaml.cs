using FacePhys.ViewModels;
using FacePhys.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace FacePhys.Pages;

public partial class ReportPage : ContentPage
{
	private UserViewModel _userViewModel;

	public ObservableCollection<HealthMetric?> BloodPressureMetrics { get; set; } = new ObservableCollection<HealthMetric>();
	public ObservableCollection<HealthMetric?> HeartRateMetrics { get; set; } = new ObservableCollection<HealthMetric>();
	public ObservableCollection<HealthMetric?> BloodOxygenMetrics { get; set; } = new ObservableCollection<HealthMetric>();
	public ObservableCollection<HealthMetric?> RespiratoryRateMetrics { get; set; } = new ObservableCollection<HealthMetric>();
	
	public ReportPage()
	{
		InitializeComponent();
		_userViewModel = App.UserViewModel;

		LoadHealthMetricValues();
	}

	private async void LoadHealthMetricValues()
	{
		// heartRateMetric, bloodPressureMetric, bloodOxygenMetric, respiratoryRateMetric
		List<HealthMetric?> healthMetrics = await _userViewModel.LoadHealthMetrics();


		for(int i=0;i<healthMetrics.Count;i++)
		{
			var type = healthMetrics[i].Type;
			switch(type)
			{
				case MetricType.HeartRate:
					HeartRateMetrics.Add(healthMetrics[i]);
					//HeartRateValue.Text = healthMetrics[i].HeartRate.ToString();
					break;
				case MetricType.BloodPressure:
					BloodPressureMetrics.Add(healthMetrics[i]);
					// BloodPressureDiastolicValue.Text = healthMetrics[i].BloodPressure.Item1.ToString();
					// BloodPressureSystolicValue.Text = healthMetrics[i].BloodPressure.Item2.ToString();
					break;
				case MetricType.BloodOxygen:
					BloodOxygenMetrics.Add(healthMetrics[i]);
					//BloodOxygenValue.Text = healthMetrics[i].BloodOxygen.ToString();
					break;
				case MetricType.RespiratoryRate:
					RespiratoryRateMetrics.Add(healthMetrics[i]);
					//RespiratoryRateValue.Text = healthMetrics[i].RespiratoryRate.ToString();
					break;
			}
		}
	
		HeartRateMetrics.Add(
			new HealthMetric
			{
				HeartRate = 60
			}
		
		);
	}
}