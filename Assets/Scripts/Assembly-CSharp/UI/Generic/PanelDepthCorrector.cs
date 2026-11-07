using JetBrains.Annotations;
using Kiloo.Common.Timer;
using NaughtyAttributes;
using UnityEngine;

namespace UI.Generic
{
	public class PanelDepthCorrector : MonoBehaviour
	{
		private enum Execute
		{
			Off = 0,
			OnEnable = 1,
			OnStart = 2,
			OnUpdate = 3,
			OnFixedUpdate = 4,
			OnLateUpdate = 5
		}

		[Tooltip("Panel to bump")]
		[SerializeField]
		private UIPanel _targetPanel;

		[Tooltip("When should the depth correction happen?")]
		[SerializeField]
		private Execute _execute;

		[UsedImplicitly]
		private void OnEnable()
		{
			if (_execute == Execute.OnEnable)
			{
				CorrectNow();
			}
		}

		[UsedImplicitly]
		private void Start()
		{
			if (_execute == Execute.OnStart)
			{
				CorrectNow();
			}
		}

		[UsedImplicitly]
		private void Update()
		{
			if (_execute == Execute.OnUpdate)
			{
				CorrectNow();
			}
		}

		[UsedImplicitly]
		private void FixedUpdate()
		{
			if (_execute == Execute.OnFixedUpdate)
			{
				CorrectNow();
			}
		}

		[UsedImplicitly]
		private void LateUpdate()
		{
			if (_execute == Execute.OnLateUpdate)
			{
				CorrectNow();
			}
		}

		[Button(null)]
		public void CorrectNow()
		{
			Timer.ExecuteNextFrame(() =>
			{
				if (!(_targetPanel.parentPanel == null))
				{
					_targetPanel.depth = _targetPanel.parentPanel.depth + 1;
				}
			});
		}
	}
}
