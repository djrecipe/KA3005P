using System.IO;
using KA3005P.UI.Voltage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KA3005P.Test.VoltageSeriesTests
{
    [TestClass]
    public class VoltageSeriesFactoryTests
    {
        [TestMethod]
        public void TestVoltageSeriesCreation()
        {
            string file_contents = "0.1,5\n0.1,7\n1.3,4.2";
            string path = Path.GetTempFileName();
            File.WriteAllText(path, file_contents);
            VoltageSeriesFactory factory = new VoltageSeriesFactory();
            VoltageSeries series = factory.Create(path);
            Assert.AreEqual(3, series.Count);
        }
    }
}
