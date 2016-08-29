using System;
using System.IO;

namespace KA3005P.UI.Voltage
{
    public class VoltageSeriesFactory
    {
        public VoltageSeriesFactory()
        {
        }

        public VoltageSeries Create(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new FileNotFoundException("Invalid voltage file path");
            string[] lines = File.ReadAllLines(path);
            if (lines.Length < 1)
                throw new FileFormatException("Invalid voltage file");
            VoltageSeries series = new VoltageSeries();
            foreach (string line in lines)
            {
                try
                {
                    string[] words = line.Split(',');
                    double time = 0.0, voltage = 0.0;
                    if (words.Length < 2)
                        continue;
                    if (double.TryParse(words[0], out time) && double.TryParse(words[1], out voltage))
                    {
                        series.Add(time, voltage);
                    }
                }
                catch (Exception ex)
                {
                    // TODO: 08/29/16 implement exception handling
                }
            }
            return series;
        }
    }
}
