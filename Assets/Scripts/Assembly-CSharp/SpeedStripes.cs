using System;
using SYBO.Subway;
using SYBO.Subway.Cameras;
using SYBO.Subway.Routes.Prototypes;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using UnityEngine;
using UnityEngine.Serialization;

public class SpeedStripes : MonoBehaviour, IRouteEffectGUIListener
{
	[SerializeField]
	private float _fadeInTime = 0.5f;

	private Color _originalColor;

	[FormerlySerializedAs("_renderes")]
	[SerializeField]
	private Renderer[] _renderers;

	[SerializeField]
	private Material _OriginalMaterial;

	private Material _material;

	private IRouteEffectGUISubject _subject;

	[SerializeField]
	private Color[] _colorSteps;

	[SerializeField]
	private int _gearActivationThreshold = 1;

	[SerializeField]
	private int _gearSteps = 4;

	private Animator _animator;

	private readonly int id_Active = Animator.StringToHash("Active");

	private bool _isActive;

	private Vector3 _originalScale = Vector3.one;

	private GameObject _scalingParent;

	private void Awake()
	{
		JetpackState state = GameStateMachine.Instance.GetState<JetpackState>(4);
		state.OnStart = (Action)Delegate.Combine(state.OnStart, new Action(OnStartJetpack));
		HeadstartState state2 = GameStateMachine.Instance.GetState<HeadstartState>(3);
		state2.OnStart = (Action)Delegate.Combine(state2.OnStart, new Action(OnStartHeadstart));
		HeadstartState state3 = GameStateMachine.Instance.GetState<HeadstartState>(3);
		state3.OnStart = (Action)Delegate.Combine(state3.OnStart, new Action(OnStopHeadstart));
		Hoverboard.Instance.OnSpeedStart += OnSpeederBoardStart;
		Hoverboard.Instance.OnSpeedEnd += OnSpeederBoardStop;
		PogostickState state4 = GameStateMachine.Instance.GetState<PogostickState>(5);
		state4.OnStart = (Action)Delegate.Combine(state4.OnStart, new Action(OnPogoStart));
		PogostickState state5 = GameStateMachine.Instance.GetState<PogostickState>(5);
		state5.OnStop = (Action)Delegate.Combine(state5.OnStop, new Action(OnPogoStop));
		Game.Instance.OnStageMenuSequence += Reset;
		_material = new Material(_OriginalMaterial);
		_originalColor = _material.GetColor("_MainColor");
		_material.SetColor("_MainColor", Color.black);
		_animator = GetComponent<Animator>();
		_scalingParent = new GameObject("SpeedStripesScalingParent");
		_scalingParent.transform.parent = base.transform;
		_scalingParent.transform.localPosition = Vector3.zero;
		_scalingParent.transform.localRotation = Quaternion.identity;
		for (int i = 0; i < _renderers.Length; i++)
		{
			_renderers[i].gameObject.SetActive(true);
			_renderers[i].material = _material;
			_renderers[i].enabled = false;
			Vector3 localScale = _renderers[i].transform.localScale;
			_renderers[i].transform.parent = _scalingParent.transform;
			_renderers[i].transform.localPosition = Vector3.zero;
			_renderers[i].transform.localScale = localScale;
		}
	}

	private void LateUpdate()
	{
		if (_isActive)
		{
			float num = CharacterCamera.Instance.Camera.fieldOfView / 68f;
			_scalingParent.transform.localScale = _originalScale * num;
			for (int i = 0; i < _renderers.Length; i++)
			{
				_renderers[i].enabled = true;
			}
		}
	}

	public virtual void Initialize(IRouteEffectGUISubject subject)
	{
		_subject = subject;
		subject.OnEnteredTallyScreen += HandleOnEnteredTallyScreen;
		subject.OnGearChanged += HandleOnGearChanged;
		_colorSteps = new Color[_gearSteps + 1];
		for (int num = _colorSteps.Length - 2; num >= 0; num--)
		{
			_colorSteps[num] = new Color((float)num * (_originalColor.r / (float)_colorSteps.Length), (float)num * (_originalColor.g / (float)_colorSteps.Length), (float)num * (_originalColor.b / (float)_colorSteps.Length));
		}
		_colorSteps[_gearSteps] = _originalColor;
	}

	public virtual void DeInitialize()
	{
		if (_subject != null)
		{
			_subject.OnEnteredTallyScreen -= HandleOnEnteredTallyScreen;
			_subject.OnGearChanged -= HandleOnGearChanged;
			_subject = null;
		}
		HandleOnEnteredTallyScreen();
	}

	private void HandleOnEnteredSkatePark()
	{
	}

	private void HandleOnEnteredTallyScreen()
	{
		Deactivate(4.2f, Color.black);
	}

	private void HandleOnGearChanged(int gear)
	{
		if (gear > _gearActivationThreshold)
		{
			int num = gear - _gearActivationThreshold;
			int num2 = ((num > 0) ? Mathf.Min(_colorSteps.Length - 1, num) : 0);
			Color color = _material.GetColor("_MainColor");
			Activate(0.4f, color, _colorSteps[num2]);
		}
	}

	private void OnSpeederBoardStart()
	{
		if (!GameStateMachine.Instance.IsCurrentState<HeadstartState>())
		{
			Activate(_fadeInTime, Color.black, _originalColor);
		}
	}

	private void OnSpeederBoardStop()
	{
		if (!Game.Instance.IsInPogostickMode && !GameStateMachine.Instance.IsCurrentState<HeadstartState>())
		{
			Deactivate(_fadeInTime, Color.black);
		}
	}

	private void OnPogoStart()
	{
		if ((Hoverboard.Instance.isActive && Hoverboard.Instance.IsHighSpeedBoard) || (Hoverboard.Instance.isActive && Hoverboard.Instance.IsSpeedBoard))
		{
			Deactivate(0.1f, Color.black);
		}
	}

	private void OnPogoStop()
	{
		if ((Hoverboard.Instance.isActive && Hoverboard.Instance.IsHighSpeedBoard) || (Hoverboard.Instance.isActive && Hoverboard.Instance.IsSpeedBoard))
		{
			Activate(0.1f, Color.black, _originalColor);
		}
	}

	private void OnStartJetpack()
	{
		Deactivate(_fadeInTime, Color.black);
	}

	private void OnStartHeadstart()
	{
		if ((!Hoverboard.Instance.isActive || !Hoverboard.Instance.IsHighSpeedBoard) && (!Hoverboard.Instance.isActive || !Hoverboard.Instance.IsSpeedBoard))
		{
			Activate(_fadeInTime, Color.black, _originalColor);
		}
	}

	private void OnStopHeadstart()
	{
		if ((!Hoverboard.Instance.isActive || !Hoverboard.Instance.IsHighSpeedBoard) && (!Hoverboard.Instance.isActive || !Hoverboard.Instance.IsSpeedBoard))
		{
			Deactivate(_fadeInTime, Color.black);
		}
	}

	private void Activate(float time, Color currentColor, Color colorTargetColor)
	{
		_isActive = true;
		for (int i = 0; i < _renderers.Length; i++)
		{
			_renderers[i].enabled = true;
		}
		if (!_animator.GetBool(id_Active))
		{
			_animator.SetBool(id_Active, true);
		}
		StartCoroutine(pTween.To(time, (float t) =>
		{
			_material.SetColor("_MainColor", Color.Lerp(currentColor, colorTargetColor, t));
		}));
	}

	private void Deactivate(float time, Color colorTarget)
	{
		_isActive = false;
		Color currentColor = _material.GetColor("_MainColor");
		StartCoroutine(pTween.To(time, (float t) =>
		{
			_material.SetColor("_MainColor", Color.Lerp(currentColor, colorTarget, t));
			if (t == 1f)
			{
				Renderer[] renderers = _renderers;
				foreach (Renderer renderer in renderers)
				{
					renderer.enabled = false;
				}
				_animator.SetBool(id_Active, false);
			}
		}));
	}

	private void Reset()
	{
		_isActive = false;
		_animator.SetBool(id_Active, false);
		StopAllCoroutines();
		_material.SetColor("_MainColor", Color.black);
		Renderer[] renderers = _renderers;
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = false;
		}
	}
}
