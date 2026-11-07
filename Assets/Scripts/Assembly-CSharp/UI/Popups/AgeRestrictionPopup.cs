using System;
using System.Collections.Generic;
using System.Globalization;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using SYBO.Subway.Info;
using UI.Generic;
using UI.KScrollView;
using UI.Popups.AgeRestrictionPopupInternal;
using UnityEngine;

namespace UI.Popups
{
	public class AgeRestrictionPopup : UIPopup
	{
		private const int YEAR_START = 1900;

		private const int YEAR_END = 2018;

		private const int INITIAL_YEAR = 2000;

		private const string DEFAULT_VALUE_MONTH = "----";

		private const string DEFAULT_VALUE_YEAR = "----";

		[SerializeField]
		private GameObject _ageCellPrefab;

		[SerializeField]
		private ScrollView _listViewMonth;

		[SerializeField]
		private ScrollView _listViewYear;

		[SerializeField]
		private UIWidget _characterModelGuideWidget;

		[SerializeField]
		private UIButtonOverlayOff _acceptButton;

		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _okLabel;

		private List<ListContentElement> _contentListMonth;

		private List<ListContentElement> _contentListYear;

		private CenterOnChild _centerOnChildMonth;

		private CenterOnChild _centerOnChildYear;

		private CharacterScreenController _character;

		protected override void Init()
		{
			_contentListMonth = CreateContentListMonth();
			_contentListYear = CreateContentListYear();
			_centerOnChildMonth = _listViewMonth.gameObject.AddComponent<CenterOnChild>();
			_centerOnChildYear = _listViewYear.gameObject.AddComponent<CenterOnChild>();
			_listViewMonth.SetAdapter(new ListContentElementAdapter(_contentListMonth, _ageCellPrefab, _listViewMonth, _centerOnChildMonth, "----"));
			_listViewYear.SetAdapter(new ListContentElementAdapter(_contentListYear, _ageCellPrefab, _listViewYear, _centerOnChildYear, "----"));
			int positionIndex = _contentListMonth.FindIndex((ListContentElement entry) => entry.content == "----");
			int positionIndex2 = _contentListYear.FindIndex((ListContentElement entry) => entry.content == "----");
			_listViewMonth.SetSelection(positionIndex, ContainerPivot.Center);
			_listViewYear.SetSelection(positionIndex2, ContainerPivot.Center);
			_centerOnChildMonth.Recenter();
			_centerOnChildYear.Recenter();
			_title.text = Strings.GetLegal("AgeRestrictionTitle");
			_title.color = ColorData.instance.midnight;
			_okLabel.text = Strings.GetLegal("AgeRestrictionOk");
			OnSelectionChanged();
		}

		protected override void Show()
		{
			_centerOnChildMonth.selectedObjectChanged += OnSelectionChanged;
			_centerOnChildMonth.selectedObjectReselected += OnSelectionChanged;
			_centerOnChildYear.selectedObjectChanged += OnSelectionChanged;
			_centerOnChildYear.selectedObjectReselected += OnSelectionChanged;
			CharacterModelPreviewFactory.Instance.GetSimpleScreenModel(Characters.CharacterId.slick, 0, (CharacterScreenController csc) =>
			{
				_character = csc;
				GameObject objectToPosition = PlaceModelUnderAnchor(csc.gameObject, UIModelController.Instance.TutorialPopupAnchor.transform, 31);
				Vector3 positionOffset = new Vector3(-5.5f, 5f, 0f);
				Vector3 newRotation = new Vector3(9f, -162f, 0f);
				Vector3 newScale = new Vector3(13f, 13f, 13f);
				_character.Freeze = 0f;
				PositionModelOnSprite(_characterModelGuideWidget, objectToPosition, positionOffset, newRotation, newScale);
			});
		}

		protected override void Hide()
		{
			_centerOnChildMonth.selectedObjectChanged -= OnSelectionChanged;
			_centerOnChildMonth.selectedObjectReselected -= OnSelectionChanged;
			_centerOnChildYear.selectedObjectChanged -= OnSelectionChanged;
			_centerOnChildYear.selectedObjectReselected -= OnSelectionChanged;
			if (_character != null)
			{
				UnityEngine.Object.Destroy(_character.gameObject);
			}
		}

		public override bool ShouldShowTopBar()
		{
			return false;
		}

		public override void BackButtonPressed()
		{
		}

		public override void LoseFocus()
		{
			_character.gameObject.SetActive(false);
		}

		public override void GainFocus()
		{
			_character.gameObject.SetActive(true);
			_character.Freeze = 0f;
		}

		[UsedImplicitly]
		public void AcceptClicked()
		{
			int value = _centerOnChildYear.selectedObject.GetComponent<ListElementHelper>().listContentElement.value;
			int value2 = _centerOnChildMonth.selectedObject.GetComponent<ListElementHelper>().listContentElement.value;
			if (IsValidDateTime(value, value2))
			{
				DateTime birthday = new DateTime(value, value2, 1);
				AgeRestriction.RegisterUserBirthday(birthday);
				UIScreenController.Instance.CloseActivePopups("AgeRestrictionPopup");
			}
			else
			{
				LogErr("Input is invalid");
			}
			if (AgeRestriction.shouldShowBehaviouralAdsConsentPopup)
			{
				UIScreenController.Instance.PushPopup("ConsentPopup");
			}
		}

		[UsedImplicitly]
		public void MonthArrowUpClicked()
		{
			ArrowClicked(true, _centerOnChildMonth, _contentListMonth, _listViewMonth);
		}

		[UsedImplicitly]
		public void MonthArrowDownClicked()
		{
			ArrowClicked(false, _centerOnChildMonth, _contentListMonth, _listViewMonth);
		}

		[UsedImplicitly]
		public void YearArrowUpClicked()
		{
			UnityEngine.Debug.Log("GJARROW year up");
			ArrowClicked(true, _centerOnChildYear, _contentListYear, _listViewYear);
		}

		[UsedImplicitly]
		public void YearArrowDownClicked()
		{
			ArrowClicked(false, _centerOnChildYear, _contentListYear, _listViewYear);
		}

		private static void ArrowClicked(bool up, CenterOnChild centerer, List<ListContentElement> content, ScrollView listView)
		{
			int currentValue = centerer.selectedObject.GetComponent<ListElementHelper>().listContentElement.value;
			int num = content.FindIndex((ListContentElement entry) => entry.value == currentValue);
			int num2 = (up ? (++num) : (--num));
			if (listView.infinite)
			{
				num2 = ((!up) ? ((num2 >= 0) ? num2 : (content.Count - 1)) : ((num2 < content.Count) ? num2 : 0));
			}
			if (content.ValidIndex(num2))
			{
				centerer.CenterOnTransform(content[num2].helper.transform);
			}
		}

		private List<ListContentElement> CreateContentListMonth()
		{
			CultureInfo legalCultureInfo = Strings.GetLegalCultureInfo();
			DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(legalCultureInfo);
			List<ListContentElement> list = new List<ListContentElement>(instance.AbbreviatedMonthNames.Length + 1);
			int num = 1;
			list.Add(new ListContentElement("----", int.MinValue));
			string[] abbreviatedMonthNames = instance.AbbreviatedMonthNames;
			foreach (string text in abbreviatedMonthNames)
			{
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(new ListContentElement(legalCultureInfo.TextInfo.ToTitleCase(text.ToLower(legalCultureInfo)), num++));
				}
			}
			return list;
		}

		private List<ListContentElement> CreateContentListYear()
		{
			List<ListContentElement> list = new List<ListContentElement>(120);
			for (int i = 1900; i <= 2018; i++)
			{
				if (i == 2000)
				{
					list.Add(new ListContentElement("----", int.MinValue));
				}
				list.Add(new ListContentElement(i.ToString(), i));
			}
			return list;
		}

		private void OnSelectionChanged()
		{
			int value = _centerOnChildYear.selectedObject.GetComponent<ListElementHelper>().listContentElement.value;
			int value2 = _centerOnChildMonth.selectedObject.GetComponent<ListElementHelper>().listContentElement.value;
			SetAcceptButtonActive(IsValidDateTime(value, value2));
		}

		private bool IsValidDateTime(int year, int month)
		{
			DateTime dt;
			return TryParseDateTime(year, month, out dt);
		}

		private static bool TryParseDateTime(int year, int month, out DateTime dt)
		{
			return DateTime.TryParseExact(string.Format("{0:0000}{1:00}", year, month), "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
		}

		private void SetAcceptButtonActive(bool active)
		{
			if (active)
			{
				_acceptButton.GoToNormalState();
			}
			else
			{
				_acceptButton.GoToNotAvailableState();
			}
			_okLabel.GetComponent<ButtonText>().Refresh();
		}

		private static GameObject PlaceModelUnderAnchor(GameObject model, Transform anchor, int layer)
		{
			NGUITools.SetLayer(model, layer);
			model.transform.parent = anchor;
			model.transform.localPosition = Vector3.zero;
			model.transform.localEulerAngles = Vector3.zero;
			model.transform.localScale = Vector3.one;
			return model;
		}

		private static void PositionModelOnSprite(UIWidget tracker, GameObject objectToPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
		{
			Timer.ExecuteNextFrame(() =>
			{
				if (!(objectToPosition == null) && !(tracker == null))
				{
					Vector3 position = tracker.transform.position;
					Vector3 vector = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(position);
					vector = new Vector3(vector.x, vector.y, 1f);
					Vector3 position2 = UIScreenController.Instance.Camera3D.ScreenToWorldPoint(vector);
					objectToPosition.transform.position = position2;
					objectToPosition.transform.localPosition = objectToPosition.transform.localPosition + positionOffset;
					objectToPosition.transform.localRotation = Quaternion.Euler(newRotation);
					objectToPosition.transform.localScale = newScale;
				}
			});
		}

		private static void LogErr(string msg, UnityEngine.Object obj = null)
		{
			SDebug.LogError("[AgeRestrictionPopup] " + msg, obj);
		}
	}
}
