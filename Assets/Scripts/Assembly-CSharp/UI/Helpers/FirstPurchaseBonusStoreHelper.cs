using UI.Screens;
using UnityEngine;
using Utilities;

namespace UI.Helpers
{
	public class FirstPurchaseBonusStoreHelper : MonoBehaviour
	{
		private float FIRST_PURCHASE_BONUS_STATIC_OBJECTS_HEIGHT = 225f;

		private float FIRST_PURCHASE_BONUS_TOP_STATIC_OBJECTS_HEIGHT = 110f;

		[SerializeField]
		private ScrollViewHeightResizer _scrollViewHeightResizer;

		private float _staticObjectsHeightStartValue;

		private float _topStaticObjectsHeightStartValue;

		private bool _hasSetSafeAreaOffsetsIfNecessary;

		private void OnFirstPurchaseBonusBecameInactive()
		{
			ResetProperties();
			_scrollViewHeightResizer.RearrangeWidgets();
		}

		private void Awake()
		{
			_staticObjectsHeightStartValue = _scrollViewHeightResizer.staticObjectsHeight;
			_topStaticObjectsHeightStartValue = _scrollViewHeightResizer.topStaticObjectsHeight;
			FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive += OnFirstPurchaseBonusBecameInactive;
			if (SafeArea.IsRestricted && !_hasSetSafeAreaOffsetsIfNecessary)
			{
				UIWidget component = GetComponent<UIWidget>();
				if (component != null)
				{
					component.topAnchor.absolute -= SafeArea.OFFSET_TOP + SafeArea.OFFSET_BOTTOM;
					_hasSetSafeAreaOffsetsIfNecessary = true;
				}
			}
		}

		private void OnEnable()
		{
			if (FirstPurchaseBonusManager.isFirstPurchaseBonusActive)
			{
				SetPropertiesNeededForFirstPurchaseBonus();
			}
		}

		private void OnDestroy()
		{
			FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive -= OnFirstPurchaseBonusBecameInactive;
		}

		private void SetPropertiesNeededForFirstPurchaseBonus()
		{
			_scrollViewHeightResizer.staticObjectsHeight = FIRST_PURCHASE_BONUS_STATIC_OBJECTS_HEIGHT;
			_scrollViewHeightResizer.topStaticObjectsHeight = FIRST_PURCHASE_BONUS_TOP_STATIC_OBJECTS_HEIGHT;
		}

		private void ResetProperties()
		{
			_scrollViewHeightResizer.staticObjectsHeight = _staticObjectsHeightStartValue;
			_scrollViewHeightResizer.topStaticObjectsHeight = _topStaticObjectsHeightStartValue;
		}
	}
}
