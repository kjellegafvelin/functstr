
namespace funcstr.triggers.http
{

    internal sealed class HttpRequestStream : Stream
    {
        public HttpRequestStream(Stream stream)
        {
            InternalStream = stream;
        }

        public override bool CanSeek => false;

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int WriteTimeout
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        internal Stream InternalStream { get; }

        public override int Read(Span<byte> buffer)
        {
            throw new InvalidOperationException("Synchronous operations are disallowed.Call ReadAsync or set AllowSynchronousIO to true instead.");
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            throw new InvalidOperationException("Synchronous operations are disallowed.Call ReadAsync or set AllowSynchronousIO to true instead.");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //if (!_bodyControl.AllowSynchronousIO)
            //{
            throw new InvalidOperationException("Synchronous operations are disallowed.Call ReadAsync or set AllowSynchronousIO to true instead.");
            //}

            //return ReadAsync(buffer, offset, count).GetAwaiter().GetResult();
        }

        public override int ReadByte()
        {
            throw new InvalidOperationException("Synchronous operations are disallowed.Call ReadAsync or set AllowSynchronousIO to true instead.");
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return this.InternalStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return this.InternalStream.ReadAsync(buffer, cancellationToken);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}