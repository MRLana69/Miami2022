using UnityEngine;

public class RandomizeAnimatorStartAndSpeedState : StateMachineBehaviour
{
	[SerializeField]
	private string _stateNameToPlay;

	[SerializeField]
	private float _speedMin;

	[SerializeField]
	private float _speedMax = 1f;

	private float _startTime = -1f;

	private bool _hadFirstRun;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (_startTime < 0f && !_hadFirstRun)
		{
			_startTime = Random.value;
			animator.speed = Random.value * (_speedMax - _speedMin) + _speedMin;
		}
		animator.Play(_stateNameToPlay, 0, _startTime);
		_startTime = 0f;
	}
}
