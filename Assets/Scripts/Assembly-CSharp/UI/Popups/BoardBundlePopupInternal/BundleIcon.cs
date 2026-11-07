using System;
using Info;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.Popups.BoardBundlePopupInternal
{
	[Serializable]
	internal class BundleIcon
	{
		[SerializeField]
		private UISprite _bundleIconFrame;

		[SerializeField]
		private UISprite _bundleIconBackground;

		[SerializeField]
		private UISprite _bundleIcon;

		[SerializeField]
		private UILabel _bundleIconLabel;

		[SerializeField]
		private HorizontalLayout _layout;

		public void SetupTheme(Boards.BoardId boardId, int theme)
		{
			EnableOrDisableTheme(true);
			HoverboardThemes.HoverboardTheme hoverboardTheme = HoverboardThemes.hoverboardCustomThemes[boardId][theme - 1];
			_bundleIconBackground.color = hoverboardTheme.buttonBgColor;
			_bundleIcon.color = hoverboardTheme.buttonIconColor;
			_bundleIcon.spriteName = hoverboardTheme.iconName;
			_bundleIconLabel.text = Strings.Get(hoverboardTheme.title);
		}

		public void SetupTheme(HoverboardThemes.HoverboardTheme themeInfo)
		{
			EnableOrDisableTheme(true);
			_bundleIconBackground.color = themeInfo.buttonBgColor;
			_bundleIcon.color = themeInfo.buttonIconColor;
			_bundleIcon.spriteName = themeInfo.iconName;
			_bundleIconLabel.text = Strings.Get(themeInfo.title);
			if (_layout != null)
			{
				_layout.Align();
			}
		}

		public void SetupDefaultTheme(Boards.BoardId boardId)
		{
			EnableOrDisableTheme(true);
			Boards.Board board = Boards.boardData[boardId];
			_bundleIconBackground.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_BASE_SKIN_FILL;
			_bundleIcon.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_BASE_SKIN_ICON;
			if (_bundleIconLabel != null)
			{
				_bundleIconLabel.text = string.Format(Strings.Get("BOARD_DISPLAY_NAME_WITH_SUFFIX"), Strings.Get(board.name));
			}
		}

		public void EnableOrDisableTheme(bool enable)
		{
			_bundleIconBackground.gameObject.SetActive(enable);
			_bundleIcon.gameObject.SetActive(enable);
			if (_bundleIconFrame != null)
			{
				_bundleIconFrame.gameObject.SetActive(enable);
			}
			if (_bundleIconLabel != null)
			{
				_bundleIconLabel.gameObject.SetActive(enable);
			}
		}
	}
}
