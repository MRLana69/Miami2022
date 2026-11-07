using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Inputs;
using UnityEngine;

namespace SYBO.Common.Managers
{
	public class TimeScaleManager : SingletonMonoBehaviour<TimeScaleManager>
	{
		private const float TimeScaleDefault = 1f;

		private float _timeScaleInMenuDefault = 1f;

		private float _timeScaleInGameDefault = 1f;

		public float TimeScale
		{
			get
			{
				return Time.timeScale;
			}
			set
			{
				Time.timeScale = value;
			}
		}

		public float TimeScaleInMenuDefault
		{
			get
			{
				return _timeScaleInMenuDefault;
			}
			set
			{
				_timeScaleInMenuDefault = value;
			}
		}

		public float TimeScaleInGameDefault
		{
			get
			{
				return _timeScaleInGameDefault;
			}
			set
			{
				_timeScaleInGameDefault = value;
			}
		}

		protected override void SubclassInitialize()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (SingletonMonoBehaviour<InputManager>.IsInstantiated() && !SingletonMonoBehaviour.IsShuttingDown)
			{
				SingletonMonoBehaviour<InputManager>.Instance.OnInputEvent -= InputManager_OnInputEvent;
			}
		}

		private void InputManager_OnInputEvent(object sender, InputEventArgs inputEventArgs)
		{
			if (inputEventArgs.InputType == InputType.KeyDownEnter)
			{
				switch (inputEventArgs.KeyCode)
				{
				case KeyCode.Alpha1:
					TimeScale = 0f;
					break;
				case KeyCode.Alpha2:
					TimeScale = Mathf.Clamp01(TimeScale - 0.1f);
					break;
				case KeyCode.Alpha3:
					TimeScale += 0.1f;
					break;
				case KeyCode.Alpha4:
					TimeScale = 1f;
					break;
				case KeyCode.Alpha5:
					TimeScale = Mathf.Clamp01(TimeScale * 0.9f);
					break;
				case KeyCode.Alpha6:
					TimeScale = TimeScale * 1f / 0.9f;
					break;
				case KeyCode.Alpha7:
					TimeScale = 20f;
					break;
				}
			}
		}
	}
}
