using Microsoft.Maui.Controls;
using System;
using FacePhys.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FacePhys.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfilePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // 在这里添加更多方法或事件处理程序
}
