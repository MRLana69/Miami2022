using System.Collections;
using System.Collections.Generic;
using SYBO.Components.Utilities;
using UnityEngine;

public class AvatarAnimations : MonoBehaviour
{
	private SkinnedMeshRenderer _head;

	private Transform _blendshapeController;

	private Animation _target;

	[SerializeField]
	private List<AnimationClip> IdlesPopup;

	public bool Paused;

	public bool PlayIdleAnimations;

	private AnimationClip startClip;

	private List<AnimationClip> _idles = new List<AnimationClip>();

	private AnimationClip _breath;

	private CharacterAssetsBundle currentCharacterBundle;

	private int _currentOutfitIndex;

	private bool usePopupIdles;

	private bool useStartClip;

	private IEnumerator animationRoutine;

	private float nextAnimationTime;

	private int _blendshapeCount;

	private int MinIdleTimes = 2;

	private int MaxIdleTimes = 3;

	private CharacterIdlesBundle _idlesBundle;

	public SkinnedMeshRenderer Head
	{
		set
		{
			_head = value;
		}
	}

	public Transform BlendshapeController
	{
		set
		{
			_blendshapeController = value;
		}
	}

	public Animation Target
	{
		set
		{
			_target = value;
		}
	}

	public void Initialize(CharacterAssetsBundle characterAssetsBundle, int outfitIndex)
	{
		_breath = characterAssetsBundle.Breath;
		currentCharacterBundle = characterAssetsBundle;
		_currentOutfitIndex = outfitIndex;
		AnimationClip outfitBreatheOrNull = characterAssetsBundle.GetOutfitBreatheOrNull(_currentOutfitIndex);
		if (outfitBreatheOrNull != null)
		{
			_breath = outfitBreatheOrNull;
			_idles.Add(outfitBreatheOrNull);
		}
		if (PlayIdleAnimations)
		{
			if (usePopupIdles)
			{
				StartIdlePopupAnimations();
			}
			else
			{
				StartIdleAnimations();
			}
		}
		if (_head.sharedMesh != null && _head.enabled)
		{
			_blendshapeCount = _head.sharedMesh.blendShapeCount;
		}
		else
		{
			_blendshapeCount = 0;
		}
	}

	private Animation FindAnimationInParent(GameObject current)
	{
		Animation component = current.GetComponent<Animation>();
		if (component != null)
		{
			return component;
		}
		if (current.transform.parent != null)
		{
			return FindAnimationInParent(current.transform.parent.gameObject);
		}
		return null;
	}

	public static float NormalizeBlendShape(float value)
	{
		float value2 = Mathf.Abs(value);
		return Mathf.Clamp(value2, 0f, 1f);
	}

	public float GetBlendShapeValue(int index)
	{
		switch (index)
		{
		case 0:
			return _blendshapeController.localPosition.x;
		case 1:
			return _blendshapeController.localPosition.y;
		case 2:
			return _blendshapeController.localPosition.z;
		case 3:
			return _blendshapeController.localRotation.x;
		case 4:
			return _blendshapeController.localRotation.y;
		case 5:
			return _blendshapeController.localRotation.z;
		case 6:
			return _blendshapeController.localScale.x;
		case 7:
			return _blendshapeController.localScale.y;
		case 8:
			return _blendshapeController.localScale.z;
		default:
			return 0f;
		}
	}

	private void Update()
	{
		if (!PlayIdleAnimations || animationRoutine == null || Paused)
		{
			return;
		}
		animationRoutine.MoveNext();
		if (_blendshapeCount <= 1)
		{
			return;
		}
		_head.SetBlendShapeWeight(1, NormalizeBlendShape(_blendshapeController.localPosition.y) * 100f);
		if (_blendshapeCount <= 2)
		{
			return;
		}
		_head.SetBlendShapeWeight(2, NormalizeBlendShape(_blendshapeController.localPosition.z) * 100f);
		if (_blendshapeCount <= 3)
		{
			return;
		}
		_head.SetBlendShapeWeight(3, NormalizeBlendShape(_blendshapeController.localRotation.x) * 100f);
		if (_blendshapeCount > 4)
		{
			_head.SetBlendShapeWeight(4, NormalizeBlendShape(_blendshapeController.localRotation.y) * 100f);
			if (_blendshapeCount > 5)
			{
				_head.SetBlendShapeWeight(5, NormalizeBlendShape(_blendshapeController.localRotation.z) * 100f);
			}
		}
	}

	private void OnDestroy()
	{
		ClearBundleAnimation();
	}

	private void ClearBundleAnimation()
	{
		if (!(_idlesBundle != null))
		{
			return;
		}
		if (startClip != null && _target.GetClip(startClip.name) != null)
		{
			_target.RemoveClip(startClip.name);
		}
		startClip = null;
		foreach (AnimationClip idle in _idles)
		{
			if (idle != null && _target.GetClip(idle.name) != null)
			{
				_target.RemoveClip(idle.name);
			}
		}
		_idles.Clear();
		Object.Destroy(_idlesBundle.gameObject);
	}

	public void StartIdleAnimations()
	{
		ClearBundleAnimation();
		SingletonMonoBehaviour<Factories>.Instance.CharacterIdlesBundle.RequestItemInstance(currentCharacterBundle.name + "_idles", (CharacterIdlesBundle idlesBundle) =>
		{
			if (idlesBundle != null)
			{
				_idlesBundle = idlesBundle;
				startClip = _idlesBundle.StartClip;
				_idles.Clear();
				_idles.AddRange(_idlesBundle.Idles);
				AnimationClip outfitStartClip = _idlesBundle.GetOutfitStartClip(_currentOutfitIndex);
				if (outfitStartClip != null)
				{
					startClip = outfitStartClip;
				}
				_idles.AddRange(_idlesBundle.GetOutfitIdles(_currentOutfitIndex));
				_idles.Add(_breath);
				if (UIScreenController.Instance.GetCurrentPopupName() == "CelebrationPopup")
				{
					for (int i = 0; i < _idlesBundle.ExcludedCelebrationIdles.Count; i++)
					{
						if (_idles.Contains(_idlesBundle.ExcludedCelebrationIdles[i]))
						{
							_idles.Remove(_idlesBundle.ExcludedCelebrationIdles[i]);
						}
						if (startClip != null && startClip == _idlesBundle.ExcludedCelebrationIdles[i])
						{
							startClip = null;
						}
					}
				}
				if (UIScreenController.Instance.GetTopScreenName() == "GameOverScreen")
				{
					for (int j = 0; j < _idlesBundle.ExcludedGameOverIdles.Count; j++)
					{
						if (_idles.Contains(_idlesBundle.ExcludedGameOverIdles[j]))
						{
							_idles.Remove(_idlesBundle.ExcludedGameOverIdles[j]);
						}
						if (startClip != null && startClip == _idlesBundle.ExcludedGameOverIdles[j])
						{
							startClip = null;
						}
					}
				}
				PlayIdleAnimations = true;
				Paused = false;
				usePopupIdles = false;
				if (startClip != null)
				{
					useStartClip = true;
					_target.AddClip(startClip, startClip.name);
				}
				else
				{
					useStartClip = false;
				}
				foreach (AnimationClip idle in _idles)
				{
					_target.AddClip(idle, idle.name);
				}
			}
			else
			{
				_idlesBundle = null;
				startClip = null;
				_idles.Clear();
				PlayIdleAnimations = true;
				Paused = false;
				usePopupIdles = false;
				useStartClip = false;
			}
			_target.AddClip(_breath, _breath.name);
			animationRoutine = Play();
			animationRoutine.MoveNext();
		});
	}

	public void StartCustomAnimations(AnimationClip[] animationClips, bool useUnscaledTime)
	{
		PlayIdleAnimations = true;
		Paused = false;
		usePopupIdles = false;
		useStartClip = false;
		foreach (AnimationClip animationClip in animationClips)
		{
			_target.AddClip(animationClip, animationClip.name);
		}
		animationRoutine = PlayAnimationsInOrder(animationClips, useUnscaledTime);
		animationRoutine.MoveNext();
	}

	public void StartIdlePopupAnimations()
	{
		PlayIdleAnimations = true;
		Paused = false;
		usePopupIdles = true;
		useStartClip = false;
		_target.AddClip(_breath, _breath.name);
		foreach (AnimationClip item in IdlesPopup)
		{
			_target.AddClip(item, item.name);
		}
		animationRoutine = Play();
		animationRoutine.MoveNext();
	}

	public void StopIdleAnimations()
	{
		animationRoutine = null;
		PlayIdleAnimations = false;
		List<AnimationClip> list = _idles;
		if (usePopupIdles)
		{
			list = IdlesPopup;
		}
		if (list == null)
		{
			return;
		}
		foreach (AnimationClip item in list)
		{
			foreach (AnimationState item2 in _target)
			{
				if (item2.clip == item)
				{
					_target.RemoveClip(item);
				}
			}
		}
	}

	public void PauseIdleAnimations()
	{
		Paused = true;
		foreach (AnimationState item in _target)
		{
			item.speed = 0f;
		}
	}

	public void ResumeIdleAnimations()
	{
		Paused = false;
		foreach (AnimationState item in _target)
		{
			item.speed = 1f;
		}
	}

	private IEnumerator PlayAnimationsInOrder(AnimationClip[] animationClips, bool useUnscaledTime)
	{
		AnimationClip selectedClip = null;
		int animationIndex = 0;
		_target.Stop();
		while (PlayIdleAnimations)
		{
			if (animationClips.Length > 0)
			{
				selectedClip = animationClips[animationIndex];
			}
			if (selectedClip != null)
			{
				_target.Play(selectedClip.name);
				nextAnimationTime = selectedClip.length;
				while (nextAnimationTime > 0f)
				{
					if (Time.timeScale <= 0f || useUnscaledTime)
					{
						nextAnimationTime -= Time.unscaledDeltaTime;
						_target[selectedClip.name].time = selectedClip.length - nextAnimationTime;
						_target.Sample();
					}
					else
					{
						nextAnimationTime -= Time.deltaTime;
					}
					yield return null;
				}
			}
			_target.Stop();
			animationIndex++;
			if (animationIndex >= animationClips.Length)
			{
				animationIndex = 0;
			}
		}
	}

	private IEnumerator Play()
	{
		AnimationClip selectedClip = null;
		_target.Stop();
		if (useStartClip)
		{
			string clipName = startClip.name;
			_target.Play(clipName);
			while (_target[clipName].enabled && _target[clipName].time + Time.deltaTime < _target[clipName].length)
			{
				yield return null;
			}
			useStartClip = false;
		}
		while (PlayIdleAnimations)
		{
			List<AnimationClip> possibleClips = (usePopupIdles ? IdlesPopup.FindAll((AnimationClip a) => a != selectedClip) : ((_idles.Count <= 0) ? new List<AnimationClip>() : _idles.FindAll((AnimationClip a) => a != selectedClip)));
			if (possibleClips.Count > 0)
			{
				selectedClip = possibleClips[Random.Range(0, possibleClips.Count)];
			}
			if (selectedClip != null)
			{
				_target.Play(selectedClip.name);
				nextAnimationTime = selectedClip.length;
				while (nextAnimationTime > 0f)
				{
					nextAnimationTime -= Time.deltaTime;
					yield return null;
				}
			}
			if (usePopupIdles)
			{
				continue;
			}
			int count = Random.Range(MinIdleTimes, MaxIdleTimes);
			for (int i = 0; i < count; i++)
			{
				if (_target != null)
				{
					_target.Play(_breath.name);
				}
				nextAnimationTime = _breath.length;
				while (nextAnimationTime > 0f)
				{
					nextAnimationTime -= Time.deltaTime;
					yield return null;
				}
			}
		}
		animationRoutine = null;
	}
}
