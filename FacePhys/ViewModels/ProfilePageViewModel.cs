using System.ComponentModel;
using System.Windows.Input;
using FacePhys.Models;
// using FacePhys.Data;
// using FacePhys.Data.Repositories;
// using FacePhys.Data.Repositories.Interfaces;

namespace FacePhys.ViewModels;

/// <summary>
/// ProfilePageViewModel类
/// 用于处理ProfilePage的数据和逻辑
/// </summary>
public class ProfilePageViewModel : INotifyPropertyChanged
{
    //private readonly IUserRepository _userRepository;

    public UserModel User { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand SaveCommand { get; }
    public ICommand ChangeAvatarCommand { get; }

    public ProfilePageViewModel(/*IUserRepository userRepository*/)
    {
        // _userRepository = userRepository;

        // User = _userRepository.GetTestUser();

        SaveCommand = new Command(async () =>
        {
            //_userRepository.UpdateUser(User);
            //await SecureStorage.SetAsync("userId", User.Id.ToString());
        });

        ChangeAvatarCommand = new Command(async () =>
        {
            var result = await MediaPicker.PickPhotoAsync();
            if (result != null)
            {
                //User.AvatarUrl = result.FullPath;
                OnPropertyChanged(nameof(User));
            }
        });
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
