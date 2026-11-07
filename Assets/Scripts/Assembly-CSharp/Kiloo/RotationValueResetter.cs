using UnityEngine;

namespace Kiloo
{
	public class RotationValueResetter : BaseTransformValueResetter
	{
		private Vector3 _startRotation;

		public RotationValueResetter(SpaceType spaceType)
			: base(spaceType)
		{
		}

		protected override void OnInitWorldSpace(Transform transform)
		{
			_startRotation = transform.eulerAngles;
		}

		protected override void OnInitLocalSpace(Transform transform)
		{
			_startRotation = transform.localEulerAngles;
		}

		protected override void OnResetWorldSpace(Transform transform)
		{
			transform.eulerAngles = _startRotation;
		}

		protected override void OnResetLocalSpace(Transform transform)
		{
			transform.localEulerAngles = _startRotation;
		}
	}
}
