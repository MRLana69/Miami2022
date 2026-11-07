using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SYBO.Components.Crypt
{
	public class CounterModeCryptoTransform : ICryptoTransform, IDisposable
	{
		private readonly byte[] _counter;

		private readonly ICryptoTransform _counterEncryptor;

		private readonly Queue<byte> _xorMask = new Queue<byte>();

		private readonly SymmetricAlgorithm _symmetricAlgorithm;

		private byte[] _counterModeBlock;

		private readonly byte[] _empty = new byte[0];

		public int InputBlockSize
		{
			get
			{
				return 1;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return 1;
			}
		}

		public bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		public bool CanReuseTransform
		{
			get
			{
				return false;
			}
		}

		public CounterModeCryptoTransform(SymmetricAlgorithm symmetricAlgorithm, byte[] key, byte[] counter)
		{
			if (symmetricAlgorithm == null)
			{
				throw new ArgumentNullException("symmetricAlgorithm");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (counter == null)
			{
				throw new ArgumentNullException("counter");
			}
			if (counter.Length != symmetricAlgorithm.BlockSize / 8)
			{
				throw new ArgumentException(string.Format("Counter size must be same as block size (actual: {0}, expected: {1})", counter.Length, symmetricAlgorithm.BlockSize / 8));
			}
			_symmetricAlgorithm = symmetricAlgorithm;
			_counter = counter;
			byte[] rgbIV = new byte[_symmetricAlgorithm.BlockSize / 8];
			_counterEncryptor = symmetricAlgorithm.CreateEncryptor(key, rgbIV);
			_counterModeBlock = new byte[_symmetricAlgorithm.BlockSize / 8];
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (inputCount == 0)
			{
				return _empty;
			}
			byte[] array = new byte[inputCount];
			TransformBlock(inputBuffer, inputOffset, inputCount, array, 0);
			return array;
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			for (int i = 0; i < inputCount; i++)
			{
				if (NeedMoreXorMaskBytes())
				{
					EncryptCounterThenIncrement();
				}
				byte b = _xorMask.Dequeue();
				outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ b);
			}
			return inputCount;
		}

		private bool NeedMoreXorMaskBytes()
		{
			return _xorMask.Count == 0;
		}

		private void EncryptCounterThenIncrement()
		{
			_counterEncryptor.TransformBlock(_counter, 0, _counter.Length, _counterModeBlock, 0);
			IncrementCounter();
			byte[] counterModeBlock = _counterModeBlock;
			foreach (byte item in counterModeBlock)
			{
				_xorMask.Enqueue(item);
			}
		}

		private void IncrementCounter()
		{
			int num = _counter.Length - 1;
			while (num >= 0 && ++_counter[num] == 0)
			{
				num--;
			}
		}

		public void Seek(long distance)
		{
			while (distance-- > 0)
			{
				if (NeedMoreXorMaskBytes())
				{
					EncryptCounterThenIncrement();
				}
				_xorMask.Dequeue();
			}
		}

		public void Dispose()
		{
		}
	}
}
