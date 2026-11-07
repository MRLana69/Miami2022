using ABTest;
using UnityEngine;

namespace UI.Popups.ABTestTesterPopupInternal
{
	public class ABTestTesterCellHelper : MonoBehaviour
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _description;

		[SerializeField]
		private SelectPopupButton _selectPayloadButton;

		[SerializeField]
		private UILabel _selectPayloadText;

		[SerializeField]
		private GameObject _offIcon;

		[SerializeField]
		private GameObject _onIcon;

		private bool _currentIconState;

		private ABTestData.ABTestDataItem _testDataItem;

		public void Init(ABTestData.ABTestDataItem testDataItem)
		{
			_testDataItem = testDataItem;
			_title.text = testDataItem.debugTitle;
			_description.text = testDataItem.identifier;
			EventDelegate.Add(_selectPayloadButton.onChange, PayloadSelected);
			int num = 0;
			_currentIconState = false;
			if (ABTestManager.HasDebugPayload(testDataItem.identifier))
			{
				int num2 = testDataItem.debugPayload.IndexOf(ABTestManager.GetDebugPayload(testDataItem.identifier));
				if (num2 > -1)
				{
					num = num2;
					_currentIconState = true;
				}
			}
			_offIcon.SetActive(!_currentIconState);
			_onIcon.SetActive(_currentIconState);
			_selectPayloadButton.SetItems(testDataItem.debugPayload, num);
			_selectPayloadText.text = testDataItem.debugPayload[num];
		}

		public void PayloadSelected()
		{
			if (_currentIconState && ABTestManager.GetDebugPayload(_testDataItem.identifier) != _selectPayloadButton.value)
			{
				ABTestManager.EnableDebugPayload(_testDataItem.identifier, _selectPayloadButton.value);
			}
		}

		public void ToggleActive()
		{
			_currentIconState = !_currentIconState;
			_offIcon.SetActive(!_currentIconState);
			_onIcon.SetActive(_currentIconState);
			if (_currentIconState)
			{
				ABTestManager.EnableDebugPayload(_testDataItem.identifier, _selectPayloadButton.value);
			}
			else
			{
				ABTestManager.DisableDebugPayload(_testDataItem.identifier);
			}
		}
	}
}
