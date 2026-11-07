using System;
using System.Diagnostics;
using System.Threading;
using AdHandling.AdConstants.Internal;
using Info;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace AdHandling.AdConstants
{
	public class AdConstants : IAdConstants
	{
		private IAdConstants _constants;

		private static AdConstants _instance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action constantsChanged__BackingField;

		public static AdConstants instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AdConstants();
				}
				return _instance;
			}
		}

		public static string FlurryApiKey
		{
			get
			{
				switch (Application.platform)
				{
				case RuntimePlatform.IPhonePlayer:
					if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
					{
						return "GDGAT7PY98MAEQQIWC68";
					}
					return "CIJUE322XIHTDWNV519J";
				case RuntimePlatform.Android:
					if (DeviceUtility.isKindleDevice)
					{
						if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
						{
							return "XJ5NMRG7R5WGC4BZC6XQ";
						}
						return "D5BDSJTZB2JX75Y2T4SN";
					}
					if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
					{
						return "XJ5NMRG7R5WGC4BZC6XQ";
					}
					return "YR898G65YFPWNMQ6X5H5";
				default:
					return string.Empty;
				}
			}
		}

		public bool isRestricted
		{
			get
			{
				return _constants.isRestricted;
			}
		}

		public string AdColonyAppId
		{
			get
			{
				return _constants.AdColonyAppId;
			}
		}

		public string TapJoyAppId
		{
			get
			{
				return _constants.TapJoyAppId;
			}
		}

		public string TapJoyAppKey
		{
			get
			{
				return _constants.TapJoyAppKey;
			}
		}

		public string ChartboostAppId
		{
			get
			{
				return _constants.ChartboostAppId;
			}
		}

		public string ChartboostAppSignature
		{
			get
			{
				return _constants.ChartboostAppSignature;
			}
		}

		public string InmobiAccountId
		{
			get
			{
				return _constants.InmobiAccountId;
			}
		}

		public string VungleAppId
		{
			get
			{
				return _constants.VungleAppId;
			}
		}

		public string SupersonicAppId
		{
			get
			{
				return _constants.SupersonicAppId;
			}
		}

		public string MobVistaAppId
		{
			get
			{
				return _constants.MobVistaAppId;
			}
		}

		public string MobVistaAppSdkKey
		{
			get
			{
				return _constants.MobVistaAppSdkKey;
			}
		}

		public static event Action constantsChanged
		{
			add
			{
				Action action = constantsChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref constantsChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = constantsChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref constantsChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private AdConstants()
		{
			SetPlatformDependentConstants();
			AgeRestriction.restrictionChanged += SetPlatformDependentConstants;
		}

		private void SetPlatformDependentConstants()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
				{
					_constants = new AdConstantsIOS_TEST();
				}
				else if (AgeRestriction.isRestricted)
				{
					_constants = new AdConstantsIOSRestricted();
				}
				else
				{
					_constants = new AdConstantsIOS();
				}
				break;
			case RuntimePlatform.Android:
				if (DeviceUtility.isKindleDevice)
				{
					if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
					{
						_constants = new AdConstantsKindle_TEST();
					}
					else if (AgeRestriction.isRestricted)
					{
						_constants = new AdConstantsKindleRestricted();
					}
					else
					{
						_constants = new AdConstantsKindle();
					}
				}
				else if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
				{
					_constants = new AdConstantsAndroid_TEST();
				}
				else if (AgeRestriction.isRestricted)
				{
					_constants = new AdConstantsAndroidRestricted();
				}
				else
				{
					_constants = new AdConstantsAndroid();
				}
				break;
			default:
				if (AgeRestriction.isRestricted)
				{
					_constants = new AdConstantsEditorRestricted();
				}
				else
				{
					_constants = new AdConstantsEditor();
				}
				break;
			}
			constantsChanged__BackingField.SafeInvoke();
		}
	}
}
