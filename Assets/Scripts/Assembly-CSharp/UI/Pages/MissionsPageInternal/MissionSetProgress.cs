using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using NaughtyAttributes;
using UnityEngine;

namespace UI.Pages.MissionsPageInternal
{
	public class MissionSetProgress : MonoBehaviour
	{
		[Tooltip("Fill sprites for mission progress... Arrange the list of sprites in order")]
		[SerializeField]
		private List<UISprite> _fillSprites = new List<UISprite>();

		[Header("Effects")]
		[Tooltip("A curve to use as a scaling effect on the progress cells and reward when they are activated")]
		[SerializeField]
		private AnimationCurve _progressCellScaleEffect;

		[SerializeField]
		private float _bounceEffectDuration;

		[SerializeField]
		private GameObject _mysteryBoxIcon;

		[SerializeField]
		private GameObject _starIcon;

		private const string INACTIVE_DOT = "mission_progress_dot";

		private const string ACTIVE_DOT = "mission_progress_dot_completed";

		private List<GameObject> _cachedBlinks = new List<GameObject>(3);

		public void SetMissionSetProgress(int progress)
		{
			_cachedBlinks.ForEach(UnityEngine.Object.Destroy);
			_cachedBlinks.Clear();
			for (int i = 0; i < _fillSprites.Count; i++)
			{
				UISprite uISprite = _fillSprites[i];
				uISprite.spriteName = ((i >= progress) ? "mission_progress_dot" : "mission_progress_dot_completed");
				uISprite.transform.localScale = Vector3.one;
			}
		}

		public void BounceNextProgressDot(Action callback = null)
		{
			int index = _fillSprites.Count((UISprite s) => s.spriteName == "mission_progress_dot_completed");
			if (_fillSprites.ValidIndex(index))
			{
				UISprite uISprite = _fillSprites[index];
				uISprite.spriteName = "mission_progress_dot_completed";
				StartCoroutine(BounceObject(uISprite, _bounceEffectDuration, callback));
			}
		}

		public void BounceReward()
		{
			StartCoroutine(BounceObject(_mysteryBoxIcon.GetComponent<UISprite>(), _bounceEffectDuration));
			StartCoroutine(BounceObject(_starIcon.GetComponent<UISprite>(), _bounceEffectDuration));
		}

		public void ChangeToMysteryBoxReward(bool animate = false)
		{
			_starIcon.SetActive(false);
			_mysteryBoxIcon.SetActive(true);
			if (animate)
			{
				StartCoroutine(BounceObject(_mysteryBoxIcon.GetComponent<UISprite>(), _bounceEffectDuration));
			}
		}

		public float GetBounceDuration()
		{
			return _bounceEffectDuration;
		}

		private IEnumerator BounceObject(UISprite spriteToBounce, float duration, Action callWhenFinished = null)
		{
			UISprite blinkSprite = UnityEngine.Object.Instantiate(spriteToBounce, spriteToBounce.transform);
			blinkSprite.depth++;
			blinkSprite.transform.Reset();
			blinkSprite.color = new Color(1f, 1f, 1f, 0f);
			_cachedBlinks.Add(blinkSprite.gameObject);
			float currentDuration = 0f;
			while (currentDuration <= duration)
			{
				currentDuration += Time.unscaledDeltaTime;
				float eval = _progressCellScaleEffect.Evaluate(currentDuration / duration);
				if (currentDuration.IsBetween(duration / 10f, duration / 5f))
				{
					float alpha = Easings.BackEaseOut(currentDuration / (duration / 5f - duration / 10f));
					blinkSprite.alpha = alpha;
				}
				else
				{
					blinkSprite.alpha = 0f;
				}
				spriteToBounce.transform.localScale = Vector3.one * eval;
				yield return null;
			}
			_cachedBlinks.Remove(blinkSprite.gameObject);
			UnityEngine.Object.Destroy(blinkSprite.gameObject);
			spriteToBounce.transform.localScale = Vector3.one;
			callWhenFinished.SafeInvoke();
		}

		[Button(null)]
		private void TestBounce1()
		{
			TestBounce(_fillSprites[0]);
		}

		[Button(null)]
		private void TestBounce2()
		{
			TestBounce(_fillSprites[1]);
		}

		[Button(null)]
		private void TestBounce3()
		{
			TestBounce(_fillSprites[2]);
		}

		private void TestBounce(UISprite sprite)
		{
			if (Application.isPlaying)
			{
				sprite.spriteName = "mission_progress_dot_completed";
				StartCoroutine(BounceObject(sprite, _bounceEffectDuration));
			}
		}

		[Button(null)]
		private void TestMysteryBoxBounce()
		{
			Timer.ExecuteWithDelay(0.21f, () =>
			{
				_starIcon.SetActive(!_starIcon.activeInHierarchy);
				_mysteryBoxIcon.SetActive(!_mysteryBoxIcon.activeInHierarchy);
			});
			StartCoroutine(BounceObject(_mysteryBoxIcon.GetComponent<UISprite>(), _bounceEffectDuration));
			StartCoroutine(BounceObject(_starIcon.GetComponent<UISprite>(), _bounceEffectDuration));
		}
	}
}
