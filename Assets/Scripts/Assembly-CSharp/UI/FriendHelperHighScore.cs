using Data.Save;
using TopRun;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
	public class FriendHelperHighScore : MonoBehaviour
	{
		[FormerlySerializedAs("friendRank")]
		[SerializeField]
		private UILabel _friendRank;

		[SerializeField]
		private UISprite _unrankedBulletPoint;

		[FormerlySerializedAs("friendName")]
		[SerializeField]
		private UILabel _friendName;

		[FormerlySerializedAs("friendScore")]
		[SerializeField]
		private UILabel _friendScore;

		[FormerlySerializedAs("friendPicture")]
		[SerializeField]
		private UITexture _friendPicture;

		[FormerlySerializedAs("friendBackground")]
		[SerializeField]
		private UISprite _friendBackground;

		[FormerlySerializedAs("medal")]
		[SerializeField]
		private UISprite _medal;

		[FormerlySerializedAs("shadow")]
		[SerializeField]
		private UISprite _shadow;

		[FormerlySerializedAs("flag")]
		[SerializeField]
		private UISprite _flag;

		private const int SCORE_SPACING = 10;

		private Color32 _cellBackgroundColorLocalUser = new Color32(11, 43, 83, 204);

		private Color32 _cellBackgroundColorClear = new Color32(184, 215, 237, 127);

		private Color32 _cellBackgroundColorDark = new Color32(120, 170, 206, 127);

		[FormerlySerializedAs("dummyImage")]
		[SerializeField]
		private Texture2D _dummyImage;

		private bool _isLocalUser;

		private bool _isDummy;

		private Friend _friend;

		private bool _initialized;

		public void InitLocalUser(int ranking, bool backgroundActive = false, bool isPlayerRanked = true)
		{
			_isLocalUser = true;
			_friendBackground.color = _cellBackgroundColorLocalUser;
			_friendRank.text = ranking.ToString();
			_friendRank.color = ColorData.instance.white;
			_friendName.text = SocialManager.instance.localDisplayName;
			_friendName.color = ColorData.instance.white;
			_friendScore.text = PlayerSave.highScore.ToString();
			_friendScore.color = ColorData.instance.white;
			_unrankedBulletPoint.color = ColorData.instance.white;
			_unrankedBulletPoint.gameObject.SetActive(!isPlayerRanked);
			_friendRank.gameObject.SetActive(isPlayerRanked);
			if (SocialManager.instance.localUserImage != null)
			{
				_friendPicture.mainTexture = SocialManager.instance.localUserImage;
			}
			else
			{
				_friendPicture.mainTexture = _dummyImage;
			}
			_friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
			InitializeMedal();
			ResizeNameIfNeeded();
			SocialManager.instance.AddLocalUserUpdatedHandler(OnLocalUserUpdated);
			_initialized = true;
		}

		public void InitFriend(Friend friend, int ranking, bool backgroundActive = false)
		{
			_friend = friend;
			if (!backgroundActive)
			{
				_friendBackground.color = _cellBackgroundColorClear;
			}
			else
			{
				_friendBackground.color = _cellBackgroundColorDark;
			}
			_friendRank.text = ranking.ToString();
			_friendName.text = _friend.name;
			_friendName.color = ColorData.instance.midnight;
			_friendScore.text = friend.score.ToString();
			_friendScore.color = ColorData.instance.midnight;
			_unrankedBulletPoint.gameObject.SetActive(false);
			if (_friend.image != null)
			{
				_friendPicture.mainTexture = friend.image;
			}
			else
			{
				_friendPicture.mainTexture = _dummyImage;
			}
			_friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
			InitializeMedal();
			ResizeNameIfNeeded();
			_friend.updated += OnFriendUpdatedHandler;
			_initialized = true;
		}

		public void InitDummy(LeaderboardDummyData.LeaderboardDummy dummy, int ranking, bool backgroundActive)
		{
			_isDummy = true;
			if (!backgroundActive)
			{
				_friendBackground.color = _cellBackgroundColorClear;
			}
			else
			{
				_friendBackground.color = _cellBackgroundColorDark;
			}
			_friendRank.text = ranking.ToString();
			_friendName.text = Strings.Get(dummy.name);
			_friendName.color = ColorData.instance.midnight;
			_friendScore.text = dummy.score.ToString();
			_friendScore.color = ColorData.instance.midnight;
			_unrankedBulletPoint.gameObject.SetActive(false);
			_friendPicture.mainTexture = _dummyImage;
			_friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
			InitializeMedal();
			ResizeNameIfNeeded();
			_initialized = true;
		}

		private void ResizeNameIfNeeded()
		{
			float num = _friendScore.transform.localPosition.x - (float)_friendScore.width;
			float num2 = _friendName.transform.localPosition.x + (float)_friendName.width;
			float num3 = num - 10f;
			float num4 = num2;
			if (_medal.enabled)
			{
				num4 += (float)_medal.width;
			}
			if (_flag.enabled)
			{
				num4 += (float)_flag.width;
			}
			if (num3 <= num4)
			{
				float f = num4 - num3;
				_friendName.overflowMethod = UILabel.Overflow.ClampContent;
				_friendName.width -= Mathf.RoundToInt(f);
				_friendName.text = _friendName.processedText;
				_friendName.overflowMethod = UILabel.Overflow.ResizeFreely;
			}
		}

		private void InitializeMedal()
		{
			IManager manager = TopRunRoot.manager;
			if (manager != null)
			{
				ITournament tournament = manager.tournament;
				if (tournament != null)
				{
					IRank rank = null;
					if (_isLocalUser)
					{
						rank = TopRunRoot.manager.rank;
						if (tournament.isFinished && tournament.hasResult && !TopRunRewardHelper.ShouldShowResults())
						{
							SetTemporaryMedal(tournament.GetRankForScore(manager.score));
						}
						else if (TopRunRewardHelper.ShouldShowResults())
						{
							HideMedal();
						}
						else if (rank != null && rank.rank < 4 && rank.rank != -1)
						{
							SetMedalForRank(rank, manager);
						}
						else
						{
							HideMedal();
						}
					}
					else if (_friend != null)
					{
						if (_friend.fbProfile != null)
						{
							rank = TopRunRoot.manager.GetPlayerRank(_friend.fbProfile.id);
							if (rank != null && rank.rank < 4 && rank.rank != -1)
							{
								SetMedalForRank(rank, manager);
							}
							else
							{
								HideMedal();
							}
						}
						else
						{
							HideMedal();
						}
					}
					else
					{
						HideMedal();
					}
				}
				else
				{
					HideMedal();
				}
			}
			else
			{
				HideMedal();
			}
		}

		private void HideMedal()
		{
			_flag.enabled = false;
			_shadow.enabled = false;
			_medal.enabled = false;
		}

		private void SetMedalForRank(IRank rankData, IManager manager)
		{
			int rank = rankData.rank;
			Country countryForID = CountryData.GetCountryForID(rankData.country);
			string text = ((countryForID != null) ? countryForID.flag : null);
			if (!string.IsNullOrEmpty(text) && rank >= 0 && rank <= 3)
			{
				_flag.spriteName = text;
				if (string.Equals(manager.tournament.country.ToLower(), rankData.country.ToLower()))
				{
					_flag.enabled = false;
					_shadow.enabled = false;
				}
				else
				{
					_flag.enabled = true;
					_shadow.enabled = true;
				}
			}
			switch (rank)
			{
			case 0:
				_medal.spriteName = "icon_diamond_medal";
				_medal.enabled = true;
				break;
			case 1:
				_medal.spriteName = "icon_gold_medal";
				_medal.enabled = true;
				break;
			case 2:
				_medal.spriteName = "icon_silver_medal";
				_medal.enabled = true;
				break;
			case 3:
				_medal.spriteName = "icon_bronze_medal";
				_medal.enabled = true;
				break;
			default:
				_medal.enabled = false;
				_flag.enabled = false;
				_shadow.enabled = false;
				break;
			}
		}

		private void SetTemporaryMedal(int rank)
		{
			_medal.enabled = true;
			_flag.enabled = false;
			_shadow.enabled = false;
			switch (rank)
			{
			case 0:
				_medal.spriteName = "icon_diamond_medal";
				break;
			case 1:
				_medal.spriteName = "icon_gold_medal";
				break;
			case 2:
				_medal.spriteName = "icon_silver_medal";
				break;
			case 3:
				_medal.spriteName = "icon_bronze_medal";
				break;
			default:
				_medal.enabled = false;
				break;
			}
		}

		private void OnDestroy()
		{
			if (_initialized)
			{
				if (_isLocalUser)
				{
					SocialManager.instance.RemoveLocalUserUpdatedHandler(OnLocalUserUpdated);
				}
				else if (!_isDummy)
				{
					_friend.updated -= OnFriendUpdatedHandler;
				}
			}
		}

		public void Refresh()
		{
			OnFriendUpdatedHandler();
		}

		private void OnFriendUpdatedHandler()
		{
			if (_friend.image != null)
			{
				_friendPicture.mainTexture = _friend.image;
			}
		}

		private void OnLocalUserUpdated()
		{
			if (SocialManager.instance.localUserImage != null)
			{
				_friendPicture.mainTexture = SocialManager.instance.localUserImage;
			}
		}
	}
}
