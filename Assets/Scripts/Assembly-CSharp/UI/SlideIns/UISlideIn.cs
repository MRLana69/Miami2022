using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using UnityEngine;
using Utilities;

namespace UI.SlideIns
{
	public class UISlideIn : MonoBehaviour
	{
		[SerializeField]
		private UIWidget _darkBackground;

		private Vector3 posOff = new Vector3(0f, 65f, 0f);

		private Vector3 posOn = new Vector3(0f, -30f, 0f);

		private bool _triggerSlideOut;

		private float _slideOutTimer = 3f;

		private bool _triggerReadyForNext;

		private float _readyForNextTimer = 1f;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onDidShowCallback__BackingField;

		private event Action onDidShowCallback
		{
			add
			{
				Action action = onDidShowCallback__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onDidShowCallback__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onDidShowCallback__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onDidShowCallback__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public virtual void SetupSlideIn(SlideIn slideIn)
		{
		}

		protected void Start()
		{
			base.transform.localPosition = posOff;
			base.gameObject.SetActive(false);
		}

		protected void SlideIn(Action onDidShowCallback = null)
		{
			if (_darkBackground != null)
			{
				_darkBackground.UpdateAnchors();
			}
			onDidShowCallback__BackingField = onDidShowCallback;
			if (SafeArea.IsRestricted)
			{
				SpringPosition.Begin(base.gameObject, new Vector3(0f, -SafeArea.OFFSET_TOP, 0f), 10f).ignoreTimeScale = true;
			}
			else
			{
				SpringPosition.Begin(base.gameObject, posOn, 10f).ignoreTimeScale = true;
			}
			_slideOutTimer = 3f;
			_readyForNextTimer = 1f;
			StartCoroutine("InvokeDidShowCallback", 0.5f);
			_triggerSlideOut = true;
		}

		private IEnumerator InvokeDidShowCallback(float waitTime)
		{
			float startPoint = 0f;
			while (startPoint < waitTime)
			{
				startPoint += RealTimeTracker.deltaTime;
				yield return null;
			}
			onDidShowCallback__BackingField.SafeInvoke();
		}

		protected virtual void SlideOut()
		{
			SpringPosition.Begin(base.gameObject, posOff, 10f).ignoreTimeScale = true;
			_triggerReadyForNext = true;
		}

		protected virtual void ReadyForNewMessage()
		{
			base.gameObject.SetActive(false);
			UIScreenController.Instance.ReadyForNextSlide();
		}

		private void Update()
		{
			if (!_triggerReadyForNext && !_triggerSlideOut)
			{
				return;
			}
			float deltaTime = RealTimeTracker.deltaTime;
			if (_triggerSlideOut)
			{
				_slideOutTimer -= deltaTime;
				if (_slideOutTimer <= 0f)
				{
					SlideOut();
					_triggerSlideOut = false;
				}
			}
			if (_triggerReadyForNext)
			{
				_readyForNextTimer -= deltaTime;
				if (_readyForNextTimer <= 0f)
				{
					_triggerReadyForNext = false;
					ReadyForNewMessage();
				}
			}
		}

		public IEnumerator PreloadSlideIn()
		{
			base.gameObject.SetActive(true);
			yield return null;
			yield return null;
			base.gameObject.SetActive(false);
		}
	}
}
