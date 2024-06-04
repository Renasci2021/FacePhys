using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using FacePhys.Models;
// using FacePhys.Data;
// using FacePhys.Data.Repositories;
// using FacePhys.Data.Repositories.Interfaces;

namespace FacePhys.ViewModels
{
    public class ReportPageViewModel : INotifyPropertyChanged
    {
        //private readonly IUserRepository _userRepository;

        public UserModel User { get; private set; }

        public ObservableCollection<BloodPressure> BloodPressureMetrics { get; private set; }
        public ObservableCollection<BloodOxygen> BloodOxygenMetrics { get; private set; }
        public ObservableCollection<HeartRate> HeartRateMetrics { get; private set; }
        public ObservableCollection<RespirationRate> RespirationRateMetrics { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ReportPageViewModel(/*IUserRepository userRepository*/)
        {
            //_userRepository = userRepository;

            //User = _userRepository.GetTestUser();

            BloodPressureMetrics = new ObservableCollection<BloodPressure> {
                //new BloodPressure { Timestamp = DateTime.Now, Systolic=User.BloodPressureMetric.Systolic, Diastolic = User.BloodPressureMetric.Diastolic }
                new BloodPressure { Timestamp = DateTime.Now, Systolic=80, Diastolic =120 }
            };

            BloodOxygenMetrics = new ObservableCollection<BloodOxygen> {
                //new BloodOxygen { Timestamp = DateTime.Now, Saturation = User.BloodOxygenMetric.Saturation }
                new BloodOxygen { Timestamp = DateTime.Now, Saturation = 90 }
            };

            HeartRateMetrics = new ObservableCollection<HeartRate> {
                //new HeartRate { Timestamp = DateTime.Now, Value = User.HeartRateMetric.Value }
                new HeartRate { Timestamp = DateTime.Now, Value = 80 }
            };

            RespirationRateMetrics = new ObservableCollection<RespirationRate> {
                //new RespirationRate { Timestamp = DateTime.Now, Value = User.RespirationRateMetric.Value }
                new RespirationRate { Timestamp = DateTime.Now, Value = 80 }
            };
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
