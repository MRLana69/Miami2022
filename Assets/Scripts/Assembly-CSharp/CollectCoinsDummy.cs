using Kiloo.Common.Extensions;

public class CollectCoinsDummy
{
	public string name;

	public bool hasBeenCollected;

	public int fakeProgress;

	public void AssignRandomDummyFriendName()
	{
		string[] list = new string[9] { "DUMMY_FRIEND_NAME_1", "DUMMY_FRIEND_NAME_2", "DUMMY_FRIEND_NAME_3", "DUMMY_FRIEND_NAME_4", "DUMMY_FRIEND_NAME_5", "DUMMY_FRIEND_NAME_6", "DUMMY_FRIEND_NAME_7", "DUMMY_FRIEND_NAME_8", "DUMMY_FRIEND_NAME_9" };
		name = list.GetRandomElement();
	}
}
