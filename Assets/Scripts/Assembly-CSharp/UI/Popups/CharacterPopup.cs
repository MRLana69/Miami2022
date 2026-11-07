using System.Collections;
using SYBO.Subway.Info;
using UnityEngine;

namespace UI.Popups
{
	public class CharacterPopup : UIPopup
	{
		private CharacterModel _characterModel;

		private bool _characterHidden;

		private UIModelController uiModelController
		{
			get
			{
				return UIModelController.Instance;
			}
		}

		protected void SetCharacter(Characters.CharacterId character, Vector2? offsetFromAnchor = null, Vector3? rotationOffset = null, float scale = 19f, bool posing = false, AnimationClip pose = null, bool useSpecialAsset = true)
		{
			if (_characterModel == null)
			{
				Vector3 localPosition = ((!offsetFromAnchor.HasValue) ? ((Vector2)new Vector3(35f, 23f, 50f)) : offsetFromAnchor.Value);
				localPosition.z = 50f;
				UIModelController.Instance.ActivateModelScreen(UIModelController.ModelScreen.TutorialPopup);
				_characterModel = uiModelController.TutorialPopupAnchor.AddChild(uiModelController.ModelPrefab).GetComponent<CharacterModel>();
				Utility.SetLayerRecursively(_characterModel.transform, uiModelController.TutorialPopupAnchor.layer);
				Transform transform = _characterModel.transform;
				transform.localPosition = localPosition;
				transform.localScale = new Vector3(scale, scale, scale);
				transform.localEulerAngles = ((!posing) ? new Vector3(50f, 200f, 0f) : rotationOffset.Value);
				_characterModel.IsVisibleShadow = false;
			}
			ShowCharacter();
			string modelName = Characters.characterData[character].modelName;
			_characterModel.ChangeCharacterModel(modelName, 0, useSpecialAsset, false);
			_characterModel.HideAllPowerups();
			if (posing)
			{
				Animation characterAnimation = _characterModel.CharacterAnimation;
				if (pose != null)
				{
					characterAnimation.AddClip(pose, pose.name);
					characterAnimation.Play(pose.name);
				}
			}
			else
			{
				_characterModel.StartIdlePopupAnimations();
			}
			CharacterScaler component = uiModelController.TutorialPopupAnchor.GetComponent<CharacterScaler>();
			if (component != null)
			{
				component.lookAtCamera = true;
			}
		}

		public void HideCharacter()
		{
			uiModelController.DeactivateModelScreen(UIModelController.ModelScreen.TutorialPopup);
			if (_characterModel != null)
			{
				_characterModel.gameObject.SetActive(false);
			}
			_characterHidden = true;
		}

		public void ShowCharacter()
		{
			if (_characterHidden)
			{
				uiModelController.ActivateModelScreen(UIModelController.ModelScreen.TutorialPopup);
				if (_characterModel != null)
				{
					_characterModel.gameObject.SetActive(true);
				}
				StartCoroutine(AnimateCharacterModelCoroutine());
				_characterHidden = false;
			}
		}

		private IEnumerator AnimateCharacterModelCoroutine()
		{
			yield return null;
			_characterModel.StartIdlePopupAnimations();
		}

		protected virtual void OnDisable()
		{
			if (_characterModel != null)
			{
				_characterModel.gameObject.SetActive(false);
			}
		}
	}
}
