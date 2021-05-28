using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xenial.Framework.WebView.Win.Helpers;

namespace Scissors.Utils.Io
{
    /// <summary>   Extension methods for the System.IO.Stream class. </summary>
    public static class StreamExtensions
    {


        public const int DefaultBufferSize = 8192;

        /// <summary>
        /// Copies the sourceStream to the targetStream to with progress. To support progress reporting,
        /// the progressObject and totalBytes must be set.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">    sourceStream and targetStreams must not be null. </exception>
        ///
        /// <param name="sourceStream"> The source stream. </param>
        /// <param name="targetStream"> The target stream. </param>
        /// <param name="progress">     (Optional) The progress. </param>
        /// <param name="token">        (Optional) The token. </param>
        /// <param name="bufferSize">   (Optional) Size of the buffer. </param>
        /// <param name="totalBytes">   (Optional) The total number of bytes. </param>
        ///
        /// <returns>   The targetStream. </returns>

        public static Stream CopyToWithProgress(
            this Stream sourceStream,
            Stream targetStream,
            IProgress<CopyStreamProgressInfo>? progress = null,
            CancellationToken token = default(CancellationToken),
            int bufferSize = DefaultBufferSize,
            long totalBytes = -1L
        )
        {
            _ = sourceStream ?? throw new ArgumentNullException(nameof(sourceStream));
            _ = targetStream ?? throw new ArgumentNullException(nameof(targetStream));

            var totalWatch = Stopwatch.StartNew();
            var progressWatch = Stopwatch.StartNew();
            var progressInterval = TimeSpan.FromMilliseconds(100);

            var canReportProgress = totalBytes != -1 && progress != null;

            var totalRead = 0L;
            var buffer = new byte[bufferSize];
            var bytesInLastInterval = 0d;
            var interval = TimeSpan.FromSeconds(1);
            var copyWatch = Stopwatch.StartNew();

            var bytesPerSecond = 0d;
            var maxBytesPerSecond = 0d;
            do
            {
                token.ThrowIfCancellationRequested();

                var read = sourceStream.Read(buffer, 0, buffer.Length);

                var isMoreToRead = read != 0;

                if (isMoreToRead)
                {
                    targetStream.Write(buffer, 0, read);
                }

                totalRead += read;
                bytesInLastInterval += read;

                if (canReportProgress)
                {
                    if (copyWatch.Elapsed > interval)
                    {
                        bytesPerSecond = bytesInLastInterval / (copyWatch.Elapsed.Ticks / (double)interval.Ticks);
                        maxBytesPerSecond = Math.Max(maxBytesPerSecond, bytesPerSecond);

                        bytesInLastInterval = 0;
                        copyWatch.Restart();
                    }

                    if (progressWatch.Elapsed > progressInterval || !isMoreToRead)
                    {
                        progress?.Report(new CopyStreamProgressInfo
                        {
                            BytesDownloaded = totalRead,
                            TotalBytes = totalBytes,
                            Percent = (totalRead * 1d) / (totalBytes * 1d) * 100,
                            Elapsed = totalWatch.Elapsed,
                            BufferSize = bufferSize,
                            CurrentBytesPerSecond = bytesPerSecond,
                            MaxBytesPerSecond = maxBytesPerSecond,
                        });

                        progressWatch.Restart();
                    }
                }

                if (!isMoreToRead)
                {
                    break;
                }

            } while (true);

            totalWatch.Stop();

            return targetStream;
        }

        /// <summary>
        /// Copies the sourceStream to the targetStream to with progress. To support progress reporting,
        /// the progressObject and totalBytes must be set.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">    sourceStream and targetStreams must not be null. </exception>
        ///
        /// <param name="sourceStream"> The source stream. </param>
        /// <param name="targetStream"> The target stream. </param>
        /// <param name="progress">     (Optional) The progress. </param>
        /// <param name="token">        (Optional) The token. </param>
        /// <param name="bufferSize">   (Optional) Size of the buffer. </param>
        /// <param name="totalBytes">   (Optional) The total number of bytes. </param>
        ///
        /// <returns>   The targetStream. </returns>

        public static async Task<Stream> CopyToAsyncWithProgress(
            this Stream sourceStream,
            Stream targetStream,
            IProgress<CopyStreamProgressInfo>? progress = null,
            CancellationToken token = default(CancellationToken),
            int bufferSize = DefaultBufferSize, long totalBytes = -1L
        )
        {
            _ = sourceStream ?? throw new ArgumentNullException(nameof(sourceStream));
            _ = targetStream ?? throw new ArgumentNullException(nameof(targetStream));

            var totalWatch = Stopwatch.StartNew();
            var progressWatch = Stopwatch.StartNew();
            var progressInterval = TimeSpan.FromMilliseconds(100);

            var canReportProgress = totalBytes != -1 && progress != null;

            var totalRead = 0L;
            var buffer = new byte[bufferSize];
            var bytesInLastInterval = 0d;
            var interval = TimeSpan.FromSeconds(1);
            var copyWatch = Stopwatch.StartNew();

            var currentBytesPerSecond = 0d;
            var maxBytesPerSecond = 0d;
            var minBytesPerSecond = 0d;
            var avgBytesPerSecond = 0d;
            var bytesPerSecond = new List<double>();

            while (true)
            {
                token.ThrowIfCancellationRequested();

                var read = await sourceStream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false);

                var isMoreToRead = read != 0;

                if (isMoreToRead)
                {
                    await targetStream.WriteAsync(buffer, 0, read, token).ConfigureAwait(false);
                }

                totalRead += read;
                bytesInLastInterval += read;

                if (canReportProgress)
                {
                    if (copyWatch.Elapsed > interval)
                    {
                        currentBytesPerSecond = bytesInLastInterval / (copyWatch.Elapsed.Ticks / (double)interval.Ticks);
                        bytesPerSecond.Add(currentBytesPerSecond);

                        if (minBytesPerSecond == 0d)
                        {
                            minBytesPerSecond = currentBytesPerSecond;
                        }

                        maxBytesPerSecond = Math.Max(maxBytesPerSecond, currentBytesPerSecond);
                        minBytesPerSecond = Math.Min(minBytesPerSecond, currentBytesPerSecond);
                        avgBytesPerSecond = bytesPerSecond.Average();

                        bytesInLastInterval = 0;
                        copyWatch.Restart();
                    }

                    if (progressWatch.Elapsed > progressInterval || !isMoreToRead)
                    {
                        progress?.Report(new CopyStreamProgressInfo
                        {
                            BytesDownloaded = totalRead,
                            TotalBytes = totalBytes,
                            Percent = (totalRead * 1d) / (totalBytes * 1d) * 100,
                            Elapsed = totalWatch.Elapsed,
                            BufferSize = bufferSize,
                            CurrentBytesPerSecond = currentBytesPerSecond,
                            MaxBytesPerSecond = maxBytesPerSecond,
                            MinBytesPerSecond = minBytesPerSecond,
                            AvgBytesPerSecond = avgBytesPerSecond,
                        });

                        progressWatch.Restart();
                    }
                }

                if (!isMoreToRead)
                {
                    break;
                }
            }

            totalWatch.Stop();

            return targetStream;
        }
    }
}
