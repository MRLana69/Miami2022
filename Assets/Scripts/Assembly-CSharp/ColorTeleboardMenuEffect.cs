using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorTeleboardMenuEffect : BatTeleboardMenuEffect
{
	[SerializeField]
	private int colorNumber;

	private List<Material> materials;

	private void OnDisable()
	{
		if (materials == null || materials.Count == 0)
		{
			return;
		}
		foreach (Material material in materials)
		{
			material.mainTextureOffset = Vector2.zero;
		}
	}

	public override void StartTeleboardEffect(float duration, CharacterModel characterModel)
	{
		base.StartTeleboardEffect(duration, characterModel);
		float num = 1f / (float)colorNumber;
		materials = CollectMaterials(characterModel);
		if (materials.Count == 0)
		{
			return;
		}
		float x = Mathf.Repeat(materials[0].mainTextureOffset.x + num, 1f);
		foreach (Material material in materials)
		{
			material.mainTextureOffset = new Vector2(x, 0f);
		}
	}

	private List<Material> CollectMaterials(CharacterModel characterModel)
	{
		List<Material> result = new List<Material>();
		AddChildrenMaterials(base.transform, ref result);
		if (characterModel != null)
		{
			AddChildrenMaterials(characterModel.AttachPoint1, ref result);
		}
		if (_boardOnly && _rootTransform != null)
		{
			AddChildrenMaterials(_rootTransform, ref result);
		}
		return result;
	}

	private void AddChildrenMaterials(Transform rootTransform, ref List<Material> materials)
	{
		MeshRenderer[] componentsInChildren = rootTransform.GetComponentsInChildren<MeshRenderer>(true);
		if (componentsInChildren != null)
		{
			materials.AddRange(componentsInChildren.Select((MeshRenderer r) => r.material));
		}
	}
}
