namespace SYBO.Components.Analytics
{
	public interface IMTRandomGen : IRandomGen
	{
		uint Seed { get; }

		int SeedIncrement { get; }
	}
}
