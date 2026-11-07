using System;
using System.Collections;
using SYBO.Common.Utility;
using UnityEngine;

namespace SYBO.Common.Benchmarking.Utility
{
	public class LastMonobehaviour : SubscribableMonobehaviour
	{
		public Action OnYieldFixedUpdate;

		private YieldInstruction _fixedUpdateYield;

		private void Awake()
		{
			_fixedUpdateYield = new WaitForFixedUpdate();
			StartCoroutine(FixedUpdateCoroutine());
		}

		private IEnumerator FixedUpdateCoroutine()
		{
			while (true)
			{
				yield return _fixedUpdateYield;
				if (OnYieldFixedUpdate != null)
				{
					OnYieldFixedUpdate();
				}
			}
		}
	}
}
