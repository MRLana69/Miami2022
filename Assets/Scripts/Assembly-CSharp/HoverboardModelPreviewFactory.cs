using System;
using System.Collections;
using System.Collections.Generic;
using Data.Managers;
using Info;
using SYBO.Components.Utilities;
using UnityEngine;

public class HoverboardModelPreviewFactory : MonoBehaviour
{
	[Serializable]
	public class HoverboardModelSetup
	{
		public string name;

		public Vector3 eulerAngles;

		public float menuYPosition;

		public AnimationClip clipHangtime;

		public AnimationClip clipRun;
	}

	[SerializeField]
	private HoverboardModelSetup[] hoverboards;

	[SerializeField]
	private AnimationClip jumperSignature;

	[SerializeField]
	private AnimationClip speederSignature;

	[SerializeField]
	private AnimationClip doublejumperSignature;

	[SerializeField]
	private AnimationClip lowriderSignature;

	[SerializeField]
	private AnimationClip lowriderRunIdle;

	[SerializeField]
	private AnimationClip teleporterSignature;

	[SerializeField]
	private AnimationClip gliderSignature;

	[SerializeField]
	private AnimationClip vanitySignature;

	public Action OnOpenGlider;

	public Action OnCloseGlider;

	public Action OnSpeederStart;

	public Action OnSpeederEnd;

	public Action OnJump;

	private Dictionary<string, HoverboardModelSetup> name2board = new Dictionary<string, HoverboardModelSetup>();

	private string currentIdleClip;

	[SerializeField]
	private GameObject teleboardWarpPrefab;

	[SerializeField]
	private Material teleboardFXMaterial;

	private GameObject teleboardWarp;

	private ATeleboardMenuEffect _teleboardMenuEffect;

	private AnimationState teleboardAnimation;

	private Vector3 teleboardWarpPosition;

	private GliderAnimation gliderAnimation;

	private readonly int id_Teleboard = Animator.StringToHash("Base Layer.HoverboardMenuTeleport");

	private readonly int id_Glider = Animator.StringToHash("Base Layer.HoverboardMenuGlider");

	private Animator animator;

	private Vector3 intialScale = new Vector3(-1f, -1f, -1f);

	private Transform root;

	private Material cachedModelMaterial;

	private Material cachedCustomizationMaterial;

	private Material[] cachedHeadMaterials;

	private Renderer cachedHoverboardBaseModelRenderer;

	private Material cachedHoverboardMaterial;

	private Material cachedSupersneakersMaterial;

	private Material cachedDoubleJumperMaterial;

	private Material[] cachedCustomHoverboardModelMaterials;

	private HoverboardBundle hoverboardBundle;

	private CharacterModel characterModel;

	private bool cachedCustomizationState;

	private bool teleportFxActive;

	private static HoverboardModelPreviewFactory instance;

	public static HoverboardModelPreviewFactory Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(HoverboardModelPreviewFactory)) as HoverboardModelPreviewFactory);
		}
	}

	public void Awake()
	{
		animator = base.gameObject.GetComponent<Animator>();
		UIScreenController.OnNavigatedToScreen += HandleOnNavigateToScreen;
		UIScreenController.OnPopupShown += HandleOnPopupShown;
		UIScreenController.OnApplicationResumed += UIScreenController_OnApplicationResumed;
		HoverboardModelSetup[] array = hoverboards;
		foreach (HoverboardModelSetup hoverboardModelSetup in array)
		{
			name2board.Add(hoverboardModelSetup.name, hoverboardModelSetup);
		}
	}

	private void UIScreenController_OnApplicationResumed()
	{
		ResetAnimator();
		ResetTeleportFX();
	}

	private void HandleOnPopupShown(string popupName)
	{
		ResetAnimator();
		if (!base.name.Equals("BoardScreen") && _teleboardMenuEffect != null)
		{
			UnityEngine.Object.Destroy(_teleboardMenuEffect.gameObject);
		}
	}

	private void HandleOnNavigateToScreen(string obj)
	{
		if (obj == "FrontScreen")
		{
			ResetAnimator();
		}
		if (!base.name.Equals("BoardScreen") && _teleboardMenuEffect != null)
		{
			UnityEngine.Object.Destroy(_teleboardMenuEffect.gameObject);
		}
	}

	public GameObject GetHoverboardModelPreview(string name, Boards.BoardId boardId)
	{
		HoverboardModelSetup value;
		if (name2board.TryGetValue(name, out value))
		{
			GameObject gameObject = new GameObject("Hoverboard: " + name);
			HoverboardBundle hoverboardBundle = SingletonMonoBehaviour<HoverboardModelFactory>.Instance.RequestHoverboardInstance(boardId.ToString());
			if (hoverboardBundle == null)
			{
			}
			hoverboardBundle.AddCustomPreviewModel();
			Renderer[] componentsInChildren = hoverboardBundle.GetComponentsInChildren<Renderer>();
			hoverboardBundle.transform.parent = gameObject.transform;
			hoverboardBundle.transform.localRotation = Quaternion.Euler(value.eulerAngles) * Quaternion.Euler(0f, 180f, 0f);
			Vector3 localPosition = hoverboardBundle.transform.localPosition;
			hoverboardBundle.transform.localPosition = localPosition + new Vector3(0f, value.menuYPosition, 0f);
			HoverboardBundle.CustomSet[] customSets = hoverboardBundle.CustomSets;
			if (customSets != null)
			{
				bool flag = true;
				if (BoardManager.IsCustomSetEquippedForBoard(boardId, 0) && !customSets[1].useDefaultmodel)
				{
					flag = false;
				}
				if (BoardManager.IsCustomSetEquippedForBoard(boardId, 1) && !customSets[2].useDefaultmodel)
				{
					flag = false;
				}
				if (flag && customSets.Length > 0 && customSets[0].modelPrefab != null)
				{
					hoverboardBundle.AddCustomSetModelAndEffect(0, true, false);
				}
				if (customSets.Length > 1 && BoardManager.IsCustomSetEquippedForBoard(boardId, 0))
				{
					hoverboardBundle.AddCustomSetModelAndEffect(1, true, false);
					if (customSets[1].baseModelPrefab != null)
					{
						hoverboardBundle.AddCustomBaseModel(1, true);
						Renderer[] array = componentsInChildren;
						foreach (Renderer renderer in array)
						{
							renderer.enabled = false;
						}
					}
				}
				if (customSets.Length > 2 && BoardManager.IsCustomSetEquippedForBoard(boardId, 1))
				{
					hoverboardBundle.AddCustomSetModelAndEffect(2, true, false);
					if (customSets[2].baseModelPrefab != null)
					{
						hoverboardBundle.AddCustomBaseModel(2, true);
						Renderer[] array2 = componentsInChildren;
						foreach (Renderer renderer2 in array2)
						{
							renderer2.enabled = false;
						}
					}
				}
			}
			GliderAnimation componentInChildren = hoverboardBundle.GetComponentInChildren<GliderAnimation>();
			if (componentInChildren != null)
			{
				componentInChildren.RemoveEventHandling();
			}
			return gameObject;
		}
		SDebug.LogError("GetHoverboardModelPreview() could not find preview board model for '" + name + "'.");
		return null;
	}

	public Quaternion GetHoverboardDefaultRotation(string name)
	{
		HoverboardModelSetup value;
		if (name2board.TryGetValue(name, out value))
		{
			return Quaternion.Euler(value.eulerAngles) * Quaternion.Euler(0f, 180f, 0f);
		}
		return Quaternion.Euler(194f, 110.5f, 180f);
	}

	private CharacterModel FindCharacterModel(GameObject characterAnimation)
	{
		CharacterModel characterModel = null;
		GameObject gameObject = characterAnimation;
		while (characterModel == null)
		{
			characterModel = gameObject.GetComponentInParent<CharacterModel>();
			Transform parent = gameObject.transform.parent;
			if (parent == null)
			{
				return null;
			}
			gameObject = parent.gameObject;
		}
		return characterModel;
	}

	public void ChangeHoverboard(Boards.BoardId boardId, GameObject hoverboardGO, Animation characterAnimation, bool willUpdateAnimation, bool willAllowSignatureAnimation, Hoverboard.BoardAbility boardAbility)
	{
		if (hoverboardBundle != null)
		{
			UnityEngine.Object.Destroy(hoverboardBundle.gameObject);
		}
		if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == id_Teleboard && willUpdateAnimation)
		{
			StopAllCoroutines();
			ResetAnimator();
		}
		else if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == id_Glider)
		{
			ResetAnimator();
		}
		if (teleportFxActive)
		{
			ResetTeleportFX();
		}
		if (_teleboardMenuEffect != null && willUpdateAnimation)
		{
			UnityEngine.Object.Destroy(_teleboardMenuEffect.gameObject);
		}
		string boardModelName = Boards.boardData[boardId].boardModelName;
		characterModel = FindCharacterModel(characterAnimation.gameObject);
		if (characterModel == null)
		{
			SDebug.LogError("HoverboardModelPreviewFactory - characterModel was null!");
			return;
		}
		if (intialScale == new Vector3(-1f, -1f, -1f))
		{
			intialScale = characterModel.transform.localScale;
		}
		HoverboardModelSetup value;
		if (!name2board.TryGetValue(boardModelName, out value))
		{
			return;
		}
		hoverboardBundle = characterModel.AddHoverboard(boardId.ToString(), true);
		if (boardModelName == "Jumpboard")
		{
			characterModel.MeshSuperSneaker.enabled = true;
		}
		else
		{
			characterModel.MeshSuperSneaker.enabled = false;
		}
		if (boardModelName == "gliderboard")
		{
			gliderAnimation = hoverboardGO.GetComponentInChildren<GliderAnimation>();
			if (gliderAnimation != null)
			{
				gliderAnimation.AddEvents(this);
			}
		}
		if (boardModelName == "teleboard")
		{
			CachMaterials();
		}
		HoverboardBundle.CustomSet[] customSets = hoverboardBundle.CustomSets;
		if (customSets == null)
		{
			return;
		}
		bool flag = true;
		if (customSets.Length > 1 && BoardManager.ShouldCustomSetBeDisplayed(0) && !customSets[1].useDefaultmodel)
		{
			flag = false;
		}
		if (customSets.Length > 2 && BoardManager.ShouldCustomSetBeDisplayed(1) && !customSets[2].useDefaultmodel)
		{
			flag = false;
		}
		if (flag == customSets.Length > 0)
		{
			hoverboardBundle.AddCustomSetModelAndEffect(0, true, true);
		}
		if (customSets.Length == 1 && willAllowSignatureAnimation)
		{
			if (customSets[0].ability != Hoverboard.BoardAbility.None)
			{
				if (customSets[0].ability == Hoverboard.BoardAbility.Teleport)
				{
					CachMaterials();
				}
				if (customSets[0].ability == Hoverboard.BoardAbility.Lowrider)
				{
					PlaySignatureMove(customSets[0], characterAnimation, true);
				}
				else
				{
					PlaySignatureMove(customSets[0], characterAnimation, false);
				}
			}
			return;
		}
		if (BoardManager.ShouldCustomSetBeDisplayed(0))
		{
			AddCustomModelsAndFX(customSets, 1, hoverboardGO.transform);
		}
		if (BoardManager.ShouldCustomSetBeDisplayed(1))
		{
			AddCustomModelsAndFX(customSets, 2, hoverboardGO.transform);
		}
		if (!willUpdateAnimation)
		{
			return;
		}
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < customSets.Length; i++)
		{
			if (i == 0 && customSets[i].ability == Hoverboard.BoardAbility.Lowrider)
			{
				flag2 = true;
			}
			if (i >= 1 && customSets[i].ability == Hoverboard.BoardAbility.Lowrider && BoardManager.ShouldCustomSetBeDisplayed(i - 1))
			{
				flag2 = true;
			}
			if (i == 0 && customSets[i].ability == Hoverboard.BoardAbility.Glider)
			{
				flag3 = true;
			}
		}
		int num = 0;
		if (BoardManager.ShouldCustomSetBeDisplayed(0) && customSets.Length > 1 && customSets[1].ability == boardAbility)
		{
			num = 1;
		}
		else if (BoardManager.ShouldCustomSetBeDisplayed(1) && customSets.Length > 2 && customSets[2].ability == boardAbility)
		{
			num = 2;
		}
		if (boardAbility == Hoverboard.BoardAbility.None || num == 0)
		{
			if (characterAnimation[value.clipHangtime.name] == null)
			{
				characterAnimation.AddClip(value.clipHangtime, value.clipHangtime.name);
			}
			characterAnimation[value.clipHangtime.name].wrapMode = WrapMode.Once;
			characterAnimation.Play(value.clipHangtime.name);
			if (flag2)
			{
				if (characterAnimation[lowriderRunIdle.name] == null)
				{
					characterAnimation.AddClip(lowriderRunIdle, lowriderRunIdle.name);
				}
				currentIdleClip = lowriderRunIdle.name;
				characterAnimation.CrossFadeQueued(lowriderRunIdle.name, 0.2f);
			}
			else
			{
				if (characterAnimation[value.clipRun.name] == null)
				{
					characterAnimation.AddClip(value.clipRun, value.clipRun.name);
				}
				currentIdleClip = value.clipRun.name;
				characterAnimation.CrossFadeQueued(value.clipRun.name, 0.2f);
			}
			if (flag3 && animator.GetCurrentAnimatorStateInfo(0).fullPathHash != id_Glider)
			{
				animator.Play(id_Glider);
			}
		}
		else if (willAllowSignatureAnimation)
		{
			PlaySignatureMove(customSets[num], characterAnimation, flag2);
		}
	}

	private void AddCustomModelsAndFX(HoverboardBundle.CustomSet[] customSets, int customSetIndex, Transform hoverboardRoot)
	{
		if (customSets.Length <= customSetIndex)
		{
			return;
		}
		if (customSets[customSetIndex].ability == Hoverboard.BoardAbility.Teleport)
		{
			CachMaterials();
		}
		if (customSets[customSetIndex].baseModelPrefab != null)
		{
			characterModel.CurrentHoverboadBasemodel.enabled = false;
			hoverboardBundle.AddCustomBaseModel(customSetIndex, true);
			cachedHoverboardBaseModelRenderer = characterModel.CurrentHoverboadBasemodel;
		}
		hoverboardBundle.AddCustomSetModelAndEffect(customSetIndex, true, true);
		if (customSets[customSetIndex].ability == Hoverboard.BoardAbility.Glider)
		{
			gliderAnimation = hoverboardBundle.gameObject.GetComponentInChildren<GliderAnimation>();
			if (gliderAnimation != null)
			{
				gliderAnimation.AddEvents(this);
			}
		}
		if (customSets[customSetIndex].ability == Hoverboard.BoardAbility.DoubleJump || customSets[customSetIndex].ability == Hoverboard.BoardAbility.Jumper)
		{
			playThruster componentInChildren = hoverboardBundle.gameObject.GetComponentInChildren<playThruster>();
			if (componentInChildren != null)
			{
				componentInChildren.InitializeMenu(this);
			}
		}
	}

	public void PlaySignatureMove(HoverboardBundle.CustomSet set, Animation characterAnimation, bool useLowrider)
	{
		Hoverboard.BoardAbility ability = set.ability;
		if (teleportFxActive)
		{
			ResetTeleportFX();
		}
		switch (ability)
		{
		case Hoverboard.BoardAbility.DoubleJump:
			StartCoroutine(JumpDelayedEvent(1f));
			if (characterAnimation[doublejumperSignature.name] == null)
			{
				characterAnimation.AddClip(doublejumperSignature, doublejumperSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(doublejumperSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(doublejumperSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.Glider:
			if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != id_Glider)
			{
				animator.Play(id_Glider);
			}
			if (characterAnimation[gliderSignature.name] == null)
			{
				characterAnimation.AddClip(gliderSignature, gliderSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(gliderSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(gliderSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.HighSpeed:
		case Hoverboard.BoardAbility.Speed:
			StartCoroutine(SpeederEvents(speederSignature.length));
			if (characterAnimation[speederSignature.name] == null)
			{
				characterAnimation.AddClip(speederSignature, speederSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(speederSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(speederSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.Jumper:
			StartCoroutine(JumpDelayedEvent(0.4f));
			if (characterAnimation[jumperSignature.name] == null)
			{
				characterAnimation.AddClip(jumperSignature, jumperSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(jumperSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(jumperSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.Lowrider:
			if (characterAnimation[lowriderSignature.name] == null)
			{
				characterAnimation.AddClip(lowriderSignature, lowriderSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(lowriderSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(lowriderSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.Teleport:
			root = characterModel.Hip;
			if (set.ExtraMenuFXPrefab != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(set.ExtraMenuFXPrefab);
				if (_teleboardMenuEffect != null)
				{
					UnityEngine.Object.Destroy(_teleboardMenuEffect.gameObject);
				}
				_teleboardMenuEffect = gameObject.GetComponent<ATeleboardMenuEffect>();
				if (_teleboardMenuEffect == null)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
				else
				{
					_teleboardMenuEffect.Initialize(root);
				}
			}
			if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != id_Teleboard)
			{
				animator.Play(id_Teleboard);
			}
			if (characterAnimation[teleporterSignature.name] == null)
			{
				characterAnimation.AddClip(teleporterSignature, teleporterSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(teleporterSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(teleporterSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.Vanity:
			if (characterAnimation[vanitySignature.name] == null)
			{
				characterAnimation.AddClip(vanitySignature, vanitySignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(vanitySignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(vanitySignature.name);
			}
			break;
		}
		if (useLowrider)
		{
			if (characterAnimation[lowriderRunIdle.name] == null)
			{
				characterAnimation.AddClip(lowriderRunIdle, lowriderRunIdle.name);
			}
			characterAnimation.CrossFadeQueued(lowriderRunIdle.name, 0.2f);
		}
		else
		{
			characterAnimation.CrossFadeQueued(currentIdleClip, 0.2f);
		}
	}

	private IEnumerator JumpDelayedEvent(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (OnJump != null)
		{
			OnJump();
		}
	}

	private IEnumerator SpeederEvents(float waitTime)
	{
		if (OnSpeederStart != null)
		{
			OnSpeederStart();
		}
		yield return new WaitForSeconds(waitTime);
		if (OnSpeederEnd != null)
		{
			OnSpeederEnd();
		}
	}

	private void CachMaterials()
	{
		if (characterModel != null)
		{
			Material sharedMaterial = characterModel.Model.sharedMaterial;
			if (sharedMaterial != teleboardFXMaterial)
			{
				cachedModelMaterial = sharedMaterial;
			}
			Material[] sharedMaterials = characterModel.HeadModel.sharedMaterials;
			if (sharedMaterials[0] != teleboardFXMaterial)
			{
				cachedHeadMaterials = sharedMaterials;
			}
			Material sharedMaterial2 = characterModel.OutfitModel.sharedMaterial;
			if (sharedMaterial2 != teleboardFXMaterial)
			{
				cachedCustomizationMaterial = sharedMaterial2;
			}
			cachedCustomizationState = characterModel.OutfitModel.enabled;
		}
	}

	private void TeleboardStart(float duration)
	{
		if (teleportFxActive)
		{
			ResetTeleportFX();
			ResetAnimator();
			StopCoroutine("TeleportFXStartScale");
		}
		if (characterModel == null)
		{
			return;
		}
		if (_teleboardMenuEffect != null)
		{
			_teleboardMenuEffect.StartTeleboardEffect(duration, characterModel);
			return;
		}
		teleportFxActive = true;
		if (teleboardWarp == null)
		{
			teleboardWarp = UnityEngine.Object.Instantiate(teleboardWarpPrefab);
			float num = 0.07f;
			teleboardWarp.transform.localScale = new Vector3(num, num, num);
			teleboardWarp.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			teleboardWarp.GetComponentInChildren<Renderer>().gameObject.layer = Layers.Instance._3DGUI;
			teleboardAnimation = teleboardWarp.GetComponent<Animation>()["teleboardWarp"];
			teleboardAnimation.normalizedTime = 0f;
			teleboardAnimation.weight = 1f;
			teleboardAnimation.enabled = true;
			teleboardAnimation.speed = 0f;
			teleboardWarp.SetActive(false);
		}
		characterModel.Model.GetComponent<Renderer>().material = teleboardFXMaterial;
		Material[] array = new Material[characterModel.HeadModel.materials.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = teleboardFXMaterial;
		}
		characterModel.HeadModel.materials = array;
		characterModel.OutfitModel.material = teleboardFXMaterial;
		cachedHoverboardBaseModelRenderer = characterModel.CurrentHoverboadBasemodel;
		Material sharedMaterial = cachedHoverboardBaseModelRenderer.sharedMaterial;
		if (sharedMaterial != teleboardFXMaterial)
		{
			cachedHoverboardMaterial = sharedMaterial;
		}
		cachedHoverboardBaseModelRenderer.material = teleboardFXMaterial;
		Material sharedMaterial2 = characterModel.MeshSuperSneaker.GetComponent<Renderer>().sharedMaterial;
		if (sharedMaterial2 != teleboardFXMaterial)
		{
			cachedSupersneakersMaterial = sharedMaterial2;
		}
		characterModel.MeshSuperSneaker.GetComponent<Renderer>().material = teleboardFXMaterial;
		Material sharedMaterial3 = characterModel.DoubleJumpers.GetComponent<Renderer>().sharedMaterial;
		if (sharedMaterial3 != teleboardFXMaterial)
		{
			cachedDoubleJumperMaterial = sharedMaterial3;
		}
		characterModel.DoubleJumpers.GetComponent<Renderer>().material = teleboardFXMaterial;
		characterModel.StopEyeAnimations();
		if (characterModel.CurrentHoverboardModelsRenderes.Count > 0)
		{
			cachedCustomHoverboardModelMaterials = new Material[characterModel.CurrentHoverboardModelsRenderes.Count];
			for (int j = 0; j < characterModel.CurrentHoverboardModelsRenderes.Count; j++)
			{
				Material material = null;
				if (characterModel.CurrentHoverboardModelsRenderes[j] != null)
				{
					material = characterModel.CurrentHoverboardModelsRenderes[j].sharedMaterial;
				}
				if (material != teleboardFXMaterial && material != null)
				{
					cachedCustomHoverboardModelMaterials[j] = material;
				}
			}
			for (int k = 0; k < characterModel.CurrentHoverboardModelsRenderes.Count; k++)
			{
				if (characterModel.CurrentHoverboardModelsRenderes[k] != null)
				{
					characterModel.CurrentHoverboardModelsRenderes[k].material = teleboardFXMaterial;
				}
			}
		}
		StartCoroutine(TeleportFXStartScale(duration));
	}

	private IEnumerator TeleportFXStartScale(float duration)
	{
		float time = 0f;
		float t = 0f;
		while (time < duration)
		{
			time += Time.deltaTime;
			t = Mathf.Clamp01(time / duration);
			if (characterModel == null)
			{
				break;
			}
			characterModel.transform.localScale = new Vector3((1f - 0.5f * t) * intialScale.x, (1f + 0.5f * t) * intialScale.y, (1f - t) * intialScale.z);
			if (t == 1f && teleportFxActive)
			{
				characterModel.transform.localScale = Vector3.zero;
				cachedHoverboardBaseModelRenderer.enabled = false;
				characterModel.Model.enabled = false;
				characterModel.HeadModel.enabled = false;
				SetEnabledWithinCurrentCustomHoverboardModels(false);
				characterModel.OutfitModel.enabled = false;
				if (characterModel.CurrentHoverboard != null)
				{
					characterModel.CurrentHoverboard.SetActive(false);
				}
			}
			yield return null;
		}
	}

	private void TeleboardEnd(float duration)
	{
		if (characterModel == null)
		{
			return;
		}
		if (_teleboardMenuEffect != null)
		{
			_teleboardMenuEffect.EndTeleboardEffect(duration, characterModel);
			return;
		}
		characterModel.transform.localScale = intialScale;
		if (duration > 0f)
		{
			if (teleboardWarp != null)
			{
				teleboardWarp.SetActive(true);
			}
			if (teleboardAnimation != null)
			{
				teleboardAnimation.normalizedTime = 1f;
				teleboardAnimation.weight = 1f;
				teleboardAnimation.speed = -1.8f;
				teleboardAnimation.enabled = true;
			}
		}
		if (root != null && teleboardWarp != null)
		{
			teleboardWarp.transform.position = root.position;
		}
		if (teleboardAnimation != null)
		{
			StartCoroutine(pTween.To(teleboardAnimation.length, (float t) =>
			{
				if (t == 1f)
				{
					if (teleboardWarp != null)
					{
						teleboardWarp.SetActive(true);
					}
					animator.StopPlayback();
				}
			}));
		}
		StartCoroutine(pTween.To(duration, (float t) =>
		{
			if (t == 1f && teleportFxActive)
			{
				ResetTeleportFX();
			}
		}));
	}

	private IEnumerator TeleboardEndTimer(float duration)
	{
		float time = 0f;
		float t = 0f;
		while (time < duration)
		{
			time += Time.deltaTime;
			t = Mathf.Clamp01(time / duration);
			if (t == 1f && teleportFxActive)
			{
				teleboardWarp.SetActive(false);
				animator.StopPlayback();
				ResetTeleportFX();
			}
			yield return null;
		}
	}

	public void ResetTeleportFX()
	{
		if (characterModel == null || !teleportFxActive)
		{
			return;
		}
		characterModel.Model.enabled = true;
		characterModel.HeadModel.enabled = true;
		if (cachedHoverboardBaseModelRenderer != null)
		{
			cachedHoverboardBaseModelRenderer.enabled = true;
		}
		SetEnabledWithinCurrentCustomHoverboardModels(true);
		characterModel.OutfitModel.enabled = cachedCustomizationState;
		if (characterModel.CurrentHoverboard != null)
		{
			characterModel.CurrentHoverboard.SetActive(true);
		}
		if (cachedModelMaterial != null)
		{
			characterModel.Model.GetComponent<Renderer>().material = cachedModelMaterial;
		}
		if (characterModel.HeadModel != null && characterModel.HeadModel.materials != null && cachedHeadMaterials != null)
		{
			characterModel.HeadModel.materials = cachedHeadMaterials;
		}
		characterModel.OutfitModel.material = cachedCustomizationMaterial;
		characterModel.MeshSuperSneaker.material = cachedSupersneakersMaterial;
		characterModel.DoubleJumpers.material = cachedDoubleJumperMaterial;
		characterModel.StartEyeAnimations(UIModelController.ModelScreen.Boards);
		if (cachedHoverboardBaseModelRenderer != null)
		{
			cachedHoverboardBaseModelRenderer.material = cachedHoverboardMaterial;
		}
		if (cachedCustomHoverboardModelMaterials != null && cachedCustomHoverboardModelMaterials.Length > 0)
		{
			for (int i = 0; i < characterModel.CurrentHoverboardModelsRenderes.Count; i++)
			{
				if (cachedCustomHoverboardModelMaterials.Length - 1 >= i)
				{
					characterModel.CurrentHoverboardModelsRenderes[i].material = cachedCustomHoverboardModelMaterials[i];
				}
			}
		}
		characterModel.transform.localScale = intialScale;
		teleportFxActive = false;
	}

	private void SetEnabledWithinCurrentCustomHoverboardModels(bool isEnabled)
	{
		if (characterModel.CurrentHoverboardModelsRenderes.Count <= 0)
		{
			return;
		}
		foreach (Renderer currentHoverboardModelsRendere in characterModel.CurrentHoverboardModelsRenderes)
		{
			if (currentHoverboardModelsRendere != null)
			{
				currentHoverboardModelsRendere.enabled = isEnabled;
			}
		}
	}

	private void OpenGlider()
	{
		if (OnOpenGlider != null)
		{
			OnOpenGlider();
		}
	}

	private void CloseGlider()
	{
		if (OnCloseGlider != null)
		{
			OnCloseGlider();
		}
	}

	public void ResetAnimator()
	{
		if (animator != null)
		{
			animator.Play("empty");
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}
}
