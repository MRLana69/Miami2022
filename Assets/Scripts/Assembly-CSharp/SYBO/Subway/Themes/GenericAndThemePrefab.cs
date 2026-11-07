using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SYBO.Subway.Themes
{
	[Serializable]
	public struct GenericAndThemePrefab
	{
		public const string FieldPatternGenericPrefab = "GenericPrefab";

		[FormerlySerializedAs("genericPrefab")]
		public GameObject GenericPrefab;

		public const string FieldPatternThemePrefab = "ThemePrefab";

		[FormerlySerializedAs("themePrefab")]
		public GameObject ThemePrefab;
	}
}
