using UnityEngine;

namespace SYBO.Components.Utilities.Scriptables
{
	public abstract class ScriptablesAssetLoader<T, Loader> : AssetLoader<T, Loader> where T : ScriptableObject where Loader : AssetLoader<T, Loader>
	{
		public T GetAsset(string name)
		{
			return _includedAssets.Find((T x) => x.name == name);
		}

		public G GetAsset<G>(string name) where G : T
		{
			return GetAsset(name) as G;
		}
	}
}
