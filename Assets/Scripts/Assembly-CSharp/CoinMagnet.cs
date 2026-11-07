using System;
using System.Collections;
using Info;
using SYBO.Common;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Coins;
using UnityEngine;

public class CoinMagnet : CharacterModifier
{
	private const int CoinMagnetAnimationLayer = 4;

	private const float CoinMagnetAnimationWeight = 0.9f;

	private const string AnimationNameHoldMagnet = "hold_magnet";

	private Character character;

	private CollisionNotifier coinMagnetCollisionNotifier;

	private CharacterRendering characterRendering;

	private Collider coinMagnetCollider;

	private Transform coinEFX;

	[SerializeField]
	private float pullSpeed = 150f;

	[SerializeField]
	private AudioClipInfo powerDownSound;

	private static CoinMagnet instance;

	public bool IsActive { get; private set; }

	public static CoinMagnet Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(CoinMagnet)) as CoinMagnet);
		}
	}

	private AnimationState CoinMagnetAnimationState
	{
		get
		{
			return characterRendering.CharacterAnimation["hold_magnet"];
		}
	}

	public void Awake()
	{
		instance = this;
	}

	public void Initialize()
	{
		character = Character.Instance;
		coinEFX = character.CharacterPickupParticleSystem.CoinEFX.transform;
		characterRendering = CharacterRendering.Instance;
		coinMagnetCollisionNotifier = character.CoinMagnetCollider;
		coinMagnetCollisionNotifier.TriggerEnter += CoinHit;
		coinMagnetCollider = coinMagnetCollisionNotifier.GetComponent<Collider>();
		character.OnResetTeleportFX += OnResetTeleportFX;
		CharacterRendering obj = characterRendering;
		obj.OnExtraTeleboardEffectEnd = (Action)Delegate.Combine(obj.OnExtraTeleboardEffectEnd, new Action(OnResetTeleportFX));
		CharacterRendering obj2 = characterRendering;
		obj2.OnResetCharacterTeleportFX = (Action)Delegate.Combine(obj2.OnResetCharacterTeleportFX, new Action(OnResetTeleportFX));
		SetUpHoldAnimation();
		Game.Instance.OnCharacterModelRecreated += Game_OnCharacterModelRecreated;
	}

	private void SetUpHoldAnimation()
	{
		CoinMagnetAnimationState.AddMixingTransform(characterRendering.CharacterModel.ShoulderTransform);
		CoinMagnetAnimationState.layer = 4;
		CoinMagnetAnimationState.weight = 0.9f;
		CoinMagnetAnimationState.enabled = false;
	}

	private void OnDestroy()
	{
		Game.Instance.OnCharacterModelRecreated -= Game_OnCharacterModelRecreated;
		if (coinMagnetCollisionNotifier != null)
		{
			coinMagnetCollisionNotifier.TriggerEnter -= CoinHit;
		}
		if (character != null)
		{
			character.OnResetTeleportFX -= OnResetTeleportFX;
		}
	}

	private void OnResetTeleportFX()
	{
		if (IsActive)
		{
			PlayCoinMagnetAnimation();
		}
	}

	private void Game_OnCharacterModelRecreated()
	{
		SetUpHoldAnimation();
	}

	public override void Reset()
	{
		base.Reset();
		EndCoinMagnetLogic();
	}

	private void PlayCoinMagnetAnimation()
	{
		CoinMagnetAnimationState.enabled = true;
		characterRendering.CharacterAnimation.Play("hold_magnet");
	}

	public override IEnumerator Begin()
	{
		if (character == null)
		{
			Initialize();
		}
		characterRendering.CharacterModel.MeshCoinMagnet.enabled = true;
		GameStats.Instance.PickedUpPowerupsIncrement();
		base.Paused = false;
		SingletonMonoBehaviour<AudioStateLoop>.Instance.ChangeLoop(AudioState.Magnet);
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		IsActive = true;
		base.enabled = true;
		PlayCoinMagnetAnimation();
		base.Powerup = GameStats.Instance.TriggerPowerup(UpgradeType.coinmagnet);
		coinMagnetCollider.enabled = true;
		stop = StopSignal.DONT_STOP;
		while (base.Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			coinEFX.position = characterRendering.CharacterModel.MeshCoinMagnet.transform.position;
			yield return null;
		}
		EndCoinMagnetLogic();
	}

	private void EndCoinMagnetLogic()
	{
		coinMagnetCollider.enabled = false;
		IsActive = false;
		base.enabled = false;
		characterRendering.CharacterModel.MeshCoinMagnet.enabled = false;
		coinEFX.localPosition = CharacterPickupParticles.coinEfxOffset;
		CoinMagnetAnimationState.enabled = false;
		SingletonMonoBehaviour<AudioStateLoop>.Instance.ChangeLoop(AudioState.MagnetStop);
		if (base.Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public void CoinHit(Collider collider)
	{
		Coin component = collider.GetComponent<Coin>();
		if (component != null)
		{
			bool flag = character.TransformPosition.y >= 70f;
			bool flag2 = component.transform.position.y >= 70f;
			if (component != null && flag == flag2)
			{
				component.MoveToMagnet(pullSpeed);
			}
		}
	}
}
