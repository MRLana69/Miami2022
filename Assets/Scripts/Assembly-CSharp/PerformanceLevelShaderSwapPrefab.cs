using SYBO.Components.Utilities;
using UnityEngine;

public class PerformanceLevelShaderSwapPrefab : MonoBehaviour, IPrefabBehaviour
{
	[SerializeField]
	private Renderer _renderer;

	[SerializeField]
	private Shader _swapShader;

	[SerializeField]
	private DeviceInfo.PerformanceLevel _minPerformanceLevelForSwap;

	public void Initialize()
	{
		if (_renderer != null && DeviceInfo.Instance.performanceLevel <= _minPerformanceLevelForSwap)
		{
			Material[] sharedMaterials = _renderer.sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				sharedMaterials[i] = new Material(sharedMaterials[i]);
				sharedMaterials[i].shader = _swapShader;
			}
			_renderer.materials = sharedMaterials;
		}
	}
}
