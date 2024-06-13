using FacePhys.ViewModels;
using FacePhys.Models;
namespace FacePhys.Pages;

public partial class RegisterPage : ContentPage
{
    private UserViewModel _userViewModel;
	public RegisterPage()
	{
		InitializeComponent();
		_userViewModel = App.UserViewModel;
	}

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        Gender gender; // 获取性别
        if(GenderPicker.SelectedIndex==0)gender = Gender.Male;
        else if(GenderPicker.SelectedIndex==1)gender = Gender.Female;
        else gender = Gender.Other;
        int age = Convert.ToInt32(UserAgeEntry.Text);
        int height = Convert.ToInt32(UserHeightEntry.Text);
        int weight = Convert.ToInt32(UserWeightEntry.Text);
        
        bool success = await _userViewModel.RegisterAsync(username, gender, age, height, weight);
        if (success)
        {
            //await DisplayAlert("注册成功", "注册成功", "确定");
            //await Shell.Current.GoToAsync("//HomePage");
        }
        await Navigation.PopAsync();
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync("//LoginPage");
        await Navigation.PopAsync();
    }
}