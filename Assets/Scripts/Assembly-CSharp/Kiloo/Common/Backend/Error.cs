using System;
using System.Collections.Generic;

namespace Kiloo.Common.Backend
{
	[Serializable]
	public class Error
	{
		public string category;

		public int code;

		public string message;

		public string exception;

		public Dictionary<string, string> info;

		public override string ToString()
		{
			if (exception != null)
			{
				return string.Format("{0} error {1}: {2} ({3})", category, code, message, exception);
			}
			return string.Format("{0} error {1}: {2}", category, code, message);
		}

		public ErrorCategory GetErrorCategory()
		{
			return GetErrorCategory(category);
		}

		public static ErrorCategory GetErrorCategory(string category)
		{
			try
			{
				return (ErrorCategory)Enum.Parse(typeof(ErrorCategory), category, true);
			}
			catch (Exception)
			{
				return ErrorCategory.Unknown;
			}
		}
	}
}
