using UnityEngine;

public class SprayDetectionAwaker : MonoBehaviour
{
	private RunnerAnimPlayer _runnerAnimPlayer;

	private void Start()
	{
		_runnerAnimPlayer = Utils.FindObject<RunnerAnimPlayer>();
		UIScreenController.OnChangedScreen += CheckScreen;
	}

	private void CheckScreen(string screenName)
	{
		_runnerAnimPlayer.PlayOrMutePaintSound(screenName == "FrontScreen");
	}
}
