using System;
using SYBO.Components.Utilities;
using UnityEngine;

public class CharacterCustomizationAssetsFactory : MonoBehaviour
{
	public static void GetAssetPrefab(string name, int customizedVersion, bool asynchronous, Action<CharacterCustomizationBundle> onSucces)
	{
		SingletonMonoBehaviour<Factories>.Instance.CharacterCustomizationBundle.RequestItemInstance(name + "_" + customizedVersion, (CharacterCustomizationBundle prefab) =>
		{
			if (onSucces != null)
			{
				onSucces(prefab);
			}
		}, asynchronous);
	}
}
