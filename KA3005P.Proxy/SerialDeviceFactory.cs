using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KA3005P.Proxy
{
    public class SerialDeviceFactory
    {
        public SerialDeviceFactory()
        {

        }
        public SerialDevice Create(string name)
        {
            List<string> ports = this.Enumerate();
            if (!ports.Contains(name))
                return null;
            return new SerialDevice(name);
        }
        public List<string> Enumerate()
        {
            return SerialPort.GetPortNames().ToList();
        }
    }
}
