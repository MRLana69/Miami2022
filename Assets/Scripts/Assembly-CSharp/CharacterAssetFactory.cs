using System;
using SYBO.Components.Utilities;
using UnityEngine;

public class CharacterAssetFactory : MonoBehaviour
{
	public static void GetAssetPrefab(string name, bool asynchronous, Action<CharacterAssetsBundle> onSucces)
	{
		SingletonMonoBehaviour<Factories>.Instance.CharacterAssetsBundle.RequestItemInstance(name, (CharacterAssetsBundle prefab) =>
		{
			if (onSucces != null)
			{
				onSucces(prefab);
			}
			else
			{
				onSucces(null);
			}
		}, asynchronous);
	}
}
