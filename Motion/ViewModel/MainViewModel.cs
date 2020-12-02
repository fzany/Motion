using Motion.Helpers;
using Motion.Model;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Motion.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private SensorSpeed speed = SensorSpeed.Default;
        public Command StartAll { get; }
        #region items
        General _gyro;
        public General Gyro
        {
            get => _gyro;
            set
            {
                SetProperty(ref _gyro, value);
            }
        }

        General _magneto;
        public General Magneto
        {
            get => _magneto;
            set
            {
                SetProperty(ref _magneto, value);
            }
        }

        General _accelero;
        public General Accelero
        {
            get => _accelero;
            set
            {
                SetProperty(ref _accelero, value);
            }

        }

        double _lowMagneto = 1000;
        public double LowMagneto
        {
            get => _lowMagneto;
            set
            {
                SetProperty(ref _lowMagneto, value);
            }
        }

        double _highMagneto = -1000;
        public double HighMagneto
        {
            get => _highMagneto;
            set
            {
                SetProperty(ref _highMagneto, value);
            }
        }

        private MagnetoMode _magnetoMethodType = Settings.MagnetoMethod;
        public MagnetoMode CurrentMagnetoMethodType
        {
            get => _magnetoMethodType;
            set
            {
                SetProperty(ref _magnetoMethodType, value);
                Settings.MagnetoMethod = value;
            }
        }

        public List<string> MagentoMethods
        {
            get
            {
                return Enum.GetNames(typeof(MagnetoMode)).ToList();
            }
        }

        int _decima = Settings.DecimalPoint;
        public int DecimaPoint
        {
            get => _decima;
            set
            {
                SetProperty(ref _decima, value);
                Settings.DecimalPoint = _decima;
            }
        }
        
        #endregion
        public MainViewModel()
        {
            //initialize.
            Gyro = new General();
            Magneto = new General();
            Accelero = new General();

            Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            ToggleAll();
        }

        private void ToggleAll()
        {
            ToggleAccelerometer();
            ToggleMagnetometer();
            ToggleGyroscope();
        }

        private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var data = e.Reading.AngularVelocity;
            Gyro = Trimmer(new General() { X = data.X*1000, Y = data.Y*1000, Z = data.Z*1000 });
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading.Acceleration;
            Accelero = Trimmer(new General() { X = data.X*100, Y = data.Y*100, Z = data.Z*100 });
        }

        private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            var data = e.Reading.MagneticField;
            var angle = Math.Atan2(data.Y, data.X);
            if (CurrentMagnetoMethodType == MagnetoMode.XRad)
                angle = Math.Atan(data.X);
            angle *= (180 / Math.PI);
            angle += 90;
            angle = (angle + 360) % 360;

            //calculate high and low.
            if (angle < LowMagneto)
                LowMagneto = Rounder(angle);

            if (angle > HighMagneto)
                HighMagneto = Rounder(angle);

            Magneto = Trimmer(new General() { X = data.X, Y = data.Y, Z = data.Z, Translate = angle });

        }

        private General Trimmer(General data)
        {
            data.X = Rounder(data.X);
            data.Y = Rounder(data.Y);
            data.Z = Rounder(data.Z);
            data.Translate = Rounder(data.Translate);
            return data;
        }

        private double Rounder(double data)
        {
            return Math.Round(data, Settings.DecimalPoint, MidpointRounding.AwayFromZero);
        }
        public void ToggleGyroscope()
        {
            try
            {
                if (Gyroscope.IsMonitoring)
                    Gyroscope.Stop();
                else
                    Gyroscope.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                CrossToastPopUp.Current.ShowToastError("Gyroscope not supported.");
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        public void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
                else
                    Accelerometer.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                CrossToastPopUp.Current.ShowToastError("Accelerometer not supported.");
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        public void ToggleMagnetometer()
        {
            try
            {
                if (Magnetometer.IsMonitoring)
                    Magnetometer.Stop();
                else
                    Magnetometer.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                CrossToastPopUp.Current.ShowToastError("Magnetometer not supported.");
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }
    }
}
