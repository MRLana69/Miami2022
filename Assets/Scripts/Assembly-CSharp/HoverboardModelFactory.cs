using System.Collections.Generic;
using Data.Save;
using Info;
using SYBO.Components.Utilities;
using UnityEngine;

public class HoverboardModelFactory : SingletonMonoBehaviour<HoverboardModelFactory>
{
	public HoverboardBundle RequestHoverboardInstance(string name)
	{
		string text = "Prefabs/HoverboardAssets/" + name;
		Object obj = Resources.Load(text);
		if (obj == null)
		{
			SDebug.LogErrorFormat("Failed to load board bundle from: {0} for name:{1}", text, name);
			return null;
		}
		GameObject gameObject = Object.Instantiate(obj) as GameObject;
		return gameObject.GetComponent<HoverboardBundle>();
	}

	public HoverboardBundle RequestEquippedHoverboardInstance()
	{
		return RequestHoverboardInstance(PlayerSave.currentBoardId.ToString());
	}

	public HoverboardBundle RequestRandomHoverboardInstance()
	{
		List<Boards.BoardId> randomBoardList = MysteryPowerup.GetRandomBoardList();
		int index = Random.Range(0, randomBoardList.Count);
		if (randomBoardList.Count > 0)
		{
			return RequestHoverboardInstance(randomBoardList[index].ToString());
		}
		SDebug.LogError("The eligible board list is empty.");
		return null;
	}
}
