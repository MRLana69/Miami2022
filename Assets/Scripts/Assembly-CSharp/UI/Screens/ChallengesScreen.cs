using System;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using Onboarding;
using UI.Footer;
using UI.Generic.Layout;
using UI.Pages;
using UnityEngine;

namespace UI.Screens
{
	public class ChallengesScreen : UIScreen
	{
		public enum PageIndex
		{
			DailyChallengePage = 0,
			MissionsPage = 1,
			WeeklyHuntPage = 2
		}

		[SerializeField]
		private UIWidget _footerGuideWidget;

		[SerializeField]
		private UIWidget _pageGuideWidget;

		[SerializeField]
		private Layout _footerLayout;

		[SerializeField]
		private FooterButton _footerButtonPrefab;

		[ReorderableList]
		[SerializeField]
		private Page[] _pagePrefabs;

		private Page[] _pages;

		private FooterButton[] _footerButtons;

		private static int _currentlySelectedPageIndex;

		public override bool BlocksCelebrationPopup()
		{
			return _currentlySelectedPageIndex == 1;
		}

		public static void SetPageIndex(int newIndex)
		{
			_currentlySelectedPageIndex = newIndex;
		}

		public static int GetPageIndex()
		{
			return _currentlySelectedPageIndex;
		}

		protected override void Init()
		{
			base.Init();
			_pages = new Page[_pagePrefabs.Length];
			_footerButtons = new FooterButton[_pagePrefabs.Length];
			LoadFooter();
		}

		protected override void Show()
		{
			base.Show();
			ChangePage(_pages.ValidIndex(_currentlySelectedPageIndex) ? _currentlySelectedPageIndex : 0);
		}

		protected override void Hide()
		{
			base.Hide();
			Page page = _pages[_currentlySelectedPageIndex];
			if ((bool)page)
			{
				page.Hide();
			}
		}

		public override void GainFocus()
		{
			base.GainFocus();
			Page page = _pages[_currentlySelectedPageIndex];
			if ((bool)page)
			{
				page.GainFocus();
			}
		}

		public override void LoseFocus()
		{
			base.GainFocus();
			Page page = _pages[_currentlySelectedPageIndex];
			if ((bool)page)
			{
				page.LoseFocus();
			}
		}

		private void ActivatePage(int pageIndex, bool calledFromFooterButton)
		{
			Page page = _pages[pageIndex];
			if (page == null)
			{
				page = UnityEngine.Object.Instantiate(_pagePrefabs[pageIndex], _pageGuideWidget.transform);
				page.transform.Reset();
				_pages[pageIndex] = page;
				_pageGuideWidget.ResetAndUpdateAnchors();
				_pageGuideWidget.leftAnchor.target = null;
				_pageGuideWidget.rightAnchor.target = null;
				_pageGuideWidget.topAnchor.target = null;
				_pageGuideWidget.bottomAnchor.target = null;
				page.Init(_pageGuideWidget);
			}
			page.Show();
			if (calledFromFooterButton)
			{
				Flurry.LogEventWithAParameter("FooterButtonClicked", "button_name", GetButtonName((PageIndex)pageIndex));
			}
			PageIndex pageIndex2 = (PageIndex)pageIndex;
			OnboardingTracking.LogScreenFlow(pageIndex2.ToString());
			SelectFooterButton(pageIndex);
		}

		private string GetButtonName(PageIndex pageIndex)
		{
			switch (pageIndex)
			{
			case PageIndex.DailyChallengePage:
				return "Word hunt";
			case PageIndex.MissionsPage:
				return "Missions";
			case PageIndex.WeeklyHuntPage:
				return "Weekly hunt";
			default:
				return string.Empty;
			}
		}

		private void LoadFooter()
		{
			FooterButton footerButton = UnityEngine.Object.Instantiate(_footerButtonPrefab, _footerGuideWidget.transform);
			footerButton.transform.Reset();
			footerButton.Init(new FooterButton.FooterPayload
			{
				clicked = () =>
				{
					UIScreenController.Instance.PushScreen("FrontScreen");
				},
				width = 72,
				colorScheme = GlobalColors.buttonColorData[UIButtonOverlayOff.ButtonType.Footer_back],
				iconName = "icon_back_menu",
				title = Strings.Get("FOOTER_MENU_BUTTON_TITLE")
			});
			footerButton.name = "Menu Button";
			for (int num = 0; num < _pagePrefabs.Length; num++)
			{
				footerButton = UnityEngine.Object.Instantiate(_footerButtonPrefab, _footerGuideWidget.transform);
				footerButton.transform.Reset();
				FooterButton.FooterPayload buttonPayload = _pagePrefabs[num].GetButtonPayload();
				int pageIndex = num;
				buttonPayload.clicked = (Action)Delegate.Combine(buttonPayload.clicked, (Action)(() =>
				{
					ChangePage(pageIndex, true);
				}));
				footerButton.Init(buttonPayload);
				footerButton.name = buttonPayload.title + " Button";
				_footerButtons[num] = footerButton;
			}
			_footerLayout.Align();
		}

		private void SelectFooterButton(int selectIndex)
		{
			for (int i = 0; i < _footerButtons.Length; i++)
			{
				_footerButtons[i].SetSelectedState(i == selectIndex);
			}
		}

		private void ChangePage(int index, bool calledFromFooterButton = false)
		{
			_currentlySelectedPageIndex = index;
			for (int i = 0; i < _pages.Length; i++)
			{
				Page page = _pages[i];
				if (i == index)
				{
					ActivatePage(index, calledFromFooterButton);
				}
				else if (page != null)
				{
					page.Hide();
				}
			}
		}

		public override string GetFlurryName()
		{
			return "ChallengesScreen";
		}
	}
}
