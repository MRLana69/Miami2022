using System;
using System.IO;
using System.Security.Cryptography;

namespace Kiloo.Common.Utility
{
	public class SHA1Stream : Stream
	{
		public enum Mode
		{
			Read = 0,
			Write = 1
		}

		public const int HASH_SIZE = 20;

		private const int BUFFER_SIZE = 128;

		private readonly int _bufferSize;

		private readonly Stream _stream;

		private readonly SHA1 _sha1;

		private readonly bool _canRead;

		private readonly bool _canWrite;

		private byte[] _buffer;

		private int _bufferOffset;

		private bool _hashFinalized;

		private bool _isDisposed;

		public int HashSize
		{
			get
			{
				if (_isDisposed)
				{
					throw new ObjectDisposedException("Object is disposed.");
				}
				return _sha1.HashSize;
			}
		}

		public byte[] Hash
		{
			get
			{
				if (_isDisposed)
				{
					throw new ObjectDisposedException("Object is disposed.");
				}
				if (!_hashFinalized)
				{
					throw new NotSupportedException("Hash not finalized.");
				}
				return _sha1.Hash;
			}
		}

		private int availableBuffer
		{
			get
			{
				return _buffer.Length - _bufferOffset;
			}
		}

		public override bool CanRead
		{
			get
			{
				return !_isDisposed && _canRead && _stream.CanRead;
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
				return !_isDisposed && _canWrite && _stream.CanWrite;
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

		public SHA1Stream(Stream stream, Mode mode, byte[] saltBytes, int bufferSize = 128)
		{
			switch (mode)
			{
			case Mode.Read:
				if (!stream.CanRead)
				{
					throw new ArgumentException("Stream is not readable.", "stream");
				}
				_canRead = true;
				break;
			case Mode.Write:
				if (!stream.CanWrite)
				{
					throw new ArgumentException("Stream is not writable", "stream");
				}
				_canWrite = true;
				break;
			default:
				throw new ArgumentException("Invalid value", "mode");
			}
			_bufferSize = bufferSize;
			_stream = stream;
			_sha1 = new SHA1Managed();
			if (saltBytes != null)
			{
				_sha1.TransformBlock(saltBytes, 0, saltBytes.Length, saltBytes, 0);
			}
		}

		public void FinalizeHash()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("Object is disposed.");
			}
			if (_hashFinalized)
			{
				throw new NotSupportedException("Hash already finalized.");
			}
			if (_buffer != null)
			{
				_sha1.TransformFinalBlock(_buffer, 0, _bufferOffset);
			}
			else
			{
				_sha1.TransformFinalBlock(new byte[0], 0, 0);
			}
			_hashFinalized = true;
			_buffer = null;
		}

		public override void Flush()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("Object is disposed.");
			}
			_stream.Flush();
		}

		private void EnsureBuffer()
		{
			if (_buffer == null)
			{
				_buffer = new byte[_bufferSize];
				_bufferOffset = 0;
			}
		}

		private void FlushBuffer()
		{
			if (_buffer != null && _bufferOffset > 0)
			{
				_sha1.TransformBlock(_buffer, 0, _bufferOffset, _buffer, 0);
				_bufferOffset = 0;
			}
		}

		private void TransformBytes(byte[] buffer, int offset, int count)
		{
			if (!_hashFinalized)
			{
				if (_buffer != null && availableBuffer < count)
				{
					FlushBuffer();
				}
				if (count > _bufferSize)
				{
					_sha1.TransformBlock(buffer, offset, count, buffer, offset);
					return;
				}
				EnsureBuffer();
				Buffer.BlockCopy(buffer, offset, _buffer, _bufferOffset, count);
				_bufferOffset += count;
			}
		}

		private void TransformByte(byte value)
		{
			if (!_hashFinalized)
			{
				if (_buffer != null && availableBuffer < 1)
				{
					FlushBuffer();
				}
				EnsureBuffer();
				_buffer[_bufferOffset] = value;
				_bufferOffset++;
			}
		}

		public override int ReadByte()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("Object is disposed.");
			}
			if (!_canRead)
			{
				throw new NotSupportedException("Stream is not readable.");
			}
			int num = _stream.ReadByte();
			if (num != -1)
			{
				TransformByte((byte)num);
			}
			return num;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("Object is disposed.");
			}
			if (!_canRead)
			{
				throw new NotSupportedException("Stream is not readable.");
			}
			int num = _stream.Read(buffer, offset, count);
			TransformBytes(buffer, offset, num);
			return num;
		}

		public override void WriteByte(byte value)
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("Object is disposed.");
			}
			if (!_canWrite)
			{
				throw new NotSupportedException("Stream is not writable.");
			}
			_stream.WriteByte(value);
			TransformByte(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("Object is disposed.");
			}
			if (!_canWrite)
			{
				throw new NotSupportedException("Stream is not writable.");
			}
			_stream.Write(buffer, offset, count);
			TransformBytes(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				((IDisposable)_sha1).Dispose();
				_isDisposed = true;
			}
			base.Dispose(disposing);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seeking not supported.");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("Seeking not supported.");
		}
	}
}
