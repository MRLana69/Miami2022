using System;
using UnityEngine;

public class AnimationTransformConstraint : MonoBehaviour
{
	[Serializable]
	private class ConstraintRange
	{
		public int StartKeyframe;

		public int EndKeyframe;
	}

	[SerializeField]
	private string _animationName = string.Empty;

	[SerializeField]
	private ConstraintRange _range;

	[SerializeField]
	private string _targetTransformName = string.Empty;

	private Animation _animation;

	private Transform _targetTransform;

	private void Start()
	{
		_animation = FindAnimationInParent(base.transform);
		Transform[] componentsInChildren = _animation.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name.Equals(_targetTransformName))
			{
				_targetTransform = transform;
				break;
			}
		}
		if (_targetTransform == null)
		{
			SDebug.LogError("Null target transform.", this);
			base.enabled = false;
		}
	}

	private Animation FindAnimationInParent(Transform current)
	{
		Animation component = current.GetComponent<Animation>();
		if (component != null)
		{
			return component;
		}
		if (current.parent != null)
		{
			return FindAnimationInParent(current.parent);
		}
		return null;
	}

	private void LateUpdate()
	{
		if (_animation.IsPlaying(_animationName))
		{
			int num = Mathf.RoundToInt(_animation[_animationName].time * _animation[_animationName].clip.frameRate);
			if (num > _range.StartKeyframe && num < _range.EndKeyframe)
			{
				_targetTransform.position = base.transform.position;
				_targetTransform.rotation = base.transform.rotation;
			}
		}
	}
}
