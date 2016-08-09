using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KA3005P.Proxy
{
    public class SerialDevice
    {
        private readonly SerialPort port = null;
        public SerialDevice(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid port name", nameof(name));
            this.port = new SerialPort(name, 9600, Parity.None)
            {
                DataBits = 8,
                StopBits = StopBits.One
            };
            return;
        }
    }
}
