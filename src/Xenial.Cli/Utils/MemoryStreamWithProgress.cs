using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spectre.Console;

namespace Xenial.Cli.Utils;

public class MemoryStreamWithProgress : MemoryStream
{
    private readonly ProgressTask progress;

    public MemoryStreamWithProgress(ProgressTask progress)
        => this.progress = progress.IsIndeterminate(true);

    /// <summary>Initializes a new non-resizable instance of the <see cref="MemoryStreamWithProgress" /> class based on the specified byte array.</summary>
    /// <param name="buffer">The array of unsigned bytes from which to create the current stream.</param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="buffer" /> is <see langword="null" />.</exception>
    public MemoryStreamWithProgress(ProgressTask progress, byte[] buffer) : base(buffer)
        => this.progress = progress.IsIndeterminate(true);

    /// <summary>Initializes a new non-resizable instance of the <see cref="MemoryStreamWithProgress" /> class based on the specified byte array with the <see cref="System.IO.MemoryStream.CanWrite" /> property set as specified.</summary>
    /// <param name="buffer">The array of unsigned bytes from which to create this stream.</param>
    /// <param name="writable">The setting of the <see cref="System.IO.MemoryStream.CanWrite" /> property, which determines whether the stream supports writing.</param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="buffer" /> is <see langword="null" />.</exception>
    public MemoryStreamWithProgress(ProgressTask progress, byte[] buffer, bool writable) : base(buffer, writable)
        => this.progress = progress.IsIndeterminate(true);

    /// <summary>Initializes a new non-resizable instance of the <see cref="MemoryStreamWithProgress" /> class based on the specified region (index) of a byte array.</summary>
    /// <param name="buffer">The array of unsigned bytes from which to create this stream.</param>
    /// <param name="index">The index into <paramref name="buffer" /> at which the stream begins.</param>
    /// <param name="count">The length of the stream in bytes.</param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="buffer" /> is <see langword="null" />.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> or <paramref name="count" /> is less than zero.</exception>
    /// <exception cref="System.ArgumentException">The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
    public MemoryStreamWithProgress(ProgressTask progress, byte[] buffer, int index, int count) : base(buffer, index, count)
    {
        this.progress = progress;
        StartProgress(count);
    }

    private void StartProgress(int count)
    {
        if (count > 0)
        {
            progress.IsIndeterminate(false);
            progress.MaxValue(count).StartTask();
        }
        else
        {
            progress.StopTask();
        }
    }

    /// <summary>Initializes a new non-resizable instance of the <see cref="MemoryStreamWithProgress" /> class based on the specified region of a byte array, with the <see cref="System.IO.MemoryStream.CanWrite" /> property set as specified.</summary>
    /// <param name="buffer">The array of unsigned bytes from which to create this stream.</param>
    /// <param name="index">The index in <paramref name="buffer" /> at which the stream begins.</param>
    /// <param name="count">The length of the stream in bytes.</param>
    /// <param name="writable">The setting of the <see cref="System.IO.MemoryStream.CanWrite" /> property, which determines whether the stream supports writing.</param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="buffer" /> is <see langword="null" />.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> or <paramref name="count" /> are negative.</exception>
    /// <exception cref="System.ArgumentException">The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
    public MemoryStreamWithProgress(ProgressTask progress, byte[] buffer, int index, int count, bool writable) : base(buffer, index, count, writable)
    {
        this.progress = progress;
        StartProgress(count);
    }

    /// <summary>Initializes a new instance of the <see cref="MemoryStreamWithProgress" /> class based on the specified region of a byte array, with the <see cref="System.IO.MemoryStream.CanWrite" /> property set as specified, and the ability to call <see cref="System.IO.MemoryStream.GetBuffer" /> set as specified.</summary>
    /// <param name="buffer">The array of unsigned bytes from which to create this stream.</param>
    /// <param name="index">The index into <paramref name="buffer" /> at which the stream begins.</param>
    /// <param name="count">The length of the stream in bytes.</param>
    /// <param name="writable">The setting of the <see cref="System.IO.MemoryStream.CanWrite" /> property, which determines whether the stream supports writing.</param>
    /// <param name="publiclyVisible">
    /// <see langword="true" /> to enable <see cref="System.IO.MemoryStream.GetBuffer" />, which returns the unsigned byte array from which the stream was created; otherwise, <see langword="false" />.</param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="buffer" /> is <see langword="null" />.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> or <paramref name="count" /> is negative.</exception>
    /// <exception cref="System.ArgumentException">The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
    public MemoryStreamWithProgress(ProgressTask progress, byte[] buffer, int index, int count, bool writable, bool publiclyVisible) : base(buffer, index, count, writable, publiclyVisible)
    {
        this.progress = progress;
        StartProgress(count);
    }

    /// <summary>Initializes a new instance of the <see cref="MemoryStreamWithProgress" /> class with an expandable capacity initialized as specified.</summary>
    /// <param name="capacity">The initial size of the internal array in bytes.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="capacity" /> is negative.</exception>
    public MemoryStreamWithProgress(ProgressTask progress, int capacity) : base(capacity)
    {
        this.progress = progress;
        StartProgress(capacity);
    }

    public override void SetLength(long value)
    {
        base.SetLength(value);
        StartProgress(GuardBufferOverflow(value));
    }

    public static int GuardBufferOverflow(long value)
        //0x7FFFFFC7 max array size
        => value > int.MaxValue || value > 0x7FFFFFC7 ? 0x7FFFFFC7 : Convert.ToInt32(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (!progress.IsIndeterminate)
        {
            progress.Increment(count);
        }
        base.Write(buffer, offset, count);
    }
}
