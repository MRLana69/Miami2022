using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SYBO.Subway.Themes
{
	[Serializable]
	public struct GenericAndThemePrefabGroup
	{
		public const string FieldPatternName = "Name";

		[HideInInspector]
		public string Name;

		public const string FieldPatternPrefabMappings = "PrefabMappings";

		[FormerlySerializedAs("prefabMappings")]
		public GenericAndThemePrefab[] PrefabMappings;
	}
}
