using System;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	public abstract class GroupedAssetLoader<T, Loader> : AssetLoader<T, Loader> where T : UnityEngine.Object where Loader : AssetLoader<T, Loader>
	{
		public Action<string, List<T>> OnNeedPopupForAssetSelection;

		public T GetGroupAsset(string groupName)
		{
			for (int i = 0; i < _includedAssets.Count; i++)
			{
				T val = _includedAssets[i];
				if (GetGroup(val) == groupName)
				{
					return val;
				}
			}
			return (T)null;
		}

		protected abstract string GetGroup(T asset);
	}
}
