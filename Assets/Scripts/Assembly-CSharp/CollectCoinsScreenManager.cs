using System;

public class CollectCoinsScreenManager
{
	public enum CollectCoinsScreenState
	{
		Offline = 0,
		Facebook = 1,
		GameCenter = 2,
		FacebookAndGamecenter = 3
	}

	private static CollectCoinsScreenManager _instance;

	public const int GCFriendsThreshold = 3;

	public Action<CollectCoinsScreenState> OnCollectCoinsScreenStateChangedEvent;

	private CollectCoinsScreenState _screenState;

	public CollectCoinsScreenState screenState
	{
		get
		{
			UpdateScreenState();
			return _screenState;
		}
		private set
		{
			if (_screenState != value)
			{
				_screenState = value;
				ScreenStateChanged(_screenState);
			}
		}
	}

	public static CollectCoinsScreenManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CollectCoinsScreenManager();
			}
			return _instance;
		}
	}

	public static bool isInitialized
	{
		get
		{
			return _instance != null;
		}
	}

	private void ScreenStateChanged(CollectCoinsScreenState newState)
	{
		Action<CollectCoinsScreenState> onCollectCoinsScreenStateChangedEvent = OnCollectCoinsScreenStateChangedEvent;
		if (onCollectCoinsScreenStateChangedEvent != null)
		{
			onCollectCoinsScreenStateChangedEvent(newState);
		}
	}

	private void UpdateScreenState()
	{
		Globals.LogInState loginState = Globals.GetLoginState();
		int consolidationState = SocialManager.instance.consolidationState;
		bool flag = loginState == Globals.LogInState.Facebook || loginState == Globals.LogInState.Both;
		bool flag2 = (consolidationState & 1) == 1;
		bool flag3 = (flag && flag2) || Globals.DEBUG_USE_DEBUG_LOGIN_STATE;
		bool flag4 = loginState == Globals.LogInState.Both || loginState == Globals.LogInState.GameCenter;
		bool flag5 = (consolidationState & 2) == 2;
		bool flag6 = flag4 && flag5;
		if (flag3)
		{
			if (flag6)
			{
				screenState = CollectCoinsScreenState.FacebookAndGamecenter;
			}
			else
			{
				screenState = CollectCoinsScreenState.Facebook;
			}
		}
		else if (flag6)
		{
			screenState = CollectCoinsScreenState.GameCenter;
		}
		else
		{
			screenState = CollectCoinsScreenState.Offline;
		}
	}
}
