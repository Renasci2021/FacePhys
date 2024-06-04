using Microsoft.Maui.Controls;
using System;
using FacePhys.ViewModels;

namespace FacePhys.Pages;

public partial class HomePage : ContentPage
{
    public HomePage(HomePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        // 在这里初始化页面或绑定数据
    }

    // 在这里添加更多方法或事件处理程序
}
