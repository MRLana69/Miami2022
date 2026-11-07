using System.Collections.Generic;
using Data.Save;
using SYBO.Components.Utilities;
using SYBO.Subway.Info;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterModel : MonoBehaviour
{
	[FormerlySerializedAs("rigPrefab")]
	[SerializeField]
	private GameObject _rigPrefab;

	[SerializeField]
	private GameObject ShadowPrefab;

	[SerializeField]
	private GameObject rigController;

	[SerializeField]
	private GameObject focusPoint;

	[SerializeField]
	private GameObject charaterAssetFactoryPrefab;

	[SerializeField]
	private TopRunIdleAnimation topRunIdleAnimation;

	[FormerlySerializedAs("ShadowPrefab")]
	[SerializeField]
	private GameObject _shadowPrefab;

	private GameObject currentHoverboard;

	[FormerlySerializedAs("rigController")]
	[SerializeField]
	private GameObject _rigController;

	[FormerlySerializedAs("focusPoint")]
	[SerializeField]
	private GameObject _focusPoint;

	[FormerlySerializedAs("sprayCanPrefab")]
	[SerializeField]
	private GameObject _sprayCanPrefab;

	[FormerlySerializedAs("coinMagnetPrefab")]
	[SerializeField]
	private GameObject _coinMagnetPrefab;

	[FormerlySerializedAs("jetpackPrefab")]
	[SerializeField]
	private GameObject _jetpackPrefab;

	[FormerlySerializedAs("pogostickPrefab")]
	[SerializeField]
	private GameObject _pogostickPrefab;

	private Renderer currentHoverboadBasemodel;

	private List<Renderer> currentCustomBoardModelsRenderes = new List<Renderer>();

	[FormerlySerializedAs("topRunIdleAnimation")]
	[SerializeField]
	private TopRunIdleAnimation _topRunIdleAnimation;

	[OptionalField]
	private GameObject _sprayCanModel;

	[OptionalField]
	private MeshRenderer _meshCoinMagnet;

	[OptionalField]
	private MeshRenderer _meshJetpack;

	[OptionalField]
	private MeshRenderer _meshPogostick;

	[OptionalField]
	private Transform _jetpackCloudPositionL;

	[OptionalField]
	private Transform _jetpackCloudPositionR;

	[OptionalField]
	private Transform[] _jetpackThrusters;

	[OptionalField]
	private Transform _attachPoint1;

	[OptionalField]
	private Transform _attachPoint2;

	[OptionalField]
	private Transform _attachPoint3;

	[OptionalField]
	private Transform _attachPoint4;

	[OptionalField]
	private Transform _attachPoint5;

	[OptionalField]
	private Transform _attachPoint6;

	[OptionalField]
	private Transform _attachPoint7;

	[OptionalField]
	private Transform _attachPoint8;

	[OptionalField]
	private Transform _attachPoint9;

	[OptionalField]
	private SkinnedMeshRenderer _model;

	[OptionalField]
	private SkinnedMeshRenderer _outfitModel;

	[OptionalField]
	private SkinnedMeshRenderer _headModel;

	[OptionalField]
	private SkinnedMeshRenderer _meshSuperSneaker;

	[OptionalField]
	private SkinnedMeshRenderer _doubleJumpers;

	[OptionalField]
	private SkinnedMeshRenderer _rocketSkates;

	[OptionalField]
	private Animation _characterAnimation;

	[OptionalField]
	private Transform _spineTransform;

	[OptionalField]
	private Transform _shoulderTransform;

	[OptionalField]
	private Transform _hip;

	[OptionalField]
	private Transform _leftFoot;

	[OptionalField]
	private Transform _rightFoot;

	[OptionalField]
	private Transform _headJoint;

	[OptionalField]
	private Transform _rightHand;

	private CharacterAssetsBundle _currentModelBundle;

	private MeshRenderer _meshBlobShadow;

	private GameObject _currentHoverboard;

	private List<GameObject> _currentCustomBoardModels = new List<GameObject>();

	private List<Renderer> _currentCustomBoardModelsRenderes = new List<Renderer>();

	private Dictionary<string, SkinnedMeshRenderer> _modelLookupTable;

	private string[] _modelNames;

	private CharacterAssetFactory _charaterAssetFactory;

	private AvatarEyeAnimation _animEye;

	private AvatarAnimations _anim;

	private CharacterRig _characterRig;

	private bool _isVisibleShadow;

	public Renderer CurrentHoverboadBasemodel
	{
		get
		{
			return currentHoverboadBasemodel;
		}
		set
		{
			currentHoverboadBasemodel = value;
		}
	}

	public List<Renderer> CurrentHoverboardModelsRenderes
	{
		get
		{
			return currentCustomBoardModelsRenderes;
		}
		set
		{
			currentCustomBoardModelsRenderes = value;
		}
	}

	public bool IsVisibleShadow
	{
		get
		{
			return _meshBlobShadow.enabled;
		}
		set
		{
			_meshBlobShadow.enabled = value;
		}
	}

	public CharacterRig CharacterRig
	{
		get
		{
			return _characterRig;
		}
	}

	public MeshRenderer MeshBlobShadow
	{
		get
		{
			return _meshBlobShadow;
		}
	}

	public GameObject CurrentHoverboard
	{
		get
		{
			return _currentHoverboard;
		}
	}

	public List<GameObject> CurrentCustomBoardModels
	{
		get
		{
			return _currentCustomBoardModels;
		}
	}

	public List<Renderer> CurrentCustomBoardModelsRenderes
	{
		get
		{
			return _currentCustomBoardModelsRenderes;
		}
	}

	public GameObject SprayCanModel
	{
		get
		{
			return GetSprayCanModel();
		}
	}

	public MeshRenderer MeshCoinMagnet
	{
		get
		{
			return GetCoinMagnet();
		}
	}

	public MeshRenderer MeshJetpack
	{
		get
		{
			return GetJetpack();
		}
	}

	public MeshRenderer MeshPogostick
	{
		get
		{
			return GetPogostick();
		}
	}

	public Transform JetpackCloudPositionL
	{
		get
		{
			return _jetpackCloudPositionL;
		}
	}

	public Transform JetpackCloudPositionR
	{
		get
		{
			return _jetpackCloudPositionR;
		}
	}

	public Transform[] JetpackThrusters
	{
		get
		{
			return _jetpackThrusters;
		}
	}

	public Transform AttachPoint1
	{
		get
		{
			return _attachPoint1;
		}
	}

	public Transform AttachPoint2
	{
		get
		{
			return _attachPoint2;
		}
	}

	public Transform AttachPoint3
	{
		get
		{
			return _attachPoint3;
		}
	}

	public Transform AttachPoint4
	{
		get
		{
			return _attachPoint4;
		}
	}

	public Transform AttachPoint5
	{
		get
		{
			return _attachPoint5;
		}
	}

	public Transform AttachPoint6
	{
		get
		{
			return _attachPoint6;
		}
	}

	public Transform AttachPoint7
	{
		get
		{
			return _attachPoint7;
		}
	}

	public Transform AttachPoint8
	{
		get
		{
			return _attachPoint8;
		}
	}

	public Transform AttachPoint9
	{
		get
		{
			return _attachPoint9;
		}
	}

	public SkinnedMeshRenderer Model
	{
		get
		{
			return _model;
		}
	}

	public SkinnedMeshRenderer OutfitModel
	{
		get
		{
			return _outfitModel;
		}
	}

	public SkinnedMeshRenderer HeadModel
	{
		get
		{
			return _headModel;
		}
	}

	public SkinnedMeshRenderer MeshSuperSneaker
	{
		get
		{
			return _meshSuperSneaker;
		}
	}

	public SkinnedMeshRenderer DoubleJumpers
	{
		get
		{
			return _doubleJumpers;
		}
	}

	public SkinnedMeshRenderer RocketSkates
	{
		get
		{
			return _rocketSkates;
		}
	}

	public Animation CharacterAnimation
	{
		get
		{
			return _characterAnimation;
		}
	}

	public Transform SpineTransform
	{
		get
		{
			return _spineTransform;
		}
	}

	public Transform ShoulderTransform
	{
		get
		{
			return _shoulderTransform;
		}
	}

	public Transform Hip
	{
		get
		{
			return _hip;
		}
	}

	public Transform LeftFoot
	{
		get
		{
			return _leftFoot;
		}
	}

	public Transform RightFoot
	{
		get
		{
			return _rightFoot;
		}
	}

	public Transform HeadJoint
	{
		get
		{
			return _headJoint;
		}
	}

	public Transform RightHand
	{
		get
		{
			return _rightHand;
		}
	}

	public TopRunIdleAnimation TopRunIdleAnimation
	{
		get
		{
			return _topRunIdleAnimation;
		}
	}

	private void Awake()
	{
		BuildRig();
		GameObject gameObject = Object.Instantiate(_shadowPrefab);
		gameObject.transform.parent = base.transform;
		_meshBlobShadow = gameObject.GetComponent<MeshRenderer>();
		IsVisibleShadow = true;
		gameObject.GetComponent<PointConstraint>().master = Hip;
		GetSprayCanModel();
		base.gameObject.GetComponentInChildren<KeyframeBehaviour>().TargetObjects = _sprayCanModel.GetComponentsInChildren<ParticleSystem>();
		_sprayCanModel.SetActive(false);
	}

	private void BuildRig()
	{
		GameObject gameObject = Object.Instantiate(_rigPrefab);
		gameObject.transform.parent = _rigController.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		_characterRig = gameObject.GetComponent<CharacterRig>();
		_characterAnimation = _characterRig.CharacterAnimation;
		_attachPoint1 = _characterRig.AttachPoint1;
		_attachPoint2 = _characterRig.AttachPoint2;
		_attachPoint3 = _characterRig.AttachPoint3;
		_attachPoint4 = _characterRig.AttachPoint4;
		_attachPoint5 = _characterRig.AttachPoint5;
		_attachPoint6 = _characterRig.AttachPoint6;
		_attachPoint7 = _characterRig.AttachPoint7;
		_attachPoint8 = _characterRig.AttachPoint8;
		_attachPoint9 = _characterRig.AttachPoint9;
		_model = _characterRig.Model;
		_outfitModel = _characterRig.OutfitModel;
		_outfitModel.enabled = false;
		_headModel = _characterRig.HeadModel;
		_headModel.enabled = false;
		_meshSuperSneaker = _characterRig.MeshSuperSneaker;
		_meshSuperSneaker.enabled = false;
		_doubleJumpers = _characterRig.DoubleJumpers;
		_doubleJumpers.enabled = false;
		_rocketSkates = _characterRig.RocketSkates;
		_rocketSkates.enabled = false;
		_characterAnimation = _characterRig.CharacterAnimation;
		_spineTransform = _characterRig.SpineTransform;
		_shoulderTransform = _characterRig.ShoulderTransform;
		_hip = _characterRig.Hip;
		_leftFoot = _characterRig.LeftFoot;
		_rightFoot = _characterRig.RightFoot;
		_headJoint = _characterRig.HeadJoint;
		_rightHand = _characterRig.RightHand;
	}

	public void InitializeCharacter()
	{
		Characters.CharacterId currentCharacterId = PlayerSave.currentCharacterId;
		if (!Characters.characterData.ContainsKey(currentCharacterId))
		{
			SDebug.LogError(string.Format("This character ({0}) does not exist on this branch. Stop the scene. In the menu bar press \"Tools -> Data files -> Delete PlayerInfo\" and try play the scene again.", currentCharacterId));
		}
		string modelName = Characters.characterData[currentCharacterId].modelName;
		int currentOutfitIndex = PlayerSave.currentOutfitIndex;
		CheckForCharactersToBeInSyncWithStaticData();
		SetCharacterModel(modelName, currentOutfitIndex, false, true);
		_outfitModel = _characterRig.OutfitModel;
		Hoverboard.Instance.intialize();
		base.gameObject.GetComponentInChildren<RunnerAnimPlayer>().Initialize();
		base.gameObject.GetComponentInChildren<KeyframeBehaviour>().Initialize();
	}

	public void RecreateCharacter()
	{
		string modelName = Characters.characterData[PlayerSave.currentCharacterId].modelName;
		int currentOutfitIndex = PlayerSave.currentOutfitIndex;
		ChangeCharacterModel(modelName, currentOutfitIndex, false, true);
		_outfitModel = _characterRig.OutfitModel;
		base.gameObject.GetComponentInChildren<RunnerAnimPlayer>().Initialize();
		base.gameObject.GetComponentInChildren<KeyframeBehaviour>().Initialize();
	}

	public void ChangeCharacterModel(string name, int outfitIndex, bool useSpecialAsset, bool deleteSpecialAsset)
	{
		StopIdleAnimations();
		if (_currentModelBundle != null)
		{
			Object.Destroy(_currentModelBundle.gameObject);
		}
		SetCharacterModel(name, outfitIndex, useSpecialAsset, deleteSpecialAsset);
	}

	public void SetCharacterModel(string name, int outfitIndex, bool useSpecialAsset, bool deleteSpecialAsset)
	{
		CharacterAssetFactory.GetAssetPrefab(name, false, (CharacterAssetsBundle bundle) =>
		{
			SetupModel(bundle, name, outfitIndex, useSpecialAsset, deleteSpecialAsset);
		});
	}

	private void SetupModel(CharacterAssetsBundle characterAssetBundle, string name, int outfitIndex, bool useSpecialAsset, bool deleteSpecialAsset)
	{
		if (!(characterAssetBundle == null))
		{
			_currentModelBundle = characterAssetBundle;
			_currentModelBundle.gameObject.name = name;
			_currentModelBundle.transform.parent = base.transform;
			_model.sharedMesh = characterAssetBundle.MainMesh;
			_anim = _characterRig.AvatarAnimations;
			_headModel.enabled = false;
			_headModel.sharedMesh = null;
			_headModel.sharedMesh = characterAssetBundle.HeadMesh;
			_headModel.GetComponent<Renderer>().enabled = true;
			_anim.Initialize(characterAssetBundle, outfitIndex);
			_animEye = characterAssetBundle._avatarEyeAnimation;
			if (_animEye != null)
			{
				_animEye.Initialize(_focusPoint, _headJoint, _characterRig.EyeSwitch, _characterRig.EyeFocusOrigin, _characterRig.EyeFocusTarget, _characterRig.BlendShapeController, _headModel, characterAssetBundle.EyeMaterialID);
			}
			_characterRig.CharacterCustomization.Customize(name, outfitIndex, _model, _outfitModel, _headModel, _characterRig, useSpecialAsset, deleteSpecialAsset, false, false, null);
		}
	}

	private GameObject GetSprayCanModel()
	{
		if (_sprayCanModel == null)
		{
			_sprayCanModel = Object.Instantiate(_sprayCanPrefab);
			_sprayCanModel.transform.parent = _rightHand;
			_sprayCanModel.transform.localPosition = Vector3.zero;
			_sprayCanModel.transform.localRotation = Quaternion.identity;
			_sprayCanModel.transform.localScale = Vector3.one;
		}
		return _sprayCanModel;
	}

	private MeshRenderer GetCoinMagnet()
	{
		if (_meshCoinMagnet == null)
		{
			GameObject gameObject = Object.Instantiate(_coinMagnetPrefab);
			CoinMagnet.Instance.Initialize();
			gameObject.transform.parent = _rightHand;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			_meshCoinMagnet = gameObject.GetComponentInChildren<MeshRenderer>();
			_meshCoinMagnet.enabled = false;
		}
		return _meshCoinMagnet;
	}

	private MeshRenderer GetJetpack()
	{
		if (_meshJetpack == null)
		{
			GameObject gameObject = Object.Instantiate(_jetpackPrefab);
			gameObject.transform.parent = _spineTransform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			JetpackModel componentInChildren = gameObject.GetComponentInChildren<JetpackModel>();
			_meshJetpack = componentInChildren.Mesh;
			_meshJetpack.enabled = false;
			_jetpackCloudPositionL = componentInChildren.JetpackCloudPositionL;
			_jetpackCloudPositionR = componentInChildren.JetpackCloudPositionR;
			_jetpackThrusters = componentInChildren.JetpackThrusters;
		}
		return _meshJetpack;
	}

	private MeshRenderer GetPogostick()
	{
		if (_meshPogostick == null)
		{
			GameObject gameObject = Object.Instantiate(_pogostickPrefab);
			gameObject.transform.parent = _attachPoint1;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			_meshPogostick = gameObject.GetComponent<MeshRenderer>();
			_meshPogostick.enabled = false;
		}
		return _meshPogostick;
	}

	public GameObject GetHoverboardGameObject()
	{
		if (_attachPoint1 != null)
		{
			return _attachPoint1.gameObject;
		}
		return null;
	}

	public HoverboardBundle AddEquippedHoverboard()
	{
		HoverboardBundle hoverboardBundle = SingletonMonoBehaviour<HoverboardModelFactory>.Instance.RequestEquippedHoverboardInstance();
		SetupBoard(hoverboardBundle, false);
		return hoverboardBundle;
	}

	public GameObject SetNewHoverboard(GameObject root, GameObject newBoard, string boardName)
	{
		if (_currentHoverboard != null)
		{
			Object.Destroy(_currentHoverboard);
		}
		if (_currentCustomBoardModels.Count > 0)
		{
			foreach (GameObject currentCustomBoardModel in _currentCustomBoardModels)
			{
				Object.Destroy(currentCustomBoardModel);
			}
			_currentCustomBoardModels.Clear();
			_currentCustomBoardModelsRenderes.Clear();
		}
		GameObject gameObject = Object.Instantiate(newBoard);
		gameObject.transform.parent = root.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.name = boardName;
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.gameObject.layer = Layers.Instance._3DGUI;
		}
		_currentHoverboard = gameObject;
		return gameObject;
	}

	public HoverboardBundle AddRandomHoverboard()
	{
		HoverboardBundle hoverboardBundle = SingletonMonoBehaviour<HoverboardModelFactory>.Instance.RequestRandomHoverboardInstance();
		SetupBoard(hoverboardBundle, false);
		return hoverboardBundle;
	}

	public HoverboardBundle AddHoverboard(string name, bool isMenu)
	{
		HoverboardBundle hoverboardBundle = SingletonMonoBehaviour<HoverboardModelFactory>.Instance.RequestHoverboardInstance(name);
		SetupBoard(hoverboardBundle, isMenu);
		return hoverboardBundle;
	}

	public Renderer AddCustomHoverboardModel(GameObject modelPrefab, bool isMenu, Transform parent)
	{
		Renderer result = null;
		if (modelPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(modelPrefab);
			result = gameObject.GetComponentInChildren<Renderer>();
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			_currentCustomBoardModels.Add(gameObject);
			_currentCustomBoardModelsRenderes.Add(gameObject.GetComponentInChildren<Renderer>());
			if (isMenu)
			{
				gameObject.layer = Layers.Instance._3DGUI;
				Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
				Renderer[] array = componentsInChildren;
				foreach (Renderer renderer in array)
				{
					renderer.gameObject.layer = Layers.Instance._3DGUI;
				}
			}
		}
		return result;
	}

	public void RemoveCurrentHoverboard()
	{
		currentCustomBoardModelsRenderes.Clear();
		if (currentHoverboard != null)
		{
			Object.Destroy(currentHoverboard);
			currentHoverboard = null;
		}
	}

	private void SetupBoard(HoverboardBundle board, bool isMenu)
	{
		RemoveCurrentHoverboard();
		board.Charactermodel = this;
		board.AddModel(isMenu);
		currentHoverboard = board.gameObject;
		board.transform.parent = _attachPoint1;
		board.transform.localPosition = Vector3.zero;
		board.transform.localRotation = Quaternion.identity;
		board.transform.localScale = Vector3.one;
	}

	public void AddCustomHoverboardVanityMenuFX(GameObject modelPrefab, Transform parent)
	{
		if (modelPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(modelPrefab);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			_currentCustomBoardModels.Add(gameObject);
		}
	}

	public void DeleteBoardModels()
	{
		if (_currentCustomBoardModels.Count <= 0)
		{
			return;
		}
		foreach (GameObject currentCustomBoardModel in _currentCustomBoardModels)
		{
			Object.Destroy(currentCustomBoardModel);
		}
		_currentCustomBoardModels.Clear();
		_currentCustomBoardModelsRenderes.Clear();
	}

	public void HideAllPowerups()
	{
		if (_currentHoverboard != null)
		{
			_currentHoverboard.SetActive(false);
		}
		if (_meshJetpack != null)
		{
			_meshJetpack.enabled = false;
		}
		if (_meshSuperSneaker != null)
		{
			_meshSuperSneaker.enabled = false;
		}
		if (_meshCoinMagnet != null)
		{
			_meshCoinMagnet.enabled = false;
		}
		if (_sprayCanModel != null)
		{
			_sprayCanModel.SetActive(false);
		}
		ParticleSystem[] componentsInChildren = _sprayCanModel.GetComponentsInChildren<ParticleSystem>(true);
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = false;
		}
	}

	public void HideBlobShadow()
	{
		_meshBlobShadow.enabled = false;
	}

	public void ShowCurrentHoverboard()
	{
		if (currentHoverboard != null)
		{
			currentHoverboard.SetActive(true);
		}
	}

	public void HideCurrentHoverboard()
	{
		if (currentHoverboard != null)
		{
			currentHoverboard.SetActive(false);
		}
	}

	public void StartEyeAnimations(UIModelController.ModelScreen modelScreen)
	{
		if (!(_model == null) && _animEye != null)
		{
			_animEye.AnimateEyes(modelScreen);
		}
	}

	public void StopEyeAnimations()
	{
		if (!(_model == null) && _animEye != null)
		{
			_animEye.StopAnimatingEyes();
		}
	}

	public void StartIdleAnimations(UIModelController.ModelScreen modelScreen)
	{
		if (!(_model == null))
		{
			if (_anim != null)
			{
				_anim.StartIdleAnimations();
			}
			if (_animEye != null)
			{
				_animEye.AnimateEyes(modelScreen);
			}
		}
	}

	public void StartIdlePopupAnimations()
	{
		if (!(_model == null))
		{
			if (_anim != null)
			{
				_anim.StartIdlePopupAnimations();
			}
			if (_animEye != null)
			{
				_animEye.AnimateEyes(UIModelController.ModelScreen.Popup);
			}
		}
	}

	public void StartCustomAnimation(UIModelController.ModelScreen modelScreen, bool useUnscaledTime, params AnimationClip[] clips)
	{
		if (!(_model == null))
		{
			if (_anim != null)
			{
				_anim.StartCustomAnimations(clips, useUnscaledTime);
			}
			if (_animEye != null)
			{
				_animEye.AnimateEyes(modelScreen);
			}
		}
	}

	public void StopIdleAnimations()
	{
		if (!(_model == null))
		{
			if (_anim != null)
			{
				_anim.StopIdleAnimations();
			}
			if (_animEye != null)
			{
				_animEye.StopAnimatingEyes();
			}
		}
	}

	public void PauseIdleAnimations()
	{
		if (!(_model == null) && _anim != null)
		{
			_anim.PauseIdleAnimations();
		}
	}

	public void ResumeIdleAnimations()
	{
		if (!(_model == null) && _anim != null)
		{
			_anim.ResumeIdleAnimations();
		}
	}

	private void CheckForCharactersToBeInSyncWithStaticData()
	{
	}

	private void OnDestroy()
	{
		Object.DestroyImmediate(focusPoint);
		Object.Destroy(_focusPoint);
		Object.DestroyImmediate(focusPoint);
	}
}
