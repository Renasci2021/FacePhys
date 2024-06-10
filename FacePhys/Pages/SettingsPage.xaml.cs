using System.Windows.Input;

namespace FacePhys.Pages;

public partial class SettingsPage : ContentPage
{
	
	public SettingsPage()
	{
		InitializeComponent();
		// NavigateToEditInfoCommand = new Command(async () =>
		// {
		// 	await Shell.Current.GoToAsync("EditPage");
		// });
		BindingContext = App.UserViewModel;
	}

}