using System;
using System.Diagnostics;
using System.Threading;
using SYBO.Common.Exceptions;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Subway.Inputs
{
	[PrefabPath("Prefabs/Managers/InputManager")]
	public class InputManager : SingletonMonoBehaviour<InputManager>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<InputEventArgs> OnInputEvent__BackingField;

		[SerializeField]
		private Game.SwipeInfo _swipeInfo;

		private bool _isEnabled = true;

		private InputEventArgs _previousInputEventArgs;

		private InputEventArgs _currentInputEventArgs;

		private Swipe _currentSwipe;

		private float _lastTapTime = float.MinValue;

		private Camera __gameCamera;

		private Camera __menuCamera;

		private int _lastInputFrame;

		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				if (_isEnabled != value)
				{
					_isEnabled = value;
				}
			}
		}

		public InputEventArgs PreviousInputEventArgs
		{
			get
			{
				return _previousInputEventArgs;
			}
		}

		public InputEventArgs CurrentInputEventArgs
		{
			get
			{
				return _currentInputEventArgs;
			}
		}

		public bool WasButtonClicked { get; set; }

		private Camera _gameCamera
		{
			get
			{
				if (__gameCamera == null)
				{
					__gameCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
				}
				return __gameCamera;
			}
		}

		private Camera _menuCamera
		{
			get
			{
				if (__menuCamera == null)
				{
					__menuCamera = GameObject.Find("1normal 2D Camera").GetComponent<Camera>();
				}
				return __menuCamera;
			}
		}

		public event EventHandler<InputEventArgs> OnInputEvent
		{
			add
			{
				EventHandler<InputEventArgs> eventHandler = OnInputEvent__BackingField;
				EventHandler<InputEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnInputEvent__BackingField, (EventHandler<InputEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<InputEventArgs> eventHandler = OnInputEvent__BackingField;
				EventHandler<InputEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnInputEvent__BackingField, (EventHandler<InputEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		protected void Start()
		{
			_currentInputEventArgs = new InputEventArgs(InputType.None, KeyCode.None);
			_previousInputEventArgs = new InputEventArgs(InputType.None, KeyCode.None);
		}

		private void FixedUpdate()
		{
			HandleControls();
		}

		protected void Update()
		{
			if (!_isEnabled || (Game.Instance.IsPaused && !Game.Instance.IsInGame.Value))
			{
				_currentSwipe = null;
			}
			else
			{
				HandleControls();
			}
		}

		private void ProcessKeyCode(KeyCode keyCode)
		{
			if ((_previousInputEventArgs.InputType == InputType.KeyDownEnter || _previousInputEventArgs.InputType == InputType.KeyDownStay) && _previousInputEventArgs.KeyCode == keyCode && Input.GetKey(keyCode))
			{
				RaiseOnInputEvent(new InputEventArgs(InputType.KeyDownStay, keyCode));
			}
			if (Input.GetKeyDown(keyCode))
			{
				RaiseOnInputEvent(new InputEventArgs(InputType.KeyDownEnter, keyCode));
			}
			if (Input.GetKeyUp(keyCode))
			{
				RaiseOnInputEvent(new InputEventArgs(InputType.KeyDownExit, keyCode));
			}
		}

		private void HandleControls()
		{
			if (Time.frameCount == _lastInputFrame)
			{
				return;
			}
			_lastInputFrame = Time.frameCount;
			if (Input.touchCount <= 0)
			{
				return;
			}
			Touch touch = Input.touches[0];
			if (touch.phase == TouchPhase.Began)
			{
				_currentSwipe = new Swipe();
				_currentSwipe.start = touch.position;
				_currentSwipe.startTime = Time.time;
			}
			if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && _currentSwipe != null)
			{
				_currentSwipe.endTime = Time.time;
				_currentSwipe.end = touch.position;
				SwipeDir swipeDir = AnalyzeSwipe(_currentSwipe);
				if (swipeDir != SwipeDir.None)
				{
					ConvertSwipeToInputEventAndRaise(_currentSwipe, swipeDir);
					_currentSwipe = null;
				}
			}
			if (touch.phase == TouchPhase.Ended && _currentSwipe != null)
			{
				_currentSwipe.endTime = Time.time;
				_currentSwipe.end = touch.position;
				SwipeDir swipeDir2 = AnalyzeSwipe(_currentSwipe);
				if (swipeDir2 == SwipeDir.None)
				{
					HandleTap();
				}
			}
		}

		private SwipeDir AnalyzeSwipe(Swipe nextSwipe)
		{
			Camera camera = _gameCamera;
			if (camera == null)
			{
				camera = _menuCamera;
			}
			SwipeDir result = SwipeDir.None;
			float num = -1f;
			if (camera != null)
			{
				Vector3 b = camera.ScreenToWorldPoint(new Vector3(nextSwipe.start.x, nextSwipe.start.y, 2f));
				Vector3 a = camera.ScreenToWorldPoint(new Vector3(nextSwipe.end.x, nextSwipe.end.y, 2f));
				num = Vector3.Distance(a, b);
				if (num < _swipeInfo.distanceMin)
				{
					result = SwipeDir.None;
				}
				else
				{
					Vector3 lhs = nextSwipe.end - nextSwipe.start;
					float num2 = 0f;
					float num3 = Vector3.Dot(lhs, Vector3.up);
					if (num3 > num2)
					{
						num2 = num3;
						result = SwipeDir.Up;
					}
					num3 = Vector3.Dot(lhs, Vector3.down);
					if (num3 > num2)
					{
						num2 = num3;
						result = SwipeDir.Down;
					}
					num3 = Vector3.Dot(lhs, Vector3.left);
					if (num3 > num2)
					{
						num2 = num3;
						result = SwipeDir.Left;
					}
					num3 = Vector3.Dot(lhs, Vector3.right);
					if (num3 > num2)
					{
						num2 = num3;
						result = SwipeDir.Right;
					}
				}
			}
			return result;
		}

		private int _gjTapCount;

		private bool HandleTap()
		{
			// Prancha com 3 toques: só liga a prancha no terceiro toque seguido (evita ligar sem querer)
			if (TrainingOptions.TripleTapBoard)
			{
				_gjTapCount = (Time.time < _lastTapTime + _swipeInfo.doubleTapDuration) ? (_gjTapCount + 1) : 1;
				_lastTapTime = Time.time;
				bool triple = false;
				if (!WasButtonClicked)
				{
					if (_gjTapCount >= 3)
					{
						_gjTapCount = 0;
						triple = true;
						RaiseOnInputEvent(new InputEventArgs(InputType.FingerDoubleTap));
					}
					else
					{
						RaiseOnInputEvent(new InputEventArgs(InputType.FingerTap));
					}
				}
				WasButtonClicked = false;
				return triple;
			}
			bool result = false;
			if (Time.time < _lastTapTime + _swipeInfo.doubleTapDuration)
			{
				if (!WasButtonClicked)
				{
					RaiseOnInputEvent(new InputEventArgs(InputType.FingerDoubleTap));
					result = true;
				}
			}
			else if (!WasButtonClicked)
			{
				RaiseOnInputEvent(new InputEventArgs(InputType.FingerTap));
			}
			WasButtonClicked = false;
			_lastTapTime = Time.time;
			return result;
		}

		private void ConvertSwipeToInputEventAndRaise(Swipe swipe, SwipeDir swipeDir)
		{
			InputType inputType = InputType.None;
			switch (swipeDir)
			{
			case SwipeDir.Up:
				inputType = InputType.SwipeUp;
				break;
			case SwipeDir.Down:
				inputType = InputType.SwipeDown;
				break;
			case SwipeDir.Left:
				inputType = InputType.SwipeLeft;
				break;
			case SwipeDir.Right:
				inputType = InputType.SwipeRight;
				break;
			default:
				SwitchDefaultException.Throw(swipeDir.ToString());
				break;
			}
			RaiseOnInputEvent(new InputEventArgs(inputType));
		}

		protected override void SubclassInitialize()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
		}

		public void SendInputEvent(InputEventArgs inputEventArgs)
		{
			RaiseOnInputEvent(inputEventArgs);
		}

		private void RaiseOnInputEvent(InputEventArgs inputEventArgs)
		{
			_currentInputEventArgs = inputEventArgs;
			if (OnInputEvent__BackingField != null)
			{
				OnInputEvent__BackingField(this, _currentInputEventArgs);
			}
			_previousInputEventArgs = _currentInputEventArgs;
		}
	}
}
