using System.Collections.Generic;
using System.Linq;
using ABTest;
using InApp;
using Kiloo.Common.Extensions;

public static class StoreABManager
{
	public enum StoreOrigin
	{
		Store = 0,
		StorePopup = 1
	}

	private static string _coinStackID = "com.kiloo.subways.coinstier1";

	private static string _lastKnownTestGroup;

	private static bool _storeNeedRefresh = true;

	private static bool _popupNeedRefresh = true;

	public static bool StoreNeedsRefresh(StoreOrigin origin)
	{
		if (TestGroupChanged())
		{
			_storeNeedRefresh = true;
			_popupNeedRefresh = true;
		}
		switch (origin)
		{
		case StoreOrigin.Store:
			if (_storeNeedRefresh)
			{
				_storeNeedRefresh = false;
				return true;
			}
			break;
		case StoreOrigin.StorePopup:
			if (_popupNeedRefresh)
			{
				_popupNeedRefresh = false;
				return true;
			}
			break;
		}
		return false;
	}

	private static bool TestGroupChanged()
	{
		if (_lastKnownTestGroup.IsNullOrEmpty())
		{
			_lastKnownTestGroup = GetTestGroup();
			return false;
		}
		if (_lastKnownTestGroup != GetTestGroup())
		{
			_lastKnownTestGroup = GetTestGroup();
			return true;
		}
		return false;
	}

	public static InAppData.BundleTier GetBundleTier(InAppData.BundleTier lastBundleTier)
	{
		string testGroup = GetTestGroup();
		switch (lastBundleTier)
		{
		case InAppData.BundleTier.None:
			if (testGroup == "A" || testGroup == "B")
			{
				return InAppData.BundleTier.BundleTier2;
			}
			return InAppData.BundleTier.BundleTier1;
		case InAppData.BundleTier.BundleTier1:
			return InAppData.BundleTier.BundleTier2;
		case InAppData.BundleTier.BundleTier2:
			return InAppData.BundleTier.BundleTier3;
		case InAppData.BundleTier.BundleTier3:
			return InAppData.BundleTier.BundleTier4;
		case InAppData.BundleTier.BundleTier4:
			return InAppData.BundleTier.BundleTier5;
		case InAppData.BundleTier.BundleTier5:
			return InAppData.BundleTier.None;
		default:
			SDebug.LogError("Unidetified bundle tier: " + lastBundleTier);
			return InAppData.BundleTier.None;
		}
	}

	public static string GetBundleLabel(InAppItemModel currentBundle)
	{
		if (GetTestGroup() == "B")
		{
			List<InAppItemModel> list = InAppData.inAppData.Values.ToList();
			int num = list.IndexOf(currentBundle);
			if (num - 1 >= 0)
			{
				return Strings.Get(list[num - 1].displayName);
			}
		}
		return Strings.Get(currentBundle.displayName);
	}

	public static void VerifyCoinList(List<InAppItemModel> coinList)
	{
		string testGroup = GetTestGroup();
		if (!(testGroup == "A") && !(testGroup == "B"))
		{
			return;
		}
		for (int i = 0; i < coinList.Count - 1; i++)
		{
			if (coinList[i].itemId == _coinStackID || coinList[i].normalOfferId == _coinStackID)
			{
				coinList.RemoveAt(i);
				break;
			}
		}
	}

	public static void VerifyCoinList(List<KeyValuePair<string, InAppItemModel>> coinList)
	{
		string testGroup = GetTestGroup();
		if (!(testGroup == "A") && !(testGroup == "B"))
		{
			return;
		}
		for (int i = 0; i < coinList.Count - 1; i++)
		{
			if (coinList[i].Value.itemId == _coinStackID || coinList[i].Value.normalOfferId == _coinStackID)
			{
				coinList.RemoveAt(i);
				break;
			}
		}
	}

	private static string GetTestGroup()
	{
		return ABTestManager.GetPayloadString("storepricetier");
	}
}
