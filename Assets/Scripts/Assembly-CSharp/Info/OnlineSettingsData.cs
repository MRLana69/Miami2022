using System;
using System.Collections.Generic;

namespace Info
{
	public class OnlineSettingsData
	{
		public readonly float mysteryBoxDoubleRewardChance = 100f;

		public readonly bool mysteryBoxDoubleRewardAllowKeys = true;

		public readonly int chartboostDelaySeconds = 20;

		public readonly string saveMeLocales = "all";

		public readonly string bundlesEnabledLocales = "all";

		public readonly string bundlesDisabledLocales = string.Empty;

		public readonly int toprunScorematchAmount;

		public readonly int toprunRefreshInterval = 600;

		public readonly bool tapjoyPreloadVideos = true;

		public readonly int toprunGracePeriod;

		public readonly string toprunBaseUrl = "http://subwaysurfers.kiloo-games.com/api/v3";

		public readonly bool kilooAnalyticsEnabled = true;

		public readonly float kilooAnalyticsSample = 2f;

		public readonly string fbAdchoiceLink = "https://m.facebook.com/ads/ad_choices";

		public readonly int interstitialGameOverTotalShowSlots = 2;

		public readonly int interstitialGameOverActiveShowSlot = 1;

		public readonly string latestVersionAndroidMinOsVersion;

		public readonly string latestVersion;

		public readonly Dictionary<string, string> latestVersionChangelog;

		public readonly int cacheTimeoutLimitInterstitial = 30;

		public readonly int ageRestrictionThreshold = 13;

		public readonly int ageRestrictionVersion;

		public readonly int toprunStrangerAmount = 20;

		public readonly bool allowLoadScreenUserTips;

		public readonly Dictionary<string, string> loadScreenTitle;

		public readonly Dictionary<string, UserTip[]> userTips;

		public readonly bool interstitialLocationGameoverAfterCountupEnabled = true;

		public readonly int interstitialLocationGameoverAfterCountupTimeLimit;

		public readonly int abtestTwokeysVersion = 3;

		public readonly bool abtestTwokeysActive = true;

		public readonly string abtestTwokeysTag = "twokeys";

		public readonly string abtestTwokeysSegmentOffset = "0;1;2;26;50";

		public readonly string abtestTwokeysSegmentSize = "1;1;24;24;50";

		public readonly string abtestTwokeysSegmentPayload = "A;B;B;B;B";

		public readonly int abtestMainscreenButtonsVersion = 1;

		public readonly bool abtestMainscreenButtonsActive = true;

		public readonly string abtestMainscreenButtonsTag = "mainscreenbuttons";

		public readonly string abtestMainscreenButtonsSegmentOffset = "0;5;10;15;20;57";

		public readonly string abtestMainscreenButtonsSegmentSize = "5;5;5;5;37;37";

		public readonly string abtestMainscreenButtonsSegmentPayload = "A;B;C;D;B2;C2";

		public readonly string syboAnalyticsEndpointBaseurl = "https://subwaysurf.prod.bladesofbrim.com";

		public readonly int syboAnalyticsMaxGamesWithoutFlush = 1;

		public readonly bool syboAnalyticsEnabled = true;

		public readonly float syboAnalyticsSamplingThreshold = 0.05f;

		public readonly string eventType = "MysteryPowerup";

		public readonly DateTime eventStart = DateTime.MaxValue;

		public readonly DateTime eventEnd = DateTime.MinValue;

		public readonly string eventValue = "moscowCharacter01";

		public readonly string eventPopupTitleFirst = "MYSTERY";

		public readonly string eventPopupTitleSecond = "MONDAY";

		public readonly string eventPopupDescriptionFirst = "Super Mysterizer";

		public readonly string eventPopupDescriptionSecond = "Pick up surprises in the Subway";

		public readonly string discountDealName;

		public readonly int inAppTier1;

		public readonly int inAppTier2;

		public readonly int inAppTier3;

		public readonly int inAppTier4;

		public readonly int inAppTier5;

		public readonly int inAppTier6;

		public readonly int inAppTier7;

		public readonly int inAppTier8;

		public readonly int inAppTier9;

		public readonly int inAppDoubleCoins;

		public readonly DateTime discountEndTime = DateTime.MinValue;

		public readonly string videoadsProviderlist = "adcolony,vungleclips,tapjoyvideoads,supersonicvideoads,chartboostvideoads";

		public readonly string adcolonyVideoPriority = "1;100";

		public readonly string vungleVideoPriority = "2;100";

		public readonly string supersonicVideoPriority = "3;50";

		public readonly string chartboostVideoPriority = "4;100";

		public readonly string tapjoyVideoPriority = "5;100";

		public readonly string defaultVideoList = "adcolony:1_100,vungle:2_100,supersonic:3_50,chartboost:4_100,tapjoy:5_100,flurry:7_100";

		public readonly int interstitialsLimitPerHour = 10;

		public readonly int interstitialsLimitPerDay = 40;

		public readonly int interstitialProvidersListVersion = 26;

		public readonly string interstitialGroups = "default";

		public readonly string interstitialGroupsDefaultCountryCodes = "AF;AL;DZ;AS;AD;AO;AI;AQ;AG;AR;AM;AW;AU;AT;AZ;BS;BH;BD;BB;BY;BE;BZ;BJ;BM;BT;BO;BA;BW;BV;BR;IO;BN;BG;BF;BI;KH;CM;CA;CV;KY;CF;TD;CL;CN;CX;CC;CO;KM;CG;CD;CK;CR;CI;HR;CU;CY;CZ;DK;DJ;DM;DO;TP;EC;EG;SV;GQ;ER;EE;ET;FK;FO;FJ;FI;FR;FX;GF;PF;TF;GA;GM;GE;DE;GH;GI;GR;GL;GD;GP;GU;GT;GN;GW;GY;HT;HM;VA;HN;HK;HU;IS;IN;ID;IR;IQ;IE;IL;IT;JM;JP;JO;KZ;KE;KI;KP;KR;KW;KG;LA;LV;LB;LS;LR;LY;LI;LT;LU;MO;MK;MG;MW;MY;MV;ML;MT;MH;MQ;MR;MU;YT;MX;FM;MD;MC;MN;ME;MS;MA;MZ;MM;NA;NR;NP;NL;AN;NC;NZ;NI;NE;NG;NU;NF;MP;NO;OM;PK;PW;PA;PG;PY;PE;PH;PN;PL;PT;PR;QA;RE;RO;RU;RW;KN;LC;VC;WS;SM;ST;SA;SN;RS;SC;SL;SG;SK;SI;SB;SO;ZA;SS;GS;ES;LK;SH;PM;SD;SR;SJ;SZ;SE;CH;SY;TW;TJ;TZ;TH;TG;TK;TO;TT;TN;TR;TM;TC;TV;UG;UA;AE;GB;US;UM;UY;UZ;VU;VE;VN;VG;VI;WF;EH;YE;ZM;ZW";

		public readonly int interstitialGroupsDefaultChartboostPriority = 2;

		public readonly int interstitialGroupsDefaultChartboostWeight = 100;

		public readonly int interstitialGroupsDefaultSupersonicPriority = 3;

		public readonly int interstitialGroupsDefaultSupersonicWeight = 100;

		public readonly int interstitialGroupsDefaultFacebookPriority = 4;

		public readonly int interstitialGroupsDefaultFacebookWeight = 100;

		public readonly int interstitialGroupsDefaultInmobiPriority = 5;

		public readonly int interstitialGroupsDefaultInmobiWeight;

		public readonly int interstitialGroupsDefaultMdotmPriority = 6;

		public readonly int interstitialGroupsDefaultMdotmWeight;

		public readonly string defaultInterstitialList = "chartboost:2_100,supersonic:3_100,facebook:4_100";

		public readonly bool adLocationHomeVideosEnabled;

		public readonly bool adLocationEarnCoinsVideosEnabled;

		public readonly bool adLocationDoubleUpVideosEnabled;

		public readonly bool adLocationWordyWeekendVideosEnabled;

		public readonly bool adLocationHomeInterstitialsEnabled;

		public readonly bool adLocationGameOverInterstitialsEnabled;

		public int consumableVideoCooldownSeconds = 28800;

		public int consumableVideoViewLimit = 2;

		public int consumableVideoRewardHoverboard = 3;

		public int missionSkipWithVideoCooldownSeconds = 28800;

		public readonly Dictionary<string, SocialLinkOverrideInfo> socialLinkOverrides;
	}
}
