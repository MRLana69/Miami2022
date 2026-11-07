using UnityEngine;

namespace Kiloo
{
	public class ResetValuesOnDisable : MonoBehaviour
	{
		[SerializeField]
		private SpaceType _positionSpaceType;

		[SerializeField]
		private SpaceType _rotationSpaceType;

		private IValueResetter<Transform>[] _valueResetters;

		private void Start()
		{
			CreateValueResetters();
			InitValueResetters();
		}

		private void InitValueResetters()
		{
			IValueResetter<Transform>[] valueResetters = _valueResetters;
			foreach (IValueResetter<Transform> valueResetter in valueResetters)
			{
				valueResetter.Init(base.transform);
			}
		}

		private void CreateValueResetters()
		{
			_valueResetters = new IValueResetter<Transform>[2]
			{
				new PositionValueResetter(_positionSpaceType),
				new RotationValueResetter(_rotationSpaceType)
			};
		}

		private void OnDisable()
		{
			ResetValues();
		}

		private void ResetValues()
		{
			IValueResetter<Transform>[] valueResetters = _valueResetters;
			foreach (IValueResetter<Transform> valueResetter in valueResetters)
			{
				valueResetter.ResetValues(base.transform);
			}
		}
	}
}
