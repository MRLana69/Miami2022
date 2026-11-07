using System.Text;

namespace SYBO.Components.Analytics
{
	public static class MTRandomGenTest
	{
		public static string TestIncrement()
		{
			MTRandomGen mTRandomGen = new MTRandomGen(123554u, 0);
			for (int i = 0; i < 10000; i++)
			{
				mTRandomGen.NextUint();
			}
			MTRandomGen mTRandomGen2 = new MTRandomGen(mTRandomGen.Seed, mTRandomGen.SeedIncrement);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			for (int j = 0; j < 100; j++)
			{
				uint num = mTRandomGen.NextUint();
				uint num2 = mTRandomGen2.NextUint();
				if (num != num2)
				{
					flag = true;
					stringBuilder.Append(" Increment: " + (j + 10000) + " " + num + "/" + num2 + " FAIL");
					stringBuilder.AppendLine();
				}
			}
			if (flag)
			{
				return "MTRandomGenTest.TestIncrement: FAILED\n" + stringBuilder.ToString();
			}
			return "MTRandomGenTest.TestIncrement: SUCCESS";
		}
	}
}
