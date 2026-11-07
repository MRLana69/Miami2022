using System;
using SYBO.Subway.Info;

namespace UI.RenderTexture
{
	[Serializable]
	public class KingHeaderRenderTextureSetup : RenderTextureSetupBase
	{
		private CharacterModel _characterModel;

		protected override void OnSetup()
		{
			base.OnSetup();
			for (int i = 0; i < _objectInstances.Count; i++)
			{
				_characterModel = _objectInstances[i].GetComponent<CharacterModel>();
				if (_characterModel != null)
				{
					_characterModel.ChangeCharacterModel(Characters.characterData[Characters.CharacterId.king].modelName, 2, true, true);
					_characterModel.HideAllPowerups();
					_characterModel.StartCustomAnimation(UIModelController.ModelScreen.Character, true, renderTextureObjects[i].animationClip);
				}
			}
		}
	}
}
