using System.ComponentModel;
using System.Runtime.CompilerServices;
using KA3005P.Proxy;
using KA3005P.UI.Annotations;

namespace KA3005P.UI
{
    internal class MainWindowModel : INotifyPropertyChanged
    {
        private Korad korad = null;
        public bool Connected => this.korad != null;
        public string ConnectionStatusText => this.Connected ? string.Format("{0} {1}", this.korad.Name, this.korad.PortName) : "Not Connected";
        public string OutputEnabledButtonText => (this.korad?.OutputEnabled ?? false) ? "Turn Off" : "Turn On";
        public string OutputEnabledStatus => this.korad.OutputEnabled ? "ENABLED" : "DISABLED";
        private bool _OutputEnabled = false;
        public bool OutputEnabled
        {
            get
            {
                return this._OutputEnabled;
            }
            set
            {
                this._OutputEnabled = this.korad.SetOutputEnabled(value);
                this.OnPropertyChanged("OutputEnabled");
                this.OnPropertyChanged("OutputEnabledButtonText");
                this.OnPropertyChanged("OutputEnabledStatus");
            }
        }
        public string OutputModeText => this.korad.OutputMode.ToString();
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
        }
        public void ConnectKorad()
        {
            SerialDeviceFactory factory = new SerialDeviceFactory();
            if (this.korad != null)
            {
                this.korad.Dispose();
                this.korad = null;
            }
            this.korad = factory.Find<Korad>();
            if (this.korad != null)
            {
                this.korad.Initialize();
                this.UpdateStatus();
            }
            this.OnPropertyChanged("Connected");
            this.OnPropertyChanged("ConnectionStatusText");
            return;
        }
        public void UpdateStatus()
        {
            if (this.korad == null)
                return;
            this.korad.UpdateStatus();
            this._Voltage = this.korad.GetVoltage();
            this._OutputEnabled = this.korad.OutputEnabled;
            this.OnPropertyChanged("Voltage");
            this.OnPropertyChanged("OutputEnabled");
            this.OnPropertyChanged("OutputEnabledButtonText");
            this.OnPropertyChanged("OutputEnabledStatus");
            this.OnPropertyChanged("OutputModeText");
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
