namespace SYBO.Common.Utilities
{
	public interface IMTRandomGen : IRandomGen
	{
		uint Seed { get; }

		int SeedIncrement { get; }
	}
}
