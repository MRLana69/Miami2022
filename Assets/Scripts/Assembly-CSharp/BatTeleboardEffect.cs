using System.Collections;
using Kiloo.Common.Utility;
using SYBO.Subway;
using SYBO.Subway.Characters;
using UnityEngine;

public class BatTeleboardEffect : ATeleboardEffect
{
	[SerializeField]
	private Animation _animation;

	[SerializeField]
	private string _leftAnimationName = string.Empty;

	[SerializeField]
	private string _rightAnimationName = string.Empty;

	[SerializeField]
	private Transform _endPoof;

	[SerializeField]
	private Vector3 _offset = Vector3.zero;

	[SerializeField]
	private float _reappearDelay = 0.2f;

	private bool _overrideEnd;

	private Vector3 pos = Vector3.zero;

	private bool _correctEndYPos;

	public override void StartTeleBoardEffect()
	{
		_overrideEnd = false;
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
		pos = Character.Instance.TransformPosition;
		string animation = ((Character.Instance.TrackMovement >= 0) ? _rightAnimationName : _leftAnimationName);
		_animation.Stop();
		_animation.Play(animation);
		_animation[animation].speed = _animation[animation].length / (20f / Game.Instance.CurrentSpeed);
		Character.Instance.CharacterModel.gameObject.SetActive(false);
		_correctEndYPos = true;
		CoroutineUtil.StopCoroutine(ResetCharacterTeleportFX());
		CoroutineUtil.StartCoroutine(ResetCharacterTeleportFX());
	}

	private void Update()
	{
		pos.z = Character.Instance.TransformPosition.z;
		base.transform.position = pos + _offset;
	}

	private void LateUpdate()
	{
		if (_correctEndYPos)
		{
			Vector3 position = _endPoof.position;
			position.y = Character.Instance.CharacterModel.transform.position.y + _offset.y;
			_endPoof.position = position;
		}
	}

	public override void ResetTeleboardEffect()
	{
		_overrideEnd = true;
	}

	private IEnumerator ResetCharacterTeleportFX()
	{
		while (!_overrideEnd && _animation != null && _animation.isPlaying)
		{
			yield return null;
		}
		if (!_overrideEnd)
		{
			yield return new WaitForSeconds(_reappearDelay);
		}
		_correctEndYPos = false;
		Character.Instance.CharacterModel.gameObject.SetActive(true);
		if (!Game.Instance.IsInJetpackMode && !Game.Instance.IsInPogostickMode)
		{
			if (Character.Instance.IsGrounded.Value)
			{
				CharacterRendering.Instance.OnRun();
			}
			else
			{
				CharacterRendering.Instance.Character_OnHangtime();
			}
			if (base.OnTeleboardEffectEnd != null)
			{
				base.OnTeleboardEffectEnd();
			}
		}
	}
}
