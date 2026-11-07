using UnityEngine;

namespace SYBO.Subway.Routes
{
	public class RouteScoreChangeExplosion : MonoBehaviour
	{
		public static string PREFAB_RESOURCE_PATH = "Prefabs/Routes/RouteScoreChangeExplosion";

		public static RouteScoreChangeExplosion CreateAtPosition(Vector3 positionVector3)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load(PREFAB_RESOURCE_PATH)) as GameObject;
			gameObject.transform.position = positionVector3;
			Object.Destroy(gameObject, 0.1f);
			return gameObject.GetComponent<RouteScoreChangeExplosion>();
		}
	}
}
