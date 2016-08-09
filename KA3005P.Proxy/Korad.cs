using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KA3005P.Proxy
{
    public class Korad : SerialDevice
    {
        private const string CMD_GETVOLTAGE = "VSET1?";
        private const string CMD_SETVOLTAGE = "VSET1:{0}";
        public override string Name => "KORADKA3005PV2.0";
        public double MinimumVoltage => 0.0;
        public double MaximumVoltage => 30.0;
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
        public double SetVoltage(double value)
        {
            value = Math.Max(Math.Min(value, this.MaximumVoltage), this.MinimumVoltage);
            string number = value.ToString("0#.##");
            this.SendCommand(Korad.CMD_SETVOLTAGE, number);
            System.Threading.Thread.Sleep(100);
            return this.GetVoltage();
        }
    }
}
