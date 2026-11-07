using UnityEngine;

namespace SYBO.Subway.Level
{
	public class SpawnSettings : MonoBehaviour
	{
		[SerializeField]
		public bool DontRepeatInMultiplePlaceholders;

		[SerializeField]
		public EnvironmentKind EnvironmentKindInfo;
	}
}
