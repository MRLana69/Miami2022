using Kiloo.Common.Extensions;

public static class Bucket
{
	public static string BucketThis(float valueToBucket, int[] buckets)
	{
		return BucketThis(valueToBucket.RoundToInt(), buckets);
	}

	public static string BucketThis(int valueToBucket, int[] buckets)
	{
		char c = '-';
		if (buckets.Length < 2)
		{
			SDebug.LogError("Can't bucket value with this few buckets.");
			return string.Empty;
		}
		string text = string.Empty;
		string text2 = string.Empty;
		for (int i = 0; i < buckets.Length; i++)
		{
			int num = buckets[i];
			if (valueToBucket >= num)
			{
				text = num.ToString();
				continue;
			}
			text2 = num.ToString();
			break;
		}
		if (text.IsNullOrEmpty())
		{
			c = '<';
		}
		if (text2.IsNullOrEmpty())
		{
			c = '>';
		}
		return text + c + text2;
	}
}
