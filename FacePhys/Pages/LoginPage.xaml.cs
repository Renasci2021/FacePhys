namespace FacePhys.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		BindingContext = App.UserViewModel;
	}
}