using System.Collections.Generic;
using System.Linq;
using SYBO.Subway.Characters;
using UnityEngine;

public class ColorTeleboardEffect : BatTeleboardEffect
{
	[SerializeField]
	private int colorNumber;

	public override void StartTeleBoardEffect()
	{
		base.StartTeleBoardEffect();
		float num = 1f / (float)colorNumber;
		List<Material> list = CollectMaterials();
		if (list.Count == 0)
		{
			return;
		}
		float num2 = (float)Random.Range(0, colorNumber) * num;
		float x = ((!Mathf.Approximately(num2, list[0].mainTextureOffset.x)) ? num2 : Mathf.Repeat(num2 + num, 1f - num));
		foreach (Material item in list)
		{
			item.mainTextureOffset = new Vector2(x, 0f);
		}
	}

	private List<Material> CollectMaterials()
	{
		List<Material> list = new List<Material>();
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>(true);
		if (componentsInChildren != null)
		{
			list.AddRange(componentsInChildren.Select((MeshRenderer r) => r.material));
		}
		componentsInChildren = CharacterRendering.Instance.CharacterModel.AttachPoint1.GetComponentsInChildren<MeshRenderer>(true);
		if (componentsInChildren != null)
		{
			list.AddRange(componentsInChildren.Select((MeshRenderer r) => r.material));
		}
		return list;
	}
}
