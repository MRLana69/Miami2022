using System;
using System.Collections.Generic;
using SYBO.Subway.Info;
using UnityEngine;

public class AssetBundleBuildLists : MonoBehaviour
{
	[Serializable]
	public class BuildListEntry
	{
		[HideInInspector]
		public string Name;

		[EnumToString(typeof(Characters.CharacterId))]
		[SerializeField]
		private string _serializedCharacter = Characters.CharacterId.amsterdamCharacter01.ToString();

		private Characters.CharacterId? _parsedId;

		public CharacterAssetsBundle CharacterAssets;

		public CharacterIdlesBundle CharacterIdles;

		public CharacterCustomizationBundle[] CharacterCustomizationAssets;

		public Texture2D[] CharacterTextures;

		public Characters.CharacterId characterId
		{
			get
			{
				if (!_parsedId.HasValue)
				{
					_parsedId = (Characters.CharacterId)Enum.Parse(typeof(Characters.CharacterId), _serializedCharacter);
				}
				return _parsedId.Value;
			}
		}

		public string[] GetDefaultAssetBundleNames()
		{
			List<string> list = new List<string>();
			TryAdd(CharacterAssets.name, list);
			TryAdd(CharacterIdles.name, list);
			return list.ToArray();
		}

		public string[] GetOutfitAssetBundleNames(int outfit)
		{
			List<string> list = new List<string>();
			TryAdd(CharacterCustomizationAssets[outfit].name, list);
			TryAdd(CharacterCustomizationAssets[outfit].Outfit.MainMaterialInfo.TextureName, list);
			CharacterCustomizationBundle.CharacterMaterialInfo[] extraMaterialInfo = CharacterCustomizationAssets[outfit].Outfit.ExtraMaterialInfo;
			for (int i = 0; i < extraMaterialInfo.Length; i++)
			{
				CharacterCustomizationBundle.CharacterMaterialInfo characterMaterialInfo = extraMaterialInfo[i];
				TryAdd(characterMaterialInfo.TextureName, list);
			}
			return list.ToArray();
		}

		private void TryAdd(UnityEngine.Object[] objs, List<string> list)
		{
			for (int i = 0; i < objs.Length; i++)
			{
				TryAdd(objs[i].name, list);
			}
		}

		private void TryAdd(string bundleName, List<string> list)
		{
			if (!string.IsNullOrEmpty(bundleName) && !list.Contains(bundleName.ToLower()))
			{
				list.Add(bundleName.ToLower());
			}
		}
	}

	public BuildListEntry[] CharacterEntries;
}
