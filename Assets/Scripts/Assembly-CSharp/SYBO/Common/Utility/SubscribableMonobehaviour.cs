using System;
using UnityEngine;

namespace SYBO.Common.Utility
{
	public class SubscribableMonobehaviour : MonoBehaviour
	{
		public Action OnUpdate;

		public Action OnLateUpdate;

		public Action OnFixedUpdate;

		public Action OnOnDisable;

		public Action OnOnEnable;

		private void Update()
		{
			if (OnUpdate != null)
			{
				OnUpdate();
			}
		}

		private void LateUpdate()
		{
			if (OnLateUpdate != null)
			{
				OnLateUpdate();
			}
		}

		private void FixedUpdate()
		{
			if (OnFixedUpdate != null)
			{
				OnFixedUpdate();
			}
		}

		private void OnDisable()
		{
			if (OnOnDisable != null)
			{
				OnOnDisable();
			}
		}

		private void OnEnable()
		{
			if (OnOnEnable != null)
			{
				OnOnEnable();
			}
		}
	}
}
