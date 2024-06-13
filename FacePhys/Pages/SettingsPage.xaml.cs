using System.Windows.Input;
using FacePhys.Models;
using FacePhys.ViewModels;
namespace FacePhys.Pages;

public partial class SettingsPage : ContentPage
{
	public UserViewModel _userViewModel;
	User User;
	
	public ICommand ChangeAvatarCommand { get; }
	public SettingsPage()
	{
		InitializeComponent();
		_userViewModel = App.UserViewModel;
		BindingContext = App.UserViewModel;

		MessagingCenter.Subscribe<EditPage>(this, "RefreshUser", async (sender) =>
		{
			await RefreshUser();
		});
	}

	public async void OnEditClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new EditPage());
		//Application.Current.MainPage = new EditPage();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		try{
			await RefreshUser();
		}
		catch (Exception ex)
		{
			DisplayAlert("Initialization Error", ex.Message, "OK");
		}
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		MessagingCenter.Unsubscribe<EditPage>(this, "RefreshUser");
    }

	private async Task RefreshUser()
	{
		try
		{
			await App.UserViewModel.UpdateUserAsync();
			this.BindingContext = null;
			this.BindingContext = App.UserViewModel;
			
			OnPropertyChanged(nameof(App.UserViewModel.User));
		
		}
		catch (Exception ex)
		{
			DisplayAlert("Initialization Error", ex.Message, "OK");
		}
	}
    
}