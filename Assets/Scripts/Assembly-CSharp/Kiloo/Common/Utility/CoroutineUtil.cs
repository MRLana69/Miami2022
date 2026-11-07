using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kiloo.Common.Utility
{
	public static class CoroutineUtil
	{
		private static MonoBehaviour _coroutineRunner;

		public static Coroutine StartCoroutine(IEnumerator coroutine)
		{
			if (!Application.isPlaying)
			{
				throw new Exception("CoroutineUtil.StartCoroutine must be in play mode (or objects will leak)");
			}
			if (_coroutineRunner == null)
			{
				GameObject gameObject = new GameObject("CoroutineRunner");
				_coroutineRunner = gameObject.AddComponent<NewMonoBehaviour>();
				UnityEngine.Object.DontDestroyOnLoad(_coroutineRunner.gameObject);
			}
			return _coroutineRunner.StartCoroutine(coroutine);
		}

		public static void StopCoroutine(IEnumerator coroutine)
		{
			if (_coroutineRunner != null && coroutine != null)
			{
				_coroutineRunner.StopCoroutine(coroutine);
			}
		}

		public static void StopCoroutine(Coroutine coroutine)
		{
			if (_coroutineRunner != null && coroutine != null)
			{
				_coroutineRunner.StopCoroutine(coroutine);
			}
		}

		public static Coroutine WaitForAll(IEnumerable<YieldInstruction> list, MonoBehaviour proxy = null)
		{
			IEnumerator enumerator = WaitForAllCoroutine(list);
			return (!(proxy != null)) ? StartCoroutine(enumerator) : proxy.StartCoroutine(enumerator);
		}

		public static Coroutine WaitForAll(params YieldInstruction[] list)
		{
			return WaitForAll(list, null);
		}

		private static IEnumerator WaitForAllCoroutine(IEnumerable<YieldInstruction> list)
		{
			foreach (YieldInstruction item in list)
			{
				yield return item;
			}
		}

		public static IEnumerator WaitForRealSeconds(float timeToWait)
		{
			float start = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup < start + timeToWait)
			{
				yield return null;
			}
		}

		public static IEnumerator ExecuteActionNextFrame(Action action)
		{
			yield return new WaitForEndOfFrame();
			action();
		}
	}
}
