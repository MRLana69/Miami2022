using System.Collections.Generic;
using SYBO.Components.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace SYBO.Components.Utilities
{
	[ExecuteInEditMode]
	public class AssetLoader : ScriptableObject
	{
		protected static bool CanUpdateAssets
		{
			get
			{
				return true;
			}
		}
	}
	public class AssetLoader<T, Loader> : AssetLoader where T : Object where Loader : AssetLoader<T, Loader>
	{
		[HideInInspector]
		public UnityEvent OnAssetsChanged = new UnityEvent();

		[Tooltip("This field allows you to filter assets based on specifyed path(s). Specify paths starting from 'Assets/'. Only assets in the specified directory (and subdirectories) will be included.")]
		[SerializeField]
		protected string[] _assetPaths;

		[HideInInspector]
		[SerializeField]
		protected List<T> _includedAssets = new List<T>();

		[HideInInspector]
		[SerializeField]
		protected List<AssetInfo> _assetInfo = new List<AssetInfo>();

		private static Loader _instance;

		public static bool HasInstance
		{
			get
			{
				return _instance != null;
			}
		}

		private static string LoaderPath
		{
			get
			{
				string text = string.Empty;
				object[] customAttributes = typeof(Loader).GetCustomAttributes(typeof(AssetLoaderPathAttribute), true);
				if (customAttributes != null)
				{
					object[] array = customAttributes;
					foreach (object obj in array)
					{
						if (obj.GetType() == typeof(AssetLoaderPathAttribute))
						{
							text = (obj as AssetLoaderPathAttribute).FullPath;
						}
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					Log.Utilities.LogErrorFormat("AssetLoader was unable to find a reasonable path for loader of type {0}", typeof(Loader).ToString());
				}
				return text;
			}
		}

		private static string LoaderName
		{
			get
			{
				string text = string.Empty;
				object[] customAttributes = typeof(Loader).GetCustomAttributes(typeof(AssetLoaderPathAttribute), true);
				if (customAttributes != null)
				{
					object[] array = customAttributes;
					foreach (object obj in array)
					{
						if (obj.GetType() == typeof(AssetLoaderPathAttribute))
						{
							text = (obj as AssetLoaderPathAttribute).AssetName;
						}
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					Log.Utilities.LogErrorFormat("AssetLoader was unable to find an assetName for loader of type {0}", typeof(Loader).ToString());
				}
				return text;
			}
		}

		public static Loader Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<Loader>(LoaderName);
					if (_instance == null)
					{
						Log.Utilities.LogErrorFormat("Unable to find AssetLoader object, this should never happen. It should have been generated editor-time.");
					}
				}
				return _instance;
			}
		}

		public List<T> LoadAssets()
		{
			return _includedAssets;
		}

		private void SetIncludedAssets(List<T> value)
		{
			_includedAssets = value;
		}
	}
}
