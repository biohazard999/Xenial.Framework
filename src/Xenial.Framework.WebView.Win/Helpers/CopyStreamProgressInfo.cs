using System;

namespace Xenial.Framework.WebView.Win.Helpers
{
    /// <summary>   Provides information about a copy progress. </summary>
    public sealed class CopyStreamProgressInfo
    {
        /// <summary>   Gets the number bytes copied so far. </summary>
        ///
        /// <value> The bytes downloaded. </value>

        public double BytesDownloaded { get; init; }

        /// <summary>   Gets the remaining bytes to copy. </summary>
        ///
        /// <value> The remaining bytes. </value>

        public double RemainingBytes => TotalBytes - BytesDownloaded;

        /// <summary>   Gets the total number of bytes to copy. </summary>
        ///
        /// <value> The total number of bytes. </value>

        public double TotalBytes { get; init; }

        /// <summary>   Gets the percentage of the current copy operation. </summary>
        ///
        /// <value> The percentage from 0 to 100. </value>

        public double Percent { get; init; }

        /// <summary>   Gets the elapsed time for the copy operation. </summary>
        ///
        /// <value> The elapsed time. </value>

        public TimeSpan Elapsed { get; init; }

        /// <summary>   Gets the current size of the buffer. </summary>
        ///
        /// <value> The size of the buffer in bytes. </value>

        public int BufferSize { get; init; }

        /// <summary>   Gets the current bytes per second. </summary>
        ///
        /// <value> The current bytes per second. </value>

        public double CurrentBytesPerSecond { get; init; }

        /// <summary>   Gets the maximum bytes per second. </summary>
        ///
        /// <value> The maximum bytes per second. </value>

        public double MaxBytesPerSecond { get; init; }

        /// <summary>   Gets the minimum bytes per second. </summary>
        ///
        /// <value> The minimum bytes per second. </value>

        public double MinBytesPerSecond { get; init; }

        /// <summary>   Gets the average bytes per second. </summary>
        ///
        /// <value> The average bytes per second. </value>

        public double AvgBytesPerSecond { get; init; }

        /// <summary>   Creates a new Progress object. </summary>
        ///
        /// <returns>   The new progress. </returns>

        public static Progress<CopyStreamProgressInfo> CreateProgress()
            => new Progress<CopyStreamProgressInfo>();
    }
}
