using System.Collections;
using System.Collections.Generic;

namespace KA3005P.UI.Voltage
{
    /// <summary>
    /// Sequential list of time/voltage pairs
    /// </summary>
    public class VoltageSeries : IEnumerator<VoltagePair>, IEnumerable<VoltagePair>
    {
        #region Instance Members
        private List<VoltagePair> voltages = new List<VoltagePair>();
        private uint position = 0;
        #endregion
        #region Instance Properties
        /// <summary>
        /// Number of pairs
        /// </summary>
        public int Count => this.voltages.Count;
        /// <summary>
        /// Current pair
        /// </summary>
        public VoltagePair Current { get; }
        /// <summary>
        /// Current pair
        /// </summary>
        object IEnumerator.Current => Current;
        #endregion
        #region Instance Methods
        /// <summary>
        /// Add a pair
        /// </summary>
        /// <param name="time">Time delay after previous voltage</param>
        /// <param name="voltage">Voltage to apply</param>
        public void Add(double time, double voltage)
        {
            this.voltages.Add(new VoltagePair() {Time = time, Voltage = voltage});
            return;
        }
        /// <summary>
        /// Remove all pairs
        /// </summary>
        public void Clear()
        {
            this.voltages.Clear();
            return;
        }
        public void Dispose()
        {
            return;
        }
        public IEnumerator<VoltagePair> GetEnumerator()
        {
            return this;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public bool MoveNext()
        {
            this.position++;
            return this.position < this.voltages.Count;
        }
        public void Reset()
        {
            this.position = 0;
        }
        #endregion

    }
}
