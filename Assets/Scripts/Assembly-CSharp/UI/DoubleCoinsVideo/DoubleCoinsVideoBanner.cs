using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using UnityEngine;

namespace UI.DoubleCoinsVideo
{
	public class DoubleCoinsVideoBanner : DoubleCoinsVideo
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private float _starAnimationPause;

		[SerializeField]
		private float _starCountDown;

		[SerializeField]
		private List<GameObject> _startSpawnPoints;

		[SerializeField]
		private GameObject _starPrefab;

		private List<GameObject> _stars = new List<GameObject>();

		public override void Setup(DoubleCoinsVideoPayload payload)
		{
			_payload = payload;
			_title.text = Strings.Get("DOUBLE_COINS_VIDEO_CAPPED_TITLE");
			SetCoinVisualValue(0);
		}

		public override void SetCoinVisualValue(int coinValue)
		{
			base.SetCoinVisualValue(coinValue);
			string text = "[c]{0}{1}[-][/c]".Populate(ColorData.instance.sun.ToUIHex(), coinValue);
			_buttonLabel.text = Strings.Get("DOUBLE_COINS_VIDEO_BUTTON").Populate(text);
		}

		[UsedImplicitly]
		public void ButtonClicked()
		{
			AdTrackingSystemLogger.LogVideoAdButtonClick(AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS);
			WatchVideo();
		}

		public void AnimateStars()
		{
			if (_startSpawnPoints.Count > 0 && _stars.Count <= 0)
			{
				StartCoroutine(ActivateStars());
			}
		}

		private IEnumerator ActivateStars()
		{
			foreach (GameObject startSpawnPoint in _startSpawnPoints)
			{
				_stars.Add(Object.Instantiate(_starPrefab, startSpawnPoint.transform.position, Quaternion.Euler(0f, 0f, 0f), startSpawnPoint.transform));
			}
			yield return new WaitForSeconds(1f);
			while (true)
			{
				foreach (GameObject star in _stars)
				{
					Animation[] subStars = star.GetComponentsInChildren<Animation>();
					Animation[] array = subStars;
					foreach (Animation animation in array)
					{
						animation.Play();
					}
					yield return new WaitForSeconds(_starCountDown);
				}
				yield return new WaitForSeconds(_starAnimationPause);
			}
		}

		private void OnEnable()
		{
			AnimateStars();
		}

		private void OnDisable()
		{
			if (_stars == null)
			{
				return;
			}
			foreach (GameObject star in _stars)
			{
				Object.Destroy(star);
			}
			_stars.Clear();
		}
	}
}
