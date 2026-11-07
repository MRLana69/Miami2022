using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	[RequireComponent(typeof(BoxCollider))]
	public class MovingCarDirectionChange : MonoBehaviour
	{
		[SerializeField]
		private Transform _targetDirection;

		public Transform TargetDirection
		{
			get
			{
				return _targetDirection;
			}
		}
	}
}
