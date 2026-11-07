using System;
using System.Diagnostics;
using System.Threading;

namespace SYBO.Components.Analytics
{
	public class MTRandomGen : IMTRandomGen, IRandomGen
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<int> OnRandomIncrementChanged__BackingField;

		private const int N = 624;

		private const int M = 397;

		private const uint UPPER_MASK = 2147483648u;

		private const uint LOWER_MASK = 2147483647u;

		private static readonly uint[] MAG01 = new uint[2] { 0u, 2567483615u };

		private uint[] _mt;

		private int _mti;

		private uint _initialSeed;

		private int _randomIncrement;

		public uint Seed
		{
			get
			{
				return _initialSeed;
			}
		}

		public int SeedIncrement
		{
			get
			{
				return _randomIncrement;
			}
		}

		public event Action<int> OnRandomIncrementChanged
		{
			add
			{
				Action<int> action = OnRandomIncrementChanged__BackingField;
				Action<int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRandomIncrementChanged__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<int> action = OnRandomIncrementChanged__BackingField;
				Action<int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRandomIncrementChanged__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public MTRandomGen(uint seed, int increment)
		{
			if (seed == 0)
			{
				seed = 1337u;
			}
			_initialSeed = seed;
			_mt = new uint[624];
			_mt[0] = seed;
			for (_mti = 1; _mti < 624; _mti++)
			{
				_mt[_mti] = (uint)((69069L * (long)_mt[_mti - 1]) & 0xFFFFFFFFu);
			}
			Twist();
			int num;
			for (num = increment; num >= 624; num -= 624)
			{
				Twist();
			}
			_mti = num;
			_randomIncrement = increment;
		}

		private void Twist()
		{
			int i;
			uint num;
			for (i = 0; i < 227; i++)
			{
				num = (_mt[i] & 0x80000000u) | (_mt[i + 1] & 0x7FFFFFFF);
				_mt[i] = _mt[i + 397] ^ (num >> 1) ^ MAG01[num & 1];
			}
			for (; i < 623; i++)
			{
				num = (_mt[i] & 0x80000000u) | (_mt[i + 1] & 0x7FFFFFFF);
				_mt[i] = _mt[i + -227] ^ (num >> 1) ^ MAG01[num & 1];
			}
			num = (_mt[i] & 0x80000000u) | (_mt[0] & 0x7FFFFFFF);
			_mt[623] = _mt[396] ^ (num >> 1) ^ MAG01[num & 1];
		}

		public uint NextUint()
		{
			if (_mti >= 624)
			{
				Twist();
				_mti = 0;
			}
			uint num = _mt[_mti++];
			num ^= num >> 11;
			num ^= (num << 7) & 0x9D2C5680u;
			num ^= (num << 15) & 0xEFC60000u;
			num ^= num >> 18;
			_randomIncrement++;
			if (OnRandomIncrementChanged__BackingField != null)
			{
				OnRandomIncrementChanged__BackingField(_randomIncrement);
			}
			return num;
		}
	}
}
