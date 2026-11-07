using UnityEngine;

public class Challenger
{
	public enum ChallengerTypes
	{
		topRunPlayer = 0,
		friend = 1,
		debugFriend = 2
	}

	public int Score;

	public Texture2D ProfilePicture;

	public bool IsBothTopRunAndFacebookFriend;

	public ChallengerTypes ChallengerType;
}
