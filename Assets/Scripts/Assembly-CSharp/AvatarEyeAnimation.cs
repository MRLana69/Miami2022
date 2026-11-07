using SYBO.Common.Utilities;
using UnityEngine;

public class AvatarEyeAnimation : MonoBehaviour
{
	[SerializeField]
	private float blinkTime = 0.15f;

	[SerializeField]
	private float blinkWaitTimeMax = 5.7f;

	[SerializeField]
	private float blinkWaitTimeMin = 0.4f;

	[SerializeField]
	private float shiftEyeHorizontalLimit = 0.5f;

	[SerializeField]
	private float shiftEyeVerticalLimit = 0.5f;

	[SerializeField]
	private float shiftEyeHorizontalMultiplier = 0.1f;

	[SerializeField]
	private float shiftEyeVerticalMultiplier = 0.1f;

	[SerializeField]
	private Transform headboneTransform;

	[SerializeField]
	private GameObject testTargetPoint;

	[SerializeField]
	private Vector2 maxLookOffset = new Vector2(25f, 20f);

	[SerializeField]
	private GameObject targetPosition;

	[SerializeField]
	private Vector3 popupMenuFocusPointOffset = new Vector3(-40f, -80f, -200f);

	[SerializeField]
	private Vector3 characterMenuFocusPointOffset = new Vector3(40f, -65f, -200f);

	[SerializeField]
	private Vector3 boardMenuFocusPointOffset = new Vector3(40f, -90f, -200f);

	[SerializeField]
	private Vector3 duelsScreenFocusPointOffset = new Vector3(10f, -80f, -200f);

	[SerializeField]
	private float timeToMoveFocus = 0.04f;

	[SerializeField]
	private float timeBetweenFocusShiftMax = 1f;

	[SerializeField]
	private float timeBetweenFocusShiftMin = 0.1f;

	[SerializeField]
	private bool animate;

	private SkinnedMeshRenderer _headMesh;

	private Transform _eyeFocusOrigin;

	private Transform _eyeFocusTarget;

	private Transform _eyeSwitch;

	private Transform _blendshapeController;

	private GameObject _duelsScreensPrefab;

	private float blinkWaitEndTime;

	private float blinkEndTime;

	private float currentHorizontalLimit;

	private float currentVerticalLimit;

	private Vector3 toprunFocusPointOffset = new Vector3(-20f, -80f, -200f);

	private Vector3 originalPosition;

	private Vector3 originalRotation;

	private Vector3 newTargetPosition;

	private Vector3 lookToCamera;

	private Vector2 offsetVector = default(Vector2);

	private float endTimeToShift;

	private bool useFocuPoint = true;

	private bool shiftToFocusBlink;

	private int _eyeMaterialID;

	private bool IsEyeControlledByAnimation
	{
		get
		{
			return 0.5f < _eyeSwitch.localPosition.y;
		}
	}

	public void Initialize(GameObject eyeFocus, Transform headJoint, Transform eyeSwitch, Transform eyeFocusOrigin, Transform eyeFocusTarget, Transform blendshapeController, SkinnedMeshRenderer headMesh, int eyeMaterialID)
	{
		_headMesh = headMesh;
		_eyeSwitch = eyeSwitch;
		_eyeSwitch.localPosition = Vector3.zero;
		_eyeFocusOrigin = eyeFocusOrigin;
		_eyeFocusTarget = eyeFocusTarget;
		_blendshapeController = blendshapeController;
		_eyeMaterialID = eyeMaterialID;
		currentHorizontalLimit = shiftEyeHorizontalLimit;
		currentVerticalLimit = shiftEyeVerticalLimit;
		blinkWaitEndTime = Random.Range(blinkWaitTimeMin, blinkWaitTimeMax);
		blinkEndTime = blinkWaitEndTime + blinkTime;
		if (testTargetPoint != null)
		{
			targetPosition = testTargetPoint;
			targetPosition.transform.LookAt(base.transform.position);
		}
		else if (eyeFocus != null)
		{
			targetPosition = eyeFocus;
			headboneTransform = headJoint;
		}
		HierarchyUtility.ReparentByHierarchyPathType(eyeFocus.transform, HierarchyPathType.Dynamic);
	}

	public void DuelsScreenAnimateEyes(GameObject root, Vector3 offset)
	{
		duelsScreenFocusPointOffset = offset;
		_duelsScreensPrefab = root;
		AnimateEyes(UIModelController.ModelScreen.DuelResultPopup);
	}

	public void AnimateEyes(UIModelController.ModelScreen modelScreen)
	{
		animate = true;
		switch (modelScreen)
		{
		case UIModelController.ModelScreen.DuelResultPopup:
			targetPosition.transform.parent = _duelsScreensPrefab.transform;
			targetPosition.transform.localPosition = duelsScreenFocusPointOffset;
			targetPosition.transform.LookAt(base.transform.position);
			originalPosition = new Vector3(targetPosition.transform.localPosition.x, targetPosition.transform.localPosition.y, targetPosition.transform.localPosition.z);
			originalRotation = new Vector3(targetPosition.transform.localRotation.x, targetPosition.transform.localRotation.y, targetPosition.transform.localRotation.z);
			break;
		case UIModelController.ModelScreen.Popup:
			if (targetPosition != null)
			{
				HierarchyUtility.ReparentByHierarchyPathType(targetPosition.transform, HierarchyPathType.Root);
				targetPosition.transform.position = popupMenuFocusPointOffset;
				targetPosition.transform.LookAt(base.transform.position);
				originalPosition = new Vector3(targetPosition.transform.localPosition.x, targetPosition.transform.localPosition.y, targetPosition.transform.localPosition.z);
				originalRotation = new Vector3(targetPosition.transform.localRotation.x, targetPosition.transform.localRotation.y, targetPosition.transform.localRotation.z);
			}
			break;
		case UIModelController.ModelScreen.Character:
		case UIModelController.ModelScreen.CelebrationCharacterUnlock:
			if (targetPosition != null)
			{
				HierarchyUtility.ReparentByHierarchyPathType(targetPosition.transform, HierarchyPathType.Root);
				targetPosition.transform.position = characterMenuFocusPointOffset;
				targetPosition.transform.LookAt(base.transform.position);
				originalPosition = new Vector3(targetPosition.transform.localPosition.x, targetPosition.transform.localPosition.y, targetPosition.transform.localPosition.z);
				originalRotation = new Vector3(targetPosition.transform.localRotation.x, targetPosition.transform.localRotation.y, targetPosition.transform.localRotation.z);
			}
			break;
		case UIModelController.ModelScreen.Boards:
		case UIModelController.ModelScreen.CelebrationBoardUnlock:
		case UIModelController.ModelScreen.LimitedBoardPopup:
			if (targetPosition != null)
			{
				HierarchyUtility.ReparentByHierarchyPathType(targetPosition.transform, HierarchyPathType.Root);
				targetPosition.transform.position = boardMenuFocusPointOffset;
				targetPosition.transform.LookAt(base.transform.position);
				originalPosition = new Vector3(targetPosition.transform.localPosition.x, targetPosition.transform.localPosition.y, targetPosition.transform.localPosition.z);
				originalRotation = new Vector3(targetPosition.transform.localRotation.x, targetPosition.transform.localRotation.y, targetPosition.transform.localRotation.z);
			}
			break;
		case UIModelController.ModelScreen.CelebrationTopRun:
			if (targetPosition != null)
			{
				HierarchyUtility.ReparentByHierarchyPathType(targetPosition.transform, HierarchyPathType.Root);
				targetPosition.transform.position = toprunFocusPointOffset;
				targetPosition.transform.LookAt(base.transform.position);
				originalPosition = new Vector3(targetPosition.transform.localPosition.x, targetPosition.transform.localPosition.y, targetPosition.transform.localPosition.z);
				originalRotation = new Vector3(targetPosition.transform.localRotation.x, targetPosition.transform.localRotation.y, targetPosition.transform.localRotation.z);
			}
			break;
		}
	}

	public void StopAnimatingEyes()
	{
		animate = false;
		if (_headMesh.sharedMesh != null && 0 < _headMesh.sharedMesh.blendShapeCount)
		{
			_headMesh.SetBlendShapeWeight(0, 0f);
		}
	}

	private void ForceBlink()
	{
		blinkWaitEndTime = Time.time;
		blinkEndTime = blinkWaitEndTime + blinkTime;
	}

	private void UpdateEyeBlinking()
	{
		if (!(_eyeSwitch != null) || !(_headMesh.sharedMesh != null))
		{
			return;
		}
		if (!IsEyeControlledByAnimation)
		{
			if (!(blinkWaitTimeMax >= blinkWaitTimeMin) || !(Time.time >= blinkWaitEndTime))
			{
				return;
			}
			if (Time.time < blinkEndTime)
			{
				if (_headMesh.GetBlendShapeWeight(0) < 1f && 0 < _headMesh.sharedMesh.blendShapeCount)
				{
					_headMesh.SetBlendShapeWeight(0, 100f);
				}
				return;
			}
			if (0 < _headMesh.sharedMesh.blendShapeCount)
			{
				_headMesh.SetBlendShapeWeight(0, 0f);
			}
			blinkWaitEndTime = Time.time + Random.Range(blinkWaitTimeMin, blinkWaitTimeMax);
			blinkEndTime = blinkWaitEndTime + blinkTime;
		}
		else if (0 < _headMesh.sharedMesh.blendShapeCount)
		{
			_headMesh.SetBlendShapeWeight(0, AvatarAnimations.NormalizeBlendShape(_blendshapeController.localPosition.x) * 100f);
		}
	}

	private Vector3 GetFocusPoint()
	{
		targetPosition.transform.localPosition = originalPosition + new Vector3(Random.Range(0f - maxLookOffset.x, maxLookOffset.x), Random.Range(0f - maxLookOffset.y, maxLookOffset.y), 0f);
		targetPosition.transform.localRotation = Quaternion.Euler(originalRotation);
		return targetPosition.transform.position;
	}

	private void ShiftFocusPoint(Vector3 focusPoint)
	{
		Vector3 fromPosition = targetPosition.transform.position;
		StartCoroutine(pTween.To(timeToMoveFocus, (float t) =>
		{
			targetPosition.transform.position = Vector3.Lerp(fromPosition, focusPoint, t);
		}));
	}

	private void ResetEyes()
	{
		offsetVector = new Vector2(0f, 0f);
		if (_headMesh.sharedMesh != null && _headMesh.sharedMesh.subMeshCount >= 2)
		{
			_headMesh.materials[_eyeMaterialID].SetTextureOffset("_MainTex", offsetVector);
		}
	}

	private void UpdateGetNewFocusPoint()
	{
		if (Time.time > endTimeToShift)
		{
			ShiftFocusPoint(GetFocusPoint());
			endTimeToShift = Time.time + Random.Range(timeBetweenFocusShiftMin, timeBetweenFocusShiftMax);
		}
	}

	private void UpdateAnimatedEyes()
	{
		if (_eyeSwitch != null && _headMesh.sharedMesh != null)
		{
			AnimateEyes();
		}
	}

	private void AnimateEyes()
	{
		if (useFocuPoint && !IsEyeControlledByAnimation)
		{
			UpdateGetNewFocusPoint();
		}
		if (IsEyeControlledByAnimation)
		{
			lookToCamera = _eyeFocusOrigin.position - _eyeFocusTarget.position;
		}
		else
		{
			lookToCamera = headboneTransform.position - targetPosition.transform.position;
		}
		lookToCamera.Normalize();
		Vector3 vector = headboneTransform.InverseTransformDirection(lookToCamera);
		offsetVector.x = Mathf.Atan2(vector.x, vector.z);
		offsetVector.y = Mathf.Atan2(vector.y, vector.z);
		if (!IsEyeControlledByAnimation)
		{
			if (offsetVector.x < 0f - currentHorizontalLimit)
			{
				useFocuPoint = false;
			}
			else if (offsetVector.x > currentHorizontalLimit)
			{
				useFocuPoint = false;
			}
			else if (offsetVector.y < 0f - currentVerticalLimit)
			{
				useFocuPoint = false;
			}
			else if (offsetVector.y > currentVerticalLimit)
			{
				useFocuPoint = false;
			}
			else
			{
				useFocuPoint = true;
			}
		}
		if (useFocuPoint)
		{
			if (!shiftToFocusBlink)
			{
				currentHorizontalLimit = shiftEyeHorizontalLimit;
				currentVerticalLimit = shiftEyeVerticalLimit;
				ForceBlink();
				shiftToFocusBlink = true;
			}
			offsetVector.x = Mathf.Clamp(offsetVector.x, 0f - currentHorizontalLimit, currentHorizontalLimit) * shiftEyeHorizontalMultiplier;
			offsetVector.y = Mathf.Clamp(offsetVector.y, 0f - currentVerticalLimit, currentVerticalLimit) * shiftEyeVerticalMultiplier;
			if (_headMesh.sharedMesh != null && _headMesh.sharedMesh.subMeshCount >= 2)
			{
				_headMesh.materials[_eyeMaterialID].SetTextureOffset("_MainTex", offsetVector);
			}
		}
		else if (shiftToFocusBlink)
		{
			ResetEyes();
			currentHorizontalLimit = shiftEyeHorizontalLimit - 0.1f;
			currentVerticalLimit = shiftEyeVerticalLimit - 0.1f;
			ForceBlink();
			shiftToFocusBlink = false;
		}
	}

	private void Update()
	{
		if (animate)
		{
			UpdateEyeBlinking();
			UpdateAnimatedEyes();
		}
	}
}
