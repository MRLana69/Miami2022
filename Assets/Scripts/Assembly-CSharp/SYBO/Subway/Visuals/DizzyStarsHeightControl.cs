using UnityEngine;

namespace SYBO.Subway.Visuals
{
	public class DizzyStarsHeightControl : MonoBehaviour
	{
		private Transform _characterTransform;

		private Transform _headTransform;

		public float HeightOffset { get; set; }

		private void Start()
		{
			_characterTransform = Game.Instance.Character.transform;
			SetHeadTransform();
			Game.Instance.OnCharacterModelRecreated += Game_OnCharacterModelRecreated;
		}

		private void Game_OnCharacterModelRecreated()
		{
			SetHeadTransform();
		}

		private void SetHeadTransform()
		{
			_headTransform = Game.Instance.Character.CharacterModel.HeadJoint;
		}

		private void LateUpdate()
		{
			Vector3 position = _characterTransform.position;
			position.y = _headTransform.position.y + HeightOffset;
			position.z = _headTransform.position.z;
			base.transform.position = position;
		}
	}
}
