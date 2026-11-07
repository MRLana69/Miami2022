using UnityEngine;

public class CharacterAnimationControl : MonoBehaviour
{
	[SerializeField]
	private AnimationClip _runAnimation;

	[SerializeField]
	private AnimationClip[] _alternateRunAnimations;

	private Animation _animationComponent;

	private AnimationClip _currentAnimation;

	public AnimationClip[] AlternateRunAnimations
	{
		get
		{
			return _alternateRunAnimations;
		}
	}

	public void Play()
	{
		CharacterModel componentInParent = base.gameObject.GetComponentInParent<CharacterModel>();
		_currentAnimation = _runAnimation;
		string value = componentInParent.CharacterRig.CharacterCustomization.OutfitInfo.HighscoreAnimationName.Value;
		if (!string.IsNullOrEmpty(value))
		{
			AnimationClip[] alternateRunAnimations = _alternateRunAnimations;
			foreach (AnimationClip animationClip in alternateRunAnimations)
			{
				if (animationClip.name.Equals(value))
				{
					_currentAnimation = animationClip;
					break;
				}
			}
		}
		if (_animationComponent == null)
		{
			_animationComponent = base.gameObject.GetComponentInChildren<Animation>();
		}
		if (!_animationComponent.GetClip(_currentAnimation.name))
		{
			_animationComponent.AddClip(_currentAnimation, _currentAnimation.name);
		}
		_animationComponent.Play(_currentAnimation.name);
	}

	public void Speed(float speed)
	{
		_animationComponent[_currentAnimation.name].speed = speed;
	}
}
