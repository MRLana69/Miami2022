using UnityEngine;

namespace UI.Popups
{
	public class CountdownPopup : UIPopup
	{
		[SerializeField]
		private UILabel _countdownStartingLabel;

		[SerializeField]
		private UILabel _countdownLabel;

		private Vector3 _cachedCountdownLabelScale = Vector3.zero;

		private CountdownPopupManager _countdownPopupManager;

		protected override void Init()
		{
			_countdownStartingLabel.text = string.Empty;
			_countdownLabel.text = string.Empty;
			_countdownPopupManager = CountdownPopupManager.instance;
		}

		protected override void Show()
		{
			_countdownPopupManager.onCountdownEnded += OnCountdownEnded;
		}

		protected override void Hide()
		{
			if (_countdownPopupManager.isCountdownActive)
			{
				_countdownPopupManager.StopCountdown();
			}
			_countdownStartingLabel.text = string.Empty;
			_countdownLabel.text = string.Empty;
			_countdownPopupManager.onCountdownEnded -= OnCountdownEnded;
		}

		public override bool ShouldShowTopBar()
		{
			return false;
		}

		public override void BackButtonPressed()
		{
			UIScreenController.Instance.PushScreen("PauseUI");
		}

		private void Update()
		{
			if (_countdownPopupManager.isCountdownActive && UIScreenController.Instance.IsApplicationFocused)
			{
				_countdownStartingLabel.text = Strings.Get("INGAME_UI_COUNTDOWN_STARTING");
				_countdownLabel.text = Mathf.CeilToInt(_countdownPopupManager.countdownInSeconds).ToString();
				if (!_countdownLabel.enabled)
				{
					_countdownStartingLabel.enabled = true;
					_countdownLabel.enabled = true;
				}
				if (_cachedCountdownLabelScale == Vector3.zero)
				{
					_cachedCountdownLabelScale = _countdownLabel.cachedTransform.localScale;
				}
				_countdownLabel.cachedTransform.localScale = _cachedCountdownLabelScale * ((1f - _countdownPopupManager.countdownInSeconds % 1f) * 0.5f + 1f);
			}
		}

		private void OnCountdownEnded()
		{
			_countdownStartingLabel.text = string.Empty;
			_countdownLabel.text = string.Empty;
			_countdownStartingLabel.enabled = false;
			_countdownLabel.enabled = false;
			UIScreenController.Instance.ClosePopup();
		}
	}
}
