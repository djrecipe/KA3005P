using System;
using System.IO.Ports;
using System.Threading;

namespace KA3005P.Proxy
{
    public delegate void SerialDeviceDelegate(SerialDevice device);
    public abstract class SerialDevice : IDisposable
    {
        #region Static Members
        private const string CMD_IDENTIFY = "*IDN?";
        #endregion
        #region Instance Members
        private SerialPort port = null;
        private readonly Mutex mutex = new Mutex(false, "KA3005P.Proxy.SerialDevice");
        #endregion
        #region Instance Properties
        public virtual string Name => null;
        public string PortName
        {
            get;
            private set;
        } = null;
        #endregion
        #region Instance Methods
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
            if (!this.mutex.WaitOne(5000))
                throw new TimeoutException("Timeout waiting for serial device mutex");
            if (this.port?.IsOpen ?? false)
                this.port.Close();
            this.mutex.ReleaseMutex();
            return;
        }
        protected abstract void HandleData(int byte_count);
        public string GetText()
        {
            if (!this.mutex.WaitOne(5000))
                throw new TimeoutException("Timeout waiting for serial device mutex");
            string raw = this.port.BytesToRead > 0 ? this.port.ReadExisting() : null;
            this.mutex.ReleaseMutex();
            string result = raw?.TrimEnd(new char[] { '\0', ' ' });
            return result;
        }
        public void SendCommand(string text)
        {
            if (!this.mutex.WaitOne(5000))
                throw new TimeoutException("Timeout waiting for serial device mutex");
            this.port.Write(text);
            // TODO 08/17/16: figure out how to get rid of these delays
            Thread.Sleep(100);
            this.mutex.ReleaseMutex();
            return;
        }
        public void SendCommand(string format_string, params string[] values)
        {
            if (!this.mutex.WaitOne(5000))
                throw new TimeoutException("Timeout waiting for serial device mutex");
            this.port.Write(string.Format(format_string, values));
            // TODO 08/17/16: figure out how to get rid of these delays
            Thread.Sleep(100);
            this.mutex.ReleaseMutex();
            return;
        }
        ~SerialDevice()
        {
            this.Dispose();
        }
        #endregion
        #region Instance Events
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.HandleData(this.port.BytesToRead);
            return;
        }
        #endregion
    }
}
