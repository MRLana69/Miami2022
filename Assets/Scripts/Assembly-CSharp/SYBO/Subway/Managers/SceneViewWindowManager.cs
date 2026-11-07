using System.Collections.Generic;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Characters;
using UnityEngine;

namespace SYBO.Subway.Managers
{
	public class SceneViewWindowManager : SingletonMonoBehaviour<SceneViewWindowManager>
	{
		private Character _character;

		private bool _isEnabled;

		private SceneViewCameraTargetType __sceneViewCameraTargetType;

		private SceneViewCameraZoomType __sceneViewCameraZoomType;

		protected List<SceneViewCameraTargetType> _sceneViewCameraTargetTypes;

		protected int _sceneViewCameraTargetTypesIndex;

		protected List<SceneViewCameraZoomType> _sceneViewCameraZoomTypes;

		protected int _sceneViewCameraZoomTypesIndex;

		public Character Character
		{
			get
			{
				return _character;
			}
			set
			{
				_character = value;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				_isEnabled = value;
			}
		}

		public SceneViewCameraTargetType SceneViewCameraTargetType
		{
			get
			{
				return __sceneViewCameraTargetType;
			}
			set
			{
				__sceneViewCameraTargetType = value;
				RepaintSceneView();
			}
		}

		public SceneViewCameraZoomType SceneViewCameraZoomType
		{
			get
			{
				return __sceneViewCameraZoomType;
			}
			set
			{
				__sceneViewCameraZoomType = value;
				if (HasSceneView())
				{
					UpdatePropertiesOnSceneView();
					RepaintSceneView();
				}
			}
		}

		public Vector3 GetSceneViewPivot()
		{
			switch (SceneViewCameraTargetType)
			{
			case SceneViewCameraTargetType.Character:
				return _character.TransformPosition;
			default:
				return new Vector3(0f, 0f, 0f);
			}
		}

		public Quaternion GetSceneViewRotation()
		{
			Quaternion result;
			switch (SceneViewCameraZoomType)
			{
			case SceneViewCameraZoomType.Default:
				result = new Quaternion(0.4f, -0.3f, 0.2f, 0.81f);
				break;
			default:
				result = new Quaternion(0.05f, 0f, 0f, 0.05f);
				break;
			}
			return result;
		}

		protected override void SubclassInitialize()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
		}

		public void ShowNextSceneViewCameraTargetType()
		{
			if (IsEnabled)
			{
				if (_sceneViewCameraTargetTypes == null)
				{
					_sceneViewCameraTargetTypes = new List<SceneViewCameraTargetType>();
					_sceneViewCameraTargetTypes.Add(SceneViewCameraTargetType.Character);
					_sceneViewCameraTargetTypes.Add(SceneViewCameraTargetType.WorldOrigin);
				}
				_sceneViewCameraTargetTypesIndex++;
				if (_sceneViewCameraTargetTypesIndex > _sceneViewCameraTargetTypes.Count - 1)
				{
					_sceneViewCameraTargetTypesIndex = 0;
				}
				SceneViewCameraTargetType = _sceneViewCameraTargetTypes[_sceneViewCameraTargetTypesIndex];
			}
		}

		public void ShowNextSceneViewZoomType()
		{
			if (IsEnabled)
			{
				if (_sceneViewCameraZoomTypes == null)
				{
					_sceneViewCameraZoomTypes = new List<SceneViewCameraZoomType>();
					_sceneViewCameraZoomTypes.Add(SceneViewCameraZoomType.Default);
					_sceneViewCameraZoomTypes.Add(SceneViewCameraZoomType.Chase);
				}
				_sceneViewCameraZoomTypesIndex++;
				if (_sceneViewCameraZoomTypesIndex > _sceneViewCameraZoomTypes.Count - 1)
				{
					_sceneViewCameraZoomTypesIndex = 0;
				}
				SceneViewCameraZoomType = _sceneViewCameraZoomTypes[_sceneViewCameraZoomTypesIndex];
			}
		}

		public bool HasSceneView()
		{
			if (!IsEnabled)
			{
				return false;
			}
			return false;
		}

		public object GetSceneView()
		{
			if (!IsEnabled)
			{
				return null;
			}
			return null;
		}

		public void UpdatePropertiesOnSceneView()
		{
			if (IsEnabled)
			{
			}
		}

		private void RepaintSceneView()
		{
			if (IsEnabled)
			{
			}
		}
	}
}
