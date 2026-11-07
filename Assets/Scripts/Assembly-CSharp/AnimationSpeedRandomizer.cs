using UnityEngine;

public class AnimationSpeedRandomizer : MonoBehaviour
{
	[SerializeField]
	private Animation _animation;

	[SerializeField]
	private float _minAnimationSpeed = 1f;

	[SerializeField]
	private float _maxAnimationSpeed = 1f;

	private void OnEnable()
	{
		if (_animation != null)
		{
			_animation[_animation.clip.name].speed = Random.Range(_minAnimationSpeed, _maxAnimationSpeed);
		}
	}
}
