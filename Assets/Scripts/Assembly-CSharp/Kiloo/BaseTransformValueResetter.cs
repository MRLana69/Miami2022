using UnityEngine;

namespace Kiloo
{
	public abstract class BaseTransformValueResetter : IValueResetter<Transform>
	{
		protected SpaceType _spaceType;

		protected BaseTransformValueResetter(SpaceType spaceType)
		{
			_spaceType = spaceType;
		}

		public void Init(Transform transform)
		{
			switch (_spaceType)
			{
			case SpaceType.World:
				OnInitWorldSpace(transform);
				break;
			case SpaceType.Local:
				OnInitLocalSpace(transform);
				break;
			}
		}

		protected abstract void OnInitWorldSpace(Transform transform);

		protected abstract void OnInitLocalSpace(Transform transform);

		public void ResetValues(Transform transform)
		{
			switch (_spaceType)
			{
			case SpaceType.World:
				OnResetWorldSpace(transform);
				break;
			case SpaceType.Local:
				OnResetLocalSpace(transform);
				break;
			}
		}

		protected abstract void OnResetWorldSpace(Transform transform);

		protected abstract void OnResetLocalSpace(Transform transform);
	}
}
