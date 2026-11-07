using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class BackgroundObject : MonoBehaviour
	{
		[SerializeField]
		private bool _alwaysVisible;

		[SerializeField]
		private bool _movable;

		private MoveGameObject _moveGameObject;

		private Transform _transform;

		private Renderer[] _renderers;

		private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();

		private List<ParticleSystemRenderer> _particleSystemRenderers = new List<ParticleSystemRenderer>();

		private List<Material> _materialInstances = new List<Material>();

		public bool AlwaysVisible
		{
			get
			{
				return _alwaysVisible;
			}
		}

		public bool Movable
		{
			get
			{
				return _movable;
			}
		}

		public Renderer[] Renderers
		{
			get
			{
				return _renderers;
			}
		}

		public List<MeshRenderer> MeshRenderers
		{
			get
			{
				return _meshRenderers;
			}
		}

		public List<ParticleSystemRenderer> ParticleSystemRenderers
		{
			get
			{
				return _particleSystemRenderers;
			}
		}

		private void Awake()
		{
			_transform = GetComponent<Transform>();
			_moveGameObject = GetComponent<MoveGameObject>();
			_renderers = GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < _renderers.Length; i++)
			{
				Renderer renderer = _renderers[i];
				if (renderer is MeshRenderer)
				{
					_meshRenderers.Add(renderer as MeshRenderer);
				}
				else if (renderer is ParticleSystemRenderer)
				{
					_particleSystemRenderers.Add(renderer as ParticleSystemRenderer);
				}
			}
		}

		public void Initialize(BackgroundLayer layer, Transform parent, Vector3 localPosition)
		{
			base.name = layer.Name + " - " + base.name;
			if (_moveGameObject != null)
			{
				_moveGameObject.SpeedMultiplier = layer.Speed;
			}
			_transform.parent = parent;
			_transform.localPosition = localPosition;
			_transform.localRotation = Quaternion.identity;
			for (int i = 0; i < _renderers.Length; i++)
			{
				Renderer renderer = _renderers[i];
				if (renderer is ParticleSystemRenderer)
				{
					renderer.material.renderQueue = 1000 + layer.LayerIndex + 1;
					continue;
				}
				Material[] array = new Material[renderer.sharedMaterials.Length];
				for (int j = 0; j < renderer.sharedMaterials.Length; j++)
				{
					if (layer.UseCommonMaterial)
					{
						array[j] = layer.MaterialInstance;
					}
					else
					{
						Material item = (array[j] = new Material(renderer.sharedMaterials[j]));
						_materialInstances.Add(item);
					}
					array[j].renderQueue = 1000 + layer.LayerIndex + 1;
				}
				renderer.sharedMaterials = array;
			}
		}

		private void OnDestroy()
		{
			foreach (Material materialInstance in _materialInstances)
			{
				Object.Destroy(materialInstance);
			}
		}
	}
}
