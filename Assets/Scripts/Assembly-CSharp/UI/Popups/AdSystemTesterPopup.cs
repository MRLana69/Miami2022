using System.Collections.Generic;
using Kiloo.Common.Ads;
using UnityEngine;

namespace UI.Popups
{
	public class AdSystemTesterPopup : UIPopup
	{
		[SerializeField]
		private GameObject _adCacheSlotCellPrefab;

		[SerializeField]
		private GameObject _cellParent;

		[SerializeField]
		private GameObject _adProviderCellPrefab;

		[SerializeField]
		private UITable _cellParentTable;

		private ICollection<AdCacheSlotInfo> _adSlots;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			_adSlots = AdCacheSlots.adCacheSlots;
			foreach (AdCacheSlotInfo adSlot in _adSlots)
			{
				if (adSlot == AdCacheSlots.UNDETERMINED_SLOT)
				{
					continue;
				}
				GameObject gameObject = _cellParent.AddChild(_adCacheSlotCellPrefab);
				gameObject.GetComponent<AdCacheSlotTesterCell>().Init(adSlot);
				List<AdProviderInfo> list = AdSystemManager.DebugGetProviderListForSlot(adSlot);
				if (list == null)
				{
					continue;
				}
				foreach (AdProviderInfo item in list)
				{
					GameObject gameObject2 = _cellParent.AddChild(_adProviderCellPrefab);
					string cacheID = AdSystemManager.DebugGetCacheIdForProvider(item, adSlot);
					gameObject2.GetComponent<AdProviderTesterCell>().Init(item, adSlot, cacheID);
				}
			}
			_cellParentTable.Reposition();
		}
	}
}
