using System;
using System.Collections;
using System.Collections.Generic;
using Info;
using Missions;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Placeholders.Lifecycles;
using UnityEngine;

[RequireComponent(typeof(PlaceholderLifecycle))]
public class FollowingGuard : MonoBehaviour
{
	[Serializable]
	public class CatchAnimationSet
	{
		public AnimationClip avatar;

		public AnimationClip guard;

		public AnimationClip dog;

		public float catchAvatarAnimationPlayOffset;

		public float waitTimeBeforeScreen;
	}

	[Serializable]
	public class DefaultAnimations
	{
		public AnimationClip[] introGuard;

		public AnimationClip[] introDog;

		public AnimationClip[] runGuard;

		public AnimationClip[] runDog;

		public AnimationClip[] jumpGuard;

		public AnimationClip[] jumpDog;

		public AnimationClip[] hangtimeGuard;

		public AnimationClip[] hangtimeDog;

		public AnimationClip[] landingGuard;

		public AnimationClip[] landingDog;

		public AnimationClip[] dodgeLeftGuard;

		public AnimationClip[] dodgeLeftDog;

		public AnimationClip[] dodgeRigthGuard;

		public AnimationClip[] dodgeRightDog;

		public AnimationClip[] rollGuard;

		public AnimationClip[] rollDog;

		public AnimationClip[] catchupGuard;

		public AnimationClip[] deathByTrainGuard;

		public AnimationClip[] deathByTrainDog;
	}

	public delegate void OnCatchPlayerDelegate(string currentChartacterCatch, float catchUpTime, float waitTimeBeforeScreen);

	[SerializeField]
	private DefaultAnimations defaultAnimations;

	[SerializeField]
	private CatchAnimationSet[] caughtLeft;

	[SerializeField]
	private CatchAnimationSet[] caughtRight;

	[SerializeField]
	private float distanceToCharacterMin = 10f;

	[SerializeField]
	private float distanceToCharacterMax = 50f;

	[SerializeField]
	private float catchUpDuration = 0.7f;

	[SerializeField]
	private float resetCatchUpDuration = 1.5f;

	[SerializeField]
	private float lastGroundedSmoothTime = 0.3f;

	[SerializeField]
	private float xSmoothTime = 0.1f;

	[SerializeField]
	private float gravity = 200f;

	[SerializeField]
	private Transform[] enemies;

	[SerializeField]
	private int debugCatchAnimationToPlay = -1;

	[SerializeField]
	private float guardProximityLoopVolume = 0.9f;

	[HideInInspector]
	[SerializeField]
	private PlaceholderLifecycle _placeholderLifecycle;

	private EnemiesRig _enemiesData;

	private Animation guardAnimation;

	private Animation dogAnimation;

	public bool isShowing;

	public OnCatchPlayerDelegate OnCatchPlayer;

	private string currentAvatarStumbleDeath;

	private string previusAvatarCaughtAnimLeft;

	private string previusAvatarCaughtAnimRight;

	private float y;

	private bool closeToCharacter;

	private float distanceToCharacter;

	private float lastGroundedSmooth;

	private float lastGroundedVelocity;

	private string previusAvatarStumbleDeath;

	private float verticalSpeed;

	private bool caught;

	private SmoothDampFloat x;

	private Renderer[] enemyRenderers;

	private Vector3[] enemiesStartPos;

	private Game game;

	private CharacterController characterController;

	private Character character;

	private CharacterRendering characterRendering;

	private static FollowingGuard instance;

	public static FollowingGuard Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(FollowingGuard)) as FollowingGuard);
		}
	}

	public void Initialize()
	{
		ReplaceSharedPrefab();
		game = Game.Instance;
		game.OnCharacterModelRecreated += Game_OnCharacterModelRecreated;
		characterController = Game.Charactercontroller;
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		enemyRenderers = base.gameObject.GetComponentsInChildren<Renderer>();
		enemiesStartPos = new Vector3[enemies.Length];
		for (int i = 0; i < enemies.Length; i++)
		{
			enemiesStartPos[i] = enemies[i].position;
		}
		x = new SmoothDampFloat(0f, xSmoothTime);
		GetComponent<AudioSource>().volume = guardProximityLoopVolume;
		game.OnPauseChange += HandleOnPauseChange;
		Settings.onSfxSettingChanged += OnSfxSettingChanged;
		OnSfxSettingChanged();
		CatchAnimationSet[] array = caughtLeft;
		foreach (CatchAnimationSet catchAnimationSet in array)
		{
			SetupAvatarAnimationsStates(characterRendering.CharacterAnimation, catchAnimationSet.avatar);
			SetupDogGuardAnimationsStates(guardAnimation, catchAnimationSet.guard);
			SetupDogGuardAnimationsStates(dogAnimation, catchAnimationSet.dog);
		}
		CatchAnimationSet[] array2 = caughtRight;
		foreach (CatchAnimationSet catchAnimationSet2 in array2)
		{
			SetupAvatarAnimationsStates(characterRendering.CharacterAnimation, catchAnimationSet2.avatar);
			SetupDogGuardAnimationsStates(guardAnimation, catchAnimationSet2.guard);
			SetupDogGuardAnimationsStates(dogAnimation, catchAnimationSet2.dog);
		}
		List<AnimationClip> list = new List<AnimationClip>();
		foreach (AnimationState item in guardAnimation)
		{
			list.Add(item.clip);
		}
		InitializeClips(guardAnimation, list, defaultAnimations.jumpGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.hangtimeGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.landingGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.runGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.introGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.dodgeRigthGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.dodgeLeftGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.rollGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.catchupGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.deathByTrainGuard);
		List<AnimationClip> list2 = new List<AnimationClip>();
		foreach (AnimationState item2 in dogAnimation)
		{
			list2.Add(item2.clip);
		}
		InitializeClips(dogAnimation, list2, defaultAnimations.runDog);
		InitializeClips(dogAnimation, list2, defaultAnimations.jumpDog);
		InitializeClips(dogAnimation, list2, defaultAnimations.hangtimeDog);
		InitializeClips(dogAnimation, list2, defaultAnimations.landingDog);
		InitializeClips(dogAnimation, list2, defaultAnimations.introDog);
		InitializeClips(dogAnimation, list2, defaultAnimations.dodgeRightDog);
		InitializeClips(dogAnimation, list2, defaultAnimations.dodgeLeftDog);
		InitializeClips(dogAnimation, list2, defaultAnimations.rollDog);
		InitializeClips(dogAnimation, list2, defaultAnimations.deathByTrainDog);
	}

	private void ReplaceSharedPrefab()
	{
		_placeholderLifecycle.Spawn();
		_enemiesData = base.gameObject.GetComponentInChildren<EnemiesRig>();
		dogAnimation = _enemiesData.DogAnimation;
		guardAnimation = _enemiesData.GuardAnimation;
		enemies = new Transform[2];
		enemies[0] = dogAnimation.transform;
		enemies[1] = guardAnimation.transform;
	}

	private Transform GetChildTransformByName(string name, Transform current)
	{
		if (current.name.Equals(name))
		{
			return current;
		}
		for (int i = 0; i < current.childCount; i++)
		{
			Transform childTransformByName = GetChildTransformByName(name, current.GetChild(i));
			if (childTransformByName != null)
			{
				return childTransformByName;
			}
		}
		return null;
	}

	private void Game_OnCharacterModelRecreated()
	{
		CatchAnimationSet[] array = caughtLeft;
		foreach (CatchAnimationSet catchAnimationSet in array)
		{
			SetupAvatarAnimationsStates(characterRendering.CharacterAnimation, catchAnimationSet.avatar);
		}
		CatchAnimationSet[] array2 = caughtRight;
		foreach (CatchAnimationSet catchAnimationSet2 in array2)
		{
			SetupAvatarAnimationsStates(characterRendering.CharacterAnimation, catchAnimationSet2.avatar);
		}
	}

	private void OnSfxSettingChanged()
	{
		if (Settings.optionSfx)
		{
			GetComponent<AudioSource>().volume = guardProximityLoopVolume;
		}
		else
		{
			GetComponent<AudioSource>().volume = 0f;
		}
	}

	private void InitializeClips(Animation animationComponent, List<AnimationClip> addedClips, AnimationClip[] clips)
	{
		foreach (AnimationClip animationClip in clips)
		{
			if (animationClip == null)
			{
				SDebug.LogError("Missing animation clip for enemies! ");
			}
			else if (!addedClips.Contains(animationClip))
			{
				addedClips.Add(animationClip);
				animationComponent.AddClip(animationClip, animationClip.name);
			}
		}
	}

	private void SetupAvatarAnimationsStates(Animation animation, AnimationClip animationClip)
	{
		AnimationClip clip = animation.GetClip(animationClip.name);
		if (clip == null)
		{
			animation.AddClip(animationClip, animationClip.name);
		}
		animation[animationClip.name].enabled = false;
		animation[animationClip.name].layer = 4;
	}

	private void SetupDogGuardAnimationsStates(Animation animation, AnimationClip animationClip)
	{
		AnimationClip clip = animation.GetClip(animationClip.name);
		if (clip == null)
		{
			animation.AddClip(animationClip, animationClip.name);
		}
	}

	private void HandleOnPauseChange(bool pause)
	{
		if (pause && GetComponent<AudioSource>().isPlaying)
		{
			GetComponent<AudioSource>().Pause();
		}
	}

	public void ResetModelRootPosition()
	{
		float z = 8f;
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].localPosition = enemiesStartPos[i] + new Vector3(0f, 0f, z);
			enemies[i].rotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public void Restart(bool closeToCharacter)
	{
		StopAllCoroutines();
		this.closeToCharacter = closeToCharacter;
		distanceToCharacter = ((!closeToCharacter) ? distanceToCharacterMax : distanceToCharacterMin);
	}

	public void OnEnable()
	{
		lastGroundedSmooth = character.LastGroundedY;
		lastGroundedVelocity = 0f;
		y = character.LastGroundedY;
		x.Value = character.TransformPosition.x;
		distanceToCharacter = distanceToCharacterMin;
		closeToCharacter = true;
		verticalSpeed = 0f;
		character.OnJump += OnJump;
		character.OnRollGuard += OnRoll;
		character.OnRoll += OnRollNoAnimation;
		character.OnChangeTrack += OnChangeTrack;
	}

	public void OnDisable()
	{
		character.OnJump -= OnJump;
		character.OnRollGuard -= OnRoll;
		character.OnRoll -= OnRollNoAnimation;
		character.OnChangeTrack += OnChangeTrack;
	}

	public void CatchUp()
	{
		CatchUp(catchUpDuration);
	}

	public void CatchUp(float duration)
	{
		if (!closeToCharacter)
		{
			float distanceFrom = distanceToCharacter;
			ShowEnemies(true);
			StopAllCoroutines();
			guardAnimation.Play(ReturnRandomAnimations(defaultAnimations.catchupGuard, null)[0]);
			guardAnimation.PlayQueued("Guard_Run");
			GetComponent<AudioSource>().timeSamples = UnityEngine.Random.Range(0, GetComponent<AudioSource>().timeSamples);
			GetComponent<AudioSource>().Play();
			GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.9f, 1.05f);
			StartCoroutine(pTween.To(duration, (float t) =>
			{
				distanceToCharacter = Mathf.SmoothStep(distanceFrom, distanceToCharacterMin, t);
			}));
			StartCoroutine(pTween.To(duration, (float t) =>
			{
				GetComponent<AudioSource>().volume = ((!Settings.optionSfx) ? 0f : Mathf.SmoothStep(0f, guardProximityLoopVolume, t));
			}));
			closeToCharacter = true;
		}
	}

	public void ResetCatchUp()
	{
		ResetCatchUp(resetCatchUpDuration);
	}

	public void ResetCatchUp(float duration)
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(ResetCatchUpCoroutine(duration));
		}
	}

	public IEnumerator ResetCatchUpCoroutine(float duration)
	{
		if (closeToCharacter)
		{
			float distanceFrom = distanceToCharacter;
			closeToCharacter = false;
			StartCoroutine(pTween.To(duration, (float t) =>
			{
				distanceToCharacter = Mathf.SmoothStep(distanceFrom, distanceToCharacterMax, t);
			}));
			yield return StartCoroutine(pTween.To(duration * 2f, (float t) =>
			{
				GetComponent<AudioSource>().volume = ((!Settings.optionSfx) ? 0f : Mathf.SmoothStep(guardProximityLoopVolume, 0f, t));
			}));
			GetComponent<AudioSource>().Stop();
			if (!game.isDead)
			{
				ShowEnemies(false);
			}
		}
	}

	public void MuteProximityLoop()
	{
		GetComponent<AudioSource>().Stop();
	}

	public void PlayIntro()
	{
		base.gameObject.transform.position = new Vector3(0f, 0f, -10f);
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].position = enemiesStartPos[i];
			enemies[i].rotation = Quaternion.Euler(0f, 0f, 0f);
		}
		string[] array = ReturnRandomAnimations(defaultAnimations.introGuard, defaultAnimations.introDog);
		guardAnimation[array[0]].time = 0f;
		guardAnimation.Play(array[0]);
		dogAnimation[array[1]].time = 0f;
		dogAnimation.Play(array[1]);
		string[] array2 = ReturnRandomAnimations(defaultAnimations.runGuard, defaultAnimations.runDog);
		guardAnimation.CrossFadeQueued(array2[0], 0.2f);
		dogAnimation.CrossFadeQueued(array2[1], 0.2f);
	}

	private string[] ReturnRandomAnimations(AnimationClip[] guardAnimations, AnimationClip[] dogAnimations)
	{
		string[] array = new string[2];
		if (guardAnimations != null)
		{
			int num = UnityEngine.Random.Range(0, guardAnimations.Length);
			array[0] = guardAnimations[num].name;
		}
		else
		{
			array[0] = string.Empty;
		}
		if (dogAnimations != null)
		{
			int num2 = UnityEngine.Random.Range(0, dogAnimations.Length);
			array[1] = dogAnimations[num2].name;
		}
		else
		{
			array[1] = string.Empty;
		}
		return array;
	}

	private void OnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (!caught)
		{
			if (characterController.isGrounded)
			{
				string[] array = ((direction != Character.OnChangeTrackDirection.Left) ? ReturnRandomAnimations(defaultAnimations.dodgeRigthGuard, defaultAnimations.dodgeRightDog) : ReturnRandomAnimations(defaultAnimations.dodgeLeftGuard, defaultAnimations.dodgeLeftDog));
				guardAnimation[array[0]].speed = game.CurrentSpeedNormalized;
				dogAnimation[array[1]].speed = game.CurrentSpeedNormalized;
				guardAnimation.CrossFade(array[0], 0.1f);
				dogAnimation.CrossFade(array[1], 0.1f);
			}
			if (!character.IsJumping)
			{
				string[] array2 = ReturnRandomAnimations(defaultAnimations.runGuard, defaultAnimations.runDog);
				guardAnimation.CrossFadeQueued(array2[0], 0.1f);
				dogAnimation.CrossFadeQueued(array2[1], 0.1f);
			}
		}
	}

	private void OnRoll()
	{
		string[] array = ReturnRandomAnimations(defaultAnimations.rollGuard, defaultAnimations.rollDog);
		guardAnimation.CrossFade(array[0], 0.1f);
		dogAnimation.CrossFade(array[1], 0.1f);
		string[] array2 = ReturnRandomAnimations(defaultAnimations.runGuard, defaultAnimations.runDog);
		guardAnimation.CrossFadeQueued(array2[0], 0.2f);
		dogAnimation.CrossFadeQueued(array2[1], 0.2f);
	}

	private void OnRollNoAnimation()
	{
		StartCoroutine(RollCoroutine(distanceToCharacter / game.CurrentSpeed));
	}

	private IEnumerator RollCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		verticalSpeed = 0f - character.CalculateJumpVerticalSpeed();
	}

	private void OnJump(float jumpHeight, bool willPlaySound)
	{
		Jump(distanceToCharacter / game.CurrentSpeed);
	}

	public void Jump(float delay)
	{
		if (distanceToCharacter <= distanceToCharacterMin)
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.GuardJump);
		}
		StartCoroutine(JumpCoroutine(delay));
	}

	private IEnumerator JumpCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		JumpAnimation();
		verticalSpeed = character.CalculateJumpVerticalSpeed() * 0.7f;
	}

	private void JumpAnimation()
	{
		string[] array = ReturnRandomAnimations(defaultAnimations.jumpGuard, defaultAnimations.jumpDog);
		guardAnimation.Play(array[0]);
		dogAnimation.Play(array[1]);
		string[] array2 = ReturnRandomAnimations(defaultAnimations.runGuard, defaultAnimations.runDog);
		guardAnimation.CrossFadeQueued(array2[0], 0.3f);
		dogAnimation.CrossFadeQueued(array2[1], 0.3f);
	}

	private void HandleOnHangtime()
	{
	}

	private IEnumerator HangtimeCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		string[] hangtimeAnimation = ReturnRandomAnimations(defaultAnimations.hangtimeGuard, defaultAnimations.hangtimeDog);
		guardAnimation.CrossFade(hangtimeAnimation[0], 0.1f);
		dogAnimation.CrossFade(hangtimeAnimation[1], 0.1f);
	}

	private void HandleOnLanding(Transform characterTransform)
	{
		StartCoroutine(LandingCoroutine(distanceToCharacter / game.CurrentSpeed));
	}

	private IEnumerator LandingCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		string[] landingAnimation = ReturnRandomAnimations(defaultAnimations.landingGuard, defaultAnimations.landingDog);
		guardAnimation.CrossFade(landingAnimation[0], 0.1f);
		dogAnimation.CrossFade(landingAnimation[1], 0.1f);
		string[] runAnimation = ReturnRandomAnimations(defaultAnimations.runGuard, defaultAnimations.runDog);
		guardAnimation.CrossFadeQueued(runAnimation[0], 0.1f);
		dogAnimation.CrossFadeQueued(runAnimation[1], 0.1f);
	}

	public void CatchPlayer(float pos)
	{
		if (game.IsInPogostickMode)
		{
			return;
		}
		GetComponent<AudioSource>().Stop();
		StopAllCoroutines();
		caught = true;
		int num = ((debugCatchAnimationToPlay <= -1 || debugCatchAnimationToPlay >= caughtLeft.Length) ? UnityEngine.Random.Range(0, caughtLeft.Length) : debugCatchAnimationToPlay);
		int num2 = ((debugCatchAnimationToPlay <= -1 || debugCatchAnimationToPlay >= caughtRight.Length) ? UnityEngine.Random.Range(0, caughtRight.Length) : debugCatchAnimationToPlay);
		float num3;
		if (pos < 20f)
		{
			guardAnimation.CrossFade(caughtLeft[num].guard.name, 0.2f);
			dogAnimation.CrossFade(caughtLeft[num].dog.name, 0.2f);
			num3 = caughtLeft[num].catchAvatarAnimationPlayOffset / 25f;
			if (OnCatchPlayer != null)
			{
				OnCatchPlayer(caughtLeft[num].avatar.name, num3, caughtLeft[num].waitTimeBeforeScreen);
			}
		}
		else
		{
			guardAnimation.CrossFade(caughtRight[num2].guard.name, 0.2f);
			dogAnimation.CrossFade(caughtRight[num2].dog.name, 0.2f);
			num3 = caughtRight[num2].catchAvatarAnimationPlayOffset / 25f;
			if (OnCatchPlayer != null)
			{
				OnCatchPlayer(caughtRight[num2].avatar.name, num3, caughtRight[num2].waitTimeBeforeScreen);
			}
		}
		StartCoroutine(pTween.To(num3, (float t) =>
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, character.TransformPosition, t);
			}
		}));
	}

	public void HitByTrainSequence()
	{
		GetComponent<AudioSource>().Stop();
		StartCoroutine(HitByTrainSequenceCoroutine());
	}

	public IEnumerator HitByTrainSequenceCoroutine()
	{
		float catchUpTime = 0.2f;
		yield return StartCoroutine(pTween.To(catchUpTime, (float t) =>
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, character.TransformPosition, t);
			}
		}));
		string[] deathByTrainAnimations = ReturnRandomAnimations(defaultAnimations.deathByTrainGuard, defaultAnimations.deathByTrainDog);
		dogAnimation.Play(deathByTrainAnimations[1]);
		yield return new WaitForSeconds(0.4f);
		Vector3 charPos = character.GamePosition;
		StartCoroutine(pTween.To(1f, (float t) =>
		{
			character.SetGamePosition(Vector3.Lerp(charPos, new Vector3(charPos.x, -5f, charPos.z), t));
		}));
		yield return new WaitForSeconds(0.2f);
		guardAnimation.Play(deathByTrainAnimations[0]);
	}

	public void ShowEnemies(bool vis)
	{
		isShowing = vis;
		caught = false;
		Renderer[] array = enemyRenderers;
		foreach (Renderer renderer in array)
		{
			renderer.gameObject.SetActive(vis);
		}
	}

	public void LateUpdate()
	{
		x.Target = character.TransformPosition.x;
		x.Update();
		lastGroundedSmooth = Mathf.SmoothDamp(lastGroundedSmooth, character.LastGroundedY, ref lastGroundedVelocity, lastGroundedSmoothTime);
		if (y > lastGroundedSmooth)
		{
			verticalSpeed -= gravity * Time.deltaTime;
		}
		y += verticalSpeed * Time.deltaTime;
		lastGroundedSmooth = ((!float.IsNaN(lastGroundedSmooth)) ? lastGroundedSmooth : 0f);
		y = Mathf.Max(y, lastGroundedSmooth);
		Vector3 position = character.TransformPosition - Vector3.forward * distanceToCharacter;
		position.y = y;
		position.x = x.Value;
		base.transform.position = position;
	}

	public void OnValidate()
	{
		_placeholderLifecycle = GetComponent<PlaceholderLifecycle>();
	}
}
