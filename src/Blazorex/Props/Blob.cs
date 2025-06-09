using System;
using System.Buffers;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Blazorex;

public sealed class Blob : IDisposable, IAsyncDisposable
{
    private readonly IMemoryOwner<byte> _memoryOwner;
    private readonly ReadOnlyMemory<byte> _data;
    private bool _disposed;

    public string Type { get; }

    public string ObjectUrl { get; }

    public long Size => _data.Length;

    /// <summary>
    /// Creates a Blob from byte data with optional MIME type.
    /// </summary>
    public Blob(
        ReadOnlySpan<byte> data,
        string type = "application/octet-stream",
        string objectUrl = ""
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);

        _memoryOwner = MemoryPool<byte>.Shared.Rent(data.Length);
        data.CopyTo(_memoryOwner.Memory.Span);
        _data = _memoryOwner.Memory[..data.Length];
        Type = type;
        ObjectUrl = objectUrl;
    }

    /// <summary>
    /// Returns the blob data as a byte array.
    /// </summary>
    public byte[] ToArray()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _data.ToArray();
    }

    public string ToDataUrl()
    {
        // Base64 encode the data
        var base64 = Convert.ToBase64String(ToArray());

        // Compose the data URL
        return $"data:{Type};base64,{base64}";
    }

    /// <summary>
    /// Returns a stream for reading the blob data.
    /// </summary>
    public Stream Stream()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return new ReadOnlyMemoryStream(_data);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _memoryOwner?.Dispose();
            _disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_memoryOwner is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else
                _memoryOwner?.Dispose();

            _disposed = true;
        }
    }
}

/// <summary>
/// High-performance read-only memory stream implementation.
/// </summary>
internal sealed class ReadOnlyMemoryStream(ReadOnlyMemory<byte> memory) : Stream
{
    private int _position;

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => memory.Length;

    public override long Position
    {
        get => _position;
        set => _position = (int)Math.Clamp(value, 0, memory.Length);
    }

    public override int Read(byte[] buffer, int offset, int count) =>
        Read(buffer.AsSpan(offset, count));

    public override int Read(Span<byte> buffer)
    {
        var bytesToRead = Math.Min(buffer.Length, memory.Length - _position);
        if (bytesToRead <= 0)
            return 0;

        memory.Span.Slice(_position, bytesToRead).CopyTo(buffer);
        _position += bytesToRead;
        return bytesToRead;
    }

    public override async Task<int> ReadAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken
    ) => await ReadAsync(buffer.AsMemory(offset, count), cancellationToken);

    public override ValueTask<int> ReadAsync(
        Memory<byte> buffer,
        CancellationToken cancellationToken = default
    ) => ValueTask.FromResult(Read(buffer.Span));

    public override long Seek(long offset, SeekOrigin origin) =>
        Position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => Position + offset,
            SeekOrigin.End => Length + offset,
            _ => throw new ArgumentException("Invalid seek origin", nameof(origin))
        };

    public override void Flush() { }

    public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException();
}

/// <summary>
/// Data transfer object for blob data from JavaScript.
/// </summary>
internal sealed record BlobData
{
    [JsonPropertyName("data")]
    public byte[] Data { get; init; } = [];

    [JsonPropertyName("type")]
    public string Type { get; init; } = "application/octet-stream";

    [JsonPropertyName("objectUrl")]
    public string ObjectUrl { get; init; } = string.Empty;

    [JsonPropertyName("size")]
    public long Size { get; init; }

    /// <summary>
    /// Converts to the Blob.
    /// </summary>
    public Blob ToBlob() => new(Data, Type, ObjectUrl);
}
