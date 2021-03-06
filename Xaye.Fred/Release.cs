﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Xaye.Fred
{
    /// <summary>A release of economic data.</summary>
    /// <remarks>See http://api.stlouisfed.org/docs/fred/realtime_period.html for information 
    /// about real-time periods.</remarks>
    public class Release : Item, IEnumerable<Series>
    {
        #region OrderBy enum

        /// <summary>
        /// What to order releases by.
        /// </summary>
        public enum OrderBy
        {
            ReleaseId,
            Name,
            PressRelease,
            RealtimeStart,
            RealtimeEnd
        }

        #endregion

        private readonly Lazy<List<Series>> _series;

        internal Release(Fred fred) : base(fred)
        {
            _series = new Lazy<List<Series>>(
                () =>
                {
                    var series = (List<Series>) Fred.GetReleaseSeries(Id, RealtimeStart, RealtimeEnd);
                    var count = series.Count;
                    var call = 1;
                    while (count == CallLimit)
                    {
                        var more = (List<Series>) Fred.GetReleaseSeries(Id, DateTime.Today, DateTime.Today, CallLimit, call*CallLimit);
                        series.AddRange(more);
                        count = more.Count;
                        call++;
                    }
                    return series;
                }
                );
        }

        /// <summary>
        /// The release's id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The release's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A link to information about the release.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Does the link provide a press release.
        /// </summary>
        public bool PressRelease { get; set; }

        /// <summary>
        /// The start of the real-time period.
        /// </summary>
        public DateTime RealtimeStart { get; set; }

        /// <summary>
        /// The end of the real-time period.
        /// </summary>        
        public DateTime RealtimeEnd { get; set; }

        /// <summary>
        /// Provides an enumerator over the
        /// <see cref="Series"/> in the release.
        /// </summary>
        [Obsolete("Please use GetSeries() instead. This property will be removed in the next release.")]
        public IEnumerable<Series> Series => GetSeries();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<Series> GetEnumerator() => GetSeries().GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Provides an enumerator over the
        /// <see cref="Xaye.Fred.Series"/> in the release.
        /// </summary>
        public IEnumerable<Series> GetSeries() => _series.Value;
    }
}