using System;
using System.IO;
using System.Security.Cryptography;

namespace SYBO.Components.Crypt
{
	public class EncryptedCTRStream : Stream, IDisposable
	{
		private const int KeySize = 16;

		private const int HeaderSize = 32;

		private byte[] _counter = new byte[16];

		private byte[] _key = new byte[16];

		private AesManaged _aes = new AesManaged
		{
			Mode = CipherMode.ECB,
			Padding = PaddingMode.None
		};

		private Stream _stream;

		private CryptoStream _cryptoStream;

		public override bool CanRead
		{
			get
			{
				return _cryptoStream.CanRead;
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
				return _cryptoStream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return _stream.Length - 32;
			}
		}

		public override long Position
		{
			get
			{
				return _stream.Position - 32;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		~EncryptedCTRStream()
		{
			Dispose();
		}

		public static EncryptedCTRStream WriteStream(Stream stream)
		{
			EncryptedCTRStream encryptedCTRStream = new EncryptedCTRStream();
			encryptedCTRStream.SetupWrite(stream);
			return encryptedCTRStream;
		}

		public static EncryptedCTRStream ReadStream(Stream stream)
		{
			EncryptedCTRStream encryptedCTRStream = new EncryptedCTRStream();
			encryptedCTRStream.SetupRead(stream);
			return encryptedCTRStream;
		}

		private void SetupWrite(Stream stream)
		{
			_stream = stream;
			if (_stream.Length >= 32)
			{
				_stream.Read(_counter, 0, 16);
				_stream.Read(_key, 0, 16);
				_stream.Seek(_stream.Length, SeekOrigin.Begin);
			}
			else
			{
				_aes.GenerateKey();
				Array.Copy(_aes.Key, _counter, 16);
				Array.Copy(_aes.Key, 16, _key, 0, 16);
				_stream.Write(_counter, 0, 16);
				_stream.Write(_key, 0, 16);
			}
			CounterModeCryptoTransform counterModeCryptoTransform = new CounterModeCryptoTransform(_aes, _key, _counter);
			counterModeCryptoTransform.Seek(Position);
			_cryptoStream = new CryptoStream(_stream, counterModeCryptoTransform, CryptoStreamMode.Write);
		}

		private void SetupRead(Stream stream)
		{
			_stream = stream;
			_stream.Read(_counter, 0, 16);
			_stream.Read(_key, 0, 16);
			CounterModeCryptoTransform transform = new CounterModeCryptoTransform(_aes, _key, _counter);
			_cryptoStream = new CryptoStream(_stream, transform, CryptoStreamMode.Read);
		}

		public new void Dispose()
		{
			if (_stream != null)
			{
				base.Dispose();
				_cryptoStream.Dispose();
				_stream.Dispose();
				_cryptoStream = null;
				_stream = null;
				GC.SuppressFinalize(this);
			}
		}

		public override void Flush()
		{
			_cryptoStream.Flush();
			_stream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _cryptoStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_cryptoStream.Write(buffer, offset, count);
		}
	}
}
