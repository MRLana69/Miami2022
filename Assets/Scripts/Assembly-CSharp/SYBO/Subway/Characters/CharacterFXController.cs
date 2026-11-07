using System.Collections;
using Kiloo.Common.Utility;
using SYBO.Components.Utilities;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using SYBO.Subway.Visuals;
using UnityEngine;

namespace SYBO.Subway.Characters
{
	public class CharacterFXController : MonoBehaviour
	{
		[Header("Dizzy Stars")]
		[SerializeField]
		private Transform _dizzyStarsPrefab;

		[SerializeField]
		private float _starHeightOffset = 10f;

		[SerializeField]
		private float _dizzyDuration = 2f;

		[Header("Bush Leaves")]
		[SerializeField]
		private Transform _bushLeavesPrefab;

		[SerializeField]
		private float _leavesDuration;

		[Header("Jump Roll")]
		[SerializeField]
		private Transform _jumpRollPrefab;

		[SerializeField]
		private float _jmupRollDespawnAddedDelay = 0.2f;

		[Header("Grind Spark")]
		[SerializeField]
		private Transform _grindSparkPrefab;

		private Game _game;

		private Hoverboard _hoverboard;

		private bool _dizzyStarsHeightOffsetSet;

		private Transform _dizzyStarsInstance;

		private IEnumerator _dizzyStarsDespawn;

		private void Awake()
		{
			_game = Game.Instance;
			_hoverboard = Hoverboard.Instance;
			Character.Instance.OnStumble += Character_OnStumble;
			Character.Instance.OnLanding += Character_OnLanding;
			Character.Instance.OnCriticalHit += Character_OnCriticalHit;
			PoolManager.GetPrefabPool(_dizzyStarsPrefab).preloadAmount = 1;
			PoolManager.GetPrefabPool(_bushLeavesPrefab).preloadAmount = 1;
			PoolManager.GetPrefabPool(_jumpRollPrefab).preloadAmount = 1;
			PoolManager.GetPrefabPool(_grindSparkPrefab).preloadAmount = 1;
		}

		private void Character_OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName, bool safeStumble)
		{
			if (stumbleType == Character.StumbleType.Bush)
			{
				HandleBushStumble();
			}
			else if (horizontalHit != Character.StumbleHorizontalHit.Center || verticalHit != Character.StumbleVerticalHit.Middle)
			{
				HandleSideStumble();
			}
		}

		private void HandleBushStumble()
		{
			Transform transform = PoolManager.Spawn(_bushLeavesPrefab);
			transform.parent = base.transform;
			transform.localPosition = Vector3.zero;
			CoroutineUtil.StartCoroutine(DespawnCollisionEffect(transform, _leavesDuration, true));
		}

		private void HandleSideStumble()
		{
			if (!_game.isInStumbleDeathSequence)
			{
				if (_dizzyStarsDespawn != null)
				{
					CoroutineUtil.StopCoroutine(_dizzyStarsDespawn);
					PoolManager.Despawn(_dizzyStarsInstance);
				}
				_dizzyStarsInstance = PoolManager.Spawn(_dizzyStarsPrefab);
				_dizzyStarsInstance.parent = base.transform;
				if (!_dizzyStarsHeightOffsetSet)
				{
					_dizzyStarsInstance.GetComponent<DizzyStarsHeightControl>().HeightOffset = _starHeightOffset;
					_dizzyStarsHeightOffsetSet = true;
				}
				_dizzyStarsDespawn = DespawnCollisionEffect(_dizzyStarsInstance, _dizzyDuration, true, true);
				CoroutineUtil.StartCoroutine(_dizzyStarsDespawn);
			}
		}

		private IEnumerator DespawnCollisionEffect(Transform instance, float delay, bool activeDuringHoverboard = false, bool despawnOnDeath = false)
		{
			while (delay > 0f && (!despawnOnDeath || !_game.isInStumbleDeathSequence) && _game.IsInGame.Value && (activeDuringHoverboard || !_hoverboard.isActive) && GameStateMachine.Instance.IsCurrentState<RunningState>())
			{
				delay -= Time.deltaTime;
				yield return null;
			}
			_dizzyStarsDespawn = null;
			PoolManager.Despawn(instance);
		}

		private void Character_OnCriticalHit(Character.CriticalHitType type)
		{
			if (_dizzyStarsDespawn != null)
			{
				CoroutineUtil.StopCoroutine(_dizzyStarsDespawn);
				_dizzyStarsDespawn = null;
				PoolManager.Despawn(_dizzyStarsInstance);
			}
		}

		private void Character_OnLanding(Character character)
		{
			if (character.IsRolling && character.CurrentRunPosition == Character.RunPositions.ground && !_hoverboard.isActive)
			{
				HandleJumpRoll(character);
			}
			else if (!character.IsRolling && _hoverboard.isActive)
			{
				HandleGrind(character);
			}
		}

		private void HandleJumpRoll(Character character)
		{
			Transform transform = PoolManager.Spawn(_jumpRollPrefab);
			transform.parent = base.transform;
			transform.localPosition = Vector3.zero;
			CoroutineUtil.StartCoroutine(DespawnJumpRollEffect(transform, character));
		}

		private IEnumerator DespawnJumpRollEffect(Transform instance, Character character)
		{
			while (character.IsRolling && character.CurrentRunPosition == Character.RunPositions.ground && _game.IsInGame.Value && !_hoverboard.isActive && GameStateMachine.Instance.IsCurrentState<RunningState>())
			{
				yield return null;
			}
			yield return new WaitForSeconds(_jmupRollDespawnAddedDelay);
			PoolManager.Despawn(instance);
		}

		private void HandleGrind(Character character)
		{
			if (InGrind(character))
			{
				Transform transform = PoolManager.Spawn(_grindSparkPrefab);
				transform.parent = base.transform;
				transform.localPosition = Vector3.zero;
				CoroutineUtil.StartCoroutine(DespawnGrindEffect(transform, character));
			}
		}

		private bool InGrind(Character character)
		{
			return character.CurrentRunPosition == Character.RunPositions.train || (character.CurrentRunPosition == Character.RunPositions.movingTrain && _game.IsInGame.Value);
		}

		private IEnumerator DespawnGrindEffect(Transform instance, Character character)
		{
			while (InGrind(character) && !character.IsRolling)
			{
				yield return null;
			}
			PoolManager.Despawn(instance);
		}
	}
}
