using System.Collections.Generic;

namespace ABTest
{
	public class ABTestData
	{
		public class ABTestDataItem
		{
			public bool forceDisable;

			public string identifier;

			public string defaultPayload;

			public TestRange testRange;

			public string debugTitle;

			public List<string> debugPayload;
		}

		public const string ABTEST_SLOWER_REVIVE_TIMER = "slowrevivetimer";

		public const string ABTEST_INFINITE_REVIVE = "infiniterevive";

		public const string ABTEST_STORE_PRICE_TIER = "storepricetier";

		public static readonly List<ABTestDataItem> testData = new List<ABTestDataItem>
		{
			new ABTestDataItem
			{
				identifier = "slowrevivetimer",
				defaultPayload = "4",
				testRange = TestRange.Kiloo
			},
			new ABTestDataItem
			{
				identifier = "infiniterevive",
				defaultPayload = "D_3",
				testRange = TestRange.Kiloo,
				debugTitle = "Infinite Revive A/B Test",
				debugPayload = new List<string> { "A_50", "B_50", "C_2", "D_2" }
			},
			new ABTestDataItem
			{
				identifier = "storepricetier",
				defaultPayload = "C",
				testRange = TestRange.Kiloo,
				debugTitle = "A/B Test Price Tier",
				debugPayload = new List<string> { "A", "B", "C" }
			}
		};
	}
}
