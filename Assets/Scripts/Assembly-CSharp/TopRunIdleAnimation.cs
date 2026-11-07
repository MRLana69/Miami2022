using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using UnityEngine;

public class TopRunIdleAnimation : MonoBehaviour
{
	[Serializable]
	public class AnimationElement
	{
		public AnimationClip clip;

		public int propability = 1;
	}

	[Serializable]
	public class TopRunAnimationSettings
	{
		public string name;

		public AnimationClip topRunStart;

		public List<AnimationClip> topRunIdles;

		public List<AnimationClip> topRunHandMiss;

		public List<AnimationElement> topRunHandHit;
	}

	public delegate void AnimationFinishDelegate();

	private Animation Target;

	public bool PlayIdleAnimations;

	public bool paused;

	private IEnumerator animationRoutine;

	private float nextAnimationTime;

	public TopRunAnimationSettings[] animationSettings;

	private List<AnimationClip> topRunHandHitWeighted = new List<AnimationClip>();

	private Dictionary<string, TopRunAnimationSettings> _characterToAnimationSettings;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AnimationFinishDelegate AnimationFinished__BackingField;

	public event AnimationFinishDelegate AnimationFinished
	{
		add
		{
			AnimationFinishDelegate animationFinishDelegate = AnimationFinished__BackingField;
			AnimationFinishDelegate animationFinishDelegate2;
			do
			{
				animationFinishDelegate2 = animationFinishDelegate;
				animationFinishDelegate = Interlocked.CompareExchange(ref AnimationFinished__BackingField, (AnimationFinishDelegate)Delegate.Combine(animationFinishDelegate2, value), animationFinishDelegate);
			}
			while ((object)animationFinishDelegate != animationFinishDelegate2);
		}
		remove
		{
			AnimationFinishDelegate animationFinishDelegate = AnimationFinished__BackingField;
			AnimationFinishDelegate animationFinishDelegate2;
			do
			{
				animationFinishDelegate2 = animationFinishDelegate;
				animationFinishDelegate = Interlocked.CompareExchange(ref AnimationFinished__BackingField, (AnimationFinishDelegate)Delegate.Remove(animationFinishDelegate2, value), animationFinishDelegate);
			}
			while ((object)animationFinishDelegate != animationFinishDelegate2);
		}
	}

	private void Awake()
	{
		Target = FindAnimationInParent(base.gameObject);
		if (Target == null)
		{
			SDebug.LogError("AvatarAnimations: No animation component for avatar animations", this);
			return;
		}
		_characterToAnimationSettings = new Dictionary<string, TopRunAnimationSettings>();
		for (int i = 0; i < animationSettings.Length; i++)
		{
			if (!_characterToAnimationSettings.ContainsKey(animationSettings[i].name))
			{
				_characterToAnimationSettings.Add(animationSettings[i].name, animationSettings[i]);
			}
		}
	}

	private Animation FindAnimationInParent(GameObject current)
	{
		Animation componentInChildren = current.GetComponentInChildren<Animation>();
		if (componentInChildren != null)
		{
			return componentInChildren;
		}
		if (current.transform.parent != null)
		{
			return FindAnimationInParent(current.transform.parent.gameObject);
		}
		return null;
	}

	private void Update()
	{
		if (PlayIdleAnimations && animationRoutine != null && !paused)
		{
			animationRoutine.MoveNext();
		}
	}

	public void TR_ActivateIdle()
	{
		TopRunAnimationSettings annimationSettingsFor = GetAnnimationSettingsFor(PlayerSave.currentCharacterId.ToString());
		PlayIdleAnimations = true;
		Target.AddClip(annimationSettingsFor.topRunStart, annimationSettingsFor.topRunStart.name);
		foreach (AnimationClip topRunIdle in annimationSettingsFor.topRunIdles)
		{
			Target.AddClip(topRunIdle, topRunIdle.name);
		}
		animationRoutine = PlayTRIdles();
		animationRoutine.MoveNext();
	}

	public void TR_PlayMiss()
	{
		TopRunAnimationSettings annimationSettingsFor = GetAnnimationSettingsFor(PlayerSave.currentCharacterId.ToString());
		foreach (AnimationClip item in annimationSettingsFor.topRunHandMiss)
		{
			Target.AddClip(item, item.name);
		}
		animationRoutine = PlayTRmiss();
		animationRoutine.MoveNext();
	}

	public void TR_PlayHit()
	{
		TopRunAnimationSettings annimationSettingsFor = GetAnnimationSettingsFor(PlayerSave.currentCharacterId.ToString());
		foreach (AnimationElement item in annimationSettingsFor.topRunHandHit)
		{
			for (int i = 0; i < item.propability; i++)
			{
				topRunHandHitWeighted.Add(item.clip);
			}
		}
		foreach (AnimationClip item2 in topRunHandHitWeighted)
		{
			Target.AddClip(item2, item2.name);
		}
		animationRoutine = PlayTRhit();
		animationRoutine.MoveNext();
	}

	private TopRunAnimationSettings GetAnnimationSettingsFor(string name)
	{
		TopRunAnimationSettings value;
		if (_characterToAnimationSettings.TryGetValue(name, out value))
		{
			return value;
		}
		return _characterToAnimationSettings[string.Empty];
	}

	private IEnumerator PlayTRIdles()
	{
		AnimationClip selectedClip = null;
		TopRunAnimationSettings aSettings = GetAnnimationSettingsFor(PlayerSave.currentCharacterId.ToString());
		Target.Play(aSettings.topRunStart.name);
		nextAnimationTime = aSettings.topRunStart.length;
		while (nextAnimationTime > 0f)
		{
			nextAnimationTime -= Time.deltaTime;
			yield return null;
		}
		while (PlayIdleAnimations)
		{
			List<AnimationClip> possibleClips = aSettings.topRunIdles.FindAll((AnimationClip a) => a != selectedClip);
			if (possibleClips.Count > 0)
			{
				selectedClip = possibleClips[UnityEngine.Random.Range(0, possibleClips.Count)];
			}
			Target.Play(selectedClip.name);
			nextAnimationTime = selectedClip.length;
			while (nextAnimationTime > 0f)
			{
				nextAnimationTime -= Time.deltaTime;
				yield return null;
			}
		}
	}

	private IEnumerator PlayTRmiss()
	{
		AnimationClip selectedClip = null;
		TopRunAnimationSettings aSettings = GetAnnimationSettingsFor(PlayerSave.currentCharacterId.ToString());
		selectedClip = aSettings.topRunHandMiss[UnityEngine.Random.Range(0, aSettings.topRunHandMiss.Count)];
		Target.CrossFade(selectedClip.name, 0.1f);
		nextAnimationTime = selectedClip.length;
		while (nextAnimationTime > 0.25f)
		{
			nextAnimationTime -= Time.deltaTime;
			yield return null;
		}
		PlayIdleAnimations = false;
		if (AnimationFinished__BackingField != null)
		{
			AnimationFinished__BackingField();
		}
	}

	private IEnumerator PlayTRhit()
	{
		AnimationClip selectedClip = null;
		selectedClip = topRunHandHitWeighted[UnityEngine.Random.Range(0, topRunHandHitWeighted.Count)];
		Target.CrossFade(selectedClip.name, 0.1f);
		nextAnimationTime = selectedClip.length;
		while (nextAnimationTime > 0.25f)
		{
			nextAnimationTime -= Time.deltaTime;
			yield return null;
		}
		PlayIdleAnimations = false;
		if (AnimationFinished__BackingField != null)
		{
			AnimationFinished__BackingField();
		}
	}
}
