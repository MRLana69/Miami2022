using System;
using UnityEngine;

namespace SYBO.Subway.Visuals
{
	public class TrainLightTrigger : CharacterPassTrigger
	{
		private MaterialAnimationController _materialAnimationController;

		private Animation _animation;

		private void Awake()
		{
			_materialAnimationController = GetComponent<MaterialAnimationController>();
			_animation = GetComponent<Animation>();
			_materialAnimationController.ColorMultiplier = 1f;
			_animation.enabled = false;
			base.Triggered += TrainLightTrigger_Triggered;
		}

		private void OnEnable()
		{
			_materialAnimationController.ColorMultiplier = 1f;
			_animation.Stop();
			_animation.Rewind();
			_animation.enabled = false;
		}

		private void TrainLightTrigger_Triggered(object sender, EventArgs e)
		{
			_animation.enabled = true;
			_animation.Play();
		}
	}
}
