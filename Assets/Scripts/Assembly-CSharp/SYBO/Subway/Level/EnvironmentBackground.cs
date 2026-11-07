using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Subway.Cameras;
using SYBO.Subway.Routes;
using SYBO.Subway.Themes;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class EnvironmentBackground : MonoBehaviour
	{
		[Tooltip("Note that tweaking background layers during runtime does not change the original Theme asset!")]
		[SerializeField]
		private List<BackgroundLayer> _backgroundLayers = new List<BackgroundLayer>();

		private List<BackgroundCulling> _backgroundCulling = new List<BackgroundCulling>();

		private Material _cullingMaterial;

		private Transform _characterCameraTransform;

		private Transform _transform;

		private ThemeManager _themeManager;

		private Theme _theme;

		private WaitForSeconds _waitForOneSecond;

		private float _layerSpacing;

		private EnvironmentType _lastSpawnedEnvironmentType;

		private void Awake()
		{
			_transform = base.transform;
			_waitForOneSecond = new WaitForSeconds(1f);
			RouteChunk.OnEnvironmentSegmentSpawned = (Action<EnvironmentKind, int, Route>)Delegate.Combine(RouteChunk.OnEnvironmentSegmentSpawned, new Action<EnvironmentKind, int, Route>(RouteChunk_OnEnvironmentSegmentSpawned));
			Game.OnMenuScreenShown += Game_OnMenuScreenShown;
		}

		private void OnDestroy()
		{
			if (_themeManager != null)
			{
				_themeManager.ThemeChanged -= ThemeManager_ThemeChanged;
			}
		}

		private IEnumerator Start()
		{
			_characterCameraTransform = CharacterCamera.Instance.transform;
			_themeManager = ThemeManager.Instance;
			_themeManager.ThemeChanged += ThemeManager_ThemeChanged;
			_theme = _themeManager.Theme;
			ResetBackground(_theme);
			while (true)
			{
				ExecuteOnEachBackgroundLayer(UpdateLayer);
				yield return _waitForOneSecond;
			}
		}

		private void ThemeManager_ThemeChanged(Theme theme)
		{
			ResetBackground(theme);
		}

		private void ResetBackground(Theme theme)
		{
			_layerSpacing = theme.BackgroundLayerSpacing;
			ExecuteOnEachBackgroundLayer(ClearData);
			_backgroundLayers.Clear();
			for (int i = 0; i < theme.BackgroundLayers.Length; i++)
			{
				BackgroundLayer backgroundLayer = theme.BackgroundLayers[i];
				if (DeviceInfo.Instance.IsDeviceCapablePerformance(backgroundLayer.RequiredPerformanceLevel))
				{
					_backgroundLayers.Add(theme.BackgroundLayers[i]);
				}
			}
			_backgroundCulling.Clear();
			for (int j = 0; j < theme.BackgroundCulling.Length; j++)
			{
				_backgroundCulling.Add(theme.BackgroundCulling[j]);
			}
			if (_cullingMaterial == null)
			{
				_cullingMaterial = new Material(Shader.Find("Hidden/Background Culling"));
				_cullingMaterial.SetColor("_Color", theme.BackgroundCullingColor);
			}
			int renderQueue = 1000 + _backgroundLayers.Count + 5;
			foreach (BackgroundCulling item in _backgroundCulling)
			{
				item.Initialize(_cullingMaterial, renderQueue);
			}
			ExecuteOnEachBackgroundLayer(PrepareLayer);
		}

		private void LateUpdate()
		{
			_transform.position = new Vector3(0f, 0f, _characterCameraTransform.position.z + 1000f);
			float z = GamePositionRoot.Instance.GetGamePosition(_transform).z;
			foreach (BackgroundCulling item in _backgroundCulling)
			{
				item.UpdateCulling(z);
			}
		}

		private void ExecuteOnEachBackgroundLayer(Action<BackgroundLayer> actionToExecute)
		{
			for (int i = 0; i < _backgroundLayers.Count; i++)
			{
				actionToExecute(_backgroundLayers[i]);
			}
		}

		private void UpdateLayer(BackgroundLayer layer)
		{
			layer.NextSingleObjectSpawnIndex = UpdateMovingSingleObjects(layer.MovingSingleObjects, layer, layer.NextSingleObjectSpawnIndex);
			UpdateSkylineFillMovement(layer.MovingSkylineFillObjects, layer);
		}

		private void ClearData(BackgroundLayer layer)
		{
			for (int i = 0; i < layer.BackgroundObjects.Count; i++)
			{
				UnityEngine.Object.Destroy(layer.BackgroundObjects[i].gameObject);
			}
			layer.BackgroundObjects.Clear();
			layer.MovingSingleObjects.Clear();
			layer.MovingSkylineFillObjects.Clear();
		}

		private void PrepareLayer(BackgroundLayer layer)
		{
			if (layer.MaterialInstance != null)
			{
				UnityEngine.Object.Destroy(layer.MaterialInstance);
				layer.MaterialInstance = null;
			}
			if (layer.UseCommonMaterial)
			{
				layer.MaterialInstance = new Material(_themeManager.Theme.BackgroundLayerCommonMaterialShader);
				layer.MaterialInstance.color = layer.CommonMaterialTintColor;
			}
			for (int i = 0; i < layer.SingleObjectPrefabs.Length; i++)
			{
				if (!(layer.SingleObjectPrefabs[i] == null))
				{
					BackgroundObject backgroundObject = InitializeBackgroundObject(layer.SingleObjectPrefabs[i], layer);
					if (backgroundObject.Movable)
					{
						MoveGameObject component = backgroundObject.GetComponent<MoveGameObject>();
						component.UpdatePosition(backgroundObject.transform.localPosition);
						layer.MovingSingleObjects.Add(backgroundObject);
					}
				}
			}
			PlaceMovingSingleObjects(layer.MovingSingleObjects, layer);
			PlaceSkylineFillObjects(layer);
		}

		private BackgroundObject InitializeBackgroundObject(BackgroundObject bgObject, BackgroundLayer layer)
		{
			BackgroundObject backgroundObject = UnityEngine.Object.Instantiate(bgObject);
			Vector3 localPosition = new Vector3(0f, 0f, GetZ(layer.LayerIndex)) + layer.OffsetFromLayerDefault;
			backgroundObject.Initialize(layer, _transform, localPosition);
			layer.BackgroundObjects.Add(backgroundObject);
			SetObjectVisibility(backgroundObject, false);
			return backgroundObject;
		}

		private void PlaceMovingSingleObjects(List<BackgroundObject> singleObjects, BackgroundLayer layer)
		{
			if (singleObjects.Count != 0)
			{
				float num = UnityEngine.Random.value * (_theme.BackgroundSkylineRightLimit - _theme.BackgroundSkylineLeftLimit);
				if (layer.DontRandomizeSingleObjectSpawnPosition)
				{
					num = layer.OffsetFromLayerDefault.x;
				}
				num += _theme.BackgroundSkylineLeftLimit;
				layer.NextSingleObjectSpawnIndex = UnityEngine.Random.Range(0, singleObjects.Count);
				singleObjects[layer.NextSingleObjectSpawnIndex].transform.localPosition = new Vector3(num, 0f, GetZ(layer.LayerIndex)) + layer.OffsetFromLayerDefault;
				SetObjectVisibility(singleObjects[layer.NextSingleObjectSpawnIndex], true);
			}
		}

		private void PlaceSkylineFillObjects(BackgroundLayer layer)
		{
			float num = _theme.BackgroundSkylineLeftLimit;
			if (layer.SkylineFillObjectPrefabs.Length == 0)
			{
				return;
			}
			float num2 = 0f;
			while (num < _theme.BackgroundSkylineRightLimit + num2)
			{
				int num3 = UnityEngine.Random.Range(0, layer.SkylineFillObjectPrefabs.Length);
				BackgroundObject backgroundObject = InitializeBackgroundObject(layer.SkylineFillObjectPrefabs[num3], layer);
				backgroundObject.transform.localPosition = new Vector3(num, 0f, GetZ(layer.LayerIndex)) + layer.OffsetFromLayerDefault;
				float maxBoundsSizeX = GetMaxBoundsSizeX(backgroundObject.MeshRenderers);
				if (maxBoundsSizeX <= float.Epsilon)
				{
					SDebug.LogErrorFormat(backgroundObject, "The size of background object '{0}' is '{1}', which will cause an endless loop! Fix this immediately! ", backgroundObject.name, maxBoundsSizeX);
					break;
				}
				num += maxBoundsSizeX;
				num2 = Mathf.Max(maxBoundsSizeX, num2);
				if (backgroundObject.Movable)
				{
					MoveGameObject component = backgroundObject.GetComponent<MoveGameObject>();
					component.UpdatePosition(backgroundObject.transform.localPosition);
					layer.MovingSkylineFillObjects.Add(backgroundObject);
				}
				SetObjectVisibility(backgroundObject, true);
			}
		}

		private float GetMaxBoundsSizeX(IEnumerable<MeshRenderer> meshRenderers)
		{
			float num = 0f;
			foreach (MeshRenderer meshRenderer in meshRenderers)
			{
				num = Mathf.Max(num, meshRenderer.bounds.size.x);
			}
			return num;
		}

		private int UpdateMovingSingleObjects(List<BackgroundObject> singleObjects, BackgroundLayer layer, int spawnElementIndex)
		{
			if (spawnElementIndex < 0)
			{
				spawnElementIndex = 0;
			}
			if (spawnElementIndex >= singleObjects.Count)
			{
				return 0;
			}
			BackgroundObject backgroundObject = singleObjects[spawnElementIndex];
			if (backgroundObject.transform.localPosition.x > _theme.BackgroundSkylineRightLimit)
			{
				backgroundObject.transform.localPosition = new Vector3(_theme.BackgroundSkylineLeftLimit - GetMaxBoundsSizeX(backgroundObject.MeshRenderers) * 0.5f, 0f, GetZ(layer.LayerIndex)) + layer.OffsetFromLayerDefault;
				SetObjectVisibility(backgroundObject, false);
				spawnElementIndex = ((spawnElementIndex != singleObjects.Count - 1) ? (spawnElementIndex + 1) : 0);
				SetObjectVisibility(singleObjects[spawnElementIndex], true);
				singleObjects[spawnElementIndex].transform.localPosition = new Vector3(_theme.BackgroundSkylineLeftLimit - GetMaxBoundsSizeX(singleObjects[spawnElementIndex].MeshRenderers) * 0.5f, 0f, GetZ(layer.LayerIndex)) + layer.OffsetFromLayerDefault;
				if (backgroundObject.Movable)
				{
					MoveGameObject component = backgroundObject.GetComponent<MoveGameObject>();
					component.UpdatePosition(backgroundObject.transform.localPosition);
				}
			}
			return spawnElementIndex;
		}

		private void UpdateSkylineFillMovement(List<BackgroundObject> bgObjects, BackgroundLayer layer)
		{
			for (int i = 0; i < bgObjects.Count; i++)
			{
				BackgroundObject backgroundObject = bgObjects[i];
				if (backgroundObject.transform.localPosition.x > _theme.BackgroundSkylineRightLimit)
				{
					int num = i + 1;
					if (num >= bgObjects.Count)
					{
						num = 0;
					}
					float num2 = GetMaxBoundsSizeX(bgObjects[num].MeshRenderers) * 0.5f + GetMaxBoundsSizeX(backgroundObject.MeshRenderers) * 0.5f;
					float x = bgObjects[num].transform.position.x - num2;
					backgroundObject.transform.localPosition = new Vector3(x, 0f, GetZ(layer.LayerIndex)) + layer.OffsetFromLayerDefault;
					if (backgroundObject.Movable)
					{
						MoveGameObject component = backgroundObject.GetComponent<MoveGameObject>();
						component.UpdatePosition(backgroundObject.transform.localPosition);
					}
				}
			}
		}

		private float GetZ(int layerIndex)
		{
			return (float)(-layerIndex) * _layerSpacing;
		}

		private void SetObjectVisibility(BackgroundObject bo, bool visibility)
		{
			if (!bo.AlwaysVisible)
			{
				bo.gameObject.SetActive(visibility);
			}
		}

		private void RouteChunk_OnEnvironmentSegmentSpawned(EnvironmentKind kind, int gamePositionZ, Route route)
		{
			if (route.Type != RouteType.Default)
			{
				return;
			}
			EnvironmentType type = kind.Type;
			foreach (BackgroundCulling item in _backgroundCulling)
			{
				if (item.EnvironmentType == type)
				{
					if (type != _lastSpawnedEnvironmentType)
					{
						item.SetCullingStartZ(gamePositionZ);
						item.SetCullingEndZ(gamePositionZ + EnvironmentInfo.GetByKind(kind).MinLengthInBlocks * 90);
					}
					else
					{
						item.SetCullingEndZ(gamePositionZ + 16200);
					}
				}
				else if (type != _lastSpawnedEnvironmentType && item.EnvironmentType == _lastSpawnedEnvironmentType)
				{
					item.SetCullingEndZ(gamePositionZ);
				}
			}
			_lastSpawnedEnvironmentType = type;
		}

		private void Game_OnMenuScreenShown()
		{
			foreach (BackgroundCulling item in _backgroundCulling)
			{
				item.Reset();
			}
		}
	}
}
