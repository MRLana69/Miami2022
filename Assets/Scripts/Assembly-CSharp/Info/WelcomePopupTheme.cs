using UI.Popups;
using UnityEngine;

namespace Info
{
	public class WelcomePopupTheme : ScriptableObject
	{
		[SerializeField]
		private Color32[] _backupColors;

		private static WelcomePopupTheme _instance;

		public Color32[] colors;

		public Color32[] backupColors
		{
			get
			{
				return _backupColors;
			}
		}

		public static WelcomePopupTheme instance
		{
			get
			{
				return _instance ?? (_instance = Resources.Load<WelcomePopupTheme>("Info/WelcomePopupTheme"));
			}
		}

		private WelcomePopupTheme()
		{
			_backupColors = new Color32[2];
			colors = new Color32[2];
		}

		public void Reset()
		{
			colors[0] = backupColors[0];
			colors[1] = backupColors[1];
		}

		public void UpdateBackupColors()
		{
			_backupColors[0] = colors[0];
			_backupColors[1] = colors[1];
		}

		public void RefreshWelcomePopup()
		{
			if (!(UIScreenController.Instance == null))
			{
				WelcomePopup welcomePopup = (WelcomePopup)UIScreenController.Instance.GetScreenFromCache("WelcomePopup");
				if (welcomePopup != null)
				{
					welcomePopup.UpdateColors();
				}
			}
		}
	}
}
