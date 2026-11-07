using Data.Save;
using Info;
using UnityEngine;

namespace UI.Screens.FrontScreenInternal
{
	public class UIHoverboardUpdateLabel : MonoBehaviour
	{
		private UILabel _amountLabel;

		private void Awake()
		{
			_amountLabel = GetComponent<UILabel>();
		}

		private void OnEnable()
		{
			InventorySave.powerupsChanged += UpdateLabels;
			UpdateLabels(UpgradeType.hoverboard, InventorySave.GetAmount(UpgradeType.hoverboard));
		}

		private void OnDisable()
		{
			InventorySave.powerupsChanged -= UpdateLabels;
		}

		private void UpdateLabels(UpgradeType upgradeType, int amount)
		{
			if (upgradeType == UpgradeType.hoverboard)
			{
				_amountLabel.text = amount.ToString();
			}
		}
	}
}
