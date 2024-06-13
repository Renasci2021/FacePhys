
using System.ComponentModel;
using FacePhys.ViewModels;
using FacePhys.Models;
namespace FacePhys.Pages
{
	public partial class EditPage : ContentPage
	{
		private UserViewModel _userViewModel;
		

		public EditPage()
		{
			InitializeComponent();
			_userViewModel = App.UserViewModel;
			BindingContext = App.UserViewModel;
		}

		private async void OnSaveButtonClicked(object sender, EventArgs e)
		{
			await _userViewModel.UpdateUserAsync();
			MessagingCenter.Send(this, "RefreshUser");
			await Navigation.PopAsync();
			//Application.Current.MainPage = new SettingsPage();
		}

		private async void OnBackButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PopAsync();
			//Application.Current.MainPage = new SettingsPage();
		}
	}
}