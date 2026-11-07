using SYBO.Common.Utilities.Flags;
using UnityEngine;

namespace SYBO.Subway.Common
{
	public class AssetTag : MonoBehaviour
	{
		[EnumFlagProperty(typeof(AssetTagThemeUsage))]
		[SerializeField]
		private AssetTagThemeUsage _themeUsage;

		public AssetTagThemeUsage ThemeUsage
		{
			get
			{
				return _themeUsage;
			}
		}

		public bool HasTag(AssetTagThemeUsage themeTags)
		{
			return (ThemeUsage & themeTags) != 0;
		}

		public void AddTags(AssetTagThemeUsage themeTags)
		{
			_themeUsage |= themeTags;
		}

		public void ClearTags(AssetTagThemeUsage themeTags)
		{
			_themeUsage &= ~themeTags;
		}
	}
}
