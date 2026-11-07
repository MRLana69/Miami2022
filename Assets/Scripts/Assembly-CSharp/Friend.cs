using System;
using System.Diagnostics;
using System.Threading;
using Kiloo;
using Kiloo.Common.Extensions;
using UnityEngine;

public class Friend
{
	public class Status
	{
		public DateTime lastPokeTime = DateTime.MinValue;

		public int gamesCashedIn;
	}

	public const int RELATION_FACEBOOK_MASK = 1;

	public const int RELATION_GAMECENTER_MASK = 2;

	public int userid;

	public int score;

	public int meters;

	public int games;

	public int rank;

	public GameCenterProfile gcProfile;

	public FacebookProfile fbProfile;

	public OfflineProfile offlineProfile;

	public Status status;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action updated__BackingField;

	public string name
	{
		get
		{
			if (fbProfile != null)
			{
				return fbProfile.name;
			}
			if (gcProfile != null)
			{
				return gcProfile.alias;
			}
			if (offlineProfile != null)
			{
				return offlineProfile.name;
			}
			SDebug.LogError("Friend not initialized");
			return null;
		}
		set
		{
			if (fbProfile == null && gcProfile == null)
			{
				if (offlineProfile == null)
				{
					offlineProfile = new OfflineProfile();
				}
				offlineProfile.name = value;
			}
		}
	}

	public Texture2D image
	{
		get
		{
			Texture2D result = null;
			if (fbProfile != null)
			{
				result = fbProfile.image;
			}
			else if (gcProfile != null)
			{
				result = gcProfile.image;
			}
			return result;
		}
	}

	public int relation
	{
		get
		{
			int num = 0;
			if (fbProfile != null)
			{
				num |= 1;
			}
			if (gcProfile != null)
			{
				num |= 2;
			}
			return num;
		}
	}

	public string id
	{
		get
		{
			if (fbProfile != null)
			{
				return fbProfile.id;
			}
			if (gcProfile != null)
			{
				return gcProfile.id;
			}
			return null;
		}
	}

	public int gamesToCashIn
	{
		get
		{
			if (games < status.gamesCashedIn)
			{
				status.gamesCashedIn = games;
			}
			return games - status.gamesCashedIn;
		}
	}

	public bool shouldBePoked
	{
		get
		{
			return IsPokeTime(status.lastPokeTime);
		}
	}

	public event Action updated
	{
		add
		{
			Action action = updated__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref updated__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = updated__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref updated__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private bool IsPokeTime(DateTime lastPokeTime)
	{
		if ((CDateTime.UtcNow - lastPokeTime).Days > 0 && status.lastPokeTime != DateTime.MinValue)
		{
			return true;
		}
		return false;
	}

	public void InvokeUpdatedHandler()
	{
		updated__BackingField.SafeInvoke();
	}
}
