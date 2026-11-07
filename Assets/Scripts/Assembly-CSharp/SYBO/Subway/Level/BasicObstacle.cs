using SYBO.Common.Utilities.Flags;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[RequireComponent(typeof(SnapObject))]
	public class BasicObstacle : MonoBehaviour
	{
		[EnumFlagProperty(typeof(BasicObstacleType))]
		[SerializeField]
		private BasicObstacleType _obstacleType;

		[SerializeField]
		private bool _withCoins;

		private void OnValidate()
		{
			if (_obstacleType == BasicObstacleType.LightSignal)
			{
				_withCoins = false;
			}
		}
	}
}
