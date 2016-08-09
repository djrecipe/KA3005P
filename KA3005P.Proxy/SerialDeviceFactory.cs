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
        public List<string> Enumerate()
        {
            return SerialPort.GetPortNames().ToList();
        }
        public T Find<T>() where T: SerialDevice, new()
        {
            foreach(string port in this.Enumerate())
            {
                T device = new T();
                if (device.Connect(port))
                    return device;
            }
            return null;
        }
    }
}
