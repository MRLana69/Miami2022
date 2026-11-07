using System.Collections.Generic;
using SYBO.Subway.Info;
using UI;
using UI.RenderTexture;

public class CharacterBoardBundleRenderTextureSetup : RenderTextureSetupBase
{
	private List<Characters.CharacterId> _characterIds;

	public void Setup(List<Characters.CharacterId> characterIds, UITexture texture, List<RenderTextureObject> renderTextureObjects, CameraSettings overrideCameraSettings = null, bool copySettings = true)
	{
		_characterIds = characterIds;
		Setup(texture, renderTextureObjects, overrideCameraSettings, copySettings);
	}

	protected override void OnSetup()
	{
		int num = 0;
		for (int i = 0; i < _objectInstances.Count; i++)
		{
			HoverboardBundle component = _objectInstances[i].GetComponent<HoverboardBundle>();
			CharacterModel component2 = _objectInstances[i].GetComponent<CharacterModel>();
			if (component != null)
			{
				_objectInstances[i].AddChild(component.GetPrefab());
			}
			else if (component2 != null)
			{
				component2.ChangeCharacterModel(Characters.characterData[_characterIds[num]].modelName, 0, true, true);
				component2.HideAllPowerups();
				if (renderTextureObjects[i].animationClip != null)
				{
					component2.StartCustomAnimation(UIModelController.ModelScreen.Character, true, renderTextureObjects[i].animationClip);
				}
				else
				{
					component2.StartIdleAnimations(UIModelController.ModelScreen.Character);
				}
				num++;
			}
		}
	}
}
