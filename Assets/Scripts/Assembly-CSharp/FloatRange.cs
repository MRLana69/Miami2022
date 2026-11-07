public class FloatRange : Range<float>
{
	public FloatRange(float min, float max)
		: base(min, max)
	{
	}

	public override bool IsValueWithinRange(float value)
	{
		return value > base.min && value <= base.max;
	}
}
