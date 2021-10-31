using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Scissors.Utils.Io;

namespace Xenial.Framework.WebView.Win.Helpers
{
    /// <summary>   Provides HttpClientExtensions for the System.Net.Http.HttpClient. </summary>
    public static class HttpClientExtensions
    {
        /// <summary>   Downloads the file asynchronous and returns a MemoryStream. </summary>
        ///
        /// <param name="client">       The client. </param>
        /// <param name="url">          The URL. </param>
        /// <param name="progress">     (Optional) The progress. </param>
        /// <param name="cancellationToken">        (Optional) The token. </param>
        /// <param name="bufferSize">   (Optional) Size of the buffer. </param>
        ///
        /// <returns>   The download file. </returns>

        public static async Task<MemoryStream> DownloadFileAsync(
            this HttpClient client,
            Uri url,
            IProgress<CopyStreamProgressInfo>? progress = null,
            int bufferSize = StreamExtensions.DefaultBufferSize,
            CancellationToken cancellationToken = default
        ) => (MemoryStream)await client.DownloadFileAsync(url, new MemoryStream(), progress, bufferSize, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Downloads the file asynchronous and returns the given Stream. Rewinds the stream if it is
        /// seekable.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="client">           The client. </param>
        /// <param name="url">              The URL. </param>
        /// <param name="streamToWrite">    The stream to write. </param>
        /// <param name="progress">         (Optional) The progress. </param>
        /// <param name="cancellationToken">            (Optional) The token. </param>
        /// <param name="bufferSize">       (Optional) Size of the buffer. </param>
        ///
        /// <returns>   The download file. </returns>

        public static async Task<Stream> DownloadFileAsync(
            this HttpClient client,
            Uri url,
            Stream streamToWrite,
            IProgress<CopyStreamProgressInfo>? progress = null,
            int bufferSize = StreamExtensions.DefaultBufferSize,
            CancellationToken cancellationToken = default
        )
        {
            _ = client ?? throw new ArgumentNullException(nameof(client));
            _ = streamToWrite ?? throw new ArgumentNullException(nameof(streamToWrite));

            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
#if NET5_0_OR_GREATER
            _ = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false); //Read Headers, Body will be empty
#else
            _ = await response.Content.ReadAsStringAsync().ConfigureAwait(false); //Read Headers, Body will be empty
#endif
            if (!response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
            }

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;

            if (totalBytes != -1L)
            {
                streamToWrite.SetLength(totalBytes);
            }
#if NET5_0_OR_GREATER
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#else
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif

            var resultStream = await stream.CopyToAsyncWithProgress(streamToWrite, progress, bufferSize, totalBytes, cancellationToken).ConfigureAwait(false);

            if (resultStream.CanSeek)
            {
                resultStream.Position = 0;
            }

            return resultStream;
        }
    }
}
