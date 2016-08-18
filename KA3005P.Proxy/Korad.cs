using System;
using System.Threading;

namespace KA3005P.Proxy
{
    public delegate void VoidDelegate();
    public delegate void DoubleDelegate(double value);
    public class Korad : SerialDevice
    {
        #region Types
        private enum Actions : int
        {
            None = 0,
            GetStatus = 1,
            GetVoltage = 2
        }
        [Flags]
        public enum StatusBits : int
        {
            Ch1CVMode = 1,
            Ch2CVMode = 2,
            SeriesTrackingMode = 4,
            ParallelTrackingMode = 12,
            BeepEnabled = 16,
            LockEnabled = 32,
            OutputEnabled = 64
        };
        public enum OutputModes : int
        {
            Voltage = 0,
            Current = 1
        }
        #endregion
        #region Instance Members
        #region Constants
        private const string CMD_SETBEEP = "BEEP{0}";
        private const string CMD_GETSTATUS= "STATUS?";
        private const string CMD_SETOUTPUTENABLE = "OUT{0}";
        private const string CMD_GETVOLTAGE = "VSET1?";
        private const string CMD_SETVOLTAGE = "VSET1:{0}";
        #endregion
        private readonly Mutex mutHandleData = new Mutex(false, "Korad.HandleData");
        public event VoidDelegate StatusUpdated;
        public event DoubleDelegate VoltageUpdated;
        #endregion
        #region Instance Properties
        private Actions Action { get;set; }
        public bool BeepEnabled => this.Status.HasFlag(StatusBits.BeepEnabled);
        private double _CurrentVoltage = 0.0;
        public double CurrentVoltage
        {
            get
            {
                double value = 0.0;
                if (this.mutHandleData.WaitOne(TimeSpan.FromSeconds(5.0)))
                {
                    value = this._CurrentVoltage;
                    this.mutHandleData.ReleaseMutex();
                }
                return value;
            }
            set
            {
                if(this.mutHandleData.WaitOne((TimeSpan.FromSeconds(5.0))))
                {
                    this._CurrentVoltage = value;
                    this.mutHandleData.ReleaseMutex();
                }
                return;
            }
        }
        public override string Name => "KORADKA3005PV2.0";
        public bool LockEnabled => this.Status.HasFlag(StatusBits.LockEnabled);
        public double MinimumVoltage => 0.0;
        public double MaximumVoltage => 30.0;
        public bool OutputEnabled => this.Status.HasFlag(StatusBits.OutputEnabled);
        public OutputModes OutputMode => this.Status.HasFlag(StatusBits.Ch1CVMode) ? OutputModes.Voltage : OutputModes.Current;
        private StatusBits _Status = 0;
        public StatusBits Status
        {
            get
            {
                StatusBits value = 0;
                if (this.mutHandleData.WaitOne(TimeSpan.FromSeconds(5.0)))
                {
                    value = this._Status;
                    this.mutHandleData.ReleaseMutex();
                }
                return value;
            }
            set
            {
                if (this.mutHandleData.WaitOne((TimeSpan.FromSeconds(5.0))))
                {
                    this._Status = value;
                    this.mutHandleData.ReleaseMutex();
                }
                return;
            }
        }
        #endregion
        #region Instance Methods
        public Korad() : base()
        {
        }
        public void GetVoltage()
        {
            this.Action = Actions.GetVoltage;
            this.SendCommand(Korad.CMD_GETVOLTAGE);
            return;
        }
        protected override void HandleData(int byte_count)
        {
            string text = null;
            switch(this.Action)
            {
                default:
                case Actions.None:
                    break;
                case Actions.GetStatus:
                    text = this.GetText();
                    int value = text != null && text.Length > 0 ? text[0] : 0;
                    Console.WriteLine("Status: {0}", value);
                    this.Status = (StatusBits)value;
                    if (this.StatusUpdated != null)
                        this.StatusUpdated();
                    break;
                case Actions.GetVoltage:
                    if (byte_count < 5)
                        return;
                    text = this.GetText();
                    double voltage = 0.0;
                    double.TryParse(text, out voltage);
                    this.CurrentVoltage = voltage;
                    if (this.VoltageUpdated != null)
                        this.VoltageUpdated(voltage);
                    break;
            }
            this.Action = Actions.None;
        }
        public void Initialize()
        {
            this.SetBeep(false);
            this.SetOutputEnabled(false);
            return;
        }
        public void SetBeep(bool enabled)
        {
            this.SendCommand(Korad.CMD_SETBEEP, (enabled ? 1 : 0).ToString());
            this.UpdateStatus();
            return;
        }
        public void SetOutputEnabled(bool enabled)
        {
            this.SendCommand(Korad.CMD_SETOUTPUTENABLE, (enabled ? 1 : 0).ToString());
            this.UpdateStatus();
            return;
        }
        public void SetVoltage(double value)
        {
            value = Math.Max(Math.Min(value, this.MaximumVoltage), this.MinimumVoltage);
            string number = value.ToString("0#.##");
            this.SendCommand(Korad.CMD_SETVOLTAGE, number);
            this.GetVoltage();
            return;
        }
        public void UpdateStatus()
        {
            // TODO 08/16/16: there is an issue deciphering the status bits whenever OVP or OCP are enabled (OVP is worse)  
            this.Action = Actions.GetStatus;
            this.SendCommand(Korad.CMD_GETSTATUS);
            return;
        }
        #endregion
    }
}
