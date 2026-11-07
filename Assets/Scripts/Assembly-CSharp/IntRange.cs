public class IntRange : Range<int>
{
	public IntRange(int min, int max)
		: base(min, max)
	{
	}

	public override bool IsValueWithinRange(int value)
	{
		return value > base.min && value <= base.max;
	}
}
