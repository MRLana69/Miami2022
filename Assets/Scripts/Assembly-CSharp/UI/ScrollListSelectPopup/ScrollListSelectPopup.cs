using System;
using System.Collections.Generic;
using UI.KScrollView;
using UI.Popups;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.ScrollListSelectPopup
{
	public class ScrollListSelectPopup : UIPopup
	{
		private class ScrollListSelectAdapter : ListAdapter<ScrollListPopupCell>
		{
			private ScrollListSelectPopup _scrollListSettingsPopup;

			private GameObject _scrollListTogglePrefab;

			private GameObject _scrollListSpacePrefab;

			private const int DEBUGSETTINGS_TOGGLE_PREFAB = 0;

			private const int DEBUGSETTINGS_BUTTON_PREFAB = 1;

			private const int DEBUGSETTINGS_SPACE_PREFAB = 2;

			public ScrollListSelectAdapter(List<ScrollListPopupCell> contentList, GameObject scrollListTogglePrefab, GameObject scrollListSpacePrefab, ScrollListSelectPopup screenReference)
				: base(contentList)
			{
				_scrollListTogglePrefab = scrollListTogglePrefab;
				_scrollListSpacePrefab = scrollListSpacePrefab;
				_scrollListSettingsPopup = screenReference;
			}

			public override int GetModelType(int index)
			{
				switch (GetDataItem(index).cellType)
				{
				case ScrollListPopupCellType.Toggle:
					return 0;
				case ScrollListPopupCellType.Space:
					return 2;
				default:
					return 0;
				}
			}

			public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
			{
				Container container2 = container;
				ScrollListPopupCell dataItem = GetDataItem(dataIndex);
				if (container2 == null)
				{
					ScrollListPopupCellReferences scrollListPopupCellReferences = new ScrollListPopupCellReferences();
					switch (dataItem.cellType)
					{
					case ScrollListPopupCellType.Toggle:
					{
						container2 = new Container(_scrollListTogglePrefab);
						ScrollListPopupCellHelper component = container2.prefabInstance.GetComponent<ScrollListPopupCellHelper>();
						scrollListPopupCellReferences.scrollListPopopCellHelper = component;
						scrollListPopupCellReferences.uiEventListener = UIEventListener.Get(container2.prefabInstance);
						scrollListPopupCellReferences.uiEventListener.parameter = container2;
						UIEventListener uiEventListener = scrollListPopupCellReferences.uiEventListener;
						uiEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uiEventListener.onClick, new UIEventListener.VoidDelegate(_scrollListSettingsPopup.OnClickContainer));
						container2.controlComponent = scrollListPopupCellReferences;
						break;
					}
					case ScrollListPopupCellType.Space:
						container2 = new Container(_scrollListSpacePrefab);
						container2.controlComponent = scrollListPopupCellReferences;
						break;
					}
				}
				if (!isMeasurePass)
				{
					ScrollListPopupCellReferences scrollListPopupCellReferences2 = (ScrollListPopupCellReferences)container2.controlComponent;
					switch (dataItem.cellType)
					{
					case ScrollListPopupCellType.Toggle:
						scrollListPopupCellReferences2.scrollListPopopCellHelper.UpdateTitle(dataItem.cellTitle);
						scrollListPopupCellReferences2.scrollListPopopCellHelper.cellID = dataItem.cellID;
						scrollListPopupCellReferences2.scrollListPopopCellHelper.SetToggleIcon(_scrollListSettingsPopup._indicators[dataItem.cellID]);
						break;
					default:
						SDebug.LogError("No cell type match for: " + dataItem.cellType);
						break;
					case ScrollListPopupCellType.Space:
						break;
					}
				}
				return container2;
			}
		}

		[FormerlySerializedAs("ScrollListTogglePrefab")]
		[SerializeField]
		private GameObject _scrollListTogglePrefab;

		[FormerlySerializedAs("ScrollListSpacePrefab")]
		[SerializeField]
		private GameObject _scrollListSpacePrefab;

		[SerializeField]
		private UISprite _popupBackground;

		private const int POPUP_MINIMUM_HEIGHT = 60;

		private const int POPUP_MAXIMUM_HEIGHT = 400;

		private const int CELL_HEIGHT_WITH_SPACE = 34;

		private List<ScrollListPopupCell> _contentList = new List<ScrollListPopupCell>();

		private ScrollListSelectAdapter _adapter;

		[SerializeField]
		private ScrollView _scrollView;

		private Dictionary<string, string> _items;

		private Dictionary<string, bool> _indicators;

		private bool _multiSelect;

		private Action<Dictionary<string, string>> _callBack;

		private static ScrollListSelectPopup _instance;

		public static ScrollListSelectPopup instance
		{
			get
			{
				return _instance;
			}
		}

		protected override void Init()
		{
			_instance = this;
			if (_contentList != null)
			{
				_adapter = new ScrollListSelectAdapter(_contentList, _scrollListTogglePrefab, _scrollListSpacePrefab, this);
			}
			SetContentList(null);
		}

		public List<string> getSelected()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, bool> indicator in _indicators)
			{
				if (indicator.Value)
				{
					list.Add(indicator.Key);
				}
			}
			return list;
		}

		protected override void Show()
		{
			RefreshScrollList();
		}

		public void Initialize(Dictionary<string, string> items, Action<Dictionary<string, string>> callback, List<string> selectedItems = null, bool multiSelect = false)
		{
			_items = items;
			_callBack = callback;
			_multiSelect = multiSelect;
			_indicators = new Dictionary<string, bool>();
			foreach (KeyValuePair<string, string> item in items)
			{
				if (selectedItems != null && selectedItems.Contains(item.Key))
				{
					_indicators.Add(item.Key, true);
				}
				else
				{
					_indicators.Add(item.Key, false);
				}
			}
			RefreshScrollList();
		}

		protected override void Hide()
		{
			SendCallBack();
			ResetList();
		}

		private void SendCallBack()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, bool> indicator in _indicators)
			{
				if (indicator.Value)
				{
					dictionary.Add(indicator.Key, _items[indicator.Key]);
				}
			}
			_callBack(dictionary);
		}

		private void SetContentList(Dictionary<string, string> items)
		{
			ResetList();
			if (items != null)
			{
				foreach (KeyValuePair<string, string> item in items)
				{
					InsertToggle(item.Value, item.Key);
					InsertSpace();
				}
			}
			if (_scrollView != null && _adapter != null)
			{
				_scrollView.SetAdapter(_adapter);
			}
		}

		private void InsertToggle(string title, string id)
		{
			_contentList.Add(new ScrollListPopupCell
			{
				cellType = ScrollListPopupCellType.Toggle,
				cellTitle = title,
				cellID = id
			});
		}

		private void InsertSpace()
		{
			_contentList.Add(new ScrollListPopupCell
			{
				cellType = ScrollListPopupCellType.Space
			});
		}

		private void ResetList()
		{
			_contentList.Clear();
		}

		private void OnClickContainer(GameObject go)
		{
			Container container = (Container)go.GetComponent<UIEventListener>().parameter;
			ScrollListPopupCellHelper scrollListPopopCellHelper = ((ScrollListPopupCellReferences)container.controlComponent).scrollListPopopCellHelper;
			if (!_multiSelect)
			{
				foreach (string key in _items.Keys)
				{
					_indicators[key] = false;
				}
			}
			_indicators[scrollListPopopCellHelper.cellID] = !_indicators[scrollListPopopCellHelper.cellID];
			scrollListPopopCellHelper.SetToggleIcon(_indicators[scrollListPopopCellHelper.cellID]);
			if (!_multiSelect && _scrollView != null)
			{
				Container firstVisibleContainerOnScreen = _scrollView.GetFirstVisibleContainerOnScreen();
				if (firstVisibleContainerOnScreen != null)
				{
					int positionIndex = firstVisibleContainerOnScreen.positionIndex;
					RefreshScrollList();
					_scrollView.SetSelection(positionIndex);
				}
			}
		}

		private void AdjustPopupSize()
		{
			int num = 60 + 34 * _items.Count;
			if (num < 400)
			{
				_popupBackground.height = num;
			}
			else
			{
				_popupBackground.height = 400;
			}
			UIRect[] componentsInChildren = _popupBackground.GetComponentsInChildren<UIRect>();
			foreach (UIRect uIRect in componentsInChildren)
			{
				uIRect.ResetAndUpdateAnchors();
			}
			if (_adapter != null)
			{
				_adapter.NotifyDataSetChanged();
			}
		}

		private void RefreshScrollList()
		{
			if (_items != null)
			{
				int positionIndex = 0;
				Container firstVisibleContainerOnScreen = _scrollView.GetFirstVisibleContainerOnScreen();
				if (firstVisibleContainerOnScreen != null)
				{
					positionIndex = firstVisibleContainerOnScreen.positionIndex;
				}
				SetContentList(_items);
				_scrollView.SetSelection(positionIndex);
				AdjustPopupSize();
			}
		}
	}
}
