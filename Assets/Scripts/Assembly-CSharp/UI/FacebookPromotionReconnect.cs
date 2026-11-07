using System;
using Kiloo.Common.Extensions;
using UnityEngine;
using Utilities;

namespace UI
{
	public class FacebookPromotionReconnect : MonoBehaviour
	{
		public class ReconnectPayload
		{
			public Action clickedAction;

			public int panelOffset;
		}

		[SerializeField]
		private GameObject _loginButton;

		[SerializeField]
		private UILabel _warningLabel;

		[SerializeField]
		private DinoLogoCharacterHelper _dinoLogoPrefab;

		[SerializeField]
		private UIWidget _modelContainer;

		private DinoLogoCharacterHelper _dino;

		public void Init(ReconnectPayload payload)
		{
			if (SafeArea.IsRestricted)
			{
				GetComponent<UIWidget>().ResetAndUpdateAnchors();
			}
			_loginButton.GetComponent<FriendsLoginFBButton>().Init(payload.clickedAction, true, true);
			_warningLabel.text = Strings.Get("FACEBOOK_PROMOTION_RECONNECT_WARNING");
			CreateDinoLogo(payload.panelOffset);
		}

		private void CreateDinoLogo(int subPanelOffset)
		{
			if (_dino == null)
			{
				_dino = UnityEngine.Object.Instantiate(_dinoLogoPrefab, _modelContainer.transform);
				_dino.transform.Reset();
				_dino.Init(new DinoLogoCharacterHelper.DinoLogoPayload
				{
					panelOffset = subPanelOffset
				});
			}
		}
	}
}
