using System;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Utilities
{
	[ExecuteInEditMode]
	[PrefabPath("Prefabs/DebugSettings")]
	public class DebugSettings : SingletonMonoBehaviour<DebugSettings>
	{
		[NonSerialized]
		public bool _active = true;

		[SerializeField]
		private bool _unlockedSceneCamera = true;

		[SerializeField]
		private bool _characterInvincible;

		[SerializeField]
		private bool _dontUseAssetBundlesInEditor = true;

		[SerializeField]
		private bool _useManifestsForAssetBundlesInEditor = true;

		[SerializeField]
		private bool _dontUseIdles;

		[SerializeField]
		private bool _forceHighResCharacterTextures;

		[SerializeField]
		private bool _editorDevicePerformanceLevelOverride;

		[SerializeField]
		private DeviceInfo.PerformanceLevel _editorDevicePerformanceLevel;

		[SerializeField]
		private bool _useDebugZDistanceTargetPosition;

		[SerializeField]
		private bool _useQueuedEnvironmentKinds;

		[SerializeField]
		private EnvironmentKind[] _queuedEnvironmentKinds;

		public static bool IsAllowed
		{
			get
			{
				return Globals.DEBUG_SETTINGS;
			}
		}

		public static bool UnlockedSceneCamera
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonoBehaviour<DebugSettings>.Instance._unlockedSceneCamera;
			}
		}

		public static bool CharacterInvincible
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonoBehaviour<DebugSettings>.Instance._characterInvincible;
			}
			set
			{
				CheckIsAllowed();
				SingletonMonoBehaviour<DebugSettings>.Instance._characterInvincible = value;
			}
		}

		public static bool DontUseBaseBundleInEditor
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonoBehaviour<DebugSettings>.Instance._dontUseAssetBundlesInEditor;
			}
			set
			{
				CheckIsAllowed();
				SingletonMonoBehaviour<DebugSettings>.Instance._dontUseAssetBundlesInEditor = value;
			}
		}

		public static bool UseManifestsForAssetBundlesInEditor
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonoBehaviour<DebugSettings>.Instance._useManifestsForAssetBundlesInEditor;
			}
		}

		public static bool DontUseIdles
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonoBehaviour<DebugSettings>.Instance._dontUseIdles;
			}
			set
			{
				CheckIsAllowed();
				SingletonMonoBehaviour<DebugSettings>.Instance._dontUseIdles = value;
			}
		}

		public static bool ForceHighResCharacterTextures
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonoBehaviour<DebugSettings>.Instance._forceHighResCharacterTextures;
			}
		}

		public static bool OverrideEditorDevicePerformanceLevel
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonoBehaviour<DebugSettings>.Instance._editorDevicePerformanceLevelOverride;
			}
		}

		public static DeviceInfo.PerformanceLevel EditorDevicePerformanceLevel
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonoBehaviour<DebugSettings>.Instance._editorDevicePerformanceLevel;
			}
		}

		public static bool UseDebugZDistanceTargetPosition
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonoBehaviour<DebugSettings>.Instance._useDebugZDistanceTargetPosition;
			}
			set
			{
				CheckIsAllowed();
				SingletonMonoBehaviour<DebugSettings>.Instance._useDebugZDistanceTargetPosition = value;
			}
		}

		public static bool UseQueuedEnvironmentKinds
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonoBehaviour<DebugSettings>.Instance._useQueuedEnvironmentKinds;
			}
			set
			{
				CheckIsAllowed();
				SingletonMonoBehaviour<DebugSettings>.Instance._useQueuedEnvironmentKinds = value;
			}
		}

		public static EnvironmentKind[] QueuedEnvironmentKinds
		{
			get
			{
				CheckIsAllowed();
				SingletonMonoBehaviour<DebugSettings>.Instance.ValidateEnvironmentKinds();
				return SingletonMonoBehaviour<DebugSettings>.Instance._queuedEnvironmentKinds;
			}
			set
			{
				CheckIsAllowed();
				SingletonMonoBehaviour<DebugSettings>.Instance._queuedEnvironmentKinds = value;
				SingletonMonoBehaviour<DebugSettings>.Instance.ValidateEnvironmentKinds();
			}
		}

		protected override void SubclassInitialize()
		{
			if (DemoVideoModeManager.IsEnabled)
			{
				CharacterInvincible = true;
			}
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.Manual);
			ValidateEnvironmentKinds();
		}

		private static void CheckIsAllowed()
		{
			if (!IsAllowed)
			{
				throw new InvalidOperationException("Use of DebugSetting not allowed");
			}
		}

		private void ValidateEnvironmentKinds()
		{
			if (_queuedEnvironmentKinds == null)
			{
				return;
			}
			for (int i = 0; i < _queuedEnvironmentKinds.Length; i++)
			{
				if (_queuedEnvironmentKinds[i].Type == EnvironmentType.All || _queuedEnvironmentKinds[i].Type == EnvironmentType.None || _queuedEnvironmentKinds[i].Type == EnvironmentType.Empty)
				{
					_queuedEnvironmentKinds[i] = EnvironmentKind.FillersLow;
				}
				_queuedEnvironmentKinds[i] = _queuedEnvironmentKinds[i].GetValidated();
			}
		}
	}
}
