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
                DiscardNull = true,
                StopBits = StopBits.One,
                RtsEnable = true
            };
            this.PortName = port_name;
            this.port.Open();
            this.SendCommand(SerialDevice.CMD_IDENTIFY);
            string name = this.GetText();
            success = this.Name == name;
            if(success)
                this.port.DataReceived += this.SerialPort_DataReceived;
            else
                this.Dispose();
            return success;
        }


        public void Dispose()
        {
            if (this.port?.IsOpen ?? false)
                this.port.Close();
            return;
        }
        protected abstract void HandleData(int byte_count);
        public string GetText()
        {
            string raw = this.port.BytesToRead > 0 ? this.port.ReadExisting() : null;
            return raw?.TrimEnd(new char[] { '\0', ' ' });
        }
        public void SendCommand(string text)
        {
            this.port.Write(text);
            // TODO 08/17/16: figure out how to get rid of these delays
            System.Threading.Thread.Sleep(100);
            return;
        }
        public void SendCommand(string format_string, params string[] values)
        {
            this.port.Write(string.Format(format_string, values));
            // TODO 08/17/16: figure out how to get rid of these delays
            System.Threading.Thread.Sleep(100);
            return;
        }
        ~SerialDevice()
        {
            this.Dispose();
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.HandleData(this.port.BytesToRead);
            return;
        }
    }
}
