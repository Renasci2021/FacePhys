using FacePhys.Models;

namespace FacePhys.Selectors;

public class HealthMetricTemplateSelector : DataTemplateSelector
{
    public DataTemplate? BloodPressureTemplate { get; set; }
    public DataTemplate? BloodOxygenTemplate { get; set; }
    public DataTemplate? HeartRateTemplate { get; set; }
    public DataTemplate? RespiratoryRateTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        var healthMetric = (HealthMetric)item;
        return healthMetric.Type switch
        {
            MetricType.BloodPressure => BloodPressureTemplate,
            MetricType.BloodOxygen => BloodOxygenTemplate,
            MetricType.HeartRate => HeartRateTemplate,
            MetricType.RespiratoryRate => RespiratoryRateTemplate,
            _ => null,
        };
    }
}
