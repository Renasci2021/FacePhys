using FacePhys.Models;
using FacePhys.ViewModels;
namespace FacePhys.Pages;

public partial class AddHealthMetricPage : ContentPage
{
    public UserViewModel _userViewModel;
    public HealthMetricsViewModel _healthMetricsViewModel;
    public AddHealthMetricPage(HealthMetricsViewModel healthMetricsViewModel)
    {
        _userViewModel = App.UserViewModel;
        _healthMetricsViewModel = healthMetricsViewModel;
        
        InitializeComponent();
        metricPicker.ItemsSource = new List<string>
        {
            "血氧",
            "心率",
            "血压",
            "呼吸率"
        };
        metricPicker.SelectedIndexChanged += OnPickerSelectedIndexChanged;
        BindingContext = this;
        try
        {
            metricPicker.SelectedIndexChanged += OnPickerSelectedIndexChanged;
        }
        catch (Exception ex)
        {
            DisplayAlert("Initialization Error", ex.Message, "OK");
        }
    }

    private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (metricPicker.SelectedIndex >= 0)
            {
                var selectedIndex = metricPicker.SelectedIndex;
                dataEntry1.IsVisible = true; // Show dataEntry1 for all except Blood Pressure
                dataEntry2.IsVisible = selectedIndex == 2; // Show dataEntry2 only for Blood Pressure

                // Set Placeholder based on selected index
                switch (selectedIndex)
                {
                    case 0: // 血氧
                        dataEntry1.Placeholder = "血氧饱和度";
                        break;
                    case 1: // 心率
                        dataEntry1.Placeholder = "平均心率";
                        break;
                    case 2: // 血压
                        dataEntry1.Placeholder = "舒张压";
                        dataEntry2.Placeholder = "收缩压";
                        break;
                    case 3: // 呼吸率
                        dataEntry1.Placeholder = "呼吸频率";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                dataEntry1.IsVisible = false;
                dataEntry2.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
    
    private async void OnAddHealthMetricClicked(object sender, EventArgs e)
    {
        HealthMetric healthMetric;
        // 添加健康数据到数据库
        var selectedIndex = metricPicker.SelectedIndex;
        switch(selectedIndex)
        {
            case 0: // 血氧
                if (string.IsNullOrWhiteSpace(dataEntry1.Text))
                {
                    await DisplayAlert("Error", "血氧饱和度不能为空", "OK");
                    return;
                }
                healthMetric = new BloodOxygen
                {
                    UserId = _userViewModel.User.Id,
                    OxygenLevel = Convert.ToInt32(dataEntry1.Text),
                    Timestamp = DateTime.Now
                };

                break;
            case 1: // 心率
                if (string.IsNullOrWhiteSpace(dataEntry1.Text))
                {
                    await DisplayAlert("Error", "心率不能为空", "OK");
                    return;
                }
                healthMetric = new HeartRate
                {
                    UserId = _userViewModel.User.Id,
                    BeatsPerMinute = float.Parse(dataEntry1.Text),
                    Timestamp = DateTime.Now
                };
                break;
            case 2: // 血压
                if (string.IsNullOrWhiteSpace(dataEntry1.Text) || string.IsNullOrWhiteSpace(dataEntry2.Text))
                {
                    await DisplayAlert("Error", "血压不能为空", "OK");
                    return;
                }
                healthMetric = new BloodPressure
                {
                    UserId = _userViewModel.User.Id,
                    Diastolic = Convert.ToInt32(dataEntry1.Text),
                    Systolic = Convert.ToInt32(dataEntry2.Text),
                    Timestamp = DateTime.Now
                };
                break;
            case 3: // 呼吸率
                if (string.IsNullOrWhiteSpace(dataEntry1.Text))
                {
                    await DisplayAlert("Error", "呼吸率不能为空", "OK");
                    return;
                }
                healthMetric = new RespiratoryRate
                {
                    UserId = _userViewModel.User.Id,
                    BreathsPerMinute = Convert.ToInt32(dataEntry1.Text),
                    Timestamp = DateTime.Now
                };
                break;
            default:
                await DisplayAlert("Error", "请选择一种健康指标", "OK");
                return;
        }
        
        await _healthMetricsViewModel.AddHealthMetric(healthMetric);

        MessagingCenter.Send(this, "RefreshHealthMetrics");

        // 返回上一页
        await Navigation.PopAsync();
        // FIXME
        //await Navigation.PushAsync(new ReportPage());
        //Shell.Current.GoToAsync("ReportPage");
        //Application.Current.MainPage = new ReportPage();
        //await Shell.Current.GoToAsync("newReportPage");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        // 返回上一页
        await Navigation.PopAsync();
        //Application.Current.MainPage = new ReportPage();
    }

}