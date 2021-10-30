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
        /// <param name="token">        (Optional) The token. </param>
        /// <param name="bufferSize">   (Optional) Size of the buffer. </param>
        ///
        /// <returns>   The download file. </returns>

        public static async Task<MemoryStream> DownloadFileAsync(
            this HttpClient client,
            Uri url,
            IProgress<CopyStreamProgressInfo>? progress = null,
            CancellationToken token = default,
            int bufferSize = StreamExtensions.DefaultBufferSize
        ) => (MemoryStream)await client.DownloadFileAsync(url, new MemoryStream(), progress, token, bufferSize).ConfigureAwait(false);

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
        /// <param name="token">            (Optional) The token. </param>
        /// <param name="bufferSize">       (Optional) Size of the buffer. </param>
        ///
        /// <returns>   The download file. </returns>

        public static async Task<Stream> DownloadFileAsync(
            this HttpClient client,
            Uri url,
            Stream streamToWrite,
            IProgress<CopyStreamProgressInfo>? progress = null,
            CancellationToken token = default,
            int bufferSize = StreamExtensions.DefaultBufferSize
        )
        {
            _ = client ?? throw new ArgumentNullException(nameof(client));
            _ = streamToWrite ?? throw new ArgumentNullException(nameof(streamToWrite));

            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);

            _ = await response.Content.ReadAsStringAsync().ConfigureAwait(false); //Read Headers, Body will be empty

            if (!response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
            }

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;

            if (totalBytes != -1L)
            {
                streamToWrite.SetLength(totalBytes);
            }

            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var resultStream = await stream.CopyToAsyncWithProgress(streamToWrite, progress, token, bufferSize, totalBytes).ConfigureAwait(false);

            if (resultStream.CanSeek)
            {
                resultStream.Position = 0;
            }

            return resultStream;
        }
    }
}
