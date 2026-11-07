using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SYBO.Subway.Level
{
	[Serializable]
	public class BackgroundLayer
	{
		public string Name;

		[Tooltip("Only one object selected at a time to be shown.")]
		public BackgroundObject[] SingleObjectPrefabs = new BackgroundObject[0];

		[Tooltip("Shown object selected randomly to fill the screen from left to right. Use Skyline shader specified in theme.")]
		public BackgroundObject[] SkylineFillObjectPrefabs = new BackgroundObject[0];

		[Tooltip("If toggled the spawned single object prefab's position will not be chosen randomly, Offset From Layer Default's x will be used.")]
		[SerializeField]
		private bool _dontRandomizeSingleObjectSpawnPosition;

		[Tooltip("The speed determines how quickly the layer pans to the right, this is a multiplier to the existing MoveGameObject. Changing the value inside of EnivornmentBackground during runtime will save to this Theme.")]
		[SerializeField]
		private float _speed;

		[Tooltip("The layer index determines ordering and position, 0 for back, higher numbers appear in front.")]
		public int LayerIndex;

		[Tooltip("The offset from layer pivot.")]
		[SerializeField]
		private Vector3 _offsetFromLayerDefault;

		[Tooltip("Required minimum device performance category")]
		[SerializeField]
		private DeviceInfo.PerformanceLevel _requiredPerformanceLevel;

		[Tooltip("Use same material on all current layer objects")]
		[SerializeField]
		private bool _useCommonMaterial;

		[FormerlySerializedAs("_tintColor")]
		[Tooltip("The tint color determines the tint for the mesh. Changing the value inside of EnivornmentBackground during runtime will save to this Theme.")]
		[SerializeField]
		private Color _commonMaterialTintColor;

		private readonly List<BackgroundObject> _backgroundObjects = new List<BackgroundObject>();

		private readonly List<BackgroundObject> _movingSingleObjects = new List<BackgroundObject>();

		private readonly List<BackgroundObject> _movingSkylineFillObjects = new List<BackgroundObject>();

		public float Speed
		{
			get
			{
				return _speed;
			}
			set
			{
				_speed = value;
			}
		}

		public Color CommonMaterialTintColor
		{
			get
			{
				return _commonMaterialTintColor;
			}
			set
			{
				_commonMaterialTintColor = value;
			}
		}

		public DeviceInfo.PerformanceLevel RequiredPerformanceLevel
		{
			get
			{
				return _requiredPerformanceLevel;
			}
		}

		public bool UseCommonMaterial
		{
			get
			{
				return _useCommonMaterial;
			}
		}

		public bool DontRandomizeSingleObjectSpawnPosition
		{
			get
			{
				return _dontRandomizeSingleObjectSpawnPosition;
			}
		}

		public Vector3 OffsetFromLayerDefault
		{
			get
			{
				return _offsetFromLayerDefault;
			}
		}

		public int NextSingleObjectSpawnIndex { get; set; }

		public Material MaterialInstance { get; set; }

		public List<BackgroundObject> BackgroundObjects
		{
			get
			{
				return _backgroundObjects;
			}
		}

		public List<BackgroundObject> MovingSingleObjects
		{
			get
			{
				return _movingSingleObjects;
			}
		}

		public List<BackgroundObject> MovingSkylineFillObjects
		{
			get
			{
				return _movingSkylineFillObjects;
			}
		}

		public BackgroundLayer()
		{
			_speed = 1f;
			_commonMaterialTintColor = Color.white;
		}

		public BackgroundLayer(BackgroundLayer layerToCopy)
		{
			SingleObjectPrefabs = layerToCopy.SingleObjectPrefabs;
			SkylineFillObjectPrefabs = layerToCopy.SkylineFillObjectPrefabs;
			_speed = layerToCopy.Speed;
			_commonMaterialTintColor = layerToCopy.CommonMaterialTintColor;
			LayerIndex = layerToCopy.LayerIndex;
		}
	}
}
