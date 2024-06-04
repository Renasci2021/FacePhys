using System.ComponentModel;
using System.Windows.Input;
using FacePhys.Models;
// using FacePhys.Data;
// using FacePhys.Data.Repositories;
// using FacePhys.Data.Repositories.Interfaces;

namespace FacePhys.ViewModels;

/// <summary>
/// MeasurePageViewModel类
/// 用于处理MeasurePage的数据和逻辑
/// </summary>
public class MeasurePageViewModel : INotifyPropertyChanged
{
    //private readonly IUserRepository _userRepository;
    private string _signalInstructions;
    public event PropertyChangedEventHandler PropertyChanged;
    public ICommand CancelCommand { get; private set; }
    public ICommand RetryCommand { get; private set; }
    public UserModel User { get; private set; }
    public string SignalInstructions
    {
        get => _signalInstructions;
        set
        {
            if (_signalInstructions != value)
            {
                _signalInstructions = value;
                OnPropertyChanged(nameof(SignalInstructions));

            }
        }
    }

    public MeasurePageViewModel(/*IUserRepository userRepository*/)
    {
        //_userRepository = userRepository;

        //User = _userRepository.GetTestUser();

        CancelCommand = new Command(async () =>
        {
            // TODO
            // 在这里添加取消操作
        });

        RetryCommand = new Command(async () =>
        {
            // TODO
            // 在这里添加重试操作
        });


    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // 示例方法来模拟更新SignalInstructions属性的情况
    public void UpdateSignalInstructions(string instructions)
    {
        SignalInstructions = instructions;
    }

}
