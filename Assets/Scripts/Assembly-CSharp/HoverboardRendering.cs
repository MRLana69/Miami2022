using System;
using System.Collections.Generic;
using SYBO.Subway.Characters;
using UnityEngine;

public class HoverboardRendering : MonoBehaviour
{
	[Serializable]
	public class AbilityAnimationList
	{
		public Hoverboard.BoardAbility ability;

		public bool prio;

		public AnimationClip defaultHoverboardAnimation;

		public AnimationPair[] runAnimations;

		public AnimationPair[] landAnimations;

		public AnimationPair[] jumpAnimations;

		public AnimationPair[] hangtimeAnimations;

		public AnimationPair[] rollAnimations;

		public AnimationPair[] dodgeLeftAnimations;

		public AnimationPair[] dodgeRightAnimations;

		public AnimationPair[] grindAnimations;

		public AnimationPair[] grindLandAnimations;

		public AnimationPair[] getOnBoardAnimations;
	}

	[Serializable]
	public class AnimationStringPair
	{
		public string characterAnimation = string.Empty;

		public string hoverboardAnimation = string.Empty;
	}

	[Serializable]
	public class AnimationPair
	{
		public AnimationClip characterAnimationClip;

		public AnimationClip hoverboardAnimationClip;
	}

	[OptionalField]
	public AnimationClip defaultHoverboardAnimation;

	public AnimationPair[] runAnimations;

	public AnimationPair[] landAnimations;

	public AnimationPair[] jumpAnimations;

	public AnimationPair[] hangtimeAnimations;

	public AnimationPair[] rollAnimations;

	public AnimationPair[] dodgeLeftAnimations;

	public AnimationPair[] dodgeRightAnimations;

	public AnimationPair[] grindAnimations;

	public AnimationPair[] grindLandAnimations;

	public AnimationPair[] getOnBoardAnimations;

	public AbilityAnimationList[] abilityAnimationLists;

	private bool hasInitializedAnimationSounds;

	public void Initialize(Animation avatarAnimation, Animation hoverboardAnimation, AbilityAnimationList defaultAbilityAnimationLists, List<AnimationClip> addedClipsList, Hoverboard.BoardAbility[] abilities)
	{
		CharacterRendering instance = CharacterRendering.Instance;
		if (jumpAnimations.Length != hangtimeAnimations.Length)
		{
			SDebug.LogError("Lists of hoverboard jumps and hoverboard hangtime animations does not have the same length", this);
		}
		AbilityAnimationList abilityAnimationList = new AbilityAnimationList();
		Hoverboard.BoardAbility boardAbility = Hoverboard.BoardAbility.None;
		if (abilityAnimationLists.Length > 1)
		{
			AbilityAnimationList[] array = abilityAnimationLists;
			foreach (AbilityAnimationList abilityAnimationList2 in array)
			{
				if (abilityAnimationList2.prio)
				{
					foreach (Hoverboard.BoardAbility boardAbility2 in abilities)
					{
						if (abilityAnimationList2.ability == boardAbility2)
						{
							abilityAnimationList = abilityAnimationList2;
						}
					}
				}
				if (abilityAnimationList2.ability == boardAbility)
				{
					abilityAnimationList = abilityAnimationList2;
				}
			}
		}
		else if (abilityAnimationLists.Length == 1)
		{
			foreach (Hoverboard.BoardAbility boardAbility3 in abilities)
			{
				if (abilityAnimationLists[0].ability == boardAbility3)
				{
					abilityAnimationList = abilityAnimationLists[0];
				}
			}
		}
		instance.animations.RUN = GetNamesAddAnimationClips(abilityAnimationList.runAnimations, (runAnimations.Length != 0) ? runAnimations : defaultAbilityAnimationLists.runAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.LAND = GetNamesAddAnimationClips(abilityAnimationList.landAnimations, (landAnimations.Length != 0) ? landAnimations : defaultAbilityAnimationLists.landAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.BIG_LAND = GetNamesAddAnimationClips(abilityAnimationList.landAnimations, (landAnimations.Length != 0) ? landAnimations : defaultAbilityAnimationLists.landAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.SMALL_LAND = GetNamesAddAnimationClips(abilityAnimationList.landAnimations, (landAnimations.Length != 0) ? landAnimations : defaultAbilityAnimationLists.landAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.JUMP = GetNamesAddAnimationClips(abilityAnimationList.jumpAnimations, (jumpAnimations.Length != 0) ? jumpAnimations : defaultAbilityAnimationLists.jumpAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.SMALL_JUMP = GetNamesAddAnimationClips(abilityAnimationList.jumpAnimations, (jumpAnimations.Length != 0) ? jumpAnimations : defaultAbilityAnimationLists.jumpAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.HANGTIME = GetNamesAddAnimationClips(abilityAnimationList.hangtimeAnimations, (hangtimeAnimations.Length != 0) ? hangtimeAnimations : defaultAbilityAnimationLists.hangtimeAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.ROLL = GetNamesAddAnimationClips(abilityAnimationList.rollAnimations, (rollAnimations.Length != 0) ? rollAnimations : defaultAbilityAnimationLists.rollAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.DODGE_LEFT = GetNamesAddAnimationClips(abilityAnimationList.dodgeLeftAnimations, (dodgeLeftAnimations.Length != 0) ? dodgeLeftAnimations : defaultAbilityAnimationLists.dodgeLeftAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.DODGE_RIGHT = GetNamesAddAnimationClips(abilityAnimationList.dodgeRightAnimations, (dodgeRightAnimations.Length != 0) ? dodgeRightAnimations : defaultAbilityAnimationLists.dodgeRightAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.GRIND = GetNamesAddAnimationClips(abilityAnimationList.grindAnimations, (grindAnimations.Length != 0) ? grindAnimations : defaultAbilityAnimationLists.grindAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		GetNamesAddAnimationClips(abilityAnimationList.grindLandAnimations, (grindLandAnimations.Length != 0) ? grindLandAnimations : defaultAbilityAnimationLists.grindLandAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.GET_ON_BOARD = GetNamesAddAnimationClips(abilityAnimationList.getOnBoardAnimations, (getOnBoardAnimations.Length != 0) ? getOnBoardAnimations : defaultAbilityAnimationLists.getOnBoardAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		if (hoverboardAnimation != null)
		{
			hoverboardAnimation.AddClip(defaultHoverboardAnimation, defaultHoverboardAnimation.name);
		}
		if (defaultHoverboardAnimation != null)
		{
			instance.animations.DEFAULT_HOVERBOARD_ANIMATION = defaultHoverboardAnimation.name;
		}
		if (!hasInitializedAnimationSounds)
		{
			hasInitializedAnimationSounds = true;
			RunnerAnimPlayer componentInChildren = instance.GetComponentInChildren<RunnerAnimPlayer>();
			componentInChildren.InitializeHoverboardClips();
			AnimationPair[] array2 = ((abilityAnimationList.jumpAnimations == null) ? jumpAnimations : abilityAnimationList.jumpAnimations);
			AnimationPair[] array3 = array2;
			foreach (AnimationPair animationPair in array3)
			{
				componentInChildren.Add(new KeyFrameAudio
				{
					KeyFrame = 1,
					clip = animationPair.characterAnimationClip.name,
					Audio = componentInChildren.h_kick
				});
				componentInChildren.Add(new KeyFrameAudio
				{
					KeyFrame = 2,
					clip = animationPair.characterAnimationClip.name,
					Audio = componentInChildren.h_long
				});
			}
			AnimationPair[] array4 = ((abilityAnimationList.landAnimations == null) ? landAnimations : abilityAnimationList.landAnimations);
			AnimationPair[] array5 = array4;
			foreach (AnimationPair animationPair2 in array5)
			{
				componentInChildren.Add(new KeyFrameAudio
				{
					KeyFrame = 0,
					clip = animationPair2.characterAnimationClip.name,
					Audio = componentInChildren.h_kick
				});
			}
			AnimationPair[] array6 = ((abilityAnimationList.rollAnimations == null) ? rollAnimations : abilityAnimationList.rollAnimations);
			AnimationPair[] array7 = array6;
			foreach (AnimationPair animationPair3 in array7)
			{
				componentInChildren.Add(new KeyFrameAudio
				{
					KeyFrame = 0,
					clip = animationPair3.characterAnimationClip.name,
					Audio = componentInChildren.h_roll
				});
				componentInChildren.Add(new KeyFrameAudio
				{
					KeyFrame = 4,
					clip = animationPair3.characterAnimationClip.name,
					Audio = componentInChildren.h_miniKick
				});
			}
		}
	}

	private List<string> GetNamesAddAnimationClips(AnimationPair[] abilityPairs, AnimationPair[] pairs, Animation avaterAnimation, Animation hoverboardAnimation, List<AnimationClip> addedClipsList)
	{
		List<string> list = new List<string>();
		AnimationPair[] array = ((abilityPairs == null || 0 >= abilityPairs.Length) ? pairs : abilityPairs);
		foreach (AnimationPair animationPair in array)
		{
			AnimationClip characterAnimationClip = animationPair.characterAnimationClip;
			if (avaterAnimation != null)
			{
				AddClipsToAnimationComp(avaterAnimation, characterAnimationClip, addedClipsList);
			}
			if (hoverboardAnimation != null && animationPair.hoverboardAnimationClip != null)
			{
				AddClipsToAnimationComp(hoverboardAnimation, animationPair.hoverboardAnimationClip, null);
			}
			if (characterAnimationClip == null)
			{
				throw new Exception("character cannot be null.");
			}
			list.Add(characterAnimationClip.name);
		}
		return list;
	}

	private void AddClipsToAnimationComp(Animation animation, AnimationClip clip, List<AnimationClip> addedClipsList)
	{
		if (!(clip != null))
		{
			return;
		}
		if (addedClipsList != null)
		{
			if (!addedClipsList.Contains(clip))
			{
				addedClipsList.Add(clip);
				animation.AddClip(clip, clip.name);
			}
		}
		else
		{
			animation.AddClip(clip, clip.name);
		}
	}
}
