using System.Collections;
using UnityEngine;

public class AwardStarHandler : MonoBehaviour
{
	[SerializeField]
	private Animation[] _stars;

	[SerializeField]
	private float minSecondsToNextAnim;

	[SerializeField]
	private float maxSecondsToNextAnim;

	[HideInInspector]
	public bool animeStars;

	public void ActivateStars()
	{
		Animation[] stars = _stars;
		foreach (Animation starAnimation in stars)
		{
			StartCoroutine(AnimateStar(starAnimation));
		}
	}

	private IEnumerator AnimateStar(Animation starAnimation)
	{
		yield return new WaitForSeconds(Random.Range(minSecondsToNextAnim, maxSecondsToNextAnim));
		while (animeStars)
		{
			starAnimation.Play();
			yield return new WaitForSeconds(Random.Range(minSecondsToNextAnim, maxSecondsToNextAnim));
		}
	}
}
