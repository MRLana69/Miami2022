namespace SYBO.Common.Utilities
{
	public class UnsyncedMTRandomGen : IMTRandomGen, IRandomGen
	{
		private MTRandomGen _gen = new MTRandomGen(666u, 0);

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

		public uint NextUint()
		{
			SDebug.LogError("Retrieving Random value from unsynced random generator. Client game state diverged");
			return _gen.NextUint();
		}
	}
}
