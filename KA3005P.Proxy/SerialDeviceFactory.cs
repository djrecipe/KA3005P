using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;

namespace KA3005P.Proxy
{
    public class SerialDeviceFactory
    {
        #region Instance Members
        #region Events
        public event SerialDeviceDelegate DeviceFound;
        #endregion
        private readonly BackgroundWorker workerFind = new BackgroundWorker();
        #endregion
        #region Instance Methods
        public SerialDeviceFactory()
        {
            this.workerFind.DoWork += this.workerFind_DoWork;
            this.workerFind.RunWorkerCompleted += this.workerFind_RunWorkerCompleted;
        }
        public List<string> Enumerate()
        {
            return SerialPort.GetPortNames().ToList();
        }
        public void Find<T>() where T: SerialDevice, new()
        {
            T device = new T();
            this.workerFind.RunWorkerAsync(device);
            return;
        }
        #endregion
        #region Instance Events
        private void workerFind_DoWork(object sender, DoWorkEventArgs e)
        {
            SerialDevice device = e.Argument as SerialDevice;
            foreach (string port in this.Enumerate())
            {
                if (device.Connect(port))
                {
                    e.Result = device;
                    break;
                }
            }
            return;
        }
        private void workerFind_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(this.DeviceFound != null)
                this.DeviceFound(e.Result as SerialDevice);
            return;
        }
        #endregion
    }
}
