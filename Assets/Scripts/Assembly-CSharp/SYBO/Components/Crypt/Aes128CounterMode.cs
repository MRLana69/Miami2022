using System;
using System.Security.Cryptography;

namespace SYBO.Components.Crypt
{
	public class Aes128CounterMode : SymmetricAlgorithm
	{
		private readonly byte[] _counter;

		private readonly AesManaged _aes;

		public override byte[] Key
		{
			get
			{
				return _aes.Key;
			}
			set
			{
				_aes.Key = value;
			}
		}

		public Aes128CounterMode(byte[] counter)
		{
			if (counter == null)
			{
				throw new ArgumentNullException("counter");
			}
			if (counter.Length != 16)
			{
				throw new ArgumentException(string.Format("Counter size must be same as block size (actual: {0}, expected: {1})", counter.Length, 16));
			}
			_aes = new AesManaged
			{
				Mode = CipherMode.ECB,
				Padding = PaddingMode.None
			};
			_counter = counter;
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] ignoredParameter)
		{
			return new CounterModeCryptoTransform(_aes, rgbKey, _counter);
		}

		public ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] ignoredParameter, int position)
		{
			CounterModeCryptoTransform counterModeCryptoTransform = new CounterModeCryptoTransform(_aes, rgbKey, _counter);
			counterModeCryptoTransform.Seek(position);
			return counterModeCryptoTransform;
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] ignoredParameter)
		{
			return new CounterModeCryptoTransform(_aes, rgbKey, _counter);
		}

		public override void GenerateKey()
		{
			_aes.GenerateKey();
		}

		public override void GenerateIV()
		{
		}
	}
}
