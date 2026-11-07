using UnityEngine;

public class CharacterRig : MonoBehaviour
{
	[SerializeField]
	private string modelName;

	[SerializeField]
	private string outfitModelName;

	[SerializeField]
	private string headModelName;

	[SerializeField]
	private string meshSuperSneakerName;

	[SerializeField]
	private string doubleJumpersName;

	[SerializeField]
	private string rocketSkatesName;

	[SerializeField]
	private string spineTransformName;

	[SerializeField]
	private string rightShoulderTransformName;

	[SerializeField]
	private string attachPoint1Name;

	[SerializeField]
	private string attachPoint2Name;

	[SerializeField]
	private string attachPoint3Name;

	[SerializeField]
	private string attachPoint4Name;

	[SerializeField]
	private string attachPoint5Name;

	[SerializeField]
	private string attachPoint6Name;

	[SerializeField]
	private string attachPoint7Name;

	[SerializeField]
	private string attachPoint8Name;

	[SerializeField]
	private string attachPoint9Name;

	[SerializeField]
	private string hipName;

	[SerializeField]
	private string leftFootName;

	[SerializeField]
	private string rightFootName;

	[SerializeField]
	private string headJointName;

	[SerializeField]
	private string rightHandName;

	[SerializeField]
	private string eyeFocusPointOriginName;

	[SerializeField]
	private string eyeFocusPointTargetName;

	[SerializeField]
	private string eyeSwitchName;

	[SerializeField]
	private string blendShapeControllerName;

	[SerializeField]
	private Material powerupMaterial;

	[OptionalField]
	private Animation characterAnimation;

	[OptionalField]
	private AvatarAnimations avatarAnimations;

	[OptionalField]
	private CharacterCustomization characterCustomization;

	[OptionalField]
	private SkinnedMeshRenderer model;

	[OptionalField]
	private SkinnedMeshRenderer outfitModel;

	[OptionalField]
	private SkinnedMeshRenderer headModel;

	[OptionalField]
	private SkinnedMeshRenderer meshSuperSneaker;

	[OptionalField]
	private SkinnedMeshRenderer doubleJumpers;

	[OptionalField]
	private SkinnedMeshRenderer rocketSkates;

	[OptionalField]
	private Transform spineTransform;

	[OptionalField]
	private Transform rightShoulderTransform;

	[OptionalField]
	private Transform attachPoint1;

	[OptionalField]
	private Transform attachPoint2;

	[OptionalField]
	private Transform attachPoint3;

	[OptionalField]
	private Transform attachPoint4;

	[OptionalField]
	private Transform attachPoint5;

	[OptionalField]
	private Transform attachPoint6;

	[OptionalField]
	private Transform attachPoint7;

	[OptionalField]
	private Transform attachPoint8;

	[OptionalField]
	private Transform attachPoint9;

	[OptionalField]
	private Transform hip;

	[OptionalField]
	private Transform leftFoot;

	[OptionalField]
	private Transform rightFoot;

	[OptionalField]
	private Transform headJoint;

	[OptionalField]
	private Transform rightHand;

	[OptionalField]
	private Transform eyeFocusTarget;

	[OptionalField]
	private Transform eyeFocusOrigin;

	[OptionalField]
	private Transform eyeSwitch;

	[OptionalField]
	private Transform blendShapeController;

	public Animation CharacterAnimation
	{
		get
		{
			return characterAnimation;
		}
	}

	public AvatarAnimations AvatarAnimations
	{
		get
		{
			return avatarAnimations;
		}
	}

	public CharacterCustomization CharacterCustomization
	{
		get
		{
			return characterCustomization;
		}
	}

	public SkinnedMeshRenderer Model
	{
		get
		{
			return model;
		}
	}

	public SkinnedMeshRenderer OutfitModel
	{
		get
		{
			return outfitModel;
		}
	}

	public SkinnedMeshRenderer HeadModel
	{
		get
		{
			return headModel;
		}
	}

	public SkinnedMeshRenderer MeshSuperSneaker
	{
		get
		{
			return meshSuperSneaker;
		}
	}

	public SkinnedMeshRenderer DoubleJumpers
	{
		get
		{
			return doubleJumpers;
		}
	}

	public SkinnedMeshRenderer RocketSkates
	{
		get
		{
			return rocketSkates;
		}
	}

	public Transform SpineTransform
	{
		get
		{
			return spineTransform;
		}
	}

	public Transform ShoulderTransform
	{
		get
		{
			return rightShoulderTransform;
		}
	}

	public Transform AttachPoint1
	{
		get
		{
			return attachPoint1;
		}
	}

	public Transform AttachPoint2
	{
		get
		{
			return attachPoint2;
		}
	}

	public Transform AttachPoint3
	{
		get
		{
			return attachPoint3;
		}
	}

	public Transform AttachPoint4
	{
		get
		{
			return attachPoint4;
		}
	}

	public Transform AttachPoint5
	{
		get
		{
			return attachPoint5;
		}
	}

	public Transform AttachPoint6
	{
		get
		{
			return attachPoint6;
		}
	}

	public Transform AttachPoint7
	{
		get
		{
			return attachPoint7;
		}
	}

	public Transform AttachPoint8
	{
		get
		{
			return attachPoint8;
		}
	}

	public Transform AttachPoint9
	{
		get
		{
			return attachPoint9;
		}
	}

	public Transform Hip
	{
		get
		{
			return hip;
		}
	}

	public Transform LeftFoot
	{
		get
		{
			return leftFoot;
		}
	}

	public Transform RightFoot
	{
		get
		{
			return rightFoot;
		}
	}

	public Transform HeadJoint
	{
		get
		{
			return headJoint;
		}
	}

	public Transform RightHand
	{
		get
		{
			return rightHand;
		}
	}

	public Transform EyeFocusTarget
	{
		get
		{
			return eyeFocusTarget;
		}
	}

	public Transform EyeFocusOrigin
	{
		get
		{
			return eyeFocusOrigin;
		}
	}

	public Transform EyeSwitch
	{
		get
		{
			return eyeSwitch;
		}
	}

	public Transform BlendShapeController
	{
		get
		{
			return blendShapeController;
		}
	}

	private void Awake()
	{
		characterAnimation = GetComponentInChildren<Animation>();
		characterCustomization = GetComponent<CharacterCustomization>();
		AnimationEventBroadcaster componentInChildren = base.gameObject.GetComponentInChildren<AnimationEventBroadcaster>();
		if (componentInChildren == null)
		{
			characterAnimation.gameObject.AddComponent<AnimationEventBroadcaster>();
		}
		SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		SkinnedMeshRenderer[] array = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			string text = skinnedMeshRenderer.name;
			if (text == modelName)
			{
				model = skinnedMeshRenderer;
			}
			else if (text == outfitModelName)
			{
				outfitModel = skinnedMeshRenderer;
			}
			else if (text == headModelName)
			{
				headModel = skinnedMeshRenderer;
			}
			else if (text == meshSuperSneakerName)
			{
				meshSuperSneaker = skinnedMeshRenderer;
				meshSuperSneaker.material = powerupMaterial;
			}
			else if (text == doubleJumpersName)
			{
				doubleJumpers = skinnedMeshRenderer;
				doubleJumpers.material = powerupMaterial;
			}
			else if (text == rocketSkatesName)
			{
				rocketSkates = skinnedMeshRenderer;
				rocketSkates.material = powerupMaterial;
			}
		}
		Transform[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array2 = componentsInChildren2;
		foreach (Transform transform in array2)
		{
			string text2 = transform.name;
			if (transform.name == spineTransformName)
			{
				spineTransform = transform;
			}
			else if (text2 == rightShoulderTransformName)
			{
				rightShoulderTransform = transform;
			}
			else if (text2 == attachPoint1Name)
			{
				attachPoint1 = transform;
			}
			else if (text2 == attachPoint2Name)
			{
				attachPoint2 = transform;
			}
			else if (text2 == attachPoint3Name)
			{
				attachPoint3 = transform;
			}
			else if (text2 == attachPoint4Name)
			{
				attachPoint4 = transform;
			}
			else if (text2 == attachPoint5Name)
			{
				attachPoint5 = transform;
			}
			else if (text2 == attachPoint6Name)
			{
				attachPoint6 = transform;
			}
			else if (text2 == attachPoint7Name)
			{
				attachPoint7 = transform;
			}
			else if (text2 == attachPoint8Name)
			{
				attachPoint8 = transform;
			}
			else if (text2 == attachPoint9Name)
			{
				attachPoint9 = transform;
			}
			else if (text2 == hipName)
			{
				hip = transform;
			}
			else if (text2 == leftFootName)
			{
				leftFoot = transform;
			}
			else if (text2 == rightFootName)
			{
				rightFoot = transform;
			}
			else if (text2 == headJointName)
			{
				headJoint = transform;
			}
			else if (text2 == rightHandName)
			{
				rightHand = transform;
			}
			else if (text2 == eyeFocusPointOriginName)
			{
				eyeFocusOrigin = transform;
			}
			else if (text2 == eyeFocusPointTargetName)
			{
				eyeFocusTarget = transform;
			}
			else if (text2 == eyeSwitchName)
			{
				eyeSwitch = transform;
			}
			else if (text2 == blendShapeControllerName)
			{
				blendShapeController = transform;
			}
		}
		avatarAnimations = base.gameObject.GetComponent<AvatarAnimations>();
		avatarAnimations.Target = characterAnimation;
		avatarAnimations.Head = headModel;
		avatarAnimations.BlendshapeController = blendShapeController;
	}
}
