using System.Text;
using Kiloo.Common.Extensions;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInCharacterEventHelper : UISlideIn
	{
		[SerializeField]
		private UILabel _textLabel;

		[SerializeField]
		private UILabel _shadowLabel;

		[SerializeField]
		private Layout _layout;

		private string _myColorActive;

		private string _myColorInactive;

		private string _shadowColorActive;

		private string _shadowColorInactive;

		private void Awake()
		{
			StoreColors();
		}

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			UpdateCharacterEventWord(slideIn.value, slideIn.stringValue);
			_layout.Align();
			SlideIn();
		}

		private void StoreColors()
		{
			_myColorActive = new Color32(238, 204, 50, byte.MaxValue).ToUIHex();
			_myColorInactive = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue).ToUIHex();
			_shadowColorActive = new Color32(75, 75, 75, byte.MaxValue).ToUIHex();
			_shadowColorInactive = _shadowColorActive;
		}

		private void UpdateCharacterEventWord(int letterMask, string extraInfo)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			char[] array = extraInfo.ToCharArray();
			int num = letterMask + 1;
			int num2 = array.Length;
			for (int i = 0; i < num2; i++)
			{
				if (IsFilteredCharacter(array[i]))
				{
					num++;
				}
				else if (num > i)
				{
					stringBuilder.Append(_myColorActive);
					stringBuilder2.Append(_shadowColorActive);
				}
				else
				{
					stringBuilder.Append(_myColorInactive);
					stringBuilder2.Append(_shadowColorInactive);
				}
				stringBuilder.Append(array[i]);
				stringBuilder2.Append(array[i]);
				if (i + 1 < num2)
				{
					stringBuilder.Append(" ");
					stringBuilder2.Append(" ");
				}
			}
			if (num2 > 0)
			{
				_textLabel.text = stringBuilder.ToString();
				_shadowLabel.text = stringBuilder2.ToString();
			}
		}

		private bool IsFilteredCharacter(char newChar)
		{
			char[] fILTERED_CHARACTERS = CharacterNameEventManager.FILTERED_CHARACTERS;
			foreach (char c in fILTERED_CHARACTERS)
			{
				if (newChar == c)
				{
					return true;
				}
			}
			return false;
		}
	}
}
