using System.Collections;
using Data.Save;
using TopRun;
using UI;
using UnityEngine;

public class TopRunPlayerCell : MonoBehaviour
{
	[SerializeField]
	private UITexture _picture;

	[SerializeField]
	private UILabel _name;

	[SerializeField]
	private UISprite _medal;

	[SerializeField]
	private UISprite _glow;

	[SerializeField]
	private UISprite _flag;

	[SerializeField]
	private UISprite _shadow;

	[SerializeField]
	private UILabel _score;

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _competitorIcon;

	[SerializeField]
	private AudioClip _medalAnimationSound;

	public Texture dummyImage;

	private bool isLocalUser;

	private IPlayer player;

	private bool useDarkColour;

	private int rank;

	private float soundVolume = 0.1f;

	private float soundPitch = 1f;

	private float _animationLerpFactor;

	private const int SCORE_SPACING = 10;

	private static readonly Color32 EFFECT_INITIAL_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

	private const float MEDAL_ANIMATION_TIME_OFFSET = 0.5f;

	public void InitCell(IPlayer playerT, bool useDarkColourT, int rankT, bool isLocalUserT)
	{
		player = playerT;
		useDarkColour = useDarkColourT;
		rank = rankT;
		isLocalUser = isLocalUserT;
		Refresh();
	}

	public bool IsLocalUser()
	{
		return isLocalUser;
	}

	public void Refresh()
	{
		if (_name == null)
		{
			return;
		}
		_name.text = string.Empty;
		_medal.enabled = false;
		_flag.enabled = false;
		_shadow.enabled = false;
		_glow.enabled = false;
		IRank rank = null;
		Texture2D texture2D = null;
		IManager manager = TopRunRoot.manager;
		if (manager == null)
		{
			SDebug.LogError("Manager is null. Aborting cell initialization.");
			return;
		}
		if (isLocalUser)
		{
			SocialManager instance = SocialManager.instance;
			texture2D = instance.localUserImage;
			_name.text = instance.localDisplayName;
			_name.color = Color.white;
			_score.color = Color.white;
			rank = manager.rank;
		}
		else
		{
			if (player.facebookProfile != null)
			{
				rank = manager.GetPlayerRank(player.facebookProfile.id);
				texture2D = player.facebookProfile.image;
				_name.text = player.facebookProfile.name;
			}
			else
			{
				_name.text = Strings.Get("TOPRUN_ANONYMOUS_PLAYER_ALIAS");
			}
			_name.color = ColorData.instance.midnight;
			_score.color = ColorData.instance.midnight;
		}
		if (rank != null && (!isLocalUser || (isLocalUser && !TopRunRewardHelper.ShouldShowResults())))
		{
			SetMedalForRank(rank.rank, rank.country);
		}
		if (isLocalUser && manager.tournament != null && manager.tournament.hasResult && !TopRunRewardHelper.ShouldShowResults() && manager.score > 0)
		{
			SetMedalForRank(manager.tournament.GetRankForScore(manager.score), string.Empty);
		}
		if (texture2D != null)
		{
			texture2D.wrapMode = TextureWrapMode.Clamp;
		}
		_picture.mainTexture = texture2D ?? dummyImage;
		_score.text = player.score.ToString();
		_competitorIcon.enabled = !player.isFriend;
		_name.overflowMethod = UILabel.Overflow.ResizeFreely;
		SetBackgroundColour(useDarkColour, this.rank);
		ResizeNameIfNeeded();
	}

	public void AnimateMedalIfNeeded()
	{
		IManager manager = TopRunRoot.manager;
		if (manager != null && manager.tournament != null && isLocalUser && _medal.enabled && ((manager.tournament.hasResult && !TopRunRewardHelper.ShouldShowResults()) || !manager.tournament.isFinished))
		{
			IRank rank = TopRunRoot.manager.rank;
			int sequenceId = manager.tournament.sequenceId;
			bool flag = rank != null && !HasShownMedalForWeek(sequenceId);
			bool flag2 = manager.tournament.hasResult && !TopRunRewardHelper.ShouldShowResults() && !HasShownMedalForWeek(sequenceId + 1) && manager.score > 0;
			if (flag || flag2)
			{
				StartCoroutine(AnimateMedalSprite(_glow, _medal, flag2));
				int lastWeekMedalWasShown = ((!flag2) ? sequenceId : (sequenceId + 1));
				TopRunSave.lastWeekMedalWasShown = lastWeekMedalWasShown;
			}
		}
	}

	private bool HasShownMedalForWeek(int weekNumber)
	{
		return TopRunSave.lastWeekMedalWasShown >= weekNumber;
	}

	private void SetMedalForRank(int rank, string countryID)
	{
		Country countryForID = CountryData.GetCountryForID(countryID);
		string text = ((countryForID != null) ? countryForID.flag : null);
		if (!string.IsNullOrEmpty(text) && rank >= 0 && rank <= 3)
		{
			_flag.spriteName = text;
			IManager manager = TopRunRoot.manager;
			if (manager != null && manager.tournament != null)
			{
				if (manager.tournament.country.ToLower() == countryID.ToLower())
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
		}
		_medal.enabled = true;
		switch (rank)
		{
		case 0:
			_medal.spriteName = "icon_diamond_medal";
			_glow.spriteName = "icon_diamond_medal_glow";
			break;
		case 1:
			_medal.spriteName = "icon_gold_medal";
			_glow.spriteName = "icon_gold_medal_glow";
			break;
		case 2:
			_medal.spriteName = "icon_silver_medal";
			_glow.spriteName = "icon_silver_medal_glow";
			break;
		case 3:
			_medal.spriteName = "icon_bronze_medal";
			_glow.spriteName = "icon_bronze_medal_glow";
			break;
		default:
			_medal.enabled = false;
			_shadow.enabled = false;
			_flag.enabled = false;
			break;
		}
	}

	private IEnumerator AnimateMedalSprite(UISprite glowSprite, UISprite medalSprite, bool isThisWeeksResult)
	{
		glowSprite.enabled = true;
		glowSprite.transform.localPosition = medalSprite.transform.localPosition;
		glowSprite.color = EFFECT_INITIAL_COLOR;
		NGUITools.PlaySound(_medalAnimationSound, soundVolume, soundPitch);
		yield return StartCoroutine(AnimateEffectColor(glowSprite, glowSprite.color, Color.white, 3f));
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(AnimateEffectColor(glowSprite, glowSprite.color, EFFECT_INITIAL_COLOR, 0.8f));
		glowSprite.enabled = false;
	}

	private IEnumerator AnimateEffectColor(UISprite spriteToAnimate, Color color1, Color color2, float speedFactor = 1f)
	{
		_animationLerpFactor = 0f;
		while (_animationLerpFactor < 1f)
		{
			_animationLerpFactor += Time.deltaTime * speedFactor;
			spriteToAnimate.color = Color.Lerp(color1, color2, _animationLerpFactor);
			yield return null;
		}
	}

	private void SetBackgroundColour(bool useDark, int rank)
	{
		if (isLocalUser)
		{
			_background.color = GlobalColors.TOPRUN_ME_PLAYER_BACKGROUND_COLOUR;
			return;
		}
		switch (rank)
		{
		case 1:
			if (useDark)
			{
				_background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_00_DARK_COLOUR;
			}
			else
			{
				_background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_00_LIGHT_COLOUR;
			}
			break;
		case 2:
			if (useDark)
			{
				_background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_10_DARK_COLOUR;
			}
			else
			{
				_background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_10_LIGHT_COLOUR;
			}
			break;
		case 3:
			if (useDark)
			{
				_background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_20_DARK_COLOUR;
			}
			else
			{
				_background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_20_LIGHT_COLOUR;
			}
			break;
		case 4:
			if (useDark)
			{
				_background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_30_DARK_COLOUR;
			}
			else
			{
				_background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_30_LIGHT_COLOUR;
			}
			break;
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
			if (useDark)
			{
				_background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_40_DARK_COLOUR;
			}
			else
			{
				_background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_40_LIGHT_COLOUR;
			}
			break;
		default:
			_background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_40_DARK_COLOUR;
			break;
		}
	}

	private void ResizeNameIfNeeded()
	{
		float num = _score.transform.localPosition.x - (float)_score.width;
		float num2 = _name.transform.localPosition.x + (float)_name.width;
		float num3 = num - 10f;
		float num4 = num2 + (float)_medal.width + (float)_flag.width;
		if (num3 <= num4)
		{
			float f = num4 - num3;
			_name.overflowMethod = UILabel.Overflow.ClampContent;
			_name.width -= Mathf.RoundToInt(f);
			_name.text = _name.processedText;
			_name.overflowMethod = UILabel.Overflow.ResizeFreely;
		}
	}
}
