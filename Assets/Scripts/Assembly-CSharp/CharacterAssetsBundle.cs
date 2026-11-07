using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAssetsBundle : MonoBehaviour
{
	[Serializable]
	private class OutfitBreathAnimation
	{
		[SerializeField]
		[HideInInspector]
		private string OutfitIdentifier = string.Empty;

		public int OutfitIndex;

		public AnimationClip Breath;
	}

	public Mesh MainMesh;

	public Mesh HeadMesh;

	public int EyeMaterialID = 1;

	public bool AnimatedEyesHaveDoubleMaterial = true;

	public AvatarEyeAnimation _avatarEyeAnimation;

	public AnimationClip Breath;

	public AnimationClip ScrollbarPose;

	public AnimationClip ScrollbarHeadPose;

	public AnimationClip EventScreenPose;

	public AnimationClip NameEventScreenIdle;

	public bool UseCustomEventScreenIdle = true;

	[SerializeField]
	private List<OutfitBreathAnimation> _outfitBreathAnimations = new List<OutfitBreathAnimation>();

	public AnimationClip GetOutfitBreatheOrNull(int outfitIndex)
	{
		foreach (OutfitBreathAnimation outfitBreathAnimation in _outfitBreathAnimations)
		{
			if (outfitBreathAnimation.OutfitIndex == outfitIndex)
			{
				return outfitBreathAnimation.Breath;
			}
		}
		return null;
	}
}
