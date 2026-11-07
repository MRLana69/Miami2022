using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace UI.DoubleCoinsVideo
{
	public class DoubleCoinsVideoButton : DoubleCoinsVideo
	{
		[SerializeField]
		private UISprite _bigFill;

		[SerializeField]
		private UISprite _smallFill;

		[SerializeField]
		private UISprite _bigOverlay;

		[SerializeField]
		private UISprite _smallOverlay;

		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _subtitle;

		public override void Setup(DoubleCoinsVideoPayload payload)
		{
			_payload = payload;
			SetupBasic();
			_title.text = Strings.Get((_payload.coinAmount != DoubleCoinsVideo.payoutCap) ? "DOUBLE_COINS_VIDEO_UNCAPPED_TITLE" : "DOUBLE_COINS_VIDEO_CAPPED_TITLE");
			SetCoinVisualValue(0);
		}

		[Button(null)]
		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_bigFill.color = instance.ocean;
			_smallFill.color = instance.grass;
			_bigOverlay.color = instance.sky.WithAlpha(0.6f);
			_smallOverlay.color = instance.spring;
			_title.color = instance.sun;
			_title.effectColor = instance.shadow;
			_subtitle.color = instance.white;
			_subtitle.effectColor = instance.shadow;
			if (!Application.isPlaying)
			{
				base.gameObject.SetActive(false);
				base.gameObject.SetActive(true);
			}
		}

		public override void SetCoinVisualValue(int coinValue)
		{
			base.SetCoinVisualValue(coinValue);
			string text = "[c]{0}{1}[-][/c]".Populate(ColorData.instance.sun.ToUIHex(), coinValue);
			_subtitle.text = Strings.Get("DOUBLE_COINS_VIDEO_BUTTON").Populate(text);
		}

		[UsedImplicitly]
		private void OnClick()
		{
			WatchVideo();
		}
	}
}
