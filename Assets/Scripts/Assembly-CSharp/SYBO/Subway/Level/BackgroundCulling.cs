using System;
using System.Collections.Generic;
using SYBO.Subway.Themes;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[Serializable]
	public class BackgroundCulling
	{
		private struct CullingBlock
		{
			public int CullingStartGamePositionZ;

			public int CullingEndGamePositionZ;
		}

		private const string BackgroundCullingAlphaPropertyName = "_BackgroundCullingAlpha";

		private const float FadeOutOffset = 100f;

		public EnvironmentType EnvironmentType;

		public Mesh CullingMesh;

		[Tooltip("Offset from environment start for the initial placement of the culling mesh.")]
		public float CullingStartOffset = 90f;

		private LinkedList<CullingBlock> _cullingBlocks = new LinkedList<CullingBlock>();

		private Transform _transform;

		private MeshRenderer _renderer;

		private static float CullingOffset
		{
			get
			{
				return 1000f - ThemeManager.Instance.Theme.FogEndDistance;
			}
		}

		public void Initialize(Material material, int renderQueue)
		{
			if (_transform != null)
			{
				UnityEngine.Object.Destroy(_transform.gameObject);
			}
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<MeshFilter>().sharedMesh = CullingMesh;
			_renderer = gameObject.AddComponent<MeshRenderer>();
			_renderer.material = material;
			_renderer.material.renderQueue = renderQueue;
			_renderer.material.SetFloat("_BackgroundCullingAlpha", 0f);
			_renderer.enabled = false;
			_transform = gameObject.transform;
		}

		public void SetCullingStartZ(int gamePositionZ)
		{
			_cullingBlocks.AddLast(new CullingBlock
			{
				CullingStartGamePositionZ = gamePositionZ
			});
		}

		public void SetCullingEndZ(int gamePositionZ)
		{
			if (_cullingBlocks.Count != 0)
			{
				CullingBlock value = _cullingBlocks.Last.Value;
				value.CullingEndGamePositionZ = gamePositionZ;
				_cullingBlocks.Last.Value = value;
			}
		}

		public void UpdateCulling(float gamePositionZ)
		{
			if (_cullingBlocks.Count > 0)
			{
				CullingBlock value = _cullingBlocks.First.Value;
				float num = CalculateCullingAlpha(gamePositionZ, _cullingBlocks.First.Value);
				if (_cullingBlocks.Count > 1)
				{
					num = Mathf.Max(num, CalculateCullingAlpha(gamePositionZ, _cullingBlocks.First.Next.Value));
				}
				if (num > 0f)
				{
					float gamePositionZ2 = Mathf.Clamp(gamePositionZ - CullingOffset, (float)value.CullingStartGamePositionZ + CullingStartOffset, value.CullingEndGamePositionZ);
					_transform.position = GamePositionRoot.Instance.GetWorldPositionZ(gamePositionZ2) * Vector3.forward;
					_renderer.enabled = true;
					_renderer.material.SetFloat("_BackgroundCullingAlpha", Mathf.Clamp01(num));
				}
				else
				{
					_renderer.enabled = false;
				}
				if (gamePositionZ > (float)value.CullingEndGamePositionZ + CullingOffset + 100f)
				{
					_cullingBlocks.RemoveFirst();
				}
			}
			else
			{
				_renderer.enabled = false;
			}
		}

		public void Reset()
		{
			_cullingBlocks.Clear();
			_renderer.enabled = false;
		}

		private static float CalculateCullingAlpha(float gamePositionZ, CullingBlock block)
		{
			if (gamePositionZ > (float)block.CullingStartGamePositionZ && gamePositionZ < (float)block.CullingEndGamePositionZ + CullingOffset + 100f)
			{
				if (gamePositionZ < (float)block.CullingEndGamePositionZ + 100f)
				{
					return (gamePositionZ - (float)block.CullingStartGamePositionZ) / CullingOffset;
				}
				return 0f - (gamePositionZ - (float)block.CullingEndGamePositionZ - CullingOffset - 100f) / CullingOffset;
			}
			return 0f;
		}
	}
}
