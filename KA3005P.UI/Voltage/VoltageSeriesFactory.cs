using System;
using System.IO;

namespace KA3005P.UI.Voltage
{
    /// <summary>
    /// Creates voltage series
    /// </summary>
    /// <seealso cref="VoltageSeries"/>
    public class VoltageSeriesFactory
    {
        #region Instance Methods
        /// <summary>
        /// Default constructor
        /// </summary>
        public VoltageSeriesFactory()
        {
        }
        /// <summary>
        /// Create a voltage series using the file at the specified path
        /// </summary>
        /// <param name="path">Path to voltage series file</param>
        /// <returns>Series of sequential time/voltage pairs</returns>
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
        #endregion
    }
}
