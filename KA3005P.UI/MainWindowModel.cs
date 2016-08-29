using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using KA3005P.Proxy;
using KA3005P.UI.Annotations;
using KA3005P.UI.Voltage;
using Microsoft.Win32;

namespace KA3005P.UI
{
    internal class MainWindowModel : INotifyPropertyChanged
    {
        #region Types

        internal enum VoltageOutputFileStatuses : uint
        {
            Disconnected = 0,
            InvalidPath = 1,
            CanOutput = 2,
            IsOuputting = 3
        }
        #endregion
        #region Instance Members
        private readonly BackgroundWorker workerVoltageFile = new BackgroundWorker();
        private readonly SerialDeviceFactory factory = new SerialDeviceFactory();
        private Korad korad = null;
        #endregion
        #region Instance Properties
        public bool Connected => this.korad != null;
        public string ConnectionStatusText => this.Connected ? string.Format("{0}, {1}", this.korad.Name, this.korad.PortName) : "Not Connected";
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
        private string _VoltageFilePath = null;
        public string VoltageFilePath
        {
            get { return this._VoltageFilePath; }
            set
            {
                this._VoltageFilePath = value;
                this.OnPropertyChanged("VoltageFilePath");
            }
        }
        public VoltageOutputFileStatuses VoltageOutputFileStatus
        {
            get
            {
                if(!this.Connected)
                    return VoltageOutputFileStatuses.Disconnected;
                if (string.IsNullOrWhiteSpace(this.VoltageFilePath) || !File.Exists(this.VoltageFilePath))
                    return VoltageOutputFileStatuses.InvalidPath;
                if (this.workerVoltageFile.IsBusy)
                    return VoltageOutputFileStatuses.IsOuputting;
                return VoltageOutputFileStatuses.CanOutput;
            }   
        }
        #endregion
        #region Instance Methods
        internal MainWindowModel()
        {
            this.factory.DeviceFound += this.SerialDeviceFactory_DeviceFound;
            this.workerVoltageFile.DoWork += this.workerVoltageFile_DoWork;
            this.workerVoltageFile.RunWorkerCompleted += this.workerVoltageFile_RunWorkerCompleted;
            this.ConnectKorad();
        }


        public void BrowseVoltageFile()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Multiselect = false
            };
            if (dialog.ShowDialog() ?? false)
            {
                this.VoltageFilePath = dialog.FileName;
            }
            return;
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
        public void StartVoltageFile()
        {
            this.workerVoltageFile.RunWorkerAsync(this.VoltageFilePath);
            this.OnPropertyChanged("VoltageOutputFileStatus");
            return;
        }
        public void StopVoltageFile()
        {
            this.workerVoltageFile.CancelAsync();
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
        private void workerVoltageFile_DoWork(object sender, DoWorkEventArgs e)
        {
            VoltageSeriesFactory factory = new VoltageSeriesFactory();
            VoltageSeries series = factory.Create(e.Argument as string);
            foreach (VoltagePair pair in series)
            {
                Thread.Sleep((int)(pair.Time * 1000.0));
                this.korad.SetVoltage(pair.Voltage);
            }
            return;
        }
        private void workerVoltageFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.OnPropertyChanged("VoltageOutputFileStatus");
            return;
        }
        #endregion
    }
}
