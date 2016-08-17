using System;
using System.IO.Ports;

namespace KA3005P.Proxy
{
    public delegate void SerialDeviceDelegate(SerialDevice device);
    public abstract class SerialDevice : IDisposable
    {
        private const string CMD_IDENTIFY = "*IDN?";
        private SerialPort port = null;
        public virtual string Name => null;
        public string PortName
        {
            get;
            private set;
        } = null;
        public SerialDevice()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
                throw new ArgumentException("Invalid device name", nameof(this.Name));
            return;
        }      
        public bool Connect(string port_name)
        {
            if (string.IsNullOrWhiteSpace(port_name))
                throw new ArgumentException("Invalid port name", nameof(port_name));
            bool success = false;
            this.port = new SerialPort(port_name, 9600, Parity.None)
            {
                DataBits = 8,
                StopBits = StopBits.One,
            };
            this.PortName = port_name;
            this.port.Open();
            success = this.Validate();
            if(!success)
                this.Dispose();
            return success;
        }
        public void Dispose()
        {
            if (this.port?.IsOpen ?? false)
                this.port.Close();
            return;
        }
        public string Identify()
        {
            return this.Query(SerialDevice.CMD_IDENTIFY);
        }
        public string GetText()
        {
            return this.port.BytesToRead > 0 ? this.port.ReadExisting()?.TrimEnd(new char[] { '\0', ' ' }) : null;
        }
        public string Query(string text)
        {
            this.SendCommand(text);
            string result = this.GetText();
            System.Threading.Thread.Sleep(100);
            return result;
        }
        public void SendCommand(string text)
        {
            this.port.Write(text);
            System.Threading.Thread.Sleep(100);
            return;
        }
        public void SendCommand(string format_string, params string[] values)
        {
            this.port.Write(string.Format(format_string, values));
            System.Threading.Thread.Sleep(100);
            return;
        }
        private bool Validate()
        {
            if (!this.port?.IsOpen ?? true)
                return false;
            string name = this.Identify();
            return this.Name == name;
        }
        ~SerialDevice()
        {
            this.Dispose();
        }
    }
}
