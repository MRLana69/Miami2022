using System.Collections.Generic;
using System.Linq;
using UI.ScrollListSelectPopup;
using UnityEngine;

[RequireComponent(typeof(UIEventTrigger), typeof(BoxCollider))]
public class SelectPopupButton : MonoBehaviour
{
	[SerializeField]
	private List<string> _options = new List<string>();

	private Dictionary<string, string> _actualOptions;

	public bool multiSelect;

	public bool disablePopup;

	[SerializeField]
	private UILabel _currentSelectedLabel;

	private List<string> _selectedItems;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	public bool returningFromSelect { get; private set; }

	public string value { get; private set; }

	public Dictionary<string, string> multipleValues { get; private set; }

	public SelectPopupButton()
	{
		returningFromSelect = false;
	}

	private void Awake()
	{
		multipleValues = new Dictionary<string, string>();
	}

	private void Start()
	{
		returningFromSelect = false;
		EventDelegate.Add(GetComponent<UIEventTrigger>().onClick, OnButtonClick);
		if (_currentSelectedLabel == null)
		{
			_currentSelectedLabel = GetComponentInChildren<UILabel>();
		}
		if (!multiSelect && string.IsNullOrEmpty(value) && _options.Count > 0)
		{
			value = _options[0];
		}
		NotifyOnChange();
	}

	private void OnEnable()
	{
		NotifyOnChange();
	}

	public void SetItems(List<string> items, int selectedIndex = 0)
	{
		returningFromSelect = false;
		_options = items;
		_actualOptions = _options.ToDictionary((string x) => x, (string x) => x);
		if (!multiSelect)
		{
			value = _options[selectedIndex];
			if (_currentSelectedLabel != null)
			{
				_currentSelectedLabel.text = value;
			}
		}
		else if (_currentSelectedLabel != null)
		{
			_currentSelectedLabel.text = "None";
		}
		NotifyOnChange();
	}

	public void SetItems(Dictionary<string, string> items)
	{
		returningFromSelect = false;
		_options = items.Values.ToList();
		_actualOptions = items;
		if (!multiSelect)
		{
			value = _options[0];
			if (_currentSelectedLabel != null)
			{
				_currentSelectedLabel.text = value;
			}
		}
		else if (_currentSelectedLabel != null)
		{
			_currentSelectedLabel.text = "None";
		}
		NotifyOnChange();
	}

	public void SetSelected(List<string> selected)
	{
		SetSelected(selected.ToDictionary((string x) => x, (string x) => x));
	}

	public void SetSelected(Dictionary<string, string> selected)
	{
		if (multiSelect)
		{
			multipleValues = selected;
			NotifyOnChange();
		}
	}

	private void OnButtonClick()
	{
		if (!disablePopup)
		{
			UIScreenController.Instance.PushPopup("ScrollListSelectPopup");
			if (_actualOptions == null)
			{
				SetItems(_options);
			}
			ScrollListSelectPopup.instance.Initialize(_actualOptions, OnSelectPopupClosed, (!multiSelect) ? new List<string> { _currentSelectedLabel.text } : multipleValues.Keys.ToList(), multiSelect);
		}
		returningFromSelect = false;
	}

	private void OnSelectPopupClosed(Dictionary<string, string> selectedItems)
	{
		int count = selectedItems.Count;
		_currentSelectedLabel.text = ((count != 1) ? string.Format("{0} selected", count) : selectedItems.Values.ElementAt(0));
		multipleValues = selectedItems;
		value = ((count <= 0) ? string.Empty : selectedItems.Values.ElementAt(0));
		returningFromSelect = true;
		NotifyOnChange();
	}

	private void NotifyOnChange()
	{
		foreach (EventDelegate item in onChange)
		{
			item.Execute();
		}
	}
}
