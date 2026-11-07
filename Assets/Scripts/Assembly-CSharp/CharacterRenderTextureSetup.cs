using System;
using System.Collections.Generic;
using SYBO.Subway.Info;
using UI;
using UI.ModelCamera;
using UI.RenderTexture;
using UnityEngine;

[Serializable]
public class CharacterRenderTextureSetup : RenderTextureSetupBase
{
	[Serializable]
	public abstract class EventModelInfo
	{
		[Header("Small Cell Settings")]
		public Vector3 position = ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localPosition;

		public Vector3 rotation = ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localRotation;

		public Vector3 scale = Vector3.one * ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localScaleOnAllAxes;

		[Header("Wide Cell Settings")]
		public bool useWideCellSettings;

		public Vector3 secondaryPosition = ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localPosition;

		public Vector3 secondaryRotation = ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localRotation;

		public Vector3 secondaryScale = Vector3.one * ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localScaleOnAllAxes;

		public virtual RenderTextureObject GetRenderTextureObject(bool isWideContainer = false)
		{
			RenderTextureObject renderTextureObject = new RenderTextureObject();
			if (useWideCellSettings && isWideContainer)
			{
				renderTextureObject.position = secondaryPosition;
				renderTextureObject.rotation = secondaryRotation;
				renderTextureObject.scale = secondaryScale;
			}
			else
			{
				renderTextureObject.position = position;
				renderTextureObject.rotation = rotation;
				renderTextureObject.scale = scale;
			}
			renderTextureObject.prefab = GetPrefab();
			return renderTextureObject;
		}

		public abstract GameObject GetPrefab();
	}

	[Serializable]
	public class EventModelCharacter : EventModelInfo
	{
		public Characters.CharacterId charID;

		public int charOutfitIndex;

		public AnimationClip animationClip;

		public override RenderTextureObject GetRenderTextureObject(bool isWideContainer = false)
		{
			RenderTextureObject renderTextureObject = base.GetRenderTextureObject(isWideContainer);
			renderTextureObject.animationClip = animationClip;
			return renderTextureObject;
		}

		public override GameObject GetPrefab()
		{
			return UIModelController.Instance.ModelPrefab;
		}
	}

	private Characters.CharacterId _character;

	private int _outfitIndex;

	private CharacterModel _characterModel;

	public void Setup(Characters.CharacterId characterId, int outfitIndex, UITexture texture, List<RenderTextureObject> renderTextureObjects, CameraSettings overrideCameraSettings = null, bool copySettings = true)
	{
		_character = characterId;
		_outfitIndex = outfitIndex;
		Setup(texture, renderTextureObjects, overrideCameraSettings, copySettings);
	}

	protected override void OnSetup()
	{
		base.OnSetup();
		for (int i = 0; i < _objectInstances.Count; i++)
		{
			_characterModel = _objectInstances[i].GetComponent<CharacterModel>();
			if (_characterModel != null)
			{
				_characterModel.ChangeCharacterModel(Characters.characterData[_character].modelName, _outfitIndex, true, true);
				_characterModel.HideAllPowerups();
				if (renderTextureObjects[i].animationClip != null)
				{
					_characterModel.StartCustomAnimation(UIModelController.ModelScreen.Character, true, renderTextureObjects[i].animationClip);
				}
				else
				{
					_characterModel.StartIdleAnimations(UIModelController.ModelScreen.Character);
				}
			}
		}
	}

	public override void Setup(UITexture texture, List<RenderTextureObject> renderTextureObjects, CameraSettings overrideCameraSettings = null, bool copySettings = true)
	{
		base.Setup(texture, renderTextureObjects, overrideCameraSettings, copySettings);
	}

	public override void Setup(CameraSettings overrideCameraSettings = null, bool copySettings = true)
	{
		base.Setup(overrideCameraSettings, copySettings);
	}
}
