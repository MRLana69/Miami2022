using System;
using System.Collections.Generic;
using Kiloo.Common.Extensions;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
	private Texture2D gradient;

	private SkinnedMeshRenderer outfitRenderer;

	private List<GameObject> specialAssets = new List<GameObject>();

	private List<Material> customMaterials = new List<Material>();

	private List<Material> headMaterials = new List<Material>();

	private List<Material> cachedMaterials = new List<Material>();

	private CharacterCustomizationBundle _characterCustomizationBundle;

	public SkinnedMeshRenderer OutfitRenderer
	{
		get
		{
			return outfitRenderer;
		}
	}

	public CharacterCustomizationBundle.OutfitInfo OutfitInfo
	{
		get
		{
			return _characterCustomizationBundle.Outfit;
		}
	}

	private void DeleteSpecialAssets()
	{
		foreach (GameObject specialAsset in specialAssets)
		{
			UnityEngine.Object.Destroy(specialAsset);
		}
		specialAssets.Clear();
		if (_characterCustomizationBundle != null)
		{
			UnityEngine.Object.Destroy(_characterCustomizationBundle.gameObject.gameObject);
			_characterCustomizationBundle = null;
		}
	}

	public void Customize(string name, int customizedVersion, SkinnedMeshRenderer model, SkinnedMeshRenderer outfitMesh, SkinnedMeshRenderer headMesh, CharacterRig characterRig, bool useSpecialAsset, bool deleteSpecialAsset, bool forceLOD0, bool asynchronous, Action onSuccess)
	{
		if (deleteSpecialAsset)
		{
			DeleteSpecialAssets();
		}
		outfitRenderer = outfitMesh;
		outfitRenderer.enabled = false;
		CharacterCustomizationAssetsFactory.GetAssetPrefab(name, customizedVersion, asynchronous, (CharacterCustomizationBundle bundledAsset) =>
		{
			if (bundledAsset != null)
			{
				_characterCustomizationBundle = bundledAsset;
				CharacterCustomizationBundle.OutfitInfo outfitInfo = _characterCustomizationBundle.Outfit;
				DestroyMaterials();
				CharacterMaterial.Request(outfitInfo.MainMaterialInfo.TextureName, outfitInfo.MainMaterialInfo.ShaderType, forceLOD0, DeviceInfo.PerformanceLevel.Low, asynchronous, (Material material) =>
				{
					model.material = material;
					if (headMesh.sharedMesh != null)
					{
						cachedMaterials.Add(material);
						for (int i = 0; i < headMesh.sharedMesh.subMeshCount; i++)
						{
							headMaterials.Add(material);
						}
						headMesh.materials = headMaterials.ToArray();
						headMaterials.AddRange(headMesh.materials);
					}
					if (outfitInfo.MainMesh != null)
					{
						model.sharedMesh = outfitInfo.MainMesh;
					}
					if (outfitInfo.ExtraMesh != null)
					{
						outfitRenderer.sharedMesh = outfitInfo.ExtraMesh;
						outfitRenderer.enabled = true;
						if (0 < outfitInfo.ExtraMaterialInfo.Length)
						{
							CharacterMaterial.Request(outfitInfo.ExtraMaterialInfo[0].TextureName, outfitInfo.ExtraMaterialInfo[0].ShaderType, forceLOD0, DeviceInfo.PerformanceLevel.Low, asynchronous, (Material customMaterial) =>
							{
								cachedMaterials.Add(customMaterial);
								if (1 < outfitInfo.ExtraMaterialInfo.Length)
								{
									CharacterMaterial.Request(outfitInfo.ExtraMaterialInfo[1].TextureName, outfitInfo.ExtraMaterialInfo[1].ShaderType, forceLOD0, DeviceInfo.PerformanceLevel.Low, asynchronous, (Material customExtraMaterial) =>
									{
										customMaterials.Add(customMaterial);
										customMaterials.Add(customExtraMaterial);
										outfitRenderer.materials = customMaterials.ToArray();
										BuildProp(outfitInfo, characterRig, useSpecialAsset, deleteSpecialAsset);
										onSuccess.SafeInvoke();
									});
								}
								else
								{
									customMaterials.Add(customMaterial);
									outfitRenderer.materials = customMaterials.ToArray();
									BuildProp(outfitInfo, characterRig, useSpecialAsset, deleteSpecialAsset);
									onSuccess.SafeInvoke();
								}
							});
						}
						else
						{
							BuildProp(outfitInfo, characterRig, useSpecialAsset, deleteSpecialAsset);
							onSuccess.SafeInvoke();
						}
					}
					else
					{
						BuildProp(outfitInfo, characterRig, useSpecialAsset, deleteSpecialAsset);
						onSuccess.SafeInvoke();
					}
				});
			}
		});
	}

	private void BuildProp(CharacterCustomizationBundle.OutfitInfo outfitInfo, CharacterRig characterRig, bool useSpecialAsset, bool deleteSpecialAsset)
	{
		if ((!(outfitInfo.SpecialAssetsName != string.Empty) || !useSpecialAsset) && (!(outfitInfo.SpecialAsset != null) || !useSpecialAsset))
		{
			return;
		}
		string text = string.Empty;
		if (UIScreenController.Instance != null)
		{
			text = UIScreenController.Instance.GetCurrentPopupName();
		}
		GameObject gameObject = null;
		if (outfitInfo.SpecialAssetsName != string.Empty)
		{
			GameObject gameObject2 = (GameObject)Resources.Load("Prefabs/CharacterSpecialProps/" + outfitInfo.SpecialAssetsName);
			if (gameObject2 != null)
			{
				gameObject = UnityEngine.Object.Instantiate(gameObject2);
			}
		}
		else if (outfitInfo.SpecialAsset != null)
		{
			gameObject = UnityEngine.Object.Instantiate(outfitInfo.SpecialAsset);
		}
		Utility.SetLayerRecursively(gameObject.transform, characterRig.gameObject.layer);
		CharacterCustomizationProp[] componentsInChildren = gameObject.GetComponentsInChildren<CharacterCustomizationProp>();
		characterRig.AttachPoint1.transform.localScale = Vector3.one;
		gameObject.transform.parent = characterRig.AttachPoint1.transform;
		bool flag = false;
		CharacterCustomizationProp[] array = componentsInChildren;
		foreach (CharacterCustomizationProp characterCustomizationProp in array)
		{
			if (deleteSpecialAsset)
			{
				specialAssets.Add(characterCustomizationProp.gameObject);
			}
			characterCustomizationProp.Initialize(characterRig, text == "CharacterNameEventPopup");
			flag = flag || gameObject == characterCustomizationProp.gameObject;
		}
		if (!flag)
		{
			gameObject.gameObject.transform.parent = characterRig.transform;
			specialAssets.Add(gameObject);
		}
	}

	private void DestroyMaterials()
	{
		if (cachedMaterials.Count > 0)
		{
			for (int i = 0; i < cachedMaterials.Count; i++)
			{
				Texture mainTexture = cachedMaterials[i].mainTexture;
				if (mainTexture != null)
				{
					UnityEngine.Object.Destroy(mainTexture);
				}
				UnityEngine.Object.Destroy(cachedMaterials[i]);
			}
			cachedMaterials.Clear();
		}
		if (headMaterials != null)
		{
			for (int j = 0; j < headMaterials.Count; j++)
			{
				Texture mainTexture2 = headMaterials[j].mainTexture;
				if (mainTexture2 != null)
				{
					UnityEngine.Object.Destroy(mainTexture2);
				}
				UnityEngine.Object.Destroy(headMaterials[j]);
			}
			headMaterials.Clear();
		}
		if (customMaterials == null)
		{
			return;
		}
		for (int k = 0; k < customMaterials.Count; k++)
		{
			Texture mainTexture3 = customMaterials[k].mainTexture;
			if (mainTexture3 != null)
			{
				UnityEngine.Object.Destroy(mainTexture3);
			}
			UnityEngine.Object.Destroy(customMaterials[k]);
		}
		customMaterials.Clear();
	}

	private void OnDestroy()
	{
		DestroyMaterials();
		if (_characterCustomizationBundle != null)
		{
			UnityEngine.Object.Destroy(_characterCustomizationBundle.gameObject);
		}
	}
}
