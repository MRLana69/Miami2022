using System;
using JetBrains.Annotations;
using NaughtyAttributes;
using UI;
using UI.Generic.Layout;
using UI.Popups;
using UnityEngine;

namespace TopRun.UI
{
	public class TopRunPopup : UIPopup
	{
		[Serializable]
		private class MedalCategory
		{
			public UILabel title;

			public UISprite background;
		}

		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _line1Big;

		[SerializeField]
		private UILabel _line1Small;

		[SerializeField]
		private UILabel _buttonText;

		[SerializeField]
		private MedalCategory _diamond;

		[SerializeField]
		private MedalCategory _gold;

		[SerializeField]
		private MedalCategory _silver;

		[SerializeField]
		private MedalCategory _bronze;

		[SerializeField]
		private UISprite _topCountryFlag;

		[SerializeField]
		private UILabel _topCountryName;

		[SerializeField]
		private UISprite _infoTextBackground;

		[SerializeField]
		private Layout _textLayout;

		public const int NEWEST_TOPRUN_POPUP_VERSION_NUMBER = 2;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			SetupBasic();
		}

		[Button(null)]
		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_title.text = Strings.Get("TOPRUN_TITLE");
			_line1Big.text = Strings.Get("POPUP_TOPRUN_V2_LINE1");
			_line1Small.text = Strings.Get("POPUP_TOPRUN_V2_LINE2");
			_textLayout.Align();
			_buttonText.text = Strings.Get("OK");
			_infoTextBackground.color = GlobalColors.TOPRUN_POPUP_TEXT_BACKGROUND_PURPLE;
			_diamond.background.color = GlobalColors.TOPRUN_POPUP_DIAMOND_BACKGROUND;
			_gold.background.color = GlobalColors.TOPRUN_POPUP_GOLD_BACKGROUND;
			_silver.background.color = GlobalColors.TOPRUN_POPUP_SILVER_BACKGROUND;
			_bronze.background.color = GlobalColors.TOPRUN_POPUP_BRONZE_BACKGROUND;
			_diamond.title.text = Strings.Get("TOPRUN_RANK_1_TITLE");
			_gold.title.text = Strings.Get("TOPRUN_RANK_2_TITLE");
			_silver.title.text = Strings.Get("TOPRUN_RANK_3_TITLE");
			_bronze.title.text = Strings.Get("TOPRUN_RANK_4_TITLE");
			_title.color = instance.midnight;
			_topCountryName.color = instance.peacock;
			_topCountryFlag.spriteName = CountryManager.instance.GetCurrentTournamentCountry().flag;
			_topCountryName.text = Strings.Get(CountryManager.instance.GetCurrentTournamentCountry().displayNameID).ToUpper();
		}

		[UsedImplicitly]
		public void OKClicked()
		{
			UIScreenController.Instance.ignoreLastPopupClosed = true;
			if (UIScreenController.Instance.GetTopScreenName() != "TopRunUI" && UIScreenController.Instance.GetTopScreenName() != "GameOverScreen")
			{
				UIScreenController.Instance.PushScreen("TopRunUI");
			}
			ClosePopup();
		}
	}
}
