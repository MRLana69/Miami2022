using System;
using System.Diagnostics;
using System.Threading;
using SYBO.Subway.Cameras;
using SYBO.Subway.Characters;
using SYBO.Subway.Themes;
using SYBO.Subway.Visuals;
using UnityEngine;

internal class CharacterRenderingEffects : MonoBehaviour
{
	public delegate void OnDoubleJumpDelegate();

	[SerializeField]
	private Renderer leftJumpThruster;

	[SerializeField]
	private Renderer rightJumpThruster;

	[SerializeField]
	private float thrusterPlayTime = 0.2f;

	[SerializeField]
	private Transform _scoreBoosterRoot;

	[SerializeField]
	private Animation _scoreBoostAnimation;

	[SerializeField]
	private ParticleSystem _scoreBoostParticle;

	[SerializeField]
	private GameObject jetpackParticles;

	[SerializeField]
	private GameObject pogostickParticles;

	public ParticleFollow pogostickParticleCloud;

	public ParticleFollow jetpackParticleCloudL;

	public ParticleFollow jetpackParticleCloudR;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnDoubleJumpDelegate OnDoubleJump__BackingField;

	[SerializeField]
	private GameObject dustPrefab;

	private ParticleSystem dustParticles;

	private AnimationState leftJumpThrusterState;

	private AnimationState rightJumpThrusterState;

	private Vector3 _starsOriginalPosition;

	private Character _character;

	private CharacterModel _characterModel;

	private CharacterCamera characterCamera;

	private Hoverboard _hoverboard;

	private RoofedCulling _environmentCulling;

	public GameObject JetpackParticles
	{
		get
		{
			return jetpackParticles;
		}
	}

	public GameObject PogostickParticles
	{
		get
		{
			return pogostickParticles;
		}
	}

	public event OnDoubleJumpDelegate OnDoubleJump
	{
		add
		{
			OnDoubleJumpDelegate onDoubleJumpDelegate = OnDoubleJump__BackingField;
			OnDoubleJumpDelegate onDoubleJumpDelegate2;
			do
			{
				onDoubleJumpDelegate2 = onDoubleJumpDelegate;
				onDoubleJumpDelegate = Interlocked.CompareExchange(ref OnDoubleJump__BackingField, (OnDoubleJumpDelegate)Delegate.Combine(onDoubleJumpDelegate2, value), onDoubleJumpDelegate);
			}
			while ((object)onDoubleJumpDelegate != onDoubleJumpDelegate2);
		}
		remove
		{
			OnDoubleJumpDelegate onDoubleJumpDelegate = OnDoubleJump__BackingField;
			OnDoubleJumpDelegate onDoubleJumpDelegate2;
			do
			{
				onDoubleJumpDelegate2 = onDoubleJumpDelegate;
				onDoubleJumpDelegate = Interlocked.CompareExchange(ref OnDoubleJump__BackingField, (OnDoubleJumpDelegate)Delegate.Remove(onDoubleJumpDelegate2, value), onDoubleJumpDelegate);
			}
			while ((object)onDoubleJumpDelegate != onDoubleJumpDelegate2);
		}
	}

	private void Awake()
	{
		_environmentCulling = new RoofedCulling();
		if (ThemeManager.Instance.Theme.RoofedCullingEnabled)
		{
			_environmentCulling.Initialize();
		}
		ThemeEffect[] effects = ThemeManager.Instance.Theme.Effects;
		foreach (ThemeEffect themeEffect in effects)
		{
			themeEffect.Initialize(base.transform);
		}
	}

	public void Initialize(CharacterModel characterModel)
	{
		_character = Character.Instance;
		_characterModel = characterModel;
		characterCamera = CharacterCamera.Instance;
		_hoverboard = Hoverboard.Instance;
		InitializeModel(characterModel);
		SetupScoorboosterFX();
	}

	public void InitializeModel(CharacterModel characterModel)
	{
		SetupDoubleJumpThrusters(characterModel.LeftFoot, characterModel.RightFoot);
	}

	public void RecreateModel(CharacterModel characterModel)
	{
		_characterModel = characterModel;
		SetupDoubleJumpThrusters(characterModel.LeftFoot, characterModel.RightFoot);
	}

	private void SetupScoorboosterFX()
	{
		_scoreBoosterRoot.parent = _character.transform;
		_scoreBoosterRoot.localPosition = Vector3.zero;
		_scoreBoosterRoot.localRotation = Quaternion.identity;
		_scoreBoosterRoot.localScale = Vector3.one;
	}

	private void SetupDoubleJumpThrusters(Transform leftFoot, Transform rightFoot)
	{
		leftJumpThruster.transform.parent = leftFoot;
		leftJumpThruster.enabled = false;
		leftJumpThruster.transform.localPosition = new Vector3(0f, 0.25f, -0.8f);
		leftJumpThruster.transform.localRotation = Quaternion.Euler(0f, 90f, 35f);
		leftJumpThrusterState = leftJumpThruster.GetComponent<Animation>()["POOF"];
		leftJumpThrusterState.enabled = true;
		leftJumpThrusterState.weight = 1f;
		rightJumpThruster.transform.parent = rightFoot;
		rightJumpThruster.enabled = false;
		rightJumpThruster.transform.localPosition = new Vector3(0f, -0.25f, 0.8f);
		rightJumpThruster.transform.localRotation = Quaternion.Euler(0f, 270f, 325f);
		rightJumpThrusterState = rightJumpThruster.GetComponent<Animation>()["POOF"];
		rightJumpThrusterState.enabled = true;
		rightJumpThrusterState.weight = 1f;
	}

	public void PlayScoreboostFX(int version)
	{
		if (version == 1)
		{
			_scoreBoostAnimation.Play("ScoreBoosterFirst");
		}
		if (version == 2)
		{
			_scoreBoostAnimation.Play("ScoreBooster");
		}
		if (version == 3)
		{
			_scoreBoostAnimation.Play("ScoreBooster");
			_scoreBoostParticle.Play();
			characterCamera.Shake();
		}
		switch (version)
		{
		}
	}

	public void PlayDoubleJumpThruster()
	{
		if (!_hoverboard.isActive)
		{
			leftJumpThruster.enabled = true;
			rightJumpThruster.enabled = true;
			StartCoroutine(pTween.To(thrusterPlayTime, (float t) =>
			{
				leftJumpThrusterState.normalizedTime = t;
				rightJumpThrusterState.normalizedTime = t;
				if (t == 1f)
				{
					leftJumpThruster.enabled = false;
					rightJumpThruster.enabled = false;
				}
			}));
		}
		else if (OnDoubleJump__BackingField != null)
		{
			OnDoubleJump__BackingField();
		}
	}

	public void SetRightAndLeftParticlesActive(bool active)
	{
		if (jetpackParticleCloudL.Target == null || jetpackParticleCloudR.Target == null)
		{
			jetpackParticleCloudL.Target = _characterModel.JetpackCloudPositionL;
			jetpackParticleCloudR.Target = _characterModel.JetpackCloudPositionR;
		}
		if (active)
		{
			jetpackParticles.SetActive(active);
			jetpackParticleCloudL.gameObject.SetActive(active);
			jetpackParticleCloudL.enabled = active;
			jetpackParticleCloudR.gameObject.SetActive(active);
			jetpackParticleCloudR.enabled = active;
			return;
		}
		jetpackParticleCloudL.enabled = active;
		jetpackParticleCloudR.enabled = active;
		StartCoroutine(pTween.To(0.15f, (float t) =>
		{
			if (t < 1f)
			{
				jetpackParticles.transform.position += new Vector3(0f, -0.05f, 0.02f) * characterCamera.Speed * t;
			}
			if (t == 1f)
			{
				jetpackParticles.SetActive(active);
				jetpackParticleCloudL.gameObject.SetActive(active);
				jetpackParticleCloudR.gameObject.SetActive(active);
			}
		}));
	}

	public void SetPogostickParticlesActive(bool active)
	{
		if (pogostickParticleCloud.Target == null)
		{
			pogostickParticleCloud.Target = _characterModel.AttachPoint1;
		}
		if (active)
		{
			pogostickParticles.SetActive(active);
			pogostickParticleCloud.gameObject.SetActive(active);
			pogostickParticleCloud.enabled = active;
			return;
		}
		pogostickParticleCloud.enabled = active;
		StartCoroutine(pTween.To(0.15f, (float t) =>
		{
			if (t < 1f)
			{
				pogostickParticles.transform.position += new Vector3(0f, -0.01f, 0.02f) * characterCamera.Speed * t;
			}
			if (t == 1f)
			{
				pogostickParticles.SetActive(active);
				pogostickParticleCloud.gameObject.SetActive(active);
			}
		}));
	}

	public void ToggleDust(bool active)
	{
		if (dustParticles == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(dustPrefab);
			dustParticles = gameObject.GetComponentInChildren<ParticleSystem>();
			gameObject.transform.parent = _characterModel.transform;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localPosition = Vector3.zero;
		}
		if (active)
		{
			dustParticles.Play();
		}
		else
		{
			dustParticles.Stop();
		}
	}

	private void Update()
	{
		if (ThemeManager.Instance.Theme.RoofedCullingEnabled)
		{
			_environmentCulling.UpdateCulling();
		}
		for (int i = 0; i < ThemeManager.Instance.Theme.Effects.Length; i++)
		{
			ThemeManager.Instance.Theme.Effects[i].UpdateEffect();
		}
	}
}
