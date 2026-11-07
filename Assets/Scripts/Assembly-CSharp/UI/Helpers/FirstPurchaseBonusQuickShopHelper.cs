using UI.Screens;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Helpers
{
	public class FirstPurchaseBonusQuickShopHelper : MonoBehaviour
	{
		[SerializeField]
		private UIWidget _scrollViewWidget;

		[FormerlySerializedAs("_coinScreen")]
		[SerializeField]
		private StoreScreen _storeScreen;

		[SerializeField]
		private Vector2 _firstPurchaseBonusScrollViewWidgetSize;

		private Vector2 _startSize;

		private void Awake()
		{
			_startSize = new Vector2(_scrollViewWidget.width, _scrollViewWidget.height);
			FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive += OnFirstPurchaseBonusBecameInactive;
		}

		private void OnFirstPurchaseBonusBecameInactive()
		{
			SetScrollWidgetSize(_startSize);
		}

		private void OnStoreScreenHasAppeared()
		{
			if (FirstPurchaseBonusManager.isFirstPurchaseBonusActive)
			{
				SetScrollWidgetSize(_firstPurchaseBonusScrollViewWidgetSize);
			}
		}

		private void SetScrollWidgetSize(Vector2 size)
		{
			_scrollViewWidget.SetDimensions((int)size.x, (int)size.y);
		}
	}
}
