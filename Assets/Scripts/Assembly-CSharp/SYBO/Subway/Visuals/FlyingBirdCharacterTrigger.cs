using System;
using SYBO.Common.Visuals;
using SYBO.Subway.Level;
using UnityEngine;

namespace SYBO.Subway.Visuals
{
	public class FlyingBirdCharacterTrigger : CharacterCollisionTrigger
	{
		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private MeshMorpher _meshMorpher;

		[SerializeField]
		private Animation _animation;

		private bool _setDirection = true;

		private void Awake()
		{
			base.OnCharacterEnter += FlyingBirdCharacterTrigger_OnCharacterEnter;
		}

		private void OnEnable()
		{
			_setDirection = true;
			_renderer.enabled = false;
			_meshMorpher.Animate = false;
			_meshMorpher.Reset();
			_animation.Stop();
			_animation.Rewind();
			_animation.transform.localPosition = Vector3.zero;
			_animation.transform.localRotation = Quaternion.identity;
		}

		private void Update()
		{
			if (_setDirection)
			{
				base.transform.rotation = Quaternion.identity;
				_setDirection = false;
			}
		}

		private void FlyingBirdCharacterTrigger_OnCharacterEnter(object sender, EventArgs e)
		{
			_renderer.enabled = true;
			_meshMorpher.Animate = true;
			_animation.Play();
		}
	}
}
