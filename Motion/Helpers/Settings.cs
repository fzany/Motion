using Motion.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Motion.Helpers
{
    public class Settings
    {
        public static int DecimalPoint
        {
            get => Preferences.Get(nameof(DecimalPoint), 2);
            set => Preferences.Set(nameof(DecimalPoint), value);
        }

        public static MagnetoMode MagnetoMethod
        {
            get => (MagnetoMode)Enum.Parse(typeof(MagnetoMode), Enum.GetName(typeof(MagnetoMode), MagnetoMode.XYRad));
            set => Preferences.Set(nameof(MagnetoMethod), Enum.GetName(typeof(MagnetoMode), value));
        }
    }
}
