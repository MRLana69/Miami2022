using System;
using System.Collections.Generic;
using Info;
using UI;
using UI.ModelCamera;
using UI.RenderTexture;
using UnityEngine;

[Serializable]
public class BoardRenderTextureSetup : RenderTextureSetupBase
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
	public class EventModelBoard : EventModelInfo
	{
		public Boards.BoardId boardID;

		public AnimationClip animationClip;

		public override RenderTextureObject GetRenderTextureObject(bool isWideContainer = false)
		{
			RenderTextureObject renderTextureObject = base.GetRenderTextureObject(isWideContainer);
			renderTextureObject.animationClip = animationClip;
			return renderTextureObject;
		}

		public override GameObject GetPrefab()
		{
			string boardModelName = Boards.boardData[boardID].boardModelName;
			string path = "Prefabs/HoverboardAssets/" + boardModelName;
			return (GameObject)Resources.Load(path);
		}
	}

	private bool _showBoardAbilities;

	private HoverboardBundle _boardBundle;

	private GameObject _boardParent;

	public void Setup(bool showBoardAbilities, UITexture texture, List<RenderTextureObject> renderTextureObjects, CameraSettings overrideCameraSettings = null, bool copySettings = true)
	{
		_showBoardAbilities = showBoardAbilities;
		Setup(texture, renderTextureObjects, overrideCameraSettings, copySettings);
	}

	protected override void OnSetup()
	{
		base.OnSetup();
		for (int i = 0; i < _objectInstances.Count; i++)
		{
			_boardBundle = _objectInstances[i].GetComponent<HoverboardBundle>();
			if (!(_boardBundle != null))
			{
				continue;
			}
			_boardParent = _objectInstances[i].AddChild(_boardBundle.GetPrefab());
			if (!_showBoardAbilities || _boardBundle.CustomSets == null || _boardBundle.CustomSets.Length <= 0)
			{
				continue;
			}
			HoverboardBundle.CustomSet[] customSets = _boardBundle.CustomSets;
			foreach (HoverboardBundle.CustomSet customSet in customSets)
			{
				if (customSet.modelPrefab != null)
				{
					_boardParent.AddChild(customSet.modelPrefab);
				}
				if (customSet.ExtraFXPrefab != null)
				{
					_boardParent.AddChild(customSet.ExtraMenuFXPrefab);
				}
				if (customSet.VanityFxPrefab != null)
				{
					_boardParent.AddChild(customSet.VanityMenuFxPrefab);
				}
			}
		}
	}
}
