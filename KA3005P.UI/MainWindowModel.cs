using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using KA3005P.Proxy;
using KA3005P.UI.Annotations;

namespace KA3005P.UI
{
    internal class MainWindowModel : INotifyPropertyChanged
    {
        private Korad korad = null;
        private double _Voltage = 0.0;
        public double Voltage
        {
            get
            {
                return this._Voltage;
            }
            set
            {
                this._Voltage = this.korad.SetVoltage(value);
                this.OnPropertyChanged("Voltage");
            }
        }
        internal MainWindowModel()
        {
            this.ConnectKorad();
            this.InitializeValues();
        }
        private void ConnectKorad()
        {
            SerialDeviceFactory factory = new SerialDeviceFactory();
            this.korad = factory.Find<Korad>();
            return;
        }
        private void InitializeValues()
        {
            this._Voltage = this.korad.GetVoltage();
            this.OnPropertyChanged("Voltage");
            return;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string property_name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
        }
    }
}
