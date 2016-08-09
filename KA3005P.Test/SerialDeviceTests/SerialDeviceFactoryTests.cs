using System;
using System.Collections.Generic;
using KA3005P.Proxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KA3005P.Test
{
    [TestClass]
    public class SerialDeviceFactoryTests
    {
        [TestMethod]
        public void TestSerialDeviceEnumeration()
        {
            SerialDeviceFactory factory = new SerialDeviceFactory();
            List<string> devices = factory.Enumerate();
            foreach(string device in devices)
                Console.WriteLine(device);
            return;
        }
    }
}
