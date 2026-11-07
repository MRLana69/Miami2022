using UnityEngine;

namespace WeeklyHunt
{
	public class PrizeIconPrefabData : ScriptableObject
	{
		private static PrizeIconPrefabData _instance;

		[SerializeField]
		private GameObject _keyIconPrefab;

		[SerializeField]
		private GameObject _coinIconPrefab;

		[SerializeField]
		private GameObject _superMysteryBoxIconPrefab;

		[SerializeField]
		private GameObject _tokenMysteryBoxIconPrefab;

		[SerializeField]
		private GameObject _hoverboardIconPrefab;

		[SerializeField]
		private GameObject _scoreBoosterIconPrefab;

		[SerializeField]
		private GameObject _headstartIconPrefab;

		[SerializeField]
		private GameObject _christmasBoardIconPrefab;

		[SerializeField]
		private GameObject _rioBoard2IconPrefab;

		[SerializeField]
		private GameObject _northpoleBoard2016IconPrefab;

		[SerializeField]
		private GameObject _cherryBoardIconPrefab;

		[SerializeField]
		private GameObject _chickyBoardIconPrefab;

		[SerializeField]
		private GameObject _eagleBoardIconPrefab;

		public static PrizeIconPrefabData instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<PrizeIconPrefabData>("Info/PrizeIconPrefabData");
				}
				return _instance;
			}
		}

		public GameObject GetPrefab(WeeklyHuntData.PrizeData prizeData)
		{
			WeeklyHuntData.WeeklyHuntPrize weeklyHuntPrize = WeeklyHuntManager.DeterminePrize(prizeData);
			GameObject gameObject;
			switch (weeklyHuntPrize)
			{
			case WeeklyHuntData.WeeklyHuntPrize.Coin:
				gameObject = _coinIconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.Key:
				gameObject = _keyIconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.SuperMysteryBox:
				gameObject = _superMysteryBoxIconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.TokenMysteryBox:
				gameObject = _tokenMysteryBoxIconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.Hoverboard:
				gameObject = _hoverboardIconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.ScoreBooster:
				gameObject = _scoreBoosterIconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.Headstart:
				gameObject = _headstartIconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.ChristmasBoard:
				gameObject = _christmasBoardIconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.RioBoard2:
				gameObject = _rioBoard2IconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.NorthpoleBoard2016:
				gameObject = _northpoleBoard2016IconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.CherryBoard:
				gameObject = _cherryBoardIconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.ChickyBoard:
				gameObject = _chickyBoardIconPrefab;
				break;
			case WeeklyHuntData.WeeklyHuntPrize.EagleBoard:
				gameObject = _eagleBoardIconPrefab;
				break;
			default:
				SDebug.LogError(string.Format("Unable to find icon prefab for prize '{0}'", prizeData.rewardType.ToString()));
				return null;
			}
			if (!gameObject)
			{
				SDebug.LogError(string.Format("Prize '{0}' doesn't have a model set", weeklyHuntPrize.ToString()));
			}
			return gameObject;
		}
	}
}
