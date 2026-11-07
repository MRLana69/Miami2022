using UnityEngine;

namespace UI.Screens.UpgradeScreenInternal
{
	public class UpgradeTierHelper : MonoBehaviour
	{
		public class UpgradeTierPayload
		{
			public string tierText;

			public string tierSpriteName;

			public Color _labelColor;

			public Color _labelEffectColor;
		}

		[SerializeField]
		private UISprite _sprite;

		[SerializeField]
		private UILabel _label;

		public void Setup(UpgradeTierPayload payload)
		{
			_sprite.spriteName = payload.tierSpriteName;
			_label.text = payload.tierText;
			_label.color = payload._labelColor;
			_label.effectColor = payload._labelEffectColor;
		}

		public void SetScale(float scale)
		{
			base.transform.localScale = Vector3.one * scale;
		}
	}
}
