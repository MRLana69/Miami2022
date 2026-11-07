using System;
using System.IO;

namespace Kiloo.Common.Utility
{
	public class BoundedInputStream : Stream
	{
		private readonly Stream _inner;

		private readonly int _length;

		public int NumberOfBytesRead { get; private set; }

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("Seeking not supported.");
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException("Seeking not supported.");
			}
			set
			{
				throw new NotSupportedException("Seeking not supported.");
			}
		}

		public BoundedInputStream(Stream stream, int length)
		{
			if (!stream.CanRead)
			{
				throw new ArgumentException("Stream is not readable.", "stream");
			}
			_inner = stream;
			_length = length;
			NumberOfBytesRead = 0;
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Value cannot be negative.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Buffer too small for the given offset and count.", "count");
			}
			int num = _length - NumberOfBytesRead;
			if (count > num)
			{
				count = num;
			}
			int num2 = _inner.Read(buffer, offset, count);
			NumberOfBytesRead += num2;
			return num2;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seeking not supported.");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("Writing not supported.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("Writing not supported.");
		}
	}
}
