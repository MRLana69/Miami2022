using System;
using System.IO;
using System.Text;

namespace Kiloo.Common.Utility
{
	public class UnbufferedStreamReader : TextReader
	{
		private readonly BinaryReader _reader;

		private readonly bool _readerCanPeek;

		private bool _hasPeeked;

		private int _peekedCharacter;

		public UnbufferedStreamReader(Stream inputStream)
			: this(inputStream, new UTF8Encoding())
		{
		}

		public UnbufferedStreamReader(Stream inputStream, Encoding encoding)
		{
			_reader = new BinaryReader(inputStream, encoding);
			_readerCanPeek = inputStream.CanSeek;
		}

		public override int Peek()
		{
			if (_readerCanPeek)
			{
				return _reader.PeekChar();
			}
			if (!_hasPeeked)
			{
				try
				{
					_peekedCharacter = _reader.ReadChar();
				}
				catch (EndOfStreamException)
				{
					_peekedCharacter = -1;
				}
				_hasPeeked = true;
			}
			return _peekedCharacter;
		}

		public override int Read()
		{
			if (_hasPeeked)
			{
				_hasPeeked = false;
				return _peekedCharacter;
			}
			try
			{
				return _reader.ReadChar();
			}
			catch (EndOfStreamException)
			{
				return -1;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				((IDisposable)_reader).Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
