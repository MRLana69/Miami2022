using System.Reflection;
using SYBO.Components.Logging;

namespace SYBO.Components.Utilities
{
	public static class AssetVerificationHelper
	{
		public static void VerifyAssetFields<T>()
		{
			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.GetValue(null) == null)
				{
					Log.Utilities.LogErrorFormat("Field {0} was found to be null", fieldInfo.Name);
				}
			}
		}
	}
}
