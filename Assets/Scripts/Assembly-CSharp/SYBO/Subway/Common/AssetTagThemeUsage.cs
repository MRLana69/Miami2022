using System;

namespace SYBO.Subway.Common
{
	[Flags]
	public enum AssetTagThemeUsage
	{
		None = 0,
		DontThemeMaterials = 1,
		DontThemeMesh = 2,
		MeshCanBeMissing = 4
	}
}
