using System;
using System.Collections.Generic;
using SYBO.Components.Utilities;
using SYBO.Subway.Info;
using UnityEngine;

public class Factories : SingletonMonoBehaviour<Factories>
{
	[Serializable]
	private class AssetBundleMapping
	{
		[Serializable]
		public class StringArray
		{
			public string[] Strings;
		}

		[HideInInspector]
		public string CharacterTypeName;

		private Characters.CharacterId? _parsedId;

		[EnumToString(typeof(Characters.CharacterId))]
		[SerializeField]
		private string _serializedCharacter = Characters.CharacterId.amsterdamCharacter01.ToString();

		public string[] DefaultAssetBundleNames;

		public StringArray[] OutfitAssetBundleNames;

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
			set
			{
				_parsedId = value;
				_serializedCharacter = value.ToString();
			}
		}

		public string[] GetAllAssetBundleNames()
		{
			List<string> list = new List<string>(DefaultAssetBundleNames);
			StringArray[] outfitAssetBundleNames = OutfitAssetBundleNames;
			foreach (StringArray stringArray in outfitAssetBundleNames)
			{
				list.AddRange(stringArray.Strings);
			}
			return list.ToArray();
		}

		public string[] GetAssetBundleNamesForOutfit(int outfit, bool includeIdles)
		{
			List<string> list = new List<string>(DefaultAssetBundleNames);
			if (!includeIdles)
			{
				list.RemoveAll((string s) => s.EndsWith("_idles"));
			}
			list.AddRange(OutfitAssetBundleNames[outfit].Strings);
			return list.ToArray();
		}
	}

	[SerializeField]
	private AssetBundleMapping[] _assetBundleMappings = new AssetBundleMapping[0];

	public Action OnLateUpdate;

	public AssetBundleFactory<CharacterCustomizationBundle> CharacterCustomizationBundle { get; private set; }

	public AssetBundleFactory<CharacterIdlesBundle> CharacterIdlesBundle { get; private set; }

	public AssetBundleFactory<CharacterAssetsBundle> CharacterAssetsBundle { get; private set; }

	public AssetBundleFactory<Material> MaterialFactory { get; private set; }

	public AssetBundleFactory<Texture2D> TextureFactory { get; private set; }

	protected override void SubclassInitialize()
	{
		CharacterCustomizationBundle = new AssetBundleFactory<CharacterCustomizationBundle>();
		CharacterIdlesBundle = new AssetBundleFactory<CharacterIdlesBundle>();
		CharacterAssetsBundle = new AssetBundleFactory<CharacterAssetsBundle>();
		MaterialFactory = new AssetBundleFactory<Material>();
		TextureFactory = new AssetBundleFactory<Texture2D>();
	}

	private void LateUpdate()
	{
		if (OnLateUpdate != null)
		{
			OnLateUpdate();
		}
	}

	public bool IsAssetBundleInUse(string assetBundleName)
	{
		return CharacterCustomizationBundle.IsAssetBundleInUse(assetBundleName) || CharacterIdlesBundle.IsAssetBundleInUse(assetBundleName) || CharacterAssetsBundle.IsAssetBundleInUse(assetBundleName) || MaterialFactory.IsAssetBundleInUse(assetBundleName) || TextureFactory.IsAssetBundleInUse(assetBundleName);
	}

	public string[] GetReferencedAssetBundleNames(Characters.CharacterId characterId)
	{
		for (int i = 0; i < _assetBundleMappings.Length; i++)
		{
			if (_assetBundleMappings[i].characterId == characterId)
			{
				return _assetBundleMappings[i].GetAllAssetBundleNames();
			}
		}
		SDebug.LogError("Missing asset bundle mapping for " + characterId);
		return new string[0];
	}

	public string[] GetReferencedAssetBundleNames(Characters.CharacterId characterId, int theme, bool includeIdles)
	{
		for (int i = 0; i < _assetBundleMappings.Length; i++)
		{
			if (_assetBundleMappings[i].characterId == characterId)
			{
				return _assetBundleMappings[i].GetAssetBundleNamesForOutfit(theme, includeIdles);
			}
		}
		SDebug.LogError("Missing asset bundle mapping for " + characterId);
		return new string[0];
	}
}
