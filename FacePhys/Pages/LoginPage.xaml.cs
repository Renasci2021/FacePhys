using FacePhys.ViewModels;

namespace FacePhys.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		BindingContext = new UserViewModel(App.DatabaseService);
	}
}