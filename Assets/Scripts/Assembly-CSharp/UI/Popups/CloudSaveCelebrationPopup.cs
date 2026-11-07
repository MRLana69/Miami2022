using System;
using System.Collections;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using NaughtyAttributes;
using UnityEngine;

namespace UI.Popups
{
	public class CloudSaveCelebrationPopup : UIPopup
	{
		[SerializeField]
		private UILabel _headline;

		[SerializeField]
		private UILabel _subline;

		[SerializeField]
		private UIWidget _skyline;

		[SerializeField]
		private UIWidget _cloud;

		[SerializeField]
		private UIWidget _checkmark;

		protected override void Init()
		{
			Localize();
			ApplyColors();
		}

		protected override void Show()
		{
			Reset();
			Animate();
		}

		private void Localize()
		{
			_headline.text = Strings.Get("CLOUDSAVE_CELEBRATION_TOP");
			_subline.text = Strings.Get("CLOUDSAVE_CELEBRATION_BOTTOM");
		}

		[Button(null)]
		private void ApplyColors()
		{
			ColorData instance = ColorData.instance;
			_headline.color = instance.white;
			_headline.effectColor = instance.midnight;
			_subline.color = instance.white;
			_subline.effectColor = instance.midnight;
			_skyline.color = instance.royale;
			base.gameObject.Blink();
		}

		public override bool ShouldShowTopBar()
		{
			return false;
		}

		public override void BackButtonPressed()
		{
			PopupClicked();
		}

		[UsedImplicitly]
		public void PopupClicked()
		{
			UIScreenController.Instance.ClosePopup();
		}

		[Button(null)]
		private void Reset()
		{
			_headline.alpha = 0f;
			_cloud.alpha = 0f;
			_cloud.transform.localScale = Vector3.one * 0.01f;
			_checkmark.alpha = 0f;
			_subline.alpha = 0f;
		}

		[Button(null)]
		private void Animate()
		{
			StartCoroutine(AnimateRoutine());
		}

		private IEnumerator AnimateRoutine()
		{
			yield return StartCoroutine(AnimateAlpha(_headline, 0.3f, 0f, 1f, Easings.Linear));
			StartCoroutine(AnimateAlpha(_cloud, 0.1f, 0f, 1f, Easings.Linear));
			Vector3 startScale = Vector3.one * 0.1f;
			Vector3 originalCloudScale = Vector3.one;
			yield return StartCoroutine(AnimateScale(_cloud, 1f / 6f, startScale, originalCloudScale * 1.2f, Easings.Linear));
			yield return StartCoroutine(AnimateScale(_cloud, 0.125f, originalCloudScale * 1.2f, originalCloudScale * 0.9f, Easings.BounceEaseInOut));
			yield return StartCoroutine(AnimateScale(_cloud, 0.1f, originalCloudScale * 0.9f, originalCloudScale, Easings.BounceEaseInOut));
			StartCoroutine(AnimateAlpha(_checkmark, 0.05f, 0f, 1f, Easings.Linear));
			Vector3 startCheckMarkScale = Vector3.one * 0.8f;
			Vector3 originalCheckmarkScale = Vector3.one;
			yield return StartCoroutine(AnimateScale(_checkmark, 1f / 15f, startCheckMarkScale, originalCheckmarkScale * 1.2f, Easings.Linear));
			yield return StartCoroutine(AnimateScale(_checkmark, 1f / 15f, originalCheckmarkScale * 1.2f, originalCheckmarkScale * 0.9f, Easings.BounceEaseInOut));
			yield return StartCoroutine(AnimateScale(_checkmark, 1f / 15f, originalCheckmarkScale * 0.9f, originalCheckmarkScale, Easings.BounceEaseInOut));
			yield return StartCoroutine(AnimateAlpha(_subline, 0.3f, 0f, 179f / 255f, Easings.Linear));
		}

		private IEnumerator AnimateAlpha(UIWidget widget, float duration, float from, float to, Func<float, float> easing)
		{
			widget.alpha = from;
			float factor = 0f;
			while (factor < 1f)
			{
				factor += Time.deltaTime / duration;
				factor = Mathf.Clamp01(factor);
				float plot = easing(factor);
				widget.alpha = Mathf.Lerp(from, to, plot);
				yield return null;
			}
		}

		private IEnumerator AnimateScale(UIWidget widget, float duration, Vector3 from, Vector3 to, Func<float, float> easing)
		{
			widget.transform.localScale = from;
			float factor = 0f;
			while (factor < 1f)
			{
				factor += Time.deltaTime / duration;
				factor = Mathf.Clamp01(factor);
				float plot = easing(factor);
				widget.transform.localScale = Vector3.LerpUnclamped(from, to, plot);
				yield return null;
			}
			widget.transform.localScale = to;
		}
	}
}
