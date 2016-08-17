using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KA3005P.Proxy
{
    public class Korad : SerialDevice
    {
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
        private const string CMD_SETBEEP = "BEEP{0}";
        private const string CMD_GETSTATUS= "STATUS?";
        private const string CMD_SETOUTPUTENABLE = "OUT{0}";
        private const string CMD_GETVOLTAGE = "VSET1?";
        private const string CMD_SETVOLTAGE = "VSET1:{0}";
        private StatusBits statusBits;
        public bool BeepEnabled => this.statusBits.HasFlag(StatusBits.BeepEnabled);
        public override string Name => "KORADKA3005PV2.0";
        public bool LockEnabled => this.statusBits.HasFlag(StatusBits.LockEnabled);
        public double MinimumVoltage => 0.0;
        public double MaximumVoltage => 30.0;
        public bool OutputEnabled => this.statusBits.HasFlag(StatusBits.OutputEnabled);
        public OutputModes OutputMode => this.statusBits.HasFlag(StatusBits.Ch1CVMode) ? OutputModes.Voltage : OutputModes.Current;
        public Korad() : base()
        {
        }
        public double GetVoltage()
        {
            string text = this.Query(Korad.CMD_GETVOLTAGE);
            double value = 0.0;
            double.TryParse(text, out value);
            return value;
        }
        public void Initialize()
        {
            this.SetBeep(false);
            this.SetOutputEnabled(false);
            return;
        }
        public bool SetBeep(bool enabled)
        {
            this.SendCommand(Korad.CMD_SETBEEP, (enabled ? 1 : 0).ToString());
            this.UpdateStatus();
            return this.BeepEnabled;
        }
        public bool SetOutputEnabled(bool enabled)
        {
            this.SendCommand(Korad.CMD_SETOUTPUTENABLE, (enabled ? 1 : 0).ToString());
            this.UpdateStatus();
            return this.OutputEnabled;
        }
        public double SetVoltage(double value)
        {
            value = Math.Max(Math.Min(value, this.MaximumVoltage), this.MinimumVoltage);
            string number = value.ToString("0#.##");
            this.SendCommand(Korad.CMD_SETVOLTAGE, number);
            return this.GetVoltage();
        }
        public void UpdateStatus()
        {
            // TODO 08/16/16: there is an issue deciphering the status bits whenever OVP or OCP are enabled (OVP is worse)
            string text = this.Query(Korad.CMD_GETSTATUS);
            int value = text.Length > 0 ? text[0] : -1;
            Console.WriteLine("Status: {0}", value);
            this.statusBits = (StatusBits) (text == null ? 0 : text[0]);  
            return;
        }
    }
}
