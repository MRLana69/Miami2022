using UnityEngine;

namespace Kiloo
{
	public class RandomTimeDelay : BaseTimeDelay
	{
		[SerializeField]
		private float _maxTimeDelay;

		[SerializeField]
		private float _minTimeDelay;

		public override float delay
		{
			get
			{
				return Random.Range(_minTimeDelay, _maxTimeDelay);
			}
		}
	}
}
