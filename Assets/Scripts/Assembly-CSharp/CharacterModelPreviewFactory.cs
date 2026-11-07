using System;
using SYBO.Common.Utilities;
using SYBO.Subway.Info;
using UnityEngine;

public class CharacterModelPreviewFactory : MonoBehaviour
{
	[Serializable]
	public class ModelSetupObject
	{
		public AnimationClip ScrollbarPose;

		public AnimationClip ScrollbarHeadPose;

		public AnimationClip EventScreenPose;

		public AnimationClip NameEventScreenIdle;

		public GameObject model;

		public bool UseCustomEventScreenIdle;
	}

	[SerializeField]
	private GameObject characterModelPreviewPrefab;

	[SerializeField]
	private CharacterAssetFactory characterAssetFactory;

	[SerializeField]
	private AnimationClip facebookIdle;

	[SerializeField]
	private AnimationClip defaultEventScreenPose;

	[SerializeField]
	private AnimationClip defaultEnventScreenIdle;

	[SerializeField]
	private Shader eventScreenShader;

	[SerializeField]
	private Shader eventScreenOutfitShader;

	[Range(0f, 1f)]
	[SerializeField]
	private float _nameEventScreenCharacterFadeFactor = 0.6f;

	[Range(0f, 1f)]
	[SerializeField]
	private float _nameEventScreenShadowFadeFactor = 0.7f;

	private SkinnedMeshRenderer headMesh;

	private static CharacterModelPreviewFactory instance;

	public static CharacterModelPreviewFactory Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(CharacterModelPreviewFactory)) as CharacterModelPreviewFactory);
		}
	}

	public void GetCharacterModelPreview(string name, int version, Action<GameObject> onSuccess, bool asynchronous)
	{
		BuildCharacter(name, version, false, false, true, (ModelSetupObject go) =>
		{
			if (go != null)
			{
				if (go.ScrollbarPose != null)
				{
					Animation componentInChildren = go.model.GetComponentInChildren<Animation>();
					if (componentInChildren.GetClip(go.ScrollbarPose.name) == null)
					{
						componentInChildren.AddClip(go.ScrollbarPose, go.ScrollbarPose.name);
						componentInChildren.Play(go.ScrollbarPose.name);
						componentInChildren[go.ScrollbarPose.name].speed = 0f;
					}
					if (go.ScrollbarHeadPose != null)
					{
						componentInChildren.AddClip(go.ScrollbarHeadPose, go.ScrollbarHeadPose.name);
						componentInChildren[go.ScrollbarHeadPose.name].blendMode = AnimationBlendMode.Blend;
						componentInChildren[go.ScrollbarHeadPose.name].weight = 1f;
						componentInChildren[go.ScrollbarHeadPose.name].layer = 10;
						componentInChildren[go.ScrollbarHeadPose.name].enabled = true;
						componentInChildren[go.ScrollbarHeadPose.name].speed = 0f;
						componentInChildren[go.ScrollbarHeadPose.name].normalizedTime = 0f;
					}
					componentInChildren.Sample();
				}
				else
				{
					SDebug.LogError("could not find Scrollbar Pose for '" + name + "'.");
				}
			}
			onSuccess(go.model);
		}, asynchronous);
	}

	public void GetEventPopupModel(string name, int version, bool useSpecialAsset, Action<GameObject> onSuccess)
	{
		BuildCharacter(name, version, useSpecialAsset, true, false, (ModelSetupObject MSO) =>
		{
			if (MSO != null)
			{
				Animation componentInChildren = MSO.model.GetComponentInChildren<Animation>();
				if (MSO.EventScreenPose != null)
				{
					componentInChildren.AddClip(MSO.EventScreenPose, "eventClip");
					componentInChildren.Play("eventClip");
					componentInChildren["eventClip"].speed = 0f;
				}
				else
				{
					componentInChildren.AddClip(defaultEventScreenPose, "defaultEventClip");
					componentInChildren.Play("defaultEventClip");
					componentInChildren["defaultEventClip"].speed = 0f;
				}
			}
			onSuccess(MSO.model);
		});
	}

	public void GetCharacterWordEventScreenModel(string name, int version, Action<CharacterScreenController> onSuccess)
	{
		BuildCharacter(name, version, false, true, false, (ModelSetupObject mso) =>
		{
			if (mso != null)
			{
				CharacterScreenController characterScreenController = null;
				characterScreenController = mso.model.AddComponent<CharacterScreenController>();
				AnimationClip idleClip = ((!(mso.NameEventScreenIdle != null)) ? defaultEnventScreenIdle : mso.NameEventScreenIdle);
				characterScreenController.Initialize(idleClip, eventScreenShader, eventScreenOutfitShader, _nameEventScreenCharacterFadeFactor, _nameEventScreenShadowFadeFactor);
				onSuccess(characterScreenController);
			}
			else
			{
				onSuccess(null);
			}
		});
	}

	public void GetSimpleScreenModel(Characters.CharacterId charId, int themeIndex, Action<CharacterScreenController> onSuccess)
	{
		BuildCharacter(charId.ToString(), themeIndex, false, true, false, (ModelSetupObject modelSetupObject) =>
		{
			if (modelSetupObject != null)
			{
				CharacterScreenController characterScreenController = null;
				characterScreenController = modelSetupObject.model.AddComponent<CharacterScreenController>();
				AnimationClip nameEventScreenIdle = defaultEnventScreenIdle;
				if (modelSetupObject.UseCustomEventScreenIdle)
				{
					nameEventScreenIdle = modelSetupObject.NameEventScreenIdle;
				}
				characterScreenController.Initialize(nameEventScreenIdle, eventScreenShader, eventScreenOutfitShader, _nameEventScreenCharacterFadeFactor, _nameEventScreenShadowFadeFactor);
				onSuccess(characterScreenController);
			}
			else
			{
				onSuccess(null);
			}
		});
	}

	public void BuildCharacter(string name, int version, bool useSpecialAsset, bool useAnimatedEyes, bool forceLOD0, Action<ModelSetupObject> onSucess, bool asynchronous = false)
	{
		CharacterAssetFactory.GetAssetPrefab(name, asynchronous, (CharacterAssetsBundle modelBundle) =>
		{
			if (modelBundle != null)
			{
				GameObject go = UnityEngine.Object.Instantiate(characterModelPreviewPrefab);
				modelBundle.transform.parent = go.transform;
				CharacterRig rig = go.GetComponentInChildren<CharacterRig>();
				rig.Model.sharedMesh = modelBundle.MainMesh;
				if (modelBundle.HeadMesh != null)
				{
					rig.HeadModel.sharedMesh = modelBundle.HeadMesh;
				}
				else
				{
					rig.HeadModel.enabled = false;
				}
				rig.OutfitModel.enabled = false;
				rig.RocketSkates.enabled = false;
				rig.MeshSuperSneaker.enabled = false;
				rig.DoubleJumpers.enabled = false;
				rig.CharacterCustomization.Customize(name, version, rig.Model, rig.OutfitModel, rig.HeadModel, rig, useSpecialAsset, false, forceLOD0, asynchronous, () =>
				{
					if (useAnimatedEyes)
					{
						AvatarEyeAnimation componentInChildren = go.GetComponentInChildren<AvatarEyeAnimation>();
						if (componentInChildren != null)
						{
							GameObject gameObject = new GameObject("FocusPoint");
							HierarchyUtility.ReparentByHierarchyPathType(gameObject.transform, HierarchyPathType.Dynamic);
							componentInChildren.Initialize(gameObject, rig.HeadJoint, rig.EyeSwitch, rig.EyeFocusOrigin, rig.EyeFocusTarget, rig.BlendShapeController, rig.HeadModel, modelBundle.EyeMaterialID);
						}
					}
					ModelSetupObject obj = new ModelSetupObject
					{
						model = go,
						ScrollbarPose = modelBundle.ScrollbarPose,
						ScrollbarHeadPose = modelBundle.ScrollbarHeadPose,
						EventScreenPose = modelBundle.EventScreenPose,
						NameEventScreenIdle = modelBundle.NameEventScreenIdle,
						UseCustomEventScreenIdle = modelBundle.UseCustomEventScreenIdle
					};
					onSucess(obj);
				});
			}
			else
			{
				SDebug.LogError("could not find preview character model for '" + name + "'.");
				onSucess(null);
			}
		});
	}

	private void OnDestroy()
	{
		instance = null;
	}
}
