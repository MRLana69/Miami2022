using Data.Save;
using NaughtyAttributes;
using UnityEngine;

namespace UI
{
	public class DailyChallengeColorChanger : MonoBehaviour
	{
		public UILabel shadowLabel;

		public Color MyColorActive;

		public Color MyColorInactive;

		public Color shadowColorActive;

		public Color shadowColorInactive;

		private string _letterColorActiveAsString;

		private string _letterColorInactiveAsString;

		private string _shadowColorActiveAsString;

		private string _shadowColorInactiveAsString;

		private UILabel _myLabel;

		private string _cachedText = string.Empty;

		private string _cachedDailyWord = string.Empty;

		private IntMask _cachedDailyMask = -1;

		private void Awake()
		{
			_myLabel = base.gameObject.GetComponent<UILabel>();
			EncodeColors();
		}

		private void OnEnable()
		{
			UpdateDailyWord();
		}

		private void Update()
		{
			UpdateDailyWord();
		}

		private void EncodeColors()
		{
			_letterColorActiveAsString = NGUIText.EncodeColor(ColorData.instance.sun);
			_letterColorInactiveAsString = NGUIText.EncodeColor(ColorData.instance.dailyLetterColorActiveShadow);
			_shadowColorActiveAsString = NGUIText.EncodeColor(ColorData.instance.dailyLetterColorInactive);
			_shadowColorInactiveAsString = NGUIText.EncodeColor(ColorData.instance.dailyLetterColorInactiveShadow);
			if (MyColorActive != Color.clear)
			{
				_letterColorActiveAsString = NGUIText.EncodeColor(MyColorActive);
			}
			if (MyColorInactive != Color.clear)
			{
				_letterColorInactiveAsString = NGUIText.EncodeColor(MyColorInactive);
			}
			if (shadowColorActive != Color.clear)
			{
				_shadowColorActiveAsString = NGUIText.EncodeColor(shadowColorActive);
			}
			if (shadowColorInactive != Color.clear)
			{
				_shadowColorInactiveAsString = NGUIText.EncodeColor(shadowColorInactive);
			}
		}

		[Button("Update Daily Word")]
		private void UpdateDailyWord()
		{
			if (_myLabel == null)
			{
				_myLabel = base.gameObject.GetComponent<UILabel>();
			}
			string text = EventsSave.wordHuntDailyWord;
			if (string.IsNullOrEmpty(text))
			{
				text = string.Empty;
			}
			if (!Application.isPlaying)
			{
				text = "JAKE";
			}
			int length = text.Length;
			IntMask intMask = EventsSave.wordHuntUnlockedMask;
			if (!Application.isPlaying)
			{
				intMask = new IntMask(1);
			}
			if (!(text == _cachedDailyWord) || (int)intMask != (int)_cachedDailyMask)
			{
				_cachedDailyWord = text;
				_cachedDailyMask = intMask;
				string text2 = string.Empty;
				for (int i = 0; i < length; i++)
				{
					text2 = ((!intMask[i]) ? (text2 + "[" + _letterColorInactiveAsString + "]") : (text2 + "[" + _letterColorActiveAsString + "]"));
					text2 = text2 + text[i] + " ";
				}
				if (length > 0)
				{
					_cachedText = text2.Substring(0, text2.Length - 1);
					_myLabel.text = _cachedText;
				}
				if (length > 0)
				{
					_cachedText = text2.Substring(0, text2.Length - 1);
					_myLabel.text = _cachedText;
				}
				string text3 = string.Empty;
				for (int j = 0; j < length; j++)
				{
					text3 = ((!intMask[j]) ? (text3 + "[" + _shadowColorInactiveAsString + "]") : (text3 + "[" + _shadowColorActiveAsString + "]"));
					text3 = text3 + text[j] + " ";
				}
				if (length > 0)
				{
					shadowLabel.text = text3.Substring(0, text3.Length - 1);
				}
			}
		}
	}
}
