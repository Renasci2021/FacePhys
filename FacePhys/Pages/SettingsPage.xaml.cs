using System.Windows.Input;

namespace FacePhys.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
		BindingContext = App.UserViewModel;
	}

	public async void OnEditClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new EditPage());
	}
}