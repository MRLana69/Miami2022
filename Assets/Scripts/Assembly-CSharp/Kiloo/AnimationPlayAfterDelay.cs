using System.Collections;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo
{
	public class AnimationPlayAfterDelay : MonoBehaviour
	{
		[SerializeField]
		private BaseTimeDelay _timeDelay;

		[SerializeField]
		private Animation _animation;

		public void OnEnable()
		{
			CoroutineUtil.StartCoroutine(PlayAnimationAfterDelay());
		}

		private IEnumerator PlayAnimationAfterDelay()
		{
			yield return new WaitForSeconds(_timeDelay.delay);
			_animation.Play();
		}
	}
}
