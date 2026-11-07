using System;
using System.Collections.Generic;
using SYBO.Components.Utilities;
using UnityEngine;

public class CharacterMaterial : MonoBehaviour
{
	public enum CharacterMaterialShaderType
	{
		Default = 0,
		Additive = 1,
		Shine = 2
	}

	private class CharacterMaterialPathInfo
	{
		public CharacterMaterialShaderType ShaderType;

		public DeviceInfo.PerformanceLevel PerformanceLevel;

		public string MaterialPath;

		public CharacterMaterialPathInfo(CharacterMaterialShaderType shaderType, DeviceInfo.PerformanceLevel performanceLevel, string materialPath)
		{
			ShaderType = shaderType;
			PerformanceLevel = performanceLevel;
			MaterialPath = materialPath;
		}
	}

	private static List<CharacterMaterialPathInfo> _materialPaths = new List<CharacterMaterialPathInfo>
	{
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Default, DeviceInfo.PerformanceLevel.Low, "Materials/avatar_default_LOD0"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Default, DeviceInfo.PerformanceLevel.Medium, "Materials/avatar_default_LOD1"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Default, DeviceInfo.PerformanceLevel.High, "Materials/avatar_default_LOD2"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Additive, DeviceInfo.PerformanceLevel.Low, "Materials/avatar_additive_LOD0"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Additive, DeviceInfo.PerformanceLevel.Medium, "Materials/avatar_additive_LOD1"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Additive, DeviceInfo.PerformanceLevel.High, "Materials/avatar_additive_LOD2"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Shine, DeviceInfo.PerformanceLevel.Low, "Materials/avatar_shine_LOD0"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Shine, DeviceInfo.PerformanceLevel.Medium, "Materials/avatar_shine_LOD1"),
		new CharacterMaterialPathInfo(CharacterMaterialShaderType.Shine, DeviceInfo.PerformanceLevel.High, "Materials/avatar_shine_LOD2")
	};

	public static void Request(string textureName, CharacterMaterialShaderType shaderType, bool forcePerformanceLevel, DeviceInfo.PerformanceLevel forcedLevel, bool asynchronous, Action<Material> onSucces)
	{
		DeviceInfo.PerformanceLevel performanceLevel = DeviceInfo.Instance.performanceLevel;
		if (forcePerformanceLevel)
		{
			performanceLevel = forcedLevel;
			if (shaderType == CharacterMaterialShaderType.Shine)
			{
				shaderType = CharacterMaterialShaderType.Default;
			}
		}
		string materialPath = GetMaterialPath(shaderType, performanceLevel);
		RequestMaterial(textureName, materialPath, asynchronous, onSucces);
	}

	private static string GetMaterialPath(CharacterMaterialShaderType shaderType, DeviceInfo.PerformanceLevel performanceLevel)
	{
		CharacterMaterialPathInfo characterMaterialPathInfo = _materialPaths.Find((CharacterMaterialPathInfo mp) => mp.ShaderType == shaderType && mp.PerformanceLevel == performanceLevel);
		return (characterMaterialPathInfo == null) ? null : characterMaterialPathInfo.MaterialPath;
	}

	private static void RequestMaterial(string textureName, string materialPath, bool asynchronous, Action<Material> onSucces)
	{
		Material mat = Resources.Load(materialPath, typeof(Material)) as Material;
		SetTexture(mat, textureName, asynchronous, onSucces);
	}

	private static void SetTexture(Material mat, string textureName, bool asynchronous, Action<Material> onSucces)
	{
		Material material = new Material(mat);
		string text = textureName;
		SingletonMonoBehaviour<Factories>.Instance.TextureFactory.RequestItemInstance(text, (Texture2D tex) =>
		{
			if (tex == null)
			{
				SingletonMonoBehaviour<Factories>.Instance.TextureFactory.RequestItemInstance(textureName, (Texture2D texLow) =>
				{
					material.SetTexture("_MainTex", texLow);
					if (onSucces != null)
					{
						onSucces(material);
					}
				}, asynchronous);
			}
			else
			{
				material.SetTexture("_MainTex", tex);
				if (onSucces != null)
				{
					onSucces(material);
				}
			}
		}, asynchronous);
	}
}
