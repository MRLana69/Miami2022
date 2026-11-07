using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Visuals
{
	public class MaterialAnimationController : MonoBehaviour
	{
		[SerializeField]
		private Shader _targetShader;

		[SerializeField]
		private string _materialNameSubstring = string.Empty;

		[SerializeField]
		private bool _searchChildrenForRenderer;

		[Header("Material Properties")]
		[SerializeField]
		private float _colorMultiplier = 1f;

		private List<Renderer> _targetRenderers = new List<Renderer>();

		private List<Material> _targetMaterials = new List<Material>();

		private bool _targetSet;

		private static MaterialPropertyBlock _propertyBlock;

		public float ColorMultiplier
		{
			set
			{
				_colorMultiplier = value;
			}
		}

		private void Update()
		{
			if (!_targetSet)
			{
				if (_targetRenderers.Count == 0)
				{
					if (!_searchChildrenForRenderer)
					{
						_targetRenderers.Add(GetComponent<Renderer>());
					}
					else
					{
						_targetRenderers.AddRange(GetComponentsInChildren<Renderer>());
					}
				}
				if (_targetRenderers.Count > 0)
				{
					if (SetTargetMaterials())
					{
						_propertyBlock = new MaterialPropertyBlock();
						_targetSet = true;
					}
					else
					{
						_targetRenderers.Clear();
					}
				}
				return;
			}
			_propertyBlock.SetFloat("_ColorMultiplier", _colorMultiplier);
			foreach (Renderer targetRenderer in _targetRenderers)
			{
				targetRenderer.SetPropertyBlock(_propertyBlock);
			}
		}

		private bool SetTargetMaterials()
		{
			_targetMaterials.Clear();
			foreach (Renderer targetRenderer in _targetRenderers)
			{
				Material[] sharedMaterials = targetRenderer.sharedMaterials;
				foreach (Material material in sharedMaterials)
				{
					if (material == null)
					{
						return false;
					}
					if (material.shader == _targetShader && (string.IsNullOrEmpty(_materialNameSubstring) || material.name.Contains(_materialNameSubstring)))
					{
						_targetMaterials.Add(material);
					}
				}
			}
			return true;
		}
	}
}
