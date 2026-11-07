using UnityEngine;

namespace UI.Popups.SaveMePopupInternal
{
	public class SaveMeAnimateClock : MonoBehaviour
	{
		public UISprite clockSprite;

		public void FillSpriteAmount(float amount)
		{
			if (clockSprite != null)
			{
				clockSprite.fillAmount = amount;
			}
		}
	}
}
