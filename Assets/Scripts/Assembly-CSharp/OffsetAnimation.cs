using UnityEngine;

public class OffsetAnimation : MonoBehaviour
{
	private Animation _anim;

	private float _time;

	private void Start()
	{
		if (_anim == null)
		{
			_anim = base.gameObject.GetComponent<Animation>();
			_time = Random.value * _anim.clip.length;
			_anim[_anim.clip.name].time = _time;
		}
	}
}
