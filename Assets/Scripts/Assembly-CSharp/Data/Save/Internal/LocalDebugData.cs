namespace Data.Save.Internal
{
	public class LocalDebugData : DiskData
	{
		public int overridePerformanceLevel = -1;

		public bool disableAdSystem;

		public bool useLocalTimeInsteadOfServerTime;

		public override void Obfuscate()
		{
		}

		public override void Deobfuscate()
		{
		}
	}
}
