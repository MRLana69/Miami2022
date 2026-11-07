using JetBrains.Annotations;
using UnityEngine;

public class FriendCrewPokeHelper : MonoBehaviour
{
	[SerializeField]
	private GameObject _pokeButtonContainer;

	[SerializeField]
	private BoxCollider _pokeCollider;

	private Friend _friend;

	public void ActivatePoke(Friend friend)
	{
		_friend = friend;
		_pokeCollider.enabled = true;
		_pokeButtonContainer.SetActive(true);
	}

	public void DeactivatePoke()
	{
		_pokeCollider.enabled = false;
		_pokeButtonContainer.SetActive(false);
	}

	[UsedImplicitly]
	private void OnClick()
	{
		DeactivatePoke();
		Stat.PokeFriend.IncrementValue();
		if (!Application.isEditor)
		{
			SocialManager.instance.Poke(_friend);
		}
	}
}
