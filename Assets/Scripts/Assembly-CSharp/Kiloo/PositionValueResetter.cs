using UnityEngine;

namespace Kiloo
{
	public class PositionValueResetter : BaseTransformValueResetter
	{
		private Vector3 _startPosition;

		public PositionValueResetter(SpaceType spaceType)
			: base(spaceType)
		{
		}

		protected override void OnInitLocalSpace(Transform transform)
		{
			_startPosition = transform.position;
		}

		protected override void OnInitWorldSpace(Transform transform)
		{
			_startPosition = transform.localPosition;
		}

		protected override void OnResetLocalSpace(Transform transform)
		{
			transform.position = _startPosition;
		}

		protected override void OnResetWorldSpace(Transform transform)
		{
			transform.localPosition = _startPosition;
		}
	}
}
