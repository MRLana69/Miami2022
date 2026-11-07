using System;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(CharacterCollisionTrigger))]
	public class RouteTransitionTrigger : MonoBehaviour, IRouteTransitionTrigger
	{
		private CharacterCollisionTrigger _characterTrigger;

		public Route TargetRoute { get; set; }

		private void Awake()
		{
			_characterTrigger = GetComponent<CharacterCollisionTrigger>();
			_characterTrigger.OnCharacterEnter += CharacterTrigger_OnCharacterEnter;
		}

		private void CharacterTrigger_OnCharacterEnter(object sender, EventArgs args)
		{
			SingletonMonoBehaviour<RouteManager>.Instance.StartRouteTransition(TargetRoute);
		}
	}
}
