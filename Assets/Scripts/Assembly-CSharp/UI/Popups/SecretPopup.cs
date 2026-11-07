using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ABTest;
using Cloud;
using Cloud.Internal;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Analytics;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;
using Missions;
using SYBO.Components.Analytics;
using SYBO.Subway.Info;
using UI.Generic.Layout;
using UI.KScrollView;
using UI.Popups.SecretPopupInternal;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Popups
{
	public class SecretPopup : UIPopup
	{
		private struct PageContent
		{
			public string key;

			public Func<string> value;

			public SecretCellType infoType;

			public string key2;

			public Func<string> value2;

			public PageContent(string key, Func<string> value, SecretCellType infoType = SecretCellType.Info, string key2 = null, Func<string> value2 = null)
			{
				this.key = key;
				this.value = value;
				this.infoType = infoType;
				this.key2 = key2;
				this.value2 = value2;
			}
		}

		private enum TabNumber
		{
			Tab1 = 0,
			Tab2 = 1,
			Tab3 = 2,
			Tab4 = 3
		}

		private enum FileLogType
		{
			Data = 0,
			Cache = 1
		}

		private class SecretListAdapter : ListAdapter<SecretCell>
		{
			private GameObject _secretPrefab;

			private GameObject _secretSubtitlePrefab;

			private GameObject _secretEndList;

			private GameObject _secretInfoDoublePrefab;

			private const int SECRET_INFO_PREFAB = 0;

			private const int SECRET_SUBTITLE_PREFAB = 1;

			private const int SECRET_ENDLIST_PREFAB = 2;

			private const int SECRET_INFO_DOUBLE_PREFAB = 3;

			public SecretListAdapter(List<SecretCell> contentList, GameObject newSecretPrefab, GameObject newInfoDoublePrefab, GameObject newSecretSubtitlePrefab, GameObject newSecretEndList)
				: base(contentList)
			{
				_secretPrefab = newSecretPrefab;
				_secretInfoDoublePrefab = newInfoDoublePrefab;
				_secretSubtitlePrefab = newSecretSubtitlePrefab;
				_secretEndList = newSecretEndList;
			}

			public void SetNewDataList(List<SecretCell> leaderboardListT)
			{
				base.dataList = leaderboardListT;
				NotifyDataSetChanged();
			}

			public override int GetModelType(int index)
			{
				switch (GetDataItem(index).cellType)
				{
				case SecretCellType.Info:
					return 0;
				case SecretCellType.Subtitle:
					return 1;
				case SecretCellType.EndList:
					return 2;
				case SecretCellType.DoubleInfo:
					return 3;
				default:
					return 0;
				}
			}

			public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
			{
				Container container2 = container;
				SecretCell dataItem = GetDataItem(dataIndex);
				if (container2 == null)
				{
					SecretCellReferences secretCellReferences = new SecretCellReferences();
					switch (dataItem.cellType)
					{
					case SecretCellType.Info:
					{
						container2 = new Container(_secretPrefab);
						SecretHelper component4 = container2.prefabInstance.GetComponent<SecretHelper>();
						secretCellReferences.secretHelper = component4;
						container2.controlComponent = secretCellReferences;
						break;
					}
					case SecretCellType.DoubleInfo:
					{
						container2 = new Container(_secretInfoDoublePrefab);
						SecretHelper component3 = container2.prefabInstance.GetComponent<SecretHelper>();
						secretCellReferences.secretHelper = component3;
						container2.controlComponent = secretCellReferences;
						break;
					}
					case SecretCellType.Subtitle:
					{
						container2 = new Container(_secretSubtitlePrefab);
						SecretHelper component2 = container2.prefabInstance.GetComponent<SecretHelper>();
						secretCellReferences.secretHelper = component2;
						container2.controlComponent = secretCellReferences;
						break;
					}
					case SecretCellType.EndList:
					{
						container2 = new Container(_secretEndList);
						SecretHelper component = container2.prefabInstance.GetComponent<SecretHelper>();
						secretCellReferences.secretHelper = component;
						container2.controlComponent = secretCellReferences;
						break;
					}
					}
				}
				SecretCellReferences secretCellReferences2 = (SecretCellReferences)container2.controlComponent;
				switch (dataItem.cellType)
				{
				case SecretCellType.Info:
					secretCellReferences2.secretHelper.UpdateInfo(dataItem.cellInfo, dataItem.cellValue);
					break;
				case SecretCellType.DoubleInfo:
					secretCellReferences2.secretHelper.UpdateInfoDouble(dataItem.cellInfo, dataItem.cellValue, dataItem.cellInfo2, dataItem.cellValue2);
					break;
				case SecretCellType.Subtitle:
					secretCellReferences2.secretHelper.UpdateTitle(dataItem.cellTitle);
					break;
				default:
					SDebug.LogError("No cell type match for: " + dataItem.cellType);
					break;
				case SecretCellType.EndList:
					break;
				}
				return container2;
			}
		}

		[SerializeField]
		private UILabel _popupTitle;

		[SerializeField]
		private GameObject _copyButton;

		[SerializeField]
		private UILabel _copyButtonLabel;

		[SerializeField]
		private Layout _copyButtonLayout;

		[SerializeField]
		private GameObject _copiedTextGO;

		[SerializeField]
		private UILabel _copiedTextLabel;

		[FormerlySerializedAs("SecretCellPrefab")]
		[SerializeField]
		private GameObject _secretCellPrefab;

		[FormerlySerializedAs("SecretCellSubtitlePrefab")]
		[SerializeField]
		private GameObject _secretCellSubtitlePrefab;

		[FormerlySerializedAs("SecretCellEndList")]
		[SerializeField]
		private GameObject _secretCellEndList;

		[FormerlySerializedAs("SecretCellInfoDouble")]
		[SerializeField]
		private GameObject _secretCellInfoDouble;

		private List<SecretCell> _contentList = new List<SecretCell>();

		private SecretListAdapter _adapter;

		[SerializeField]
		private ScrollView _scrollView;

		private StringBuilder _infoToBeCopiedToClipboard;

		private int _lastTab1Index;

		private int _lastTab2Index;

		private int _lastTab3Index;

		private int _lastTab4Index;

		private TabNumber _lastTabOpen;

		private string _advertisingIdentifier = "N/A";

		private const string ANDROID_BASED_BUNDLE_IDENTIFIER = "com.kiloo.subwaysurf";

		private const string IOS_BUNDLE_IDENTIFIER = "com.kiloo.subwaysurfers";

		private Dictionary<TabNumber, List<PageContent>> _contentDict = new Dictionary<TabNumber, List<PageContent>>();

		private bool _listHasBeenDisplayedOnce;

		private const string ONLINESETTINGS_ANALYTICS_SAMPLE_SIZE_KEY = "kiloo_analytics_sample";

		private const float DEFAULT_ANALYTICS_SAMPLE_SIZE = 0f;

		private List<FileInfo> _files = new List<FileInfo>();

		private StringBuilder fileInfoStringBuilder = new StringBuilder();

		private long _sectionSize;

		private long _totalSize;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			if (_contentList != null)
			{
				_adapter = new SecretListAdapter(_contentList, _secretCellPrefab, _secretCellInfoDouble, _secretCellSubtitlePrefab, _secretCellEndList);
			}
			LoadAdvertisingInfoAsync();
			PopulateContentDictionary();
		}

		protected override void Show()
		{
			AdSystemManager.onCachedVideo += CacheStateChangedForLocation;
			AdSystemManager.onCachedInterstitial += CacheStateChangedForLocation;
			AdSystemManager.onFailedToCacheVideo += CacheStateChangedForLocation;
			AdSystemManager.onFailedToCacheInterstitial += CacheStateChangedForLocation;
			AdSystemManager.onPriorityListWrapped += CacheStateChangedForLocation;
			AdSystemManager.onChangedProvider += CacheStateChangedForLocation;
			switch (_lastTabOpen)
			{
			case TabNumber.Tab1:
				DisplayList1();
				break;
			case TabNumber.Tab2:
				DisplayList2();
				break;
			case TabNumber.Tab3:
				DisplayList3();
				break;
			case TabNumber.Tab4:
				DisplayList4();
				break;
			}
			_copyButtonLabel.text = Strings.Get("SECRET_POPUP_BUTTON_COPY");
			_copiedTextLabel.text = Strings.Get("SECRET_POPUP_BUTTON_COPIED");
			_copiedTextGO.SetActive(false);
			_copyButton.SetActive(true);
			_copyButtonLayout.Align();
		}

		protected override void Hide()
		{
			AdSystemManager.onCachedVideo -= CacheStateChangedForLocation;
			AdSystemManager.onCachedInterstitial -= CacheStateChangedForLocation;
			AdSystemManager.onFailedToCacheVideo -= CacheStateChangedForLocation;
			AdSystemManager.onFailedToCacheInterstitial -= CacheStateChangedForLocation;
			AdSystemManager.onPriorityListWrapped -= CacheStateChangedForLocation;
			AdSystemManager.onChangedProvider -= CacheStateChangedForLocation;
			SaveIndex();
		}

		private bool IsAndroidDevice()
		{
			return Application.platform == RuntimePlatform.Android;
		}

		private bool IsiOSDevice()
		{
			return Application.platform == RuntimePlatform.IPhonePlayer;
		}

		private void CacheStateChangedForLocation(string waterfallID)
		{
			CacheStateChangedForLocation(AdCacheSlots.UNDETERMINED_SLOT);
		}

		private void CacheStateChangedForLocation(AdCacheSlotInfo adCacheSlotInfo)
		{
			CacheStateChangedForLocation(adCacheSlotInfo, false);
		}

		private void CacheStateChangedForLocation(AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
		{
			if (_lastTabOpen == TabNumber.Tab2)
			{
				DisplayList2();
			}
		}

		private unsafe void PopulateContentDictionary()
		{
			_contentDict = new Dictionary<TabNumber, List<PageContent>>();
			List<PageContent> list = new List<PageContent>();
			list.Add(new PageContent("Version", GetVersion));
			list.Add(new PageContent("Platform", GetOnlineSettingsPlatform, SecretCellType.DoubleInfo, "Debug", GetDebugMode));
			list.Add(new PageContent("Build", ((object)Globals.BUILD_PURPOSE).ToString, SecretCellType.DoubleInfo, "OS", GetOnlineSettings));
			list.Add(new PageContent("Device type", ((object)DeviceInfo.Instance.GetMobileDeviceType()).ToString));
			list.Add(new PageContent("Previous Version", GetPreviouslyInstalledVersion));
			list.Add(new PageContent("Initial Version Installed", GetFirstInstalledVersion));
			list.Add(new PageContent("Initial Date Installed", GetInitialInstallDate));
			list.Add(new PageContent("Most Recent Date Updated", () => UserSave.lastTimeAppWasUpdatedOrInstalled.ToString()));
			list.Add(new PageContent("Device Timezone", GetTimezone));
			list.Add(new PageContent("Device Time", () => CDateTime.Now.ToString()));
			list.Add(new PageContent("Server Time", () => UserSave.lastKnownServerTime.ToString()));
			list.Add(new PageContent("Bundle Identifier", GetBundleIdentifier));
			list.Add(new PageContent("Installer", GetApplicationInstaller));
			list.Add(new PageContent("Unity", () => Application.unityVersion.ToString()));
			_contentDict[TabNumber.Tab1] = list;
			List<PageContent> list2 = new List<PageContent>();
			list2.Add(new PageContent("Online Save Status", GetOnlineSaveStatus));
			foreach (AdCacheSlotInfo adCacheSlot in AdCacheSlots.adCacheSlots)
			{
				if (adCacheSlot != AdCacheSlots.UNDETERMINED_SLOT)
				{
					AdCacheSlotInfo info = adCacheSlot;
					list2.Add(new PageContent(adCacheSlot.id, () => GetAdCacheSlotStatus(info)));
				}
			}
			list2.Add(new PageContent("KA", GetKilooAnalyticsInfo, SecretCellType.DoubleInfo, "SA", GetKilooAnalyticsSample));
			list2.Add(new PageContent("SK", GetAnalyticsSamplingKey));
			if (Globals.DEBUG)
			{
				list2.Add(new PageContent("SID", new Func<string>(() => SyboAnalyticsSave.syboAnalyticsUserId)));
			}
			list2.Add(new PageContent("SAE", GetSyboAnalyticsEnabled, SecretCellType.DoubleInfo, "SST", GetSyboAnalyticsSamplingThreshold));
			list2.Add(new PageContent("RA", GetRestrictionActive, SecretCellType.DoubleInfo, "BD", GetUserBirthday));
			list2.Add(new PageContent("KLID", GetKilooLoggerID));
			list2.Add(new PageContent("Pre-cached Bundles", AssetBundleManager.Instance.GetPreCachedBundles));
			list2.Add(new PageContent("Downloaded Bundles", AssetBundleManager.Instance.GetDownloadedBundles));
			list2.Add(new PageContent("ABT", GetABTestStatus));
			list2.Add(new PageContent("MPI", GetMigratedPlayerInfo, SecretCellType.DoubleInfo, "MPP", GetMigratedPlayerPrefsVersion));
			_contentDict[TabNumber.Tab2] = list2;
			List<PageContent> list3 = new List<PageContent>();
			list3.Add(new PageContent("Facebook ID", GetFacebookID));
			if (IsiOSDevice())
			{
				list3.Add(new PageContent("GID", GetGameCenterID));
			}
			list3.Add(new PageContent("AID", GetAdvertisingID));
			list3.Add(new PageContent("KTAG", GetKilooTag));
			if (GetOldKilooTags() != string.Empty)
			{
				list3.Add(new PageContent("Old KTAGs", GetOldKilooTags));
			}
			list3.Add(new PageContent("IAP Legacy", CheckForInAppLegacyPurchase, SecretCellType.DoubleInfo, "IAP Restore", CheckForInAppRestoredPurchase));
			list3.Add(new PageContent("IAP Cons", CheckForInAppConsumablePurchase, SecretCellType.DoubleInfo, "IAP Noncons", CheckForInAppNonconsumablePurchase));
			list3.Add(new PageContent("Current Coins", () => InventorySave.coins.ToString(), SecretCellType.DoubleInfo, "Current Keys", () => InventorySave.keys.ToString()));
			list3.Add(new PageContent("Runs", () => PlayerSave.completedRunCount.ToString(), SecretCellType.DoubleInfo, "Words in a Row", () => EventsSave.wordHuntWordsInRow.ToString()));
			list3.Add(new PageContent("Double Coins", () => (!UserSave.hasDoubleCoinsUpgrade) ? "No" : "Yes", SecretCellType.DoubleInfo, "Jackpots Won", () => Stat.TotalJackpotsWon.GetValue().ToString()));
			list3.Add(new PageContent("Diamond Tier", () => AwardsManager.instance.GetNumberOfFinishedAwardsExactTier(AwardTierType.Diamond).ToString(), SecretCellType.DoubleInfo, "Gold Tier", () => AwardsManager.instance.GetNumberOfFinishedAwardsExactTier(AwardTierType.Gold).ToString()));
			list3.Add(new PageContent("Silver Tier", () => AwardsManager.instance.GetNumberOfFinishedAwardsExactTier(AwardTierType.Silver).ToString(), SecretCellType.DoubleInfo, "Bronze Tier", () => AwardsManager.instance.GetNumberOfFinishedAwardsExactTier(AwardTierType.Bronze).ToString()));
			list3.Add(new PageContent("Characters", GetAmountOfOwnedCharacters, SecretCellType.DoubleInfo, "Hoverboards", GetAmountOfOwnedHoverboards));
			list3.Add(new PageContent("Mission Sets", () => PlayerSave.missionSetCompletedCount.ToString()));
			_contentDict[TabNumber.Tab3] = list3;
			List<PageContent> list4 = new List<PageContent>();
			list4.Add(new PageContent("Device Model", new Func<string>(() => SystemInfo.deviceModel)));
			list4.Add(new PageContent("Device Name", new Func<string>(() => SystemInfo.deviceName)));
			list4.Add(new PageContent("Operating System", new Func<string>(() => SystemInfo.operatingSystem)));
			list4.Add(new PageContent("Device Locale", DeviceUtility.GetLocale, SecretCellType.DoubleInfo, "Language", () => Application.systemLanguage.ToString()));
			list4.Add(new PageContent("Active Locale", () => Strings.language.locale.combined, SecretCellType.DoubleInfo, "UF Locale", DeviceUtility.GetUnfilteredLocale));
			if (IsAndroidDevice())
			{
				list4.Add(new PageContent("RD", CheckForRootedDevice));
			}
			if (IsiOSDevice())
			{
				list4.Add(new PageContent("JD", CheckForJailbrokenDevice));
			}
			if (Globals.DEBUG)
			{
				list4.Add(new PageContent("SDID", GetSyboAnalyticsHashedDeviceId));
			}
			list4.Add(new PageContent("Environment Details", () => DeviceInfo.Instance.performanceLevel.ToString()));
			list4.Add(new PageContent("ResType", () => (!DeviceInfo.Instance.isHighres) ? "Low" : "High", SecretCellType.DoubleInfo, "Resolution", () => Screen.width + "x" + Screen.height));
			list4.Add(new PageContent("D.RAM Used", GetUsedDeviceMemory, SecretCellType.DoubleInfo, "D.RAM Total", () => SystemInfo.systemMemorySize + " MB"));
			list4.Add(new PageContent("A.MEM Used", GetApplicationMemoryUsage, SecretCellType.DoubleInfo, "A.MEM Limit", GetApplicationMemoryLimit));
			if (IsiOSDevice() || IsAndroidDevice())
			{
				list4.Add(new PageContent("Disk Used", GetUsedDiskSpace, SecretCellType.DoubleInfo, "Disk Total", GetTotalDiskSpace));
			}
			if (IsAndroidDevice())
			{
				list4.Add(new PageContent("SD Used", GetUsedSDCardSpace, SecretCellType.DoubleInfo, "SD Total", GetTotalSDCardSpace));
			}
			if (StorageSizeLogging.HasFlurryLoggedInSession())
			{
				list4.Add(new PageContent("Data Size", () => StorageSizeLogging.DataSize.ToString(), SecretCellType.DoubleInfo, "Cache Size", () => StorageSizeLogging.CacheSize.ToString()));
				list4.Add(new PageContent("AssetBundle Size", () => StorageSizeLogging.AssetBundlesSize.ToString()));
			}
			list4.Add(new PageContent("CPU", new Func<string>(() => SystemInfo.processorType)));
			list4.Add(new PageContent("GPU", new Func<string>(() => SystemInfo.graphicsDeviceName)));
			list4.Add(new PageContent("Graphics API", () => SystemInfo.graphicsDeviceType.ToString()));
			list4.Add(new PageContent("Screen DPI", GetScreenDPI));
			list4.Add(new PageContent("Battery Level", GetBatteryLevel));
			list4.Add(new PageContent("Battery Temperature", GetBatteryTemperature));
			if (Globals.DEBUG_SHOW_PERFORMANCE_INFO)
			{
				list4.Add(new PageContent("GPU score", () => DeviceInfo.Instance.gpuScore.ToString()));
				list4.Add(new PageContent("GPU category", () => DeviceInfo.Instance.gpuCategory.ToString()));
				list4.Add(new PageContent("FPS", () => InRunAverageFPSCounter.runAverageFPS.ToString()));
				list4.Add(new PageContent("Run Length", () => InRunAverageFPSCounter.runLength.ToString()));
			}
			_contentDict[TabNumber.Tab4] = list4;
		}

		private void SetListContent(TabNumber tab)
		{
			ResetList();
			foreach (PageContent item in _contentDict[tab])
			{
				if (item.infoType == SecretCellType.Info)
				{
					InsertInfo(item.key, item.value());
				}
				else
				{
					InsertDoubleInfo(item.key, item.value(), item.key2, item.value2());
				}
			}
			SetEndOfList();
		}

		public void DisplayList1()
		{
			SaveIndex();
			SetListContent(TabNumber.Tab1);
			_lastTabOpen = TabNumber.Tab1;
			ScrollToLastIndex(_lastTab1Index);
			_listHasBeenDisplayedOnce = true;
		}

		public void DisplayList2()
		{
			SaveIndex();
			SetListContent(TabNumber.Tab2);
			_lastTabOpen = TabNumber.Tab2;
			ScrollToLastIndex(_lastTab2Index);
		}

		public void DisplayList3()
		{
			SaveIndex();
			SetListContent(TabNumber.Tab3);
			_lastTabOpen = TabNumber.Tab3;
			ScrollToLastIndex(_lastTab3Index);
		}

		public void DisplayList4()
		{
			SaveIndex();
			SetListContent(TabNumber.Tab4);
			_lastTabOpen = TabNumber.Tab4;
			ScrollToLastIndex(_lastTab4Index);
		}

		private void SetEndOfList()
		{
			_contentList.Add(new SecretCell
			{
				cellType = SecretCellType.EndList
			});
			if (_scrollView != null && _adapter != null)
			{
				_scrollView.SetAdapter(_adapter);
			}
		}

		private void InsertInfo(string label, string value)
		{
			_contentList.Add(new SecretCell
			{
				cellInfo = label,
				cellValue = value,
				cellType = SecretCellType.Info
			});
		}

		private void InsertDoubleInfo(string label1, string value1, string label2, string value2)
		{
			_contentList.Add(new SecretCell
			{
				cellInfo = label1,
				cellValue = value1,
				cellInfo2 = label2,
				cellValue2 = value2,
				cellType = SecretCellType.DoubleInfo
			});
		}

		private void ResetList()
		{
			_contentList.Clear();
		}

		private void SaveIndex()
		{
			if (!_listHasBeenDisplayedOnce)
			{
				return;
			}
			Container firstVisibleContainerOnScreen = _scrollView.GetFirstVisibleContainerOnScreen();
			if (firstVisibleContainerOnScreen != null)
			{
				switch (_lastTabOpen)
				{
				case TabNumber.Tab1:
					_lastTab1Index = firstVisibleContainerOnScreen.positionIndex;
					break;
				case TabNumber.Tab2:
					_lastTab2Index = firstVisibleContainerOnScreen.positionIndex;
					break;
				case TabNumber.Tab3:
					_lastTab3Index = firstVisibleContainerOnScreen.positionIndex;
					break;
				case TabNumber.Tab4:
					_lastTab4Index = firstVisibleContainerOnScreen.positionIndex;
					break;
				}
			}
		}

		private void ScrollToLastIndex(int index)
		{
			_scrollView.SetSelection(index);
		}

		private IEnumerator ShowTextCopied()
		{
			_copiedTextGO.SetActive(true);
			_copiedTextLabel.alpha = 1f;
			_copyButton.SetActive(false);
			yield return new WaitForSecondsRealtime(0.6f);
			float duration = 0.7f;
			float fromAlpha = 1f;
			float toAlpha = 0f;
			float factor = 0f;
			while (factor < 1f)
			{
				factor += RealTimeTracker.deltaTime / duration;
				factor = Mathf.Clamp01(factor);
				_copiedTextLabel.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
				yield return null;
			}
			yield return new WaitForSecondsRealtime(0.05f);
			_copiedTextGO.SetActive(false);
			_copyButton.SetActive(true);
		}

		private string GetBundleIdentifier()
		{
			string identifier = Application.identifier;
			if (IsiOSDevice())
			{
				if (identifier == "com.kiloo.subwaysurfers")
				{
					return "Validated";
				}
				return identifier;
			}
			if (identifier == "com.kiloo.subwaysurf")
			{
				return "Validated";
			}
			return identifier;
		}

		private string GetVersion()
		{
			if (IsAndroidDevice())
			{
				return DeviceUtility.GetBundleVersion() + " (" + DeviceUtility.GetAndroidVersionCode() + ")";
			}
			if (IsiOSDevice())
			{
				return DeviceUtility.GetBundleVersion() + " (" + DeviceUtility.GetiOSBuildNumber() + ")";
			}
			return DeviceUtility.GetBundleVersion();
		}

		private string GetPreviouslyInstalledVersion()
		{
			string text = UserSave.previouslySeenBundleVersion;
			if (text.IsNullOrEmpty())
			{
				text = "None";
			}
			return text;
		}

		private string GetFirstInstalledVersion()
		{
			if (!string.IsNullOrEmpty(UserSave.firstInstalledVersion))
			{
				return UserSave.firstInstalledVersion;
			}
			return "Earlier than 1.73.0";
		}

		private string GetInitialInstallDate()
		{
			if (UserSave.firstInstallDate == DateTime.MinValue)
			{
				return "Earlier than January 2015";
			}
			return UserSave.firstInstallDate.ToString();
		}

		private string GetUsedDeviceMemory()
		{
			int systemMemorySize = SystemInfo.systemMemorySize;
			int deviceFreeMemory = DeviceUtility.GetDeviceFreeMemory();
			string result = systemMemorySize - deviceFreeMemory + " MB";
			if (deviceFreeMemory == 0)
			{
				result = "Unknown";
			}
			return result;
		}

		private string GetApplicationMemoryLimit()
		{
			int memoryUsageLimit = DeviceUtility.GetMemoryUsageLimit();
			string result = memoryUsageLimit + " MB";
			if (memoryUsageLimit == 0)
			{
				result = "Unknown";
			}
			return result;
		}

		private string GetApplicationMemoryUsage()
		{
			int currentMemoryUsage = DeviceUtility.GetCurrentMemoryUsage();
			string result = currentMemoryUsage + " MB";
			if (currentMemoryUsage == 0)
			{
				result = "Unknown";
			}
			return result;
		}

		private string GetPeakMemoryUsage()
		{
			int peakMemoryUsage = DeviceUtility.GetPeakMemoryUsage();
			if (peakMemoryUsage == 0)
			{
				return "Unknown";
			}
			return peakMemoryUsage + " MB";
		}

		private string GetDebugMode()
		{
			if (Globals.DEBUG)
			{
				return "ON";
			}
			return "Off";
		}

		private string GetScreenDPI()
		{
			if (Screen.dpi == 0f)
			{
				return "Unknown";
			}
			return Screen.dpi + " DPI";
		}

		private string GetBatteryLevel()
		{
			int batteryLevel = DeviceUtility.GetBatteryLevel();
			if (batteryLevel == -1)
			{
				return "Unknown";
			}
			return batteryLevel + "%";
		}

		private string GetBatteryTemperature()
		{
			float batteryTemperature = DeviceUtility.GetBatteryTemperature();
			if (batteryTemperature <= -1f)
			{
				return "Unknown";
			}
			return string.Format("{0:F1} C", batteryTemperature);
		}

		private string GetTotalDiskSpace()
		{
			float num = (float)DeviceUtility.GetTotalDiskSpace() / 1024f;
			string text = string.Format("{0:F2}", num);
			text += " GB";
			if (num == -1f)
			{
				text = "Unknown";
			}
			return text;
		}

		private string GetUsedDiskSpace()
		{
			float num = (float)DeviceUtility.GetFreeDiskSpace() / 1024f;
			float num2 = (float)DeviceUtility.GetTotalDiskSpace() / 1024f;
			float num3 = num2 - num;
			string text = string.Format("{0:F2}", num3);
			text += " GB";
			if (num == -1f)
			{
				text = "Unknown";
			}
			return text;
		}

		private string GetUsedSDCardSpace()
		{
			float num = (float)DeviceUtility.GetFreeSDCardSpace() / 1024f;
			float num2 = (float)DeviceUtility.GetTotalSDCardSpace() / 1024f;
			float num3 = num2 - num;
			string text = string.Format("{0:F2}", num3);
			text += " GB";
			if (num == -1f)
			{
				text = "Unknown";
			}
			return text;
		}

		private string GetTotalSDCardSpace()
		{
			float num = (float)DeviceUtility.GetTotalSDCardSpace() / 1024f;
			string text = string.Format("{0:F2}", num);
			text += " GB";
			if (num == -1f)
			{
				text = "Unknown";
			}
			return text;
		}

		private string GetOnlineSettings()
		{
			if (Globals.DEBUG_USE_ONLINESETTINGS_SET && !string.IsNullOrEmpty(Globals.ONLINESETTINGS_SET))
			{
				return Globals.ONLINESETTINGS_SET;
			}
			return "Live";
		}

		private string GetOnlineSettingsPlatform()
		{
			string currentPlatform = OnlineSettings.instance.GetCurrentPlatform();
			string text = currentPlatform[0].ToString();
			if (currentPlatform == "ios")
			{
				return "iOS";
			}
			return (currentPlatform.Length <= 0) ? currentPlatform : (text.ToUpper() + currentPlatform.Substring(1));
		}

		private string GetKilooAnalyticsSample()
		{
			float value = OnlineSettings.instance.GetValue("kiloo_analytics_sample", 0f);
			return value + "%";
		}

		private string GetKilooAnalyticsInfo()
		{
			return (!Manager.isPartOfSample) ? "No" : "Yes";
		}

		private string GetAnalyticsSamplingKey()
		{
			return AnalyticsSave.analyticsSamplingKey.ToString("0.000000");
		}

		private string GetTimezone()
		{
			double totalHours = CDateTime.Now.GetCurrentTimeZoneUTCOffset().TotalHours;
			if (totalHours > 0.0)
			{
				return "GMT+" + totalHours.ToString("00") + ":00";
			}
			return "GMT" + totalHours.ToString("00") + ":00";
		}

		private string CheckForInAppLegacyPurchase()
		{
			return UserSave.inAppLegacyPurchaseCount.ToString();
		}

		private string CheckForInAppConsumablePurchase()
		{
			return UserSave.inAppConsumablePurchaseCount.ToString();
		}

		private string CheckForInAppNonconsumablePurchase()
		{
			return UserSave.inAppNonConsumablePurchaseCount.ToString();
		}

		private string CheckForInAppRestoredPurchase()
		{
			return UserSave.inAppRestoredPurchaseCount.ToString();
		}

		private string GetFacebookID()
		{
			if (SocialManager.instance.facebookIsLoggedIn && SocialManager.instance.facebookProfile != null && SocialManager.instance.facebookProfile.fullName != null && SocialManager.instance.facebookProfile.id != null)
			{
				return SocialManager.instance.facebookProfile.fullName + " (id: " + SocialManager.instance.facebookProfile.id + ")";
			}
			return "Not logged in to Facebook";
		}

		private void LoadAdvertisingInfoAsync()
		{
			if (!Application.RequestAdvertisingIdentifierAsync((string advertisingId, bool trackingEnabled, string error) =>
			{
				if (trackingEnabled)
				{
					SetAdvertisingInfo(advertisingId);
				}
				else
				{
					SetAdvertisingInfo("User opted out");
				}
			}))
			{
				SetAdvertisingInfo("Platform not supported");
			}
		}

		private void SetAdvertisingInfo(string text)
		{
			_advertisingIdentifier = text;
			PopulateContentDictionary();
		}

		private string GetAdvertisingID()
		{
			return _advertisingIdentifier;
		}

		private string GetGameCenterID()
		{
			if (SocialManager.instance.gamecenterIsLoggedIn)
			{
				string id = SocialManager.instance.gamecenterProfile.id;
				byte[] bytes = Encoding.ASCII.GetBytes(id);
				string text = string.Empty;
				byte[] array = bytes;
				foreach (byte b in array)
				{
					text = text + b + " ";
				}
				return string.Empty + text;
			}
			return "Not logged in.";
		}

		private string CheckForRootedDevice()
		{
			bool flag = false;
			string[] array = new string[8] { "/sbin/", "/system/bin/", "/system/xbin/", "/data/local/xbin/", "/data/local/bin/", "/system/sd/xbin/", "/system/bin/failsafe/", "/data/local/" };
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (File.Exists(text + "su"))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return "Yes";
			}
			return "No";
		}

		private string TruncateProviderTier(string providerTier)
		{
			string text = "none";
			if (providerTier == text || providerTier == null)
			{
				return text;
			}
			if (!string.IsNullOrEmpty(providerTier) && providerTier.Length >= 2)
			{
				string format;
				if (providerTier.EndsWith("_(saved)"))
				{
					format = "({0})";
					int length = providerTier.Length - "_(saved)".Length;
					providerTier = providerTier.Substring(0, length);
				}
				else
				{
					format = "{0}";
				}
				string text2 = string.Empty;
				string[] array = providerTier.Split('_');
				if (array.Length > 1)
				{
					text2 = ((array[1].Length != 1) ? array[1].Substring(0, 2) : array[1]);
				}
				if (!providerTier.IsNullOrEmpty())
				{
					text = string.Format(format, providerTier.Substring(0, 2) + text2);
				}
			}
			return text;
		}

		private string GetAdCacheSlotStatus(AdCacheSlotInfo adCacheSlotInfo)
		{
			string text = ((!AdSystemManager.GetAvailabilityForCacheSlot(adCacheSlotInfo.id)) ? "n/a" : (AdSystemManager.IsCachedDebug(adCacheSlotInfo) ? "ready" : (AdSystemManager.HasPriorityListWrapped(adCacheSlotInfo.id) ? "pending" : ((!AdSystemManager.IsCachingDebug(adCacheSlotInfo)) ? "not cached" : "caching"))));
			return string.Format("{0}    {1}lp: {2}    cp: {3}    nsp: {4}", text, (!(AdSystemManager.GetLastPlayedCacheSlotID(adCacheSlotInfo.adType) == adCacheSlotInfo.id)) ? string.Empty : "*", TruncateProviderTier(AdSystemManager.GetLastShowedProviderForCacheSlot(adCacheSlotInfo)), TruncateProviderTier(AdSystemManager.GetCurrentProviderTierForCacheSlot(adCacheSlotInfo)), TruncateProviderTier(AdSystemManager.GetHighestPriorityCachedProviderTierForSlot(adCacheSlotInfo)));
		}

		private string CheckForJailbrokenDevice()
		{
			if (Directory.Exists("/bin/bash"))
			{
				return "Yes";
			}
			return "No";
		}

		private string GetSyboAnalyticsHashedDeviceId()
		{
			Func<string> getDeviceID = BaseSessionEvent.GetDeviceID;
			string result = "NA";
			if (getDeviceID != null)
			{
				result = getDeviceID();
			}
			return result;
		}

		private string GetSyboAnalyticsSamplingThreshold()
		{
			return OnlineSettings.instance.GetValue("sybo_analytics_sampling_threshold", 0f).ToString();
		}

		private string GetSyboAnalyticsEnabled()
		{
			return OnlineSettings.instance.GetValue("sybo_analytics_enabled", false).ToString();
		}

		private string GetRestrictionActive()
		{
			return (!AgeRestriction.isRestricted) ? "No" : "Yes";
		}

		private string GetKilooLoggerID()
		{
			string text = AnalyticsSave.flurryUserId;
			if (text.IsNullOrEmpty())
			{
				text = "None";
			}
			return text;
		}

		private string GetKilooTag()
		{
			string kilooTag = SaveManager.GetKilooTag();
			return (!string.IsNullOrEmpty(kilooTag)) ? kilooTag : "Not set";
		}

		private string GetOldKilooTags()
		{
			return SaveManager.GetOldKilooTags();
		}

		private string GetOnlineSaveStatus()
		{
			string text = CloudSave.state.ToString();
			if (CloudSave.state == CloudSaveState.Error)
			{
				text = text + ": " + CloudSave.errorState;
			}
			return text;
		}

		private string GetUserBirthday()
		{
			return AgeRestriction.GetUserBirthday().ToString("MM/yy");
		}

		private string GetApplicationInstaller()
		{
			string text = Application.installMode.ToString();
			string installerName = Application.installerName;
			if (!string.IsNullOrEmpty(installerName))
			{
				return text + " (" + installerName + ")";
			}
			return text;
		}

		private string GetAmountOfOwnedHoverboards()
		{
			int num = 0;
			foreach (Boards.BoardId item in Boards.boardOrder)
			{
				if (BoardManager.IsBoardOwned(item))
				{
					num++;
				}
			}
			return num.ToString();
		}

		private string GetAmountOfOwnedCharacters()
		{
			int num = 0;
			foreach (Characters.CharacterId item in Characters.characterOrder)
			{
				if (CharacterManager.IsCharacterOwned(item))
				{
					num++;
				}
			}
			return num.ToString();
		}

		private string GetABTestStatus()
		{
			string text = string.Empty;
			foreach (ABTestData.ABTestDataItem testDatum in ABTestData.testData)
			{
				if (ABTestManager.IsActive(testDatum.identifier))
				{
					if (!text.IsNullOrEmpty())
					{
						text += ", ";
					}
					text = text + testDatum.identifier + ":" + ABTestManager.GetPayloadString(testDatum.identifier);
				}
			}
			if (text.IsNullOrEmpty())
			{
				text = ((!ABTestManager.isInitialized) ? "Not initialized" : "None");
			}
			return text;
		}

		private string GetMigratedPlayerInfo()
		{
			return UserSave.hasMigratedPlayerInfo.ToString();
		}

		private string GetMigratedPlayerPrefsVersion()
		{
			return UserSave.migratedPlayerPrefsVersion.ToString();
		}

		public void CopyToClipboard()
		{
			_infoToBeCopiedToClipboard = new StringBuilder();
			foreach (object value in Enum.GetValues(typeof(TabNumber)))
			{
				foreach (PageContent item in _contentDict[(TabNumber)value])
				{
					if (item.infoType == SecretCellType.Info)
					{
						_infoToBeCopiedToClipboard.AppendLine(item.key + ": " + item.value());
						continue;
					}
					_infoToBeCopiedToClipboard.AppendLine(item.key + ": " + item.value());
					_infoToBeCopiedToClipboard.AppendLine(item.key2 + ": " + item.value2());
				}
			}
			DeviceUtility.SetDeviceClipBoard(_infoToBeCopiedToClipboard.ToString());
			StartCoroutine(ShowTextCopied());
		}

		public void CopyAppFileInformationToClipboard()
		{
			_files.Clear();
			fileInfoStringBuilder.Length = 0;
			AppendSection("User Data", DeviceUtility.GetUserDataPath(), FileLogType.Data);
			if (IsAndroidDevice())
			{
				AppendSection("Android External", DeviceUtility.GetAndroidExternalSavePath(), FileLogType.Data);
				AppendSection("Temporary Cache", Application.temporaryCachePath, FileLogType.Cache);
			}
			AppendSection("Cache", DeviceUtility.GetCachePath(), FileLogType.Cache);
			if (IsiOSDevice())
			{
				AppendSection("iOS tmp", DeviceUtility.GetUserTempPathiOS(), FileLogType.Cache);
			}
			fileInfoStringBuilder.AppendLine("Total overall size is " + BytesToMegabytes(_totalSize) + " MB");
			DeviceUtility.SetDeviceClipBoard(fileInfoStringBuilder.ToString());
			_totalSize = 0L;
		}

		private void AppendSection(string sectionName, string path, FileLogType fileSectionType)
		{
			if (Directory.Exists(path))
			{
				WalkDirectoryTree(new DirectoryInfo(path), fileSectionType);
				_sectionSize = 0L;
			}
		}

		private static string BytesToMegabytes(double len)
		{
			return string.Format("{0:0.00}", len / 1000.0 / 1000.0);
		}

		private void WalkDirectoryTree(DirectoryInfo directoryInfo, FileLogType fileSectionType)
		{
			FileInfo[] array = null;
			DirectoryInfo[] array2 = null;
			try
			{
				array = directoryInfo.GetFiles("*.*");
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (DirectoryNotFoundException)
			{
			}
			if (array != null)
			{
				FileInfo[] array3 = array;
				foreach (FileInfo fileInfo in array3)
				{
					_sectionSize += fileInfo.Length;
					_totalSize += fileInfo.Length;
					DateTime creationTime = File.GetCreationTime(fileInfo.FullName);
					DateTime lastWriteTime = File.GetLastWriteTime(fileInfo.FullName);
					fileInfoStringBuilder.AppendLine(string.Concat(fileSectionType, ",", fileInfo.FullName, ",", creationTime.ToString("yyyy-MM-dd"), ",", lastWriteTime.ToString("yyyy-MM-dd"), ",", fileInfo.Length));
				}
				array2 = directoryInfo.GetDirectories();
				DirectoryInfo[] array4 = array2;
				foreach (DirectoryInfo directoryInfo2 in array4)
				{
					WalkDirectoryTree(directoryInfo2, fileSectionType);
				}
			}
		}
	}
}
