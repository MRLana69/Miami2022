using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Data.Managers;
using Data.Save;
using InAppPurchase;
using InAppPurchase.Internal;
using Info;
using JetBrains.Annotations;
using Kiloo;
using Kiloo.Analytics;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using SYBO.Common.Utilities;
using SYBO.Subway.Info;
using UI.Screens;
using UI.SlideIns;
using UnityEngine;

namespace InApp
{
	public class InAppManager : MonoBehaviour, IInAppManager
	{
		public delegate void OnProductRequestDone();

		public delegate void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult);

		public delegate void OnPurchaseFailure(string productId, PurchaseResult result);

		public delegate void OnPurchaseCanceled(string productId, PurchaseResult result);

		public delegate void OnPurchaseItemNotAvailable(string productId, PurchaseResult result);

		public delegate void OnInitializeFinished(bool success);

		public delegate void OnItemRestored(string productId);

		public delegate void OnItemRestoreFinished(RestoreFinishedResult result);

		public delegate void OnItemRefunded(string productId);

		public delegate void OnValidatedItemRestored();

		private const string ONLINESETTINGS_IAP_VALIDATION_URL_KEY = "iap_validation_url";

		private const string DEFAULT_IAP_VALIDATION_URL = "https://purchasevalidation.kiloo-games.com/";

		private const string ONLINESETTINGS_IAP_VALIDATION_ENABLED = "iap_validation_enabled";

		private const string ONLINESETTINGS_DISCOUNT_END_TIME_KEY = "discount_end_time";

		private const double DEFAULT_DISCOUNT_END_TIME = 0.0;

		private const bool DEFAULT_IAP_VALIDATION_ENABLED = true;

		private const string ONLINESETTINGS_DISCOUNT_SPECIAL_SALE_WINTER_HOLIDAY_ACTIVE = "winter_holiday_sale_active";

		private const string ONLINESETTINGS_DISCOUNT_SPECIAL_SALE_NEW_YEAR_ACTIVE = "new_year_sale_active";

		private const string ONLINESETTINGS_DISCOUNT_SPECIAL_SALE_BLACK_FRIDAY = "black_friday_sale_active";

		private const bool DEFAULT_DISCOUNT_SPECIAL_SALE_ACTIVE = false;

		private static readonly Dictionary<string, int> _debugDiscount = new Dictionary<string, int>
		{
			{ "in_app_tier_1", 7500 },
			{ "in_app_tier_2", 45000 },
			{ "in_app_tier_2_5", 90000 },
			{ "in_app_tier_3", 180000 },
			{ "in_app_tier_4", 500000 },
			{ "in_app_tier_5", 1200000 },
			{ "in_app_tier_6", 25 },
			{ "in_app_tier_6_5", 50 },
			{ "in_app_tier_7", 110 },
			{ "in_app_tier_8", 300 },
			{ "in_app_tier_9", 650 }
		};

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnProductRequestDone onProductRequestDone__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnPurchaseSuccess onPurchaseSuccess__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnPurchaseFailure onPurchaseFailure__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnPurchaseCanceled onPurchaseCanceled__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnPurchaseItemNotAvailable onPurchaseItemNotAvailable__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnItemRestored onItemRestored__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnValidatedItemRestored onValidatedItemRestored__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnItemRefunded onItemRefunded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnItemRestoreFinished onItemRestoreFinished__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static OnInitializeFinished onInitializeFinished__BackingField;

		private static InAppManager _instance;

		private static PurchaseHandler _purchaseHandler;

		private string _itemRequested = string.Empty;

		private bool _productRequestSucceeded;

		private static bool _isRestoredManually = false;

		private const string GAME_OBJECT_NAME = "InAppManager";

		private InAppManagerPopupData _popupData;

		[NonSerialized]
		public AudioClip purchaseSuccessSound;

		private IPurchaseValidationServer _purchaseValidationServer;

		private IPurchaseValidationServer _simulatedPurchaseValidationServer;

		private static bool _hasSuccessfullyInitialized = false;

		private static bool _applicationIsQuiting = false;

		private bool _isQueryingProductInfo;

		private static readonly int[] SEGMENT_78S9H5S6D78 = new int[78]
		{
			90, 74, 114, 81, 56, 97, 55, 118, 51, 70,
			116, 83, 69, 78, 78, 65, 72, 113, 106, 56,
			118, 49, 48, 52, 83, 110, 114, 99, 43, 52,
			73, 67, 71, 71, 105, 120, 113, 89, 108, 102,
			55, 51, 109, 50, 106, 108, 68, 89, 106, 84,
			70, 53, 78, 48, 102, 75, 102, 87, 105, 117,
			106, 121, 74, 114, 110, 76, 57, 43, 70, 69,
			79, 89, 79, 117, 100, 103, 119, 81
		};

		private static readonly int[] SEGMENT_8865FHJ76FG = new int[79]
		{
			65, 111, 68, 85, 105, 101, 112, 78, 70, 51,
			76, 68, 72, 79, 89, 85, 68, 99, 67, 107,
			103, 99, 100, 105, 54, 72, 75, 48, 83, 83,
			106, 52, 70, 114, 87, 87, 57, 104, 114, 114,
			104, 120, 77, 106, 82, 120, 108, 77, 110, 104,
			75, 117, 75, 80, 50, 110, 49, 119, 115, 55,
			98, 52, 99, 119, 79, 106, 77, 80, 67, 97,
			90, 85, 119, 73, 68, 65, 81, 65, 66
		};

		private static readonly int[] SEGMENT_8907SDFGF56 = new int[78]
		{
			67, 120, 57, 113, 109, 114, 73, 66, 98, 73,
			67, 110, 111, 100, 113, 101, 99, 78, 74, 101,
			87, 110, 120, 119, 117, 116, 117, 105, 70, 100,
			81, 50, 47, 87, 79, 119, 67, 49, 98, 113,
			72, 86, 86, 119, 115, 56, 81, 122, 43, 75,
			81, 78, 113, 81, 56, 54, 103, 70, 81, 82,
			77, 49, 107, 66, 76, 90, 97, 69, 103, 56,
			116, 108, 53, 110, 50, 87, 115, 69
		};

		private static readonly int[] SEGMENT_0A8ASDF7EWF = new int[80]
		{
			77, 73, 73, 66, 73, 106, 65, 78, 66, 103,
			107, 113, 104, 107, 105, 71, 57, 119, 48, 66,
			65, 81, 69, 70, 65, 65, 79, 67, 65, 81,
			56, 65, 77, 73, 73, 66, 67, 103, 75, 67,
			65, 81, 69, 65, 110, 86, 51, 86, 53, 72,
			116, 52, 85, 56, 100, 82, 50, 82, 47, 82,
			55, 70, 118, 107, 52, 97, 73, 121, 78, 116,
			117, 84, 90, 107, 57, 48, 100, 114, 117, 108
		};

		private static readonly int[] SEGMENT_5S678DF5G6S = new int[77]
		{
			100, 100, 113, 74, 55, 84, 43, 47, 84, 104,
			101, 89, 116, 109, 105, 109, 111, 121, 69, 101,
			57, 120, 120, 88, 114, 56, 118, 66, 48, 75,
			121, 110, 88, 101, 122, 51, 82, 119, 73, 121,
			85, 106, 118, 98, 86, 68, 87, 87, 55, 116,
			108, 65, 98, 85, 106, 82, 79, 101, 73, 69,
			76, 68, 85, 87, 77, 107, 65, 82, 102, 68,
			67, 101, 109, 56, 120, 108, 98
		};

		private static string[] _mostPopularStickerProductIds;

		private const string ONLINESETTINGS_MOST_POPULAR_STICKER_PRODUCT_ID_WITHOUT_PREFIX = "most_popular_sticker_product_id_without_prefix";

		private static readonly string[] MOST_POPULAR_STICKER_DEFAULT_PRODUCT_ID_WITHOUT_PREFIX = new string[2]
		{
			InAppData.StripProductPrefixes("com.kiloo.subways.coinstier2.5"),
			InAppData.StripProductPrefixes("com.kiloo.subways.keystier1.5")
		};

		public string itemRequested
		{
			get
			{
				return _itemRequested;
			}
		}

		public static bool isInitialized
		{
			get
			{
				return _instance != null && _hasSuccessfullyInitialized;
			}
		}

		[NotNull]
		public static InAppManager instance
		{
			get
			{
				Init();
				return _instance;
			}
		}

		public static bool IsAnyDiscountAvailable
		{
			get
			{
				double discountEndTime = GetDiscountEndTime();
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(discountEndTime);
				if ((dateTime - CDateTime.UtcNow).TotalSeconds > 0.0)
				{
					return true;
				}
				return false;
			}
		}

		public static bool IsDoubleCoinsDiscountActive
		{
			get
			{
				if (!UserSave.hasDoubleCoinsUpgrade && IsAnyDiscountAvailable && GetDiscountValue(InAppData.GetInAppOnlineSettingsId("com.kiloo.subways.doublecoins"), 0) < 0 && InAppData.inAppData["com.kiloo.subways.doublecoinsdiscount"].isValidated)
				{
					return true;
				}
				return false;
			}
		}

		public static bool IsAnyCurrencyDiscountActive
		{
			get
			{
				string[] currencyTiers = InAppData.GetCurrencyTiers();
				foreach (string text in currencyTiers)
				{
					if (!IsAnyDiscountAvailable)
					{
						continue;
					}
					string inAppOnlineSettingsId = InAppData.GetInAppOnlineSettingsId(text);
					if (!string.IsNullOrEmpty(inAppOnlineSettingsId) && IsDiscountActive(inAppOnlineSettingsId))
					{
						string discountIdForNormalOffer = InAppData.GetDiscountIdForNormalOffer(text);
						if (InAppData.inAppData[discountIdForNormalOffer].isValidated)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public static bool IsSpecialSaleActive
		{
			get
			{
				return IsWinterHolidaySpecialSaleActive || IsNewYearSpecialSaleActive || IsBlackFridaySpecialSaleActive;
			}
		}

		public static bool IsWinterHolidaySpecialSaleActive
		{
			get
			{
				bool flag = OnlineSettings.instance.GetValue("winter_holiday_sale_active", false) || Globals.DEBUG_ENABLE_HOLIDAY_SALES;
				return IsAnyDiscountAvailable && flag;
			}
		}

		public static bool IsNewYearSpecialSaleActive
		{
			get
			{
				bool flag = OnlineSettings.instance.GetValue("new_year_sale_active", false) || Globals.DEBUG_ENABLE_NEW_YEAR_SALES;
				return IsAnyDiscountAvailable && flag;
			}
		}

		public static bool IsBlackFridaySpecialSaleActive
		{
			get
			{
				bool flag = OnlineSettings.instance.GetValue("black_friday_sale_active", false) || Globals.DEBUG_ENABLE_BLACK_FRIDAY_SALES;
				return IsAnyDiscountAvailable && flag;
			}
		}

		public event OnProductRequestDone onProductRequestDone
		{
			add
			{
				OnProductRequestDone onProductRequestDone = onProductRequestDone__BackingField;
				OnProductRequestDone onProductRequestDone2;
				do
				{
					onProductRequestDone2 = onProductRequestDone;
					onProductRequestDone = Interlocked.CompareExchange(ref onProductRequestDone__BackingField, (OnProductRequestDone)Delegate.Combine(onProductRequestDone2, value), onProductRequestDone);
				}
				while ((object)onProductRequestDone != onProductRequestDone2);
			}
			remove
			{
				OnProductRequestDone onProductRequestDone = onProductRequestDone__BackingField;
				OnProductRequestDone onProductRequestDone2;
				do
				{
					onProductRequestDone2 = onProductRequestDone;
					onProductRequestDone = Interlocked.CompareExchange(ref onProductRequestDone__BackingField, (OnProductRequestDone)Delegate.Remove(onProductRequestDone2, value), onProductRequestDone);
				}
				while ((object)onProductRequestDone != onProductRequestDone2);
			}
		}

		public event OnPurchaseSuccess onPurchaseSuccess
		{
			add
			{
				OnPurchaseSuccess onPurchaseSuccess = onPurchaseSuccess__BackingField;
				OnPurchaseSuccess onPurchaseSuccess2;
				do
				{
					onPurchaseSuccess2 = onPurchaseSuccess;
					onPurchaseSuccess = Interlocked.CompareExchange(ref onPurchaseSuccess__BackingField, (OnPurchaseSuccess)Delegate.Combine(onPurchaseSuccess2, value), onPurchaseSuccess);
				}
				while ((object)onPurchaseSuccess != onPurchaseSuccess2);
			}
			remove
			{
				OnPurchaseSuccess onPurchaseSuccess = onPurchaseSuccess__BackingField;
				OnPurchaseSuccess onPurchaseSuccess2;
				do
				{
					onPurchaseSuccess2 = onPurchaseSuccess;
					onPurchaseSuccess = Interlocked.CompareExchange(ref onPurchaseSuccess__BackingField, (OnPurchaseSuccess)Delegate.Remove(onPurchaseSuccess2, value), onPurchaseSuccess);
				}
				while ((object)onPurchaseSuccess != onPurchaseSuccess2);
			}
		}

		public event OnPurchaseFailure onPurchaseFailure
		{
			add
			{
				OnPurchaseFailure onPurchaseFailure = onPurchaseFailure__BackingField;
				OnPurchaseFailure onPurchaseFailure2;
				do
				{
					onPurchaseFailure2 = onPurchaseFailure;
					onPurchaseFailure = Interlocked.CompareExchange(ref onPurchaseFailure__BackingField, (OnPurchaseFailure)Delegate.Combine(onPurchaseFailure2, value), onPurchaseFailure);
				}
				while ((object)onPurchaseFailure != onPurchaseFailure2);
			}
			remove
			{
				OnPurchaseFailure onPurchaseFailure = onPurchaseFailure__BackingField;
				OnPurchaseFailure onPurchaseFailure2;
				do
				{
					onPurchaseFailure2 = onPurchaseFailure;
					onPurchaseFailure = Interlocked.CompareExchange(ref onPurchaseFailure__BackingField, (OnPurchaseFailure)Delegate.Remove(onPurchaseFailure2, value), onPurchaseFailure);
				}
				while ((object)onPurchaseFailure != onPurchaseFailure2);
			}
		}

		public event OnPurchaseCanceled onPurchaseCanceled
		{
			add
			{
				OnPurchaseCanceled onPurchaseCanceled = onPurchaseCanceled__BackingField;
				OnPurchaseCanceled onPurchaseCanceled2;
				do
				{
					onPurchaseCanceled2 = onPurchaseCanceled;
					onPurchaseCanceled = Interlocked.CompareExchange(ref onPurchaseCanceled__BackingField, (OnPurchaseCanceled)Delegate.Combine(onPurchaseCanceled2, value), onPurchaseCanceled);
				}
				while ((object)onPurchaseCanceled != onPurchaseCanceled2);
			}
			remove
			{
				OnPurchaseCanceled onPurchaseCanceled = onPurchaseCanceled__BackingField;
				OnPurchaseCanceled onPurchaseCanceled2;
				do
				{
					onPurchaseCanceled2 = onPurchaseCanceled;
					onPurchaseCanceled = Interlocked.CompareExchange(ref onPurchaseCanceled__BackingField, (OnPurchaseCanceled)Delegate.Remove(onPurchaseCanceled2, value), onPurchaseCanceled);
				}
				while ((object)onPurchaseCanceled != onPurchaseCanceled2);
			}
		}

		public event OnPurchaseItemNotAvailable onPurchaseItemNotAvailable
		{
			add
			{
				OnPurchaseItemNotAvailable onPurchaseItemNotAvailable = onPurchaseItemNotAvailable__BackingField;
				OnPurchaseItemNotAvailable onPurchaseItemNotAvailable2;
				do
				{
					onPurchaseItemNotAvailable2 = onPurchaseItemNotAvailable;
					onPurchaseItemNotAvailable = Interlocked.CompareExchange(ref onPurchaseItemNotAvailable__BackingField, (OnPurchaseItemNotAvailable)Delegate.Combine(onPurchaseItemNotAvailable2, value), onPurchaseItemNotAvailable);
				}
				while ((object)onPurchaseItemNotAvailable != onPurchaseItemNotAvailable2);
			}
			remove
			{
				OnPurchaseItemNotAvailable onPurchaseItemNotAvailable = onPurchaseItemNotAvailable__BackingField;
				OnPurchaseItemNotAvailable onPurchaseItemNotAvailable2;
				do
				{
					onPurchaseItemNotAvailable2 = onPurchaseItemNotAvailable;
					onPurchaseItemNotAvailable = Interlocked.CompareExchange(ref onPurchaseItemNotAvailable__BackingField, (OnPurchaseItemNotAvailable)Delegate.Remove(onPurchaseItemNotAvailable2, value), onPurchaseItemNotAvailable);
				}
				while ((object)onPurchaseItemNotAvailable != onPurchaseItemNotAvailable2);
			}
		}

		public event OnItemRestored onItemRestored
		{
			add
			{
				OnItemRestored onItemRestored = onItemRestored__BackingField;
				OnItemRestored onItemRestored2;
				do
				{
					onItemRestored2 = onItemRestored;
					onItemRestored = Interlocked.CompareExchange(ref onItemRestored__BackingField, (OnItemRestored)Delegate.Combine(onItemRestored2, value), onItemRestored);
				}
				while ((object)onItemRestored != onItemRestored2);
			}
			remove
			{
				OnItemRestored onItemRestored = onItemRestored__BackingField;
				OnItemRestored onItemRestored2;
				do
				{
					onItemRestored2 = onItemRestored;
					onItemRestored = Interlocked.CompareExchange(ref onItemRestored__BackingField, (OnItemRestored)Delegate.Remove(onItemRestored2, value), onItemRestored);
				}
				while ((object)onItemRestored != onItemRestored2);
			}
		}

		public event OnValidatedItemRestored onValidatedItemRestored
		{
			add
			{
				OnValidatedItemRestored onValidatedItemRestored = onValidatedItemRestored__BackingField;
				OnValidatedItemRestored onValidatedItemRestored2;
				do
				{
					onValidatedItemRestored2 = onValidatedItemRestored;
					onValidatedItemRestored = Interlocked.CompareExchange(ref onValidatedItemRestored__BackingField, (OnValidatedItemRestored)Delegate.Combine(onValidatedItemRestored2, value), onValidatedItemRestored);
				}
				while ((object)onValidatedItemRestored != onValidatedItemRestored2);
			}
			remove
			{
				OnValidatedItemRestored onValidatedItemRestored = onValidatedItemRestored__BackingField;
				OnValidatedItemRestored onValidatedItemRestored2;
				do
				{
					onValidatedItemRestored2 = onValidatedItemRestored;
					onValidatedItemRestored = Interlocked.CompareExchange(ref onValidatedItemRestored__BackingField, (OnValidatedItemRestored)Delegate.Remove(onValidatedItemRestored2, value), onValidatedItemRestored);
				}
				while ((object)onValidatedItemRestored != onValidatedItemRestored2);
			}
		}

		public event OnItemRefunded onItemRefunded
		{
			add
			{
				OnItemRefunded onItemRefunded = onItemRefunded__BackingField;
				OnItemRefunded onItemRefunded2;
				do
				{
					onItemRefunded2 = onItemRefunded;
					onItemRefunded = Interlocked.CompareExchange(ref onItemRefunded__BackingField, (OnItemRefunded)Delegate.Combine(onItemRefunded2, value), onItemRefunded);
				}
				while ((object)onItemRefunded != onItemRefunded2);
			}
			remove
			{
				OnItemRefunded onItemRefunded = onItemRefunded__BackingField;
				OnItemRefunded onItemRefunded2;
				do
				{
					onItemRefunded2 = onItemRefunded;
					onItemRefunded = Interlocked.CompareExchange(ref onItemRefunded__BackingField, (OnItemRefunded)Delegate.Remove(onItemRefunded2, value), onItemRefunded);
				}
				while ((object)onItemRefunded != onItemRefunded2);
			}
		}

		public event OnItemRestoreFinished onItemRestoreFinished
		{
			add
			{
				OnItemRestoreFinished onItemRestoreFinished = onItemRestoreFinished__BackingField;
				OnItemRestoreFinished onItemRestoreFinished2;
				do
				{
					onItemRestoreFinished2 = onItemRestoreFinished;
					onItemRestoreFinished = Interlocked.CompareExchange(ref onItemRestoreFinished__BackingField, (OnItemRestoreFinished)Delegate.Combine(onItemRestoreFinished2, value), onItemRestoreFinished);
				}
				while ((object)onItemRestoreFinished != onItemRestoreFinished2);
			}
			remove
			{
				OnItemRestoreFinished onItemRestoreFinished = onItemRestoreFinished__BackingField;
				OnItemRestoreFinished onItemRestoreFinished2;
				do
				{
					onItemRestoreFinished2 = onItemRestoreFinished;
					onItemRestoreFinished = Interlocked.CompareExchange(ref onItemRestoreFinished__BackingField, (OnItemRestoreFinished)Delegate.Remove(onItemRestoreFinished2, value), onItemRestoreFinished);
				}
				while ((object)onItemRestoreFinished != onItemRestoreFinished2);
			}
		}

		public static event OnInitializeFinished onInitializeFinished
		{
			add
			{
				OnInitializeFinished onInitializeFinished = onInitializeFinished__BackingField;
				OnInitializeFinished onInitializeFinished2;
				do
				{
					onInitializeFinished2 = onInitializeFinished;
					onInitializeFinished = Interlocked.CompareExchange(ref onInitializeFinished__BackingField, (OnInitializeFinished)Delegate.Combine(onInitializeFinished2, value), onInitializeFinished);
				}
				while ((object)onInitializeFinished != onInitializeFinished2);
			}
			remove
			{
				OnInitializeFinished onInitializeFinished = onInitializeFinished__BackingField;
				OnInitializeFinished onInitializeFinished2;
				do
				{
					onInitializeFinished2 = onInitializeFinished;
					onInitializeFinished = Interlocked.CompareExchange(ref onInitializeFinished__BackingField, (OnInitializeFinished)Delegate.Remove(onInitializeFinished2, value), onInitializeFinished);
				}
				while ((object)onInitializeFinished != onInitializeFinished2);
			}
		}

		private InAppManager()
		{
			if (!OnlineSettings.instance.GetValue("iap_validation_enabled", true))
			{
				return;
			}
			string value = OnlineSettings.instance.GetValue("iap_validation_url", "https://purchasevalidation.kiloo-games.com/");
			try
			{
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					_purchaseValidationServer = new PurchaseValidationServer(new Uri(value), new KilooSerializer(), Globals.DEBUG);
				}
				else
				{
					_purchaseValidationServer = new PurchaseValidationServer(new Uri(value), new KilooSerializer());
				}
			}
			catch (Exception ex)
			{
				LogError(ex.Message);
			}
		}

		public static void Init()
		{
			if (_instance == null && !_applicationIsQuiting)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "InAppManager";
				GameObject gameObject2 = gameObject;
				UnityEngine.Object.DontDestroyOnLoad(gameObject2);
				gameObject2.AddComponent<InAppManager>();
				HierarchyUtility.ReparentByHierarchyPathType(gameObject2.transform, HierarchyPathType.Dynamic);
				HierarchyUtility.SetAsLastSibling(gameObject2.transform);
			}
		}

		private void Awake()
		{
			if (_instance != null)
			{
				UnityEngine.Object.DestroyImmediate(this);
				return;
			}
			_instance = this;
			InAppPurchaseProvider.instance.Init(this, _purchaseValidationServer);
		}

		[UsedImplicitly]
		private void OnApplicationQuit()
		{
			_applicationIsQuiting = true;
			_instance = null;
		}

		public void RequestIAPInfo()
		{
			if (_isQueryingProductInfo)
			{
				return;
			}
			_isQueryingProductInfo = true;
			foreach (KeyValuePair<string, InAppItemModel> inAppDatum in InAppData.inAppData)
			{
				inAppDatum.Value.isValidated = false;
			}
			InAppPurchaseProvider.instance.RequestProductInfo(InAppData.GetAllProductIds());
		}

		public void StartPurchase(string productId)
		{
			if (Globals.DEBUG_FREE_INAPP_PURCHASE)
			{
				PayoutInAppPurchase(productId);
				if (_simulatedPurchaseValidationServer == null)
				{
					_simulatedPurchaseValidationServer = new SimulatedPurchaseValidationServer();
				}
				_simulatedPurchaseValidationServer.ValidateTransaction(productId, "DummyPurchaseReceipt", (IPurchaseValidationResult purchaseValidationResult) =>
				{
					if (onPurchaseSuccess__BackingField != null)
					{
						onPurchaseSuccess__BackingField(productId, false, purchaseValidationResult);
					}
				});
			}
			else
			{
				if (UIScreenController.IsInstanced)
				{
					UIScreenController.Instance.ShowInAppPurchaseOverlay(GetContactingStoreOverlayText());
				}
				InAppPurchaseProvider.instance.StartPurchase(productId);
			}
		}

		public bool RestoreNonConsumablePurchases(string[] productIds, bool calledManually = true)
		{
			if (calledManually && UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.ShowInAppPurchaseOverlay(GetContactingStoreOverlayText());
			}
			_isRestoredManually = calledManually;
			return InAppPurchaseProvider.instance.RestoreNonConsumablePurchases(productIds);
		}

		public bool ProductRequestSucceeded()
		{
			return _productRequestSucceeded;
		}

		public void InitFinishedCallback(bool success)
		{
			_hasSuccessfullyInitialized = success;
			OnInitializeFinished onInitializeFinished = onInitializeFinished__BackingField;
			if (onInitializeFinished != null)
			{
				onInitializeFinished(success);
			}
			if (success)
			{
				RequestIAPInfo();
			}
		}

		public string getInAppAPIKey()
		{
			StringBuilder stringBuilder = new StringBuilder();
			AddSegment(stringBuilder, SEGMENT_0A8ASDF7EWF);
			AddSegment(stringBuilder, SEGMENT_78S9H5S6D78);
			AddSegment(stringBuilder, SEGMENT_8907SDFGF56);
			AddSegment(stringBuilder, SEGMENT_5S678DF5G6S);
			AddSegment(stringBuilder, SEGMENT_8865FHJ76FG);
			return stringBuilder.ToString();
		}

		private void AddSegment(StringBuilder sb, int[] section)
		{
			for (int i = 0; i < section.Length; i++)
			{
				sb.Append(Convert.ToChar(section[i]));
			}
		}

		public IsIAPConsumableValue IsProductConsumable(string productID)
		{
			return (!InAppData.inAppData[productID].isConsumable) ? IsIAPConsumableValue.NotConsumable : IsIAPConsumableValue.Consumable;
		}

		public void ProductInfoRequestCallback(bool success, ProductRequestResultInfo[] productInfo)
		{
			if (success)
			{
				_productRequestSucceeded = true;
				for (int i = 0; i < productInfo.Length; i++)
				{
					ProductRequestResultInfo productRequestResultInfo = productInfo[i];
					string productId = productRequestResultInfo.productId;
					if (InAppData.inAppData.ContainsKey(productId))
					{
						InAppData.inAppData[productId].isValidated = true;
						InAppData.inAppData[productId].localizedPrice = productRequestResultInfo.localizedPrice;
						if (!string.IsNullOrEmpty(InAppData.inAppData[productId].localizedPrice))
						{
							InAppData.inAppData[productId].localizedPrice = Regex.Replace(InAppData.inAppData[productId].localizedPrice, "\\u00A0", " ");
						}
						else
						{
							LogError("InAppManager.ProductInfoRequestCallback(): Null or Empty Localized Price");
						}
						continue;
					}
					InAppItemModel inAppItemModel = new InAppItemModel();
					inAppItemModel.isDynamic = true;
					inAppItemModel.localizedPrice = productRequestResultInfo.localizedPrice;
					InAppItemModel inAppItemModel2 = inAppItemModel;
					if (!string.IsNullOrEmpty(inAppItemModel2.localizedPrice))
					{
						inAppItemModel2.localizedPrice = Regex.Replace(inAppItemModel2.localizedPrice, "\\u00A0", " ");
					}
					else
					{
						LogError("InAppManager.ProductInfoRequestCallback(): Null or Empty Localized Price");
					}
					inAppItemModel2.isValidated = true;
					InAppData.inAppData.Add(productId, inAppItemModel2);
				}
			}
			else
			{
				_productRequestSucceeded = false;
			}
			OnProductRequestDone onProductRequestDone = onProductRequestDone__BackingField;
			if (onProductRequestDone != null)
			{
				onProductRequestDone();
			}
			_isQueryingProductInfo = false;
		}

		public void PurchaseRequestCallback(string productId, string transactionId, string transactionIdBase64, string cookie, PurchaseResult result, IPurchaseValidationResult validationResult)
		{
			switch (result)
			{
			case PurchaseResult.Completed:
			{
				bool flag = TryAddTransactionToHistory(transactionId, productId);
				bool flag2 = false;
				if (Application.platform == RuntimePlatform.Android && !string.IsNullOrEmpty(transactionIdBase64))
				{
					flag2 = TryAddTransactionToHistory(transactionIdBase64, productId);
				}
				if (!flag && !flag2)
				{
					break;
				}
				PayoutInAppPurchase(productId);
				InAppPurchaseProvider.instance.FinishPurchase(productId, transactionId, transactionIdBase64);
				bool flag3 = validationResult != null && validationResult.receiptStatus == ReceiptStatus.Valid;
				if (!Application.isEditor)
				{
					Flurry.LogInAppPurchase(productId, _itemRequested, flag3);
					if (flag3)
					{
						if (!UserSave.hasMadeOneValidPurchase)
						{
							Manager.LogEvent(Kiloo.Analytics.EventType.SingleValidPurchase, true);
							Flurry.LogEventWithAParameter("First Validated Purchase", "Product ID", productId);
							UserSave.hasMadeOneValidPurchase = true;
						}
						Kiloo.Analytics.EventType eventType = Kiloo.Analytics.EventType.MadeValidPurchase;
						Manager.LogEvent(eventType, false, productId);
					}
					_itemRequested = string.Empty;
				}
				OnPurchaseSuccess onPurchaseSuccess = onPurchaseSuccess__BackingField;
				if (onPurchaseSuccess != null)
				{
					onPurchaseSuccess(productId, false, validationResult);
				}
				break;
			}
			case PurchaseResult.Canceled:
			{
				PurchaseCanceled(productId);
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					InAppPurchaseProvider.instance.FinishPurchase(productId, transactionId, transactionIdBase64);
				}
				OnPurchaseCanceled onPurchaseCanceled = onPurchaseCanceled__BackingField;
				if (onPurchaseCanceled != null)
				{
					onPurchaseCanceled(productId, result);
				}
				break;
			}
			case PurchaseResult.ItemUnavailable:
			{
				PurchaseItemUnavailable(productId);
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					InAppPurchaseProvider.instance.FinishPurchase(productId, transactionId, transactionIdBase64);
				}
				OnPurchaseItemNotAvailable onPurchaseItemNotAvailable = onPurchaseItemNotAvailable__BackingField;
				if (onPurchaseItemNotAvailable != null)
				{
					onPurchaseItemNotAvailable(productId, result);
				}
				break;
			}
			default:
			{
				PurchaseFailed(productId);
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					InAppPurchaseProvider.instance.FinishPurchase(productId, transactionId, transactionIdBase64);
				}
				OnPurchaseFailure onPurchaseFailure = onPurchaseFailure__BackingField;
				if (onPurchaseFailure != null)
				{
					onPurchaseFailure(productId, result);
				}
				break;
			}
			}
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.HideInAppPurchaseOverlay();
			}
		}

		public void ItemRestoredCallback(string productId, string transactionId, string cookie)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				InAppPurchaseProvider.instance.FinishPurchase(productId, transactionId, null);
			}
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.DelayedHideInAppPurchaseOverlay();
			}
			InvokeRestoreEvents(productId);
		}

		private void InvokeRestoreEvents(string productId)
		{
			PurchaseRestored(productId);
			OnItemRestored onItemRestored = onItemRestored__BackingField;
			if (onItemRestored != null)
			{
				onItemRestored(productId);
			}
			OnPurchaseSuccess onPurchaseSuccess = onPurchaseSuccess__BackingField;
			if (onPurchaseSuccess != null)
			{
				onPurchaseSuccess(productId, true, null);
			}
		}

		public void CheckedItemRestoredCallback(string productId, string transactionId, string cookie, IPurchaseValidationResult validationResult)
		{
			if (validationResult.receiptStatus == ReceiptStatus.Valid && !_purchaseValidationServer.HasDetectedAnyInvalidPurchases())
			{
				Flurry.LogEventWithAParameter("Restored item", "productID", productId);
				if (onValidatedItemRestored__BackingField != null)
				{
					onValidatedItemRestored__BackingField();
				}
			}
		}

		public void ItemRefundedCallback(string productid, string transactionid, string cookie, IPurchaseValidationResult validationResult)
		{
			OnItemRefunded onItemRefunded = onItemRefunded__BackingField;
			if (onItemRefunded != null)
			{
				onItemRefunded(productid);
			}
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.HideInAppPurchaseOverlay();
			}
		}

		public void RestoreRequestFinishedCallback(RestoreFinishedResult result)
		{
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.HideInAppPurchaseOverlay();
			}
			switch (result)
			{
			case RestoreFinishedResult.Completed:
				if (_purchaseValidationServer.HasDetectedAnyInvalidPurchases())
				{
					result = RestoreFinishedResult.ValidationFailed;
				}
				break;
			case RestoreFinishedResult.Failed:
				if (_isRestoredManually)
				{
					UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get("SLIDEIN_RESTORE_NETWORK_PROBLEM"));
				}
				break;
			case RestoreFinishedResult.NothingToRestore:
				if (_isRestoredManually)
				{
					UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get("SLIDEIN_RESTORE_NO_PREVIOUS_PURCHASES"));
				}
				break;
			}
			OnItemRestoreFinished onItemRestoreFinished = onItemRestoreFinished__BackingField;
			if (onItemRestoreFinished != null)
			{
				onItemRestoreFinished(result);
			}
			_isRestoredManually = false;
		}

		private void PurchaseRestored(string productId)
		{
			if (productId == "com.kiloo.subways.doublecoins" || productId == "com.kiloo.subways.doublecoinsdiscount")
			{
				if (!UserSave.hasDoubleCoinsUpgrade)
				{
					UserSave.hasDoubleCoinsUpgrade = true;
					if (UIScreenController.IsInstanced)
					{
						UIScreenController.Instance.QueueSlideIn(new SlideIn
						{
							type = SlideInData.SlideInType.Unlock,
							stringValue = Strings.Get("INAPP_DATA_DOUBLE_COINS")
						});
					}
					UserSave.inAppRestoredPurchaseCount++;
				}
			}
			else if (InAppData.GetAdRemovalTiers().Contains(productId))
			{
				if (!UserSave.hasAdRemovalUpgrade)
				{
					int payoutAmount = InAppData.inAppData[productId].payoutAmount;
					InventorySave.IncreaseKeys(payoutAmount, productId);
					if (UIScreenController.IsInstanced)
					{
						UIScreenController.Instance.QueueSlideIn(new SlideIn
						{
							type = SlideInData.SlideInType.RemoveAds
						});
						UIScreenController.Instance.QueueSlideIn(new SlideIn
						{
							type = SlideInData.SlideInType.Gift,
							stringValue = string.Format(Strings.Get("GIFT_NUMBER_OF_KEYS"), payoutAmount)
						});
					}
					UserSave.inAppRestoredPurchaseCount++;
					UserSave.hasAdRemovalUpgrade = true;
				}
			}
			else if (InAppData.GetInAppBoardBundleTiers().Contains(productId))
			{
				DeliverBundle(productId, true);
				UserSave.inAppRestoredPurchaseCount++;
			}
			else if (InAppData.GetInAppSpecialBundles().Contains(productId) && !IsBoardBundleUnlocked(productId))
			{
				DeliverBundle(productId, true);
				UserSave.inAppRestoredPurchaseCount++;
			}
			else if (InAppData.InappidIsDynamicLimitedCharacterPurchase(productId))
			{
				LimitedCharacterPurchaseSuccesful(productId, true);
			}
			else if (InAppData.InappidIsDynamicLimitedBoardPurchase(productId))
			{
				LimitedBoardPurchaseSuccesful(productId, true);
			}
			else if (InAppData.GetInAppLimitedBundleTiers().Contains(productId))
			{
				DeliverBundle(productId, true);
				UserSave.inAppRestoredPurchaseCount++;
			}
			SaveManager.Save();
		}

		private bool IsBoardBundleUnlocked(string bundleId)
		{
			InAppItemModel.BundleItem boardItemFromBundleId = GetBoardItemFromBundleId(bundleId);
			if (boardItemFromBundleId == null)
			{
				return false;
			}
			return BoardManager.IsBoardOwned(boardItemFromBundleId.boardId);
		}

		private InAppItemModel.BundleItem GetBoardItemFromBundleId(string bundleId)
		{
			InAppItemModel inAppItemModel = InAppData.inAppData[bundleId];
			return inAppItemModel.bundleContents.Find((InAppItemModel.BundleItem item) => item.type == InAppItemModel.ItemType.Board);
		}

		private void PurchaseItemUnavailable(string productId)
		{
			TriggerFailForPurchaseHandler();
			NativePurchaseFinished(false);
			if (UIScreenController.IsInstanced)
			{
				_popupData = new InAppManagerPopupData
				{
					popupTitle = Strings.Get("PURCHASE_FAILED_TITLE"),
					popupDescription = Strings.Get("PURCHASE_FAILED_BODY"),
					canMakePurchase = false,
					isCoinsPack = false,
					isErrorMessage = true
				};
				UIScreenController.Instance.PushPopup("PurchaseErrorPopup");
			}
		}

		private void PurchaseCanceled(string productId)
		{
			TriggerFailForPurchaseHandler();
			NativePurchaseFinished(false);
		}

		private void PurchaseFailed(string productId)
		{
			TriggerFailForPurchaseHandler();
			if (UIScreenController.IsInstanced)
			{
				_popupData = new InAppManagerPopupData
				{
					popupTitle = Strings.Get("PURCHASE_FAILED_TITLE"),
					popupDescription = Strings.Get("PURCHASE_FAILED_BODY"),
					canMakePurchase = false,
					isCoinsPack = false,
					isErrorMessage = true
				};
				UIScreenController.Instance.PushPopup("PurchaseErrorPopup");
			}
		}

		private void PayoutInAppPurchase(string productId)
		{
			bool flag = false;
			string[] currencyTiers = InAppData.GetCurrencyTiers();
			string[] currencyDiscountTiers = InAppData.GetCurrencyDiscountTiers();
			string[] currencySpecialDiscountTiers = InAppData.GetCurrencySpecialDiscountTiers();
			if (!flag)
			{
				string[] array = currencyTiers;
				foreach (string text in array)
				{
					if (text == productId)
					{
						if (InAppData.inAppData[productId].itemType == InAppItemModel.ItemType.Coins)
						{
							InventorySave.IncreaseCoins(InAppData.inAppData[productId].payoutAmount, productId);
						}
						else
						{
							InventorySave.IncreaseKeys(InAppData.inAppData[productId].payoutAmount, productId);
						}
						UserSave.inAppConsumablePurchaseCount++;
						PlayPurchaseSucessSound();
						flag = true;
						break;
					}
				}
			}
			if (!flag && currencyDiscountTiers.Any((string id) => id == productId))
			{
				string inAppOnlineSettingsId = InAppData.GetInAppOnlineSettingsId(InAppData.inAppData[productId].normalOfferId);
				if (string.IsNullOrEmpty(inAppOnlineSettingsId))
				{
					LogError("Could not find a discout id for the pricepoint: " + productId);
					inAppOnlineSettingsId = string.Empty;
					return;
				}
				if (InAppData.inAppData[productId].itemType == InAppItemModel.ItemType.Coins)
				{
					InventorySave.IncreaseCoins(GetItemPayoutAmount(productId, inAppOnlineSettingsId), productId);
				}
				else
				{
					InventorySave.IncreaseKeys(GetItemPayoutAmount(productId, inAppOnlineSettingsId), productId);
				}
				UserSave.inAppConsumablePurchaseCount++;
				PlayPurchaseSucessSound();
				flag = true;
			}
			if (!flag && currencySpecialDiscountTiers.Any((string id) => id == productId))
			{
				string inAppOnlineSettingsId2 = InAppData.GetInAppOnlineSettingsId(InAppData.inAppData[productId].normalOfferId);
				if (string.IsNullOrEmpty(inAppOnlineSettingsId2))
				{
					LogError("Could not find a discount id for the pricepoint: " + productId);
					inAppOnlineSettingsId2 = string.Empty;
					return;
				}
				if (InAppData.inAppData[productId].itemType == InAppItemModel.ItemType.Coins)
				{
					InventorySave.IncreaseCoins(GetItemPayoutAmount(productId, inAppOnlineSettingsId2), productId);
				}
				else
				{
					InventorySave.IncreaseKeys(GetItemPayoutAmount(productId, inAppOnlineSettingsId2), productId);
				}
				UserSave.inAppConsumablePurchaseCount++;
				PlayPurchaseSucessSound();
				flag = true;
			}
			if (!flag && (productId == "com.kiloo.subways.doublecoins" || productId == "com.kiloo.subways.doublecoinsdiscount"))
			{
				if (!UserSave.hasDoubleCoinsUpgrade)
				{
					UserSave.hasDoubleCoinsUpgrade = true;
					if (UIScreenController.IsInstanced)
					{
						UIScreenController.Instance.HideInAppPurchaseOverlay();
						UIScreenController.Instance.QueueSlideIn(new SlideIn
						{
							type = SlideInData.SlideInType.Unlock,
							stringValue = Strings.Get("INAPP_DATA_DOUBLE_COINS")
						});
					}
				}
				UserSave.inAppNonConsumablePurchaseCount++;
				flag = true;
			}
			if (!flag && InAppData.GetInAppBoardBundleTiers().Contains(productId))
			{
				DeliverBundle(productId, false);
				PlayPurchaseSucessSound();
				UserSave.inAppNonConsumablePurchaseCount++;
				flag = true;
			}
			if (!flag)
			{
				if (InAppData.InappidIsDynamicLimitedCharacterPurchase(productId))
				{
					LimitedCharacterPurchaseSuccesful(productId, false);
					UserSave.inAppNonConsumablePurchaseCount++;
					flag = true;
				}
				else if (InAppData.InappidIsDynamicLimitedBoardPurchase(productId))
				{
					LimitedBoardPurchaseSuccesful(productId, false);
					UserSave.inAppNonConsumablePurchaseCount++;
					flag = true;
				}
			}
			if (!flag && InAppData.GetInAppSpecialBundles().Contains(productId))
			{
				PlayPurchaseSucessSound();
				UserSave.inAppNonConsumablePurchaseCount++;
				flag = true;
			}
			if (!flag && InAppData.GetInAppBundleTiers().Any((string bundleInApp) => bundleInApp == productId))
			{
				UpdateLastPurchasedBundle(productId);
				PlayPurchaseSucessSound();
				UserSave.inAppConsumablePurchaseCount++;
				flag = true;
			}
			if (!flag && InAppData.GetInAppLimitedBundleTiers().Contains(productId))
			{
				DeliverBundle(productId, false);
				PlayPurchaseSucessSound();
				flag = true;
			}
			SaveManager.Save();
			NativePurchaseFinished(true);
		}

		public int GetItemPayoutAmount(string productId, string onlineSettingsDiscountId = null)
		{
			if (onlineSettingsDiscountId == null)
			{
				onlineSettingsDiscountId = InAppData.GetInAppOnlineSettingsId(InAppData.inAppData[productId].normalOfferId);
			}
			int num = 0;
			if (!string.IsNullOrEmpty(onlineSettingsDiscountId))
			{
				num = Mathf.Clamp(GetDiscountValue(onlineSettingsDiscountId, 0), 0, int.MaxValue);
			}
			return InAppData.inAppData[productId].payoutAmount + num;
		}

		private void PlayPurchaseSucessSound()
		{
			if (purchaseSuccessSound != null)
			{
				NGUITools.PlaySound(purchaseSuccessSound);
			}
		}

		private void LimitedCharacterPurchaseSuccesful(string itemId, bool unlockedByRestore)
		{
			Characters.CharacterId charReturned;
			if (InAppData.TryConvertDynamicPurchaseIdToCharacterType(itemId, out charReturned) && !CharacterManager.IsCharacterOwned(charReturned))
			{
				Characters.Model model = Characters.characterData[charReturned];
				CharacterManager.UnlockCharacter(charReturned, unlockedByRestore);
				if (_purchaseHandler != null)
				{
					_purchaseHandler.OnPurchaseCharacterWithInappSucceded(charReturned, itemId);
				}
				if (unlockedByRestore && UIScreenController.IsInstanced)
				{
					UserSave.inAppRestoredPurchaseCount++;
					CharacterScreen.forceRefreshOnCharacterScreen = true;
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.Unlock,
						stringValue = Strings.Get(model.name)
					});
				}
			}
			if (_purchaseHandler != null)
			{
				_purchaseHandler.ResetOnFailedToPurchaseEventItemWithInapp();
			}
			_purchaseHandler = null;
		}

		private void LimitedBoardPurchaseSuccesful(string itemId, bool unlockedByRestore)
		{
			Boards.BoardId boardReturned;
			if (InAppData.TryConvertDynamicBoardIdToBoardType(itemId, out boardReturned) && !BoardManager.IsBoardOwned(boardReturned))
			{
				BoardManager.UnlockBoard(boardReturned);
				if (_purchaseHandler != null)
				{
					_purchaseHandler.OnPurchaseBoardWithInappSucceded(itemId, boardReturned);
				}
				if (unlockedByRestore && UIScreenController.IsInstanced)
				{
					UserSave.inAppRestoredPurchaseCount++;
					BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
					ShowSlideInForBoard(boardReturned);
				}
			}
			if (_purchaseHandler != null)
			{
				_purchaseHandler.ResetOnFailedToPurchaseEventItemWithInapp();
			}
			_purchaseHandler = null;
		}

		private void TriggerFailForPurchaseHandler()
		{
			if (_purchaseHandler != null)
			{
				_purchaseHandler.TriggerOnFailedToPurchaseEventItemWithInapp();
				_purchaseHandler = null;
			}
		}

		public void NativePurchaseFinished(bool success)
		{
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.HideInAppPurchaseOverlay();
			}
			if (success)
			{
				SaveMeManager.SendReviveIfPurchaseSucceeded();
			}
			else
			{
				SaveMeManager.SkipReviveIfPurchaseFailed();
			}
		}

		public void NativePurchaseErrorForReviveWithKeys()
		{
			SaveMeManager.SkipReviveIfPurchaseFailed();
		}

		public void NativePurchaseCancelled()
		{
			SaveMeManager.SkipReviveIfPurchaseFailed();
		}

		public void NativePurchaseFail()
		{
			NativePurchaseFinished(false);
		}

		public void BuyExpiredLimitedCharacter(string inappId, string characterName, PurchaseHandler sender)
		{
			_purchaseHandler = sender;
			if (!string.IsNullOrEmpty(inappId))
			{
				StartPurchase(inappId);
			}
		}

		public void BuyExpiredLimitedBoard(string inappId, PurchaseHandler sender)
		{
			_purchaseHandler = sender;
			if (!string.IsNullOrEmpty(inappId))
			{
				StartPurchase(inappId);
			}
		}

		private void DeliverBundle(string bundleId, bool unlockedByRestore)
		{
			InAppItemModel inAppItemModel = InAppData.inAppData[bundleId];
			foreach (InAppItemModel.BundleItem bundleContent in inAppItemModel.bundleContents)
			{
				if (bundleContent.type == InAppItemModel.ItemType.Coins && bundleContent.amount > 0)
				{
					InventorySave.IncreaseCoins(bundleContent.amount, bundleId);
				}
				if (bundleContent.type == InAppItemModel.ItemType.Keys && bundleContent.amount > 0)
				{
					InventorySave.IncreaseKeys(bundleContent.amount, bundleId);
				}
				if (bundleContent.type == InAppItemModel.ItemType.Hoverboards && bundleContent.amount > 0)
				{
					InventorySave.IncreaseAmount(UpgradeType.hoverboard, bundleContent.amount);
				}
				if (bundleContent.type == InAppItemModel.ItemType.Scoreboosters && bundleContent.amount > 0)
				{
					InventorySave.IncreaseAmount(UpgradeType.scorebooster, bundleContent.amount);
				}
				if (bundleContent.type == InAppItemModel.ItemType.Headstarts && bundleContent.amount > 0)
				{
					InventorySave.IncreaseAmount(UpgradeType.headstart2000, bundleContent.amount);
				}
				if (bundleContent.type == InAppItemModel.ItemType.Board && bundleContent.isBoardTypeValid && !BoardManager.IsBoardOwned(bundleContent.boardId))
				{
					BoardManager.UnlockBoard(bundleContent.boardId);
					BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
					BoardScreenManager.Instance.refreshBoardsWhenNotShowingPopup = true;
					if (unlockedByRestore && UIScreenController.IsInstanced)
					{
						ShowSlideInForBoard(bundleContent.boardId);
					}
				}
				if (bundleContent.type == InAppItemModel.ItemType.BoardAbility && bundleContent.isBoardTypeValid && bundleContent.isThemeIdentifierValid && !BoardManager.IsThemeOwned(bundleContent.boardId, bundleContent.themeIdentifier))
				{
					BoardManager.SetThemeAcquired(bundleContent.boardId, bundleContent.themeIdentifier);
				}
				if (bundleContent.type == InAppItemModel.ItemType.Character && bundleContent.isCharacterTypeValid && !CharacterManager.IsCharacterOwned(bundleContent.characterId))
				{
					CharacterManager.UnlockCharacter(bundleContent.characterId, true);
					CharacterScreen.forceRefreshOnCharacterScreen = true;
					if (unlockedByRestore && UIScreenController.IsInstanced)
					{
						ShowSlideInForCharacter(bundleContent.characterId);
					}
				}
			}
		}

		private void ShowSlideInForBoard(Boards.BoardId boardId)
		{
			UIScreenController.Instance.QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.Unlock,
				stringValue = Strings.Get(Boards.boardData[boardId].name)
			});
		}

		private void ShowSlideInForCharacter(Characters.CharacterId characterId)
		{
			UIScreenController.Instance.QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.Unlock,
				stringValue = Strings.Get(Characters.characterData[characterId].name)
			});
		}

		public void SuggestInAppPopup(int cost, string senderName, InAppItemModel.ItemType type)
		{
			_itemRequested = senderName;
			int num = 0;
			num = ((type != InAppItemModel.ItemType.Coins) ? (cost - InventorySave.keys) : (cost - InventorySave.coins));
			List<InAppItemModel> list = new List<InAppItemModel>();
			if (_productRequestSucceeded)
			{
				List<KeyValuePair<string, InAppItemModel>> list2 = GetActiveInAppOffers().ToList();
				list2 = list2.FindAll((KeyValuePair<string, InAppItemModel> x) => x.Value.itemType == type);
				StoreABManager.VerifyCoinList(list2);
				list2.OrderBy((KeyValuePair<string, InAppItemModel> x) => x.Value.payoutAmount);
				int num2 = -1;
				for (int num3 = 0; num3 < list2.Count; num3++)
				{
					if (list2[num3].Value.payoutAmount > num)
					{
						num2 = num3;
						break;
					}
				}
				if (num2 < 0 || num2 > list2.Count - 3)
				{
					list.AddRange(list2.Select((KeyValuePair<string, InAppItemModel> x) => x.Value).ToList().GetRange(list2.Count - 3, 3));
				}
				else
				{
					list.AddRange(list2.Select((KeyValuePair<string, InAppItemModel> x) => x.Value).ToList().GetRange(num2, 3));
				}
			}
			string popupTitle = string.Empty;
			bool isCoinsPack = false;
			if (type == InAppItemModel.ItemType.Coins)
			{
				isCoinsPack = true;
				popupTitle = Strings.Get("NOT_ENOUGH_COINS");
			}
			else if (type == InAppItemModel.ItemType.Keys)
			{
				popupTitle = Strings.Get("NOT_ENOUGH_KEYS");
			}
			else
			{
				LogError("No handling implemented for item type: " + type);
			}
			if (list.Count > 0)
			{
				string popupDescription = string.Empty;
				if (type == InAppItemModel.ItemType.Coins)
				{
					string format = Strings.Get("NOT_ENOUGH_COINS_POPUP_ADD_ON_COINS");
					popupDescription = string.Format(format, num);
				}
				else if (type == InAppItemModel.ItemType.Keys)
				{
					string format2 = Strings.Get("NOT_ENOUGH_COINS_POPUP_ADD_ON_KEYS");
					popupDescription = string.Format(format2, num);
				}
				else
				{
					LogError("No handling implemented for item type: " + type);
				}
				if (UIScreenController.IsInstanced)
				{
					_popupData = new InAppManagerPopupData
					{
						popupTitle = popupTitle,
						popupDescription = popupDescription,
						bestOffers = list,
						canMakePurchase = true,
						isCoinsPack = isCoinsPack,
						currencyMissing = num,
						isErrorMessage = false
					};
					UIScreenController.Instance.PushPopup("NotEnoughCurrencyPopup");
				}
			}
			else if (type == InAppItemModel.ItemType.Coins)
			{
				string empty = string.Empty;
				empty = ((num >= 2) ? Strings.Get("NOT_ENOUGH_COINS_POPUP_ADD_ON_COINS") : Strings.Get("NOT_ENOUGH_COINS_POPUP_ADD_ON_COINS"));
				string popupDescription2 = string.Format(empty, num);
				if (UIScreenController.IsInstanced)
				{
					_popupData = new InAppManagerPopupData
					{
						popupTitle = popupTitle,
						popupDescription = popupDescription2,
						canMakePurchase = false,
						isCoinsPack = isCoinsPack,
						isErrorMessage = false
					};
					UIScreenController.Instance.PushPopup("NotEnoughCurrencyPopup");
				}
			}
			else if (type == InAppItemModel.ItemType.Keys)
			{
				string empty2 = string.Empty;
				empty2 = ((num >= 2) ? Strings.Get("NOT_ENOUGH_COINS_POPUP_ADD_ON_KEYS") : Strings.Get("NOT_ENOUGH_COINS_POPUP_ADD_ON_KEYS"));
				string popupDescription3 = string.Format(empty2, num);
				if (UIScreenController.IsInstanced)
				{
					_popupData = new InAppManagerPopupData
					{
						popupTitle = popupTitle,
						popupDescription = popupDescription3,
						canMakePurchase = false,
						isCoinsPack = isCoinsPack,
						isErrorMessage = false
					};
					UIScreenController.Instance.PushPopup("NotEnoughCurrencyPopup");
				}
			}
			else
			{
				LogError("No handling implemented for item type: " + type);
			}
		}

		private static bool IsDiscountActive(string onlineSettingsId)
		{
			if (Globals.DEBUG_ENABLE_SALES && _debugDiscount.ContainsKey(onlineSettingsId))
			{
				return true;
			}
			return OnlineSettings.instance.HasValue(onlineSettingsId);
		}

		public static int GetDiscountValue(string onlineSettingsId, int defaultValue)
		{
			if (Globals.DEBUG_ENABLE_SALES && _debugDiscount.ContainsKey(onlineSettingsId))
			{
				return _debugDiscount[onlineSettingsId];
			}
			return OnlineSettings.instance.GetValue(onlineSettingsId, defaultValue);
		}

		public static Dictionary<string, InAppItemModel> GetActiveInAppOffers()
		{
			Dictionary<string, InAppItemModel> dictionary = new Dictionary<string, InAppItemModel>();
			string[] currencyTiers = InAppData.GetCurrencyTiers();
			string[] array = currencyTiers;
			foreach (string text in array)
			{
				string text2 = InAppData.GetIgnoredInAppOnlineSettingsId(text);
				if (string.IsNullOrEmpty(text2))
				{
					LogError("could not find an ignored inApp id for the pricepont: " + text);
					text2 = string.Empty;
				}
				if (OnlineSettings.instance.HasValue(text2))
				{
					continue;
				}
				string text3 = InAppData.GetInAppOnlineSettingsId(text);
				if (string.IsNullOrEmpty(text3))
				{
					LogError("could not find a discout id for the pricepont: " + text);
					text3 = string.Empty;
				}
				InAppItemModel value = InAppItemModel.Clone(InAppData.inAppData[text]);
				if (IsDiscountActive(text3) && IsAnyDiscountAvailable)
				{
					string key = ((!IsSpecialSaleActive) ? InAppData.GetDiscountIdForNormalOffer(text) : InAppData.GetSpecialDiscountIdForNormalOffer(text));
					InAppItemModel inAppItemModel = InAppItemModel.Clone(InAppData.inAppData[key]);
					int discountValue = GetDiscountValue(text3, 0);
					if (discountValue < 0)
					{
						dictionary.Add(key, inAppItemModel);
					}
					else if (discountValue > 0)
					{
						if (inAppItemModel.itemType == InAppItemModel.ItemType.Keys || inAppItemModel.itemType == InAppItemModel.ItemType.Coins)
						{
							inAppItemModel.ConvertToPromotion(discountValue);
							dictionary.Add(key, inAppItemModel);
						}
						else
						{
							LogError(string.Concat("Invalid item type: ", inAppItemModel.itemType, ". No sales are implemented for this item type."));
							dictionary.Add(text, value);
						}
					}
					else
					{
						LogError("Invalid discount value for item: " + text);
						dictionary.Add(text, value);
					}
				}
				else
				{
					dictionary.Add(text, value);
				}
			}
			return dictionary;
		}

		public static double GetDiscountEndTime()
		{
			if (Globals.DEBUG_ENABLE_SALES)
			{
				return CDateTime.UtcNow.AddDays(1.0).ToUnixTime();
			}
			return OnlineSettings.instance.GetValue("discount_end_time", 0.0);
		}

		private string GetContactingStoreOverlayText()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				if (DeviceUtility.isKindleDevice)
				{
					return Strings.Get("INAPP_MANAGER_OVERLAY_TEXT_AMAZONSTORE");
				}
				return Strings.Get("INAPP_MANAGER_OVERLAY_TEXT_GOOGLEPLAY");
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WebGLPlayer:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.IPhonePlayer:
				return Strings.Get("INAPP_MANAGER_OVERLAY_TEXT_APPSTORE");
			default:
				return string.Empty;
			}
		}

		public static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError(msg, context);
		}

		public static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_INAPPMANAGER_LOGS")]
		public static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		public InAppManagerPopupData GetPopupData()
		{
			return _popupData;
		}

		public void SetPopupData(InAppManagerPopupData popupData)
		{
			_popupData = popupData;
		}

		private void UpdateLastPurchasedBundle(string productId)
		{
			InAppData.BundleTier bundleTier = InAppData.GetBundleTier(productId);
			UserSave.lastPurchasedBundle = bundleTier;
		}

		public static bool TryAddTransactionToHistory(string orderId, string itemId)
		{
			if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
			{
				return true;
			}
			if (!UserSave.inAppPurchaseHistory.ContainsKey(orderId))
			{
				UserSave.inAppPurchaseHistory.Add(orderId, itemId);
				return true;
			}
			return false;
		}

		public static bool RemoveTransactionFromHistory(string orderId)
		{
			if (UserSave.inAppPurchaseHistory.ContainsKey(orderId))
			{
				UserSave.inAppPurchaseHistory.Remove(orderId);
				return true;
			}
			return false;
		}

		public static bool HasTransactionHistoryItemId(string itemId)
		{
			bool result = false;
			foreach (KeyValuePair<string, string> item in UserSave.inAppPurchaseHistory)
			{
				string key = item.Key;
				string value = item.Value;
				if (value == itemId)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static bool IsTransactionInHistory(string orderId)
		{
			return UserSave.inAppPurchaseHistory.ContainsKey(orderId);
		}

		public static void PrintAllInappHistory()
		{
			string text = Globals.convertStringStringDictionaryToString(UserSave.inAppPurchaseHistory);
		}

		public static bool ShouldShowMostPopular(InAppItemModel itemModel)
		{
			RefreshMostPopularProductIds();
			return _mostPopularStickerProductIds.Contains(itemModel.itemId) && itemModel.itemOfferType == InAppItemModel.OfferType.Normal;
		}

		private static void RefreshMostPopularProductIds()
		{
			string[] mostPopularProductIdsWithoutPrefix = GetMostPopularProductIdsWithoutPrefix();
			for (int i = 0; i < mostPopularProductIdsWithoutPrefix.Length; i++)
			{
				mostPopularProductIdsWithoutPrefix[i] = "com.kiloo.subways." + mostPopularProductIdsWithoutPrefix[i];
			}
			_mostPopularStickerProductIds = mostPopularProductIdsWithoutPrefix;
		}

		private static string[] GetMostPopularProductIdsWithoutPrefix()
		{
			string defaultValue = StringUtility.ArrayToString(MOST_POPULAR_STICKER_DEFAULT_PRODUCT_ID_WITHOUT_PREFIX, ",");
			string value = OnlineSettings.instance.GetValue("most_popular_sticker_product_id_without_prefix", defaultValue);
			return value.Split(',');
		}
	}
}
