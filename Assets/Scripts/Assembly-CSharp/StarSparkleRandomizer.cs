using System.Collections;
using System.Collections.Generic;
using Kiloo.Common.Utility;
using UnityEngine;

public class StarSparkleRandomizer : MonoBehaviour
{
	private const int MINIMUM_WAIT_TIME = 1;

	private const int MAXIMUM_WAIT_TIME = 3;

	[SerializeField]
	private List<StarSparkler> _stars = new List<StarSparkler>();

	private Coroutine _sparkleCoroutine;

	private int _lastStarSparkledIndex;

	private void OnEnable()
	{
		ResetAlphas();
		_sparkleCoroutine = CoroutineUtil.StartCoroutine(Sparkle());
	}

	private void OnDisable()
	{
		ResetAlphas();
		CoroutineUtil.StopCoroutine(_sparkleCoroutine);
	}

	private IEnumerator Sparkle()
	{
		int sparkleIndex = Random.Range(0, _stars.Count);
		while (_lastStarSparkledIndex == sparkleIndex)
		{
			sparkleIndex = Random.Range(0, _stars.Count);
		}
		_stars[sparkleIndex].animation.Play();
		_lastStarSparkledIndex = sparkleIndex;
		yield return new WaitForSeconds(Random.Range(1, 3));
		_sparkleCoroutine = CoroutineUtil.StartCoroutine(Sparkle());
	}

	private void ResetAlphas()
	{
		foreach (StarSparkler star in _stars)
		{
			star.sprite.alpha = 0f;
		}
	}
}
