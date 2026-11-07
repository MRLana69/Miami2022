using System;
using System.Collections;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using SYBO.Subway.Visuals;
using UnityEngine;

[RequireComponent(typeof(HuntTokenCollectVFXManager))]
public class CharacterPickupParticles : MonoBehaviour
{
	[Serializable]
	private class EffectDetails
	{
		public Transform target;

		public float duration = 0.05f;

		public AnimationCurve scaleCurve;

		private Renderer _renderer;

		private Material _materialInstance;

		public Renderer Renderer
		{
			get
			{
				if (_renderer == null)
				{
					if (target != null)
					{
						_renderer = target.GetComponent<Renderer>();
					}
					else
					{
						SDebug.LogErrorFormat("A EffectDetails.target of CharacterPickupParticles is null, please fix!");
					}
				}
				return _renderer;
			}
		}

		public Material MaterialInstance
		{
			get
			{
				if (_materialInstance == null)
				{
					if (target != null)
					{
						_materialInstance = Renderer.material;
					}
					else
					{
						SDebug.LogErrorFormat("A EffectDetails.target of CharacterPickupParticles is null, please fix!");
					}
				}
				return _materialInstance;
			}
		}
	}

	public static Vector3 coinEfxOffset = 1.2f * Vector3.forward;

	public GameObject CoinEFX;

	public GameObject PowerUpEFX;

	[OptionalField]
	public Transform master;

	public AudioClipInfo CoinPickup;

	public AudioClipInfo PowerUpPickup;

	public float CoinDistanceForStairway;

	private int coinStairway;

	private int flyWay;

	private int[] pentatonicScale = new int[17]
	{
		12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
		22, 23, 24, 25, 26, 27, 28
	};

	public AnimationCurve compressCurve;

	[SerializeField]
	private EffectDetails coinEffect;

	[SerializeField]
	private EffectDetails powerupEffect;

	private HuntTokenCollectVFXManager huntTokenCollectVFXManager;

	private void Start()
	{
		JetpackState state = GameStateMachine.Instance.GetState<JetpackState>(4);
		state.OnStop = (Action)Delegate.Combine(state.OnStop, new Action(OnJetpackStop));
		huntTokenCollectVFXManager = GetComponent<HuntTokenCollectVFXManager>();
	}

	private void OnJetpackStop()
	{
		flyWay = 0;
	}

	public void PickedUpCoin(Transform trans)
	{
		if (80f < trans.position.y)
		{
			coinStairway = 0;
			CoinPickup.maxPitch = Mathf.Pow(2f, compressCurve.Evaluate((float)flyWay / 48f));
			CoinPickup.minPitch = Mathf.Pow(2f, compressCurve.Evaluate((float)flyWay / 48f));
			flyWay++;
		}
		else if (trans.position.y < 0.1f || (8.795f < trans.position.y && trans.position.y < 8.805f) || (9.95f < trans.position.y && trans.position.y < 10.05f) || (28.95f < trans.position.y && trans.position.y < 29.05f) || (34.95f < trans.position.y && trans.position.y < 35.05f))
		{
			flyWay = 0;
			coinStairway = 0;
			CoinPickup.maxPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
			CoinPickup.minPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
		}
		else
		{
			flyWay = 0;
			if (coinStairway < pentatonicScale.Length - 1)
			{
				coinStairway++;
			}
			CoinPickup.maxPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
			CoinPickup.minPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
		}
		So.Instance.playSound(CoinPickup);
		StartCoroutine(EffectCoroutine(coinEffect));
	}

	private IEnumerator EffectCoroutine(EffectDetails details)
	{
		details.target.Rotate(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		Material material = details.MaterialInstance;
		yield return StartCoroutine(pTween.To(details.duration, (float t) =>
		{
			details.Renderer.enabled = true;
			material.SetColor("_MainColor", Color.Lerp(Color.white, Color.black, t));
			details.target.localScale = Vector3.one * details.scaleCurve.Evaluate(t);
		}));
		details.Renderer.enabled = false;
	}

	public void PickedUpPowerUp()
	{
		So.Instance.playSound(PowerUpPickup);
		PickedUpDefaultPowerUp();
	}

	public void PickedUpDefaultPowerUp()
	{
		StartCoroutine(EffectCoroutine(powerupEffect));
	}

	public void PickedUpHuntToken()
	{
		if (huntTokenCollectVFXManager == null)
		{
			throw new Exception("PickedUpHuntToken is called before the huntTokenCollectVFXManager is set!");
		}
		huntTokenCollectVFXManager.PlayEffect();
	}
}
