using UnityEngine;

public class AwardCollectibleHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite _itemIconSprite;

	[SerializeField]
	private GameObject _completeIcon;

	[SerializeField]
	private GameObject _glitterOverlay;

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private GameObject _socialLockOverlay;

	[SerializeField]
	private GameObject _glowElement;

	private const string GLOW_LOOP_ANIMATION = "MainCanGlow";

	private const string GLOW_SHORT_ANIMATION = "MainCanGlowShort";

	private static readonly Color32 SOCIAL_LOCK_CAN_COLOR = new Color32(111, 111, 111, byte.MaxValue);

	public void SetupAwardCollectible(string backgroundToUse, string itemIconName, bool useGlitter, bool isItemCollected = false, bool animateStars = false)
	{
		_itemIconSprite.transform.localRotation = Quaternion.Euler(Vector3.zero);
		_itemIconSprite.spriteName = itemIconName;
		_completeIcon.SetActive(isItemCollected);
		_background.spriteName = backgroundToUse;
		if (_glitterOverlay != null)
		{
			_glitterOverlay.gameObject.SetActive(useGlitter);
			if (useGlitter && animateStars)
			{
				_glitterOverlay.gameObject.GetComponent<AwardStarHandler>().animeStars = true;
				_glitterOverlay.gameObject.GetComponent<AwardStarHandler>().ActivateStars();
			}
			else
			{
				_glitterOverlay.gameObject.GetComponent<AwardStarHandler>().animeStars = false;
				_glowElement.SetActive(false);
			}
		}
	}

	public void AddSocialLockCanOverlay(bool socialLock)
	{
		_socialLockOverlay.SetActive(socialLock);
	}

	public void ActivateCanGlow(bool active)
	{
		_glowElement.SetActive(active);
		if (active)
		{
			_glowElement.GetComponent<Animation>().Play("MainCanGlow");
		}
	}

	public void ChangeCanColorForSocialLock(bool socialLock)
	{
		if (socialLock)
		{
			_itemIconSprite.color = SOCIAL_LOCK_CAN_COLOR;
		}
		else
		{
			_itemIconSprite.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
	}
}
