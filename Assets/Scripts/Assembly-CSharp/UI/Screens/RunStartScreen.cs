using JetBrains.Annotations;
using SYBO.Subway;
using UnityEngine;

namespace UI.Screens
{
	public abstract class RunStartScreen : UIScreen
	{
		[UsedImplicitly]
		public void StartRun()
		{
			if (Input.touchCount <= 1)
			{
				if (Game.Instance != null)
				{
					SSEventsManagerController.instance.RestoreBoardAfterEventIfNeeded();
					Game.Instance.StartNewGame();
					UIScreenController.Instance.PushScreen("IngameUI");
					SaveMeManager.ResetSaveMeForNewRun();
				}
				else
				{
					UIScreenController.Instance.PushScreen("GameOverScreen");
				}
			}
		}
	}
}
