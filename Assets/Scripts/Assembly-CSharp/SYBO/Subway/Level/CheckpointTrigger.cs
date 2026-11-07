using System;
using SYBO.Subway.Characters;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[RequireComponent(typeof(CharacterCollisionTrigger))]
	public class CheckpointTrigger : MonoBehaviour
	{
		private CharacterCollisionTrigger _trigger;

		private void Awake()
		{
			_trigger = GetComponent<CharacterCollisionTrigger>();
			_trigger.OnCharacterEnter += Handler_OnCharacterEnter;
		}

		private void Handler_OnCharacterEnter(object sender, EventArgs e)
		{
			Character.Instance.SetLastCheckpointGamePositionZ(GamePositionRoot.Instance.GetGamePosition(base.transform).z);
		}
	}
}
