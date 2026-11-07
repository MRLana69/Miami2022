using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterCustomizationBundle : MonoBehaviour
{
	[Serializable]
	public struct CharacterMaterialInfo
	{
		[SerializeField]
		public string TextureName;

		[SerializeField]
		public CharacterMaterial.CharacterMaterialShaderType ShaderType;

		public CharacterMaterialInfo(string textureName, CharacterMaterial.CharacterMaterialShaderType shaderType)
		{
			TextureName = textureName;
			ShaderType = shaderType;
		}
	}

	[Serializable]
	public class OutfitInfo
	{
		[SerializeField]
		public CharacterMaterialInfo MainMaterialInfo;

		[SerializeField]
		public CharacterMaterialInfo[] ExtraMaterialInfo;

		public Mesh MainMesh;

		public Mesh ExtraMesh;

		public GameObject SpecialAsset;

		public string SpecialAssetsName;

		public List<string> ExcludedAnimationsNames;

		public AnimationEnumString HighscoreAnimationName;
	}

	[Serializable]
	public class AnimationEnumString
	{
		public const string ValueFieldName = "Value";

		public string Value;
	}

	[FormerlySerializedAs("_customSet")]
	[SerializeField]
	private OutfitInfo _outfit;

	public OutfitInfo Outfit
	{
		get
		{
			return _outfit;
		}
	}
}
