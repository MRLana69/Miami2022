using UnityEngine;

namespace SYBO.Components.Utilities.Attributes
{
	public class SectionHeaderAttribute : PropertyAttribute
	{
		public const int SizeH1 = 18;

		public const int SizeH2 = 16;

		public const int SizeH3 = 12;

		public const int SizeDefault = 12;

		public string Text;

		public int Size;

		public SectionHeaderAttribute(string text, int size)
		{
			Text = text;
			Size = size;
		}

		public SectionHeaderAttribute(string text)
		{
			Text = text;
			Size = 12;
		}
	}
}
