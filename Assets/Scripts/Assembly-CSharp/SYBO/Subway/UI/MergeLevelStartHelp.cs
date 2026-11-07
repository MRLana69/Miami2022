using UnityEngine;
using UnityEngine.SceneManagement;

namespace SYBO.Subway.UI
{
	public class MergeLevelStartHelp : MonoBehaviour
	{
		public static string MERGE_SCENE_NAME_FOR_BUILDS = "Merge";

		public static string GUI_SCENE_NAME = "GUI";

		public static string MERGE_SCENE_NAME_SUBSTRING = "Merge";

		public static bool IsValidLoadedLevelNameForGameplay()
		{
			return SceneManager.GetActiveScene().name.ToLower().Contains(MERGE_SCENE_NAME_SUBSTRING.ToLower());
		}
	}
}
