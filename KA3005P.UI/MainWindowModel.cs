using System.ComponentModel;
using System.Runtime.CompilerServices;
using KA3005P.Proxy;
using KA3005P.UI.Annotations;

namespace KA3005P.UI
{
    internal class MainWindowModel : INotifyPropertyChanged
    {
        #region Instance Members
        private readonly SerialDeviceFactory factory = new SerialDeviceFactory();
        private Korad korad = null;
        #endregion
        #region Instance Properties
        public bool Connected => this.korad != null;
        public string ConnectionStatusText => this.Connected ? string.Format("{0} {1}", this.korad.Name, this.korad.PortName) : "Not Connected";
        public string OutputEnabledButtonText => this.OutputEnabled ? "Turn Off" : "Turn On";
        public string OutputEnabledStatus => this.OutputEnabled ? "ENABLED" : "DISABLED";
        public bool OutputEnabled
        {
            get
            {
                return this.korad?.OutputEnabled ?? false;
            }
            set
            {
                this.korad?.SetOutputEnabled(value);               
            }
        }
        public string OutputModeText => this.korad?.OutputMode.ToString();
        public double Voltage
        {
            get
            {
                return this.korad?.CurrentVoltage ?? 0.0;
            }
            set
            {
                this.korad?.SetVoltage(value);
            }
        }
        #endregion
        #region Instance Methods
        internal MainWindowModel()
        {
            this.factory.DeviceFound += this.SerialDeviceFactory_DeviceFound;
            this.ConnectKorad();
        }
        public void ConnectKorad()
        {
            if (this.korad != null)
            {
                this.korad.Dispose();
                this.korad = null;
                this.OnPropertyChanged("Connected");
                this.OnPropertyChanged("ConnectionStatusText");
            }
            this.factory.Find<Korad>();
            return;
        }
        public void UpdateStatus()
        {
            if (this.korad == null)
                return;
            this.korad.UpdateStatus();
            this.korad.GetVoltage();
            return;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string property_name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
            return;
        }
        #endregion
        #region Instance Events   
        private void Korad_StatusUpdated()
        {
            this.OnPropertyChanged("OutputEnabled");
            this.OnPropertyChanged("OutputEnabledButtonText");
            this.OnPropertyChanged("OutputEnabledStatus");
            this.OnPropertyChanged("OutputModeText");
        }
        private void Korad_VoltageUpdated(double value)
        {
            this.OnPropertyChanged("Voltage");
        }
        private void SerialDeviceFactory_DeviceFound(SerialDevice device)
        {
            this.korad = device as Korad;
            this.OnPropertyChanged("Connected");
            this.OnPropertyChanged("ConnectionStatusText");
            if (this.korad != null)
            {
                this.korad.StatusUpdated += this.Korad_StatusUpdated;
                this.korad.VoltageUpdated += this.Korad_VoltageUpdated;
                this.korad.Initialize();
                this.UpdateStatus();
            }
            return;
        }
        #endregion
    }
}
