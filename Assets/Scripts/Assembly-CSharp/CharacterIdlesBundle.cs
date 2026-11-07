using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterIdlesBundle : MonoBehaviour
{
	[Serializable]
	private class OutfitIdleAnimations
	{
		[SerializeField]
		[HideInInspector]
		private string OutfitIdentifier = string.Empty;

		public int OutfitIndex;

		public AnimationClip StartClip;

		public List<AnimationClip> Idles = new List<AnimationClip>();
	}

	[FormerlySerializedAs("startClip")]
	public AnimationClip StartClip;

	public List<AnimationClip> Idles;

	public List<AnimationClip> ExcludedCelebrationIdles;

	public List<AnimationClip> ExcludedGameOverIdles;

	[SerializeField]
	private List<OutfitIdleAnimations> _outfitIdles;

	public AnimationClip GetOutfitStartClip(int outfitIndex)
	{
		foreach (OutfitIdleAnimations outfitIdle in _outfitIdles)
		{
			if (outfitIdle.OutfitIndex == outfitIndex)
			{
				return outfitIdle.StartClip;
			}
		}
		return null;
	}

	public List<AnimationClip> GetOutfitIdles(int outfitIndex)
	{
		foreach (OutfitIdleAnimations outfitIdle in _outfitIdles)
		{
			if (outfitIdle.OutfitIndex == outfitIndex)
			{
				return outfitIdle.Idles;
			}
		}
		return new List<AnimationClip>();
	}
}
