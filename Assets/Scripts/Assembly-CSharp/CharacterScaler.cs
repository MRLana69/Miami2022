using UI.Screens;
using UnityEngine;
using Utilities;

public class CharacterScaler : MonoBehaviour
{
	public enum ScaleAnchorType
	{
		CharacterAnchor = 0,
		GameOverAnchor = 1,
		TutorialPupupAnchor = 2,
		CelebrationPopupAnchor = 3,
		LimitedBoardBundlePopupAnchor = 4,
		BirthdayAnchor = 5,
		BoardPromotionPopupAnchor = 6,
		LimitedCharacterBundlePopup = 7
	}

	[SerializeField]
	public bool lookAtCamera;

	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private ScaleAnchorType _anchorType;

	private float _posX = 90f;

	private float _posY = 225f;

	private float _scaleMultiplierForRotation = 56f;

	private float _scaleFactor = 1f;

	private float _scaleDelta = 450f;

	private float _gameOverScreenOffset;

	private const float STANDARD_ASPECT_RATIO = 1.5f;

	private UIRoot _root;

	private void Start()
	{
		GameOverScreen.onBottomElementsSetup += GameOverScreenChanged;
		if (_camera == null)
		{
			_camera = NGUITools.FindInParents<Camera>(base.gameObject);
		}
		_root = UIScreenController.Instance.root;
		SetScreenRelatedSettings();
		if (_root == null)
		{
		}
		ScaleCharacter();
		PositionCharacter();
		RotateCharacter();
	}

	private void OnDestroy()
	{
		GameOverScreen.onBottomElementsSetup -= GameOverScreenChanged;
	}

	private void GameOverScreenChanged(float offset)
	{
		if (_anchorType == ScaleAnchorType.GameOverAnchor)
		{
			_gameOverScreenOffset = offset;
			SetScreenRelatedSettings();
			ScaleCharacter();
			PositionCharacter();
			RotateCharacter();
		}
	}

	private void Update()
	{
		if (lookAtCamera)
		{
			RotateCharacter();
			lookAtCamera = false;
		}
	}

	private void SetScreenRelatedSettings()
	{
		if (_anchorType == ScaleAnchorType.CharacterAnchor)
		{
			_posX = -75f;
			_posY = -45f;
			_scaleMultiplierForRotation = 56f;
			_scaleDelta = 450f;
		}
		else if (_anchorType == ScaleAnchorType.GameOverAnchor)
		{
			_posX = -95f;
			_posY = (float)(_root.manualHeight / 2) - 280f - _gameOverScreenOffset;
			_posY -= Mathf.Clamp(50f * (Utils.GetAspectRatio() - 1.5f), 0f, float.PositiveInfinity);
			if (SafeArea.IsRestricted)
			{
				_posY -= (float)SafeArea.OFFSET_TOP / 2f;
			}
			_scaleMultiplierForRotation = 32f;
			_scaleDelta = 450f;
		}
		else if (_anchorType == ScaleAnchorType.TutorialPupupAnchor)
		{
			_posX = 92f;
			_posY = 40f;
			_scaleMultiplierForRotation = 0f;
			_scaleDelta = 380f;
		}
		else if (_anchorType == ScaleAnchorType.CelebrationPopupAnchor)
		{
			_posX = 0f;
			_posY = 0f;
			_scaleMultiplierForRotation = 0f;
			_scaleDelta = _root.manualHeight;
		}
		else if (_anchorType == ScaleAnchorType.LimitedBoardBundlePopupAnchor)
		{
			_posX = -100f;
			_posY = 45f;
			_scaleMultiplierForRotation = 56f;
			_scaleDelta = -520f;
		}
		else if (_anchorType == ScaleAnchorType.BirthdayAnchor)
		{
			_posX = 30f;
			_posY = -35f;
			_scaleDelta = -380f;
		}
		else if (_anchorType == ScaleAnchorType.LimitedCharacterBundlePopup)
		{
			_posX = -60f;
			_posY = 25f;
			_scaleDelta = -380f;
			_scaleMultiplierForRotation = 532f;
		}
	}

	private void ScaleCharacter()
	{
		float num = _root.manualHeight;
		Transform transform = base.transform;
		Vector3 localPosition = transform.localPosition;
		localPosition.z = ((_scaleDelta == 0f) ? 0f : (num - _scaleDelta));
		transform.transform.localPosition = localPosition;
	}

	private void RotateCharacter()
	{
		if (_camera != null)
		{
			Vector3 zero = Vector3.zero;
			float num = (float)_root.manualHeight - 480f;
			float num2 = ((_scaleMultiplierForRotation == 0f) ? 0f : (num / _scaleMultiplierForRotation + 5f));
			if (_anchorType == ScaleAnchorType.GameOverAnchor)
			{
				num2 *= Utils.GetAspectRatio() - 1.5f + 1f;
				num2 = Mathf.Clamp(num2, 5f, 25f);
			}
			else if (_anchorType == ScaleAnchorType.LimitedCharacterBundlePopup)
			{
				num2 = 20f;
			}
			zero.x = 0f - num2;
			zero.y = 0f;
			zero.z = 0f;
			base.gameObject.transform.localRotation = Quaternion.Euler(zero);
		}
	}

	private void PositionCharacter()
	{
		float posX = _posX;
		float posY = _posY;
		Transform transform = base.transform;
		Vector3 localPosition = transform.localPosition;
		localPosition.x = posX;
		localPosition.y = posY;
		transform.localPosition = localPosition;
		transform.localScale *= _scaleFactor;
	}
}
