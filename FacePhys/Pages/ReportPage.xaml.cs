using FacePhys.ViewModels;

namespace FacePhys.Pages;

public partial class ReportPage : ContentPage
{
    public ReportPage(ReportPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    // 在这里添加更多方法或事件处理程序
}