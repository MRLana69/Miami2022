namespace SYBO.Common.Utilities
{
	public class LoggingRandomGen : IMTRandomGen, IRandomGen
	{
		private IMTRandomGen _gen;

		public uint Seed
		{
			get
			{
				return _gen.Seed;
			}
		}

		public int SeedIncrement
		{
			get
			{
				return _gen.SeedIncrement;
			}
		}

		public LoggingRandomGen(IMTRandomGen gen)
		{
			_gen = gen;
		}

		public uint NextUint()
		{
			return _gen.NextUint();
		}
	}
}
