using System;
using System.Collections;
using Data.Managers;
using Data.Save;
using Info;
using SYBO.Common;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Coins;
using SYBO.Subway.Pickups;
using UnityEngine;

public class LongMagnet : MonoBehaviour
{
	private Game game;

	private CharacterController characterController;

	private CharacterRendering characterRendering;

	private Character character;

	private CollisionNotifier coinMagnetCollider;

	public float pullSpeed = 200f;

	private VariableBool longMagnetSuction = new VariableBool();

	public bool ShouldBeActive
	{
		get
		{
			return (Hoverboard.Instance.isActive && (BoardManager.IsBoardAbilityActive(PlayerSave.currentBoardId, Hoverboard.BoardAbility.Glider) || PlayerSave.currentBoardId == Boards.BoardId.gliderboard)) || SuperSneakers.Instance.isActive;
		}
	}

	public VariableBool LongMagnetSuction
	{
		get
		{
			return longMagnetSuction;
		}
	}

	public void Start()
	{
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		coinMagnetCollider = character.CoinMagnetLongCollider;
		characterController = character.CharacterController;
		game = Game.Instance;
		Variable<bool> isInGame = game.IsInGame;
		isInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame.OnChange, (Variable<bool>.OnChangeDelegate)((bool value) =>
		{
			if (value)
			{
				longMagnetSuction.Clear();
			}
		}));
		VariableBool variableBool = longMagnetSuction;
		variableBool.OnChange = (VariableBool.OnChangeDelegate)Delegate.Combine(variableBool.OnChange, (VariableBool.OnChangeDelegate)((bool value) =>
		{
			if (value)
			{
				coinMagnetCollider.TriggerEnter += CoinHit;
				coinMagnetCollider.GetComponent<Collider>().enabled = true;
			}
			else
			{
				coinMagnetCollider.GetComponent<Collider>().enabled = false;
				coinMagnetCollider.TriggerEnter -= CoinHit;
			}
		}));
	}

	public void CoinHit(Collider collider)
	{
		if (game.IsInJetpackMode)
		{
			return;
		}
		Coin component = collider.GetComponent<Coin>();
		if (component != null)
		{
			component.MoveToSuperSneakers(pullSpeed);
			return;
		}
		PickupSpawnPoint component2 = collider.GetComponent<PickupSpawnPoint>();
		if (component2 != null && component2.SpawnPointState == SpawnPointState.Spawned)
		{
			StartCoroutine(PullPickupSpawnPoint(component2));
		}
	}

	private IEnumerator PullPickupSpawnPoint(PickupSpawnPoint pickupSpawnPoint)
	{
		Vector3 position = pickupSpawnPoint.SpawnedPickupIcon.transform.position;
		float distance = (position - characterController.transform.position).magnitude;
		yield return StartCoroutine(pTween.To(distance / (pullSpeed * game.CurrentSpeedNormalized), (float t) =>
		{
			if (pickupSpawnPoint.SpawnedPickupIcon != null)
			{
				pickupSpawnPoint.SpawnedPickupIcon.transform.position = Vector3.Lerp(position, characterRendering.CharacterModel.MeshSuperSneaker.transform.position, t * t);
			}
		}));
		if (pickupSpawnPoint.SpawnPointState == SpawnPointState.Spawned)
		{
			pickupSpawnPoint.TriggerCollisionWithCharacter(Character.Instance.CharacterPickupParticleSystem);
		}
	}

	public void Activate()
	{
		longMagnetSuction.Add(this);
	}

	public void Deactivate()
	{
		longMagnetSuction.Remove(this);
	}
}
