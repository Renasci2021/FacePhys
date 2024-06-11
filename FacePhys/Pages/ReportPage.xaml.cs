using FacePhys.ViewModels;
using FacePhys.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace FacePhys.Pages;

public partial class ReportPage : ContentPage
{
	private UserViewModel _userViewModel;

	public ReportPage()
	{
		InitializeComponent();
		_userViewModel = App.UserViewModel;
	}
}