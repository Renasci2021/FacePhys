using FacePhys.Models;

namespace FacePhys.Selectors;

public class HealthMetricTemplateSelector : DataTemplateSelector
{
    public DataTemplate? BloodPressureTemplate { get; set; }
    public DataTemplate? BloodOxygenTemplate { get; set; }
    public DataTemplate? HeartRateTemplate { get; set; }
    public DataTemplate? RespirationRateTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        if (item is BloodPressure)
            return BloodPressureTemplate;
        if (item is HeartRate)
            return HeartRateTemplate;
        if (item is BloodOxygen)
            return BloodOxygenTemplate;
        if (item is RespirationRate)
            return RespirationRateTemplate;

        return null;
    }
}
