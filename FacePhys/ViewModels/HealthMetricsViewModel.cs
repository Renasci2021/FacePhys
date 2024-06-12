using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Android.AccessibilityServices;
using FacePhys.Models;
using FacePhys.Services;

namespace FacePhys.ViewModels;

public class HealthMetricsViewModel : BaseViewModel
{
    public List<HeartRate> HeartRateMetrics { get; set; } = [];

    public List<BloodOxygen> BloodOxygenMetrics{ get; set;} = [];

    public List<BloodPressure> BloodPressureMetrics{ get; set;} = [];

    public List<RespiratoryRate> RespiratoryRateMetrics{ get; set;} = [];

    private readonly DatabaseService _databaseService;

    // public event PropertyChangedEventHandler PropertyChanged;
    // protected virtual void OnPropertyChanged(string propertyName)
    // {
    //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    // }

    public HealthMetricsViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await LoadHealthMetricsAsync();
    }

    public async Task LoadHealthMetricsAsync()
    {
        HeartRateMetrics = await GetHealthMetricsAsync<HeartRate>();
        BloodOxygenMetrics = await GetHealthMetricsAsync<BloodOxygen>();
        BloodPressureMetrics = await GetHealthMetricsAsync<BloodPressure>();
        RespiratoryRateMetrics = await GetHealthMetricsAsync<RespiratoryRate>();

        OnPropertyChanged(nameof(HeartRateMetrics));
        OnPropertyChanged(nameof(BloodOxygenMetrics));
        OnPropertyChanged(nameof(BloodPressureMetrics));
        OnPropertyChanged(nameof(RespiratoryRateMetrics));

        if(HeartRateMetrics == null)
        {
            App.Current.MainPage.DisplayAlert("Error", "HeartRateMetrics is null", "OK");
        }
    }

    public async Task<List<T>> GetHealthMetricsAsync<T>() where T : HealthMetric, new()
    {
        var user = App.UserViewModel.User;
        return await _databaseService.GetHealthMetricsAsync<T>(user!.Id);
    }

    public async Task AddHealthMetric(HealthMetric metric)
    {
        await _databaseService.SaveHealthMetricAsync(metric);

        if(metric is HeartRate heartRate)
        {
            // int heartRateCount1 = HeartRateMetrics.Count;
            // await App.Current.MainPage.DisplayAlert("Success", $"Health Metric Added. Total Heart Rate Metrics: {heartRateCount1}", "OK");
            HeartRateMetrics.Add(heartRate);
            OnPropertyChanged(nameof(HeartRateMetrics));
            // int heartRateCount = HeartRateMetrics.Count;
            // await App.Current.MainPage.DisplayAlert("Success", $"Health Metric Added. Total Heart Rate Metrics: {heartRateCount}", "OK");
        }
        
        else if(metric is BloodOxygen bloodOxygen)
        {
            BloodOxygenMetrics.Add(bloodOxygen);
            OnPropertyChanged(nameof(BloodOxygenMetrics));
        }
        else if(metric is BloodPressure bloodPressure)
        {
            BloodPressureMetrics.Add(bloodPressure);
            OnPropertyChanged(nameof(BloodPressureMetrics));
        }
        else if(metric is RespiratoryRate respiratoryRate)
        {
            RespiratoryRateMetrics.Add(respiratoryRate);
            OnPropertyChanged(nameof(RespiratoryRateMetrics));
        }

    }
}