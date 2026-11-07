using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class SectionAttribute : MetaAttribute
	{
		private string sectionLabel;

		public string SectionLabel
		{
			get
			{
				return sectionLabel;
			}
		}

		public SectionAttribute(string sectionLabel)
		{
			this.sectionLabel = sectionLabel;
		}
	}
}
