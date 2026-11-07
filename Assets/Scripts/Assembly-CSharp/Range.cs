public abstract class Range<T>
{
	public T max { get; set; }

	public T min { get; set; }

	protected Range(T min, T max)
	{
		this.max = max;
		this.min = min;
	}

	public abstract bool IsValueWithinRange(T value);

	public override string ToString()
	{
		return string.Concat(min, " - ", max);
	}
}
