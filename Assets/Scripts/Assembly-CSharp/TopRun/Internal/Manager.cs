using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using AppRefresh;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common;
using Kiloo.Common.Backend;
using Kiloo.Common.Extensions;
using Kiloo.Common.HTTP;
using Kiloo.Common.Serialization;
using Kiloo.Common.Serialization.Parsing;
using Kiloo.Common.Utility;
using UnityEngine;

namespace TopRun.Internal
{
	internal class Manager : IManager, ITournamentPlayerManager
	{
		private delegate IList ParticipantSelector(TournamentData tour);

		private delegate void ParticipantSetter(TournamentData tour, IList list);

		private delegate void TimestampSetter(TournamentData tour, DateTime? timestamp);

		private enum FacebookLoginResult
		{
			Success = 0,
			Failed = 1,
			NotFound = 2
		}

		private Dictionary<string, Player> _currentTournamentPlayers;

		private Rank[] _currentTournamentRanks = new Rank[0];

		private bool _fetchingNewTournament;

		private bool _isRefreshingTournament;

		private bool _isRefreshPending;

		private long _lastFriendsUpdate;

		private long _lastTournamentUpdate;

		private IList<string> _lastTournamentUpdateParticipantsPublicKeys;

		private bool _pauseRefreshing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<TournamentChangedEventArgs> tournamentChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<PlayerInfoChangedEventArgs> playerInfoChanged__BackingField;

		private ManagerData managerData
		{
			get
			{
				return TopRunSave.managerData;
			}
		}

		private RanksData rankData
		{
			get
			{
				return TopRunSave.ranksData;
			}
			set
			{
				TopRunSave.ranksData = value;
			}
		}

		private PlayerIdsData playerIdData
		{
			get
			{
				return TopRunSave.playerIdsData;
			}
		}

		public string country
		{
			get
			{
				if (managerData.country.IsNullOrEmpty())
				{
					country = CountryManager.instance.GetDefaultCountryCodeFromDevice();
				}
				return managerData.country;
			}
			set
			{
				string text = managerData.country;
				if (!(text == value))
				{
					managerData.country = value;
					managerData.nextTournamentTime = null;
				}
			}
		}

		public int multiplier
		{
			get
			{
				return ScoreMultiplierInfo.rawMultiplier;
			}
		}

		public int highScore
		{
			get
			{
				return PlayerSave.highScore;
			}
		}

		public bool canSubmitScore
		{
			get
			{
				TournamentData currentTournament = managerData.currentTournament;
				if (currentTournament == null)
				{
					return false;
				}
				if (currentTournament.closed)
				{
					return false;
				}
				DateTime endTime = currentTournament.endTime;
				DateTime utcNow = CDateTime.UtcNow;
				if (utcNow >= endTime)
				{
					return false;
				}
				string lastSubmittedScoreCountry = managerData.lastSubmittedScoreCountry;
				if (lastSubmittedScoreCountry != null && lastSubmittedScoreCountry != currentTournament.country)
				{
					DateTime lastSubmittedScoreEndTime = managerData.lastSubmittedScoreEndTime;
					if (lastSubmittedScoreEndTime > utcNow)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool isRefreshing
		{
			get
			{
				return _isRefreshingTournament || _isRefreshPending;
			}
		}

		public bool pauseRefreshing
		{
			get
			{
				return _pauseRefreshing;
			}
			set
			{
				_pauseRefreshing = value;
			}
		}

		public bool hasFetchedTournament
		{
			get
			{
				TournamentData currentTournament = managerData.currentTournament;
				return currentTournament != null && !string.IsNullOrEmpty(currentTournament.id);
			}
		}

		public int score
		{
			get
			{
				TournamentData currentTournament = managerData.currentTournament;
				if (currentTournament != null)
				{
					if (currentTournament.unsubmittedScore > 0)
					{
						return currentTournament.unsubmittedScore;
					}
					if (currentTournament.submittedScore > 0)
					{
						return currentTournament.submittedScore;
					}
				}
				return 0;
			}
		}

		public IRank rank { get; private set; }

		public DateTime nextTournamentFetchTime
		{
			get
			{
				DateTime? nextTournamentTime = managerData.nextTournamentTime;
				if (nextTournamentTime.HasValue && nextTournamentTime.Value > DateTime.MinValue)
				{
					return nextTournamentTime.Value;
				}
				TournamentData currentTournament = managerData.currentTournament;
				if (currentTournament != null)
				{
					if (currentTournament.nextStartTime > DateTime.MinValue)
					{
						return currentTournament.nextStartTime;
					}
					return DateTime.MaxValue;
				}
				return DateTime.MinValue;
			}
		}

		public ITournament tournament { get; private set; }

		private float requestTimeout
		{
			get
			{
				return (float)OnlineSettings.instance.GetValue("toprun_request_timeout", 10.0);
			}
		}

		private float requestProfileImageTimeout
		{
			get
			{
				return (float)OnlineSettings.instance.GetValue("toprun_request_image_timeout", 20.0);
			}
		}

		private float requestMinBackOffTime
		{
			get
			{
				return (float)OnlineSettings.instance.GetValue("toprun_request_min_back_off_time", 1.0);
			}
		}

		private float requestMaxBackOffTime
		{
			get
			{
				return (float)OnlineSettings.instance.GetValue("toprun_request_max_back_off_time", 120.0);
			}
		}

		private int facebookFriendPublicKeyExpirySecondsMin
		{
			get
			{
				return OnlineSettings.instance.GetValue("toprun_facebook_friend_public_key_expiry_min", 86400);
			}
		}

		private int facebookFriendPublicKeyExpirySecondsMax
		{
			get
			{
				return OnlineSettings.instance.GetValue("toprun_facebook_friend_public_key_expiry_max", 604800);
			}
		}

		private int maxFriendPublicKeyRequestsToSend
		{
			get
			{
				return OnlineSettings.instance.GetValue("toprun_max_facebook_public_key_requests", 20);
			}
		}

		public event EventHandler<TournamentChangedEventArgs> tournamentChanged
		{
			add
			{
				EventHandler<TournamentChangedEventArgs> eventHandler = tournamentChanged__BackingField;
				EventHandler<TournamentChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref tournamentChanged__BackingField, (EventHandler<TournamentChangedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<TournamentChangedEventArgs> eventHandler = tournamentChanged__BackingField;
				EventHandler<TournamentChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref tournamentChanged__BackingField, (EventHandler<TournamentChangedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event EventHandler<PlayerInfoChangedEventArgs> playerInfoChanged
		{
			add
			{
				EventHandler<PlayerInfoChangedEventArgs> eventHandler = playerInfoChanged__BackingField;
				EventHandler<PlayerInfoChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref playerInfoChanged__BackingField, (EventHandler<PlayerInfoChangedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<PlayerInfoChangedEventArgs> eventHandler = playerInfoChanged__BackingField;
				EventHandler<PlayerInfoChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref playerInfoChanged__BackingField, (EventHandler<PlayerInfoChangedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public Manager()
		{
			foreach (string key in this.playerIdData.players.Keys)
			{
				PlayerIdsData.PlayerIdData playerIdData = this.playerIdData.players[key];
				if (playerIdData != null && !playerIdData.expiry.HasValue)
				{
					playerIdData.expiry = GetRandomisedPlayerIdExpiry();
				}
			}
			InitializeCurrentTournamentPlayers();
			AppRefreshHandler.hasRefreshed += AppHasRefreshed;
		}

		private void AppHasRefreshed(RefreshType refreshType)
		{
			if (refreshType.HasFlag(RefreshType.Data))
			{
				InitializeCurrentTournamentPlayers();
			}
		}

		private DateTime GetRandomisedPlayerIdExpiry()
		{
			float num = KRandom.Range(facebookFriendPublicKeyExpirySecondsMin, facebookFriendPublicKeyExpirySecondsMax);
			return CDateTime.UtcNow.AddSeconds(num);
		}

		private void InitializeCurrentTournamentPlayers()
		{
			_currentTournamentPlayers = new Dictionary<string, Player>();
			AddFacebookFriendsWithIdsToCurrentTournamentPlayers();
			AddCurrentTournamentParticipantsToCurrentTournamentPlayers();
		}

		private void AddCurrentTournamentParticipantsToCurrentTournamentPlayers()
		{
			TournamentData currentTournament = managerData.currentTournament;
			if (currentTournament == null)
			{
				return;
			}
			IList[] array = new IList[2] { currentTournament.participants, currentTournament.scoreMatchedParticipants };
			IList[] array2 = array;
			foreach (IList list in array2)
			{
				if (list == null)
				{
					continue;
				}
				for (int num = list.Count - 1; num >= 0; num--)
				{
					Player player = null;
					string text = null;
					string text2 = list[num] as string;
					if (text2 != null)
					{
						text = FindFacebookIdFromPublicKey(text2);
						PlayerIdsData.PlayerIdData value;
						if (!string.IsNullOrEmpty(text) && playerIdData.players.TryGetValue(text, out value) && !string.IsNullOrEmpty(value.name))
						{
							FacebookProfile facebookProfile = new FacebookProfile();
							facebookProfile.id = text;
							facebookProfile.name = value.name;
							facebookProfile.fullName = value.name;
							FacebookProfile facebookProfile2 = facebookProfile;
							Player player2 = new Player();
							player2.publicKey = text2;
							player2.isFriend = false;
							player2.facebookProfile = facebookProfile2;
							player = player2;
						}
					}
					if (player != null)
					{
						_currentTournamentPlayers[text2] = player;
					}
					else
					{
						if (!string.IsNullOrEmpty(text))
						{
							playerIdData.players.Remove(text);
						}
						list.RemoveAt(num);
						currentTournament.participantsTimeStamp = null;
						currentTournament.scoreMatchedParticipantsTimeStamp = null;
					}
				}
			}
		}

		private void AddFacebookFriendsWithIdsToCurrentTournamentPlayers()
		{
			IList<Player> facebookFriendsWithPublicKeysAsPlayers = GetFacebookFriendsWithPublicKeysAsPlayers();
			foreach (Player item in facebookFriendsWithPublicKeysAsPlayers)
			{
				_currentTournamentPlayers[item.publicKey] = item;
			}
		}

		public IRank GetPlayerRank(string facebookId)
		{
			Rank value = null;
			PlayerIdsData.PlayerIdData value2;
			string text = ((!playerIdData.players.TryGetValue(facebookId, out value2)) ? null : value2.publicKey);
			if (text != null)
			{
				ITournament tournament = this.tournament;
				Player value3;
				if (tournament != null && tournament.hasResult && _currentTournamentPlayers.TryGetValue(text, out value3))
				{
					int rankForScore = tournament.GetRankForScore(value3.score);
					if (rankForScore >= _currentTournamentRanks.Length || tournament.country != _currentTournamentRanks[0].country)
					{
						int rankCount = tournament.rankCount;
						_currentTournamentRanks = new Rank[rankCount];
						for (int i = 0; i < rankCount; i++)
						{
							_currentTournamentRanks[i] = new Rank(tournament.country, i);
						}
					}
					if (rankForScore < _currentTournamentRanks.Length)
					{
						value = _currentTournamentRanks[rankForScore];
					}
				}
				if (value == null)
				{
					rankData.ranks.TryGetValue(text, out value);
				}
			}
			return value;
		}

		public void FetchNextTournament()
		{
			if (_isRefreshingTournament)
			{
				LogWarning("FetchNextTournament: Cannot fetch next tournament while refreshing, ignoring");
				return;
			}
			_isRefreshingTournament = true;
			InProcessFetchNextTournament();
		}

		public void RefreshTournament()
		{
			CoroutineUtil.StartCoroutine(RefreshTournamentCoroutine());
		}

		public void SubmitScore(int scoreToSubmit, DateTime gameStartedTime)
		{
			CoroutineUtil.StartCoroutine(RequestSubmitScoreCoroutine(scoreToSubmit, false, false));
		}

		public IPlayer GetPlayerByPublicKey(string publicKey)
		{
			Player value;
			if (_currentTournamentPlayers != null && _currentTournamentPlayers.TryGetValue(publicKey, out value))
			{
				return value;
			}
			return null;
		}

		internal static void ClearSavedData()
		{
			TopRunSave.managerData = new ManagerData();
			TopRunSave.playerIdsData = new PlayerIdsData();
			TopRunSave.ranksData = new RanksData();
		}

		private void FirePlayerInfoChangedEvent(IPlayer player)
		{
			EventHandler<PlayerInfoChangedEventArgs> eventHandler = playerInfoChanged__BackingField;
			if (eventHandler != null)
			{
				eventHandler(this, new PlayerInfoChangedEventArgs
				{
					player = player
				});
			}
		}

		private void FireTournamentChangedEvent(bool wasUpdated, bool didChange)
		{
			EventHandler<TournamentChangedEventArgs> eventHandler = tournamentChanged__BackingField;
			if (eventHandler != null)
			{
				eventHandler(this, new TournamentChangedEventArgs
				{
					tournamentWasUpdated = wasUpdated,
					tournamentDidChange = didChange
				});
			}
		}

		private string MakeApiURL(string apiResource)
		{
			return OnlineSettings.instance.GetValue("toprun_base_url", "https://subwaysurfers.kiloo-games.com/api/v3") + apiResource;
		}

		private string FindFacebookIdFromPublicKey(string publicKey)
		{
			return playerIdData.players.FirstOrDefault((KeyValuePair<string, PlayerIdsData.PlayerIdData> x) => x.Value.publicKey == publicKey).Key;
		}

		private static TimeSpan GetClientTimeOffset(IRequest request)
		{
			IResponse response = request.response;
			if (response != null && response.headers.Contains("Date"))
			{
				DateTime dateTime = response.headers.Get("Date", DateTime.MinValue);
				if (dateTime != DateTime.MinValue)
				{
					return CDateTime.UtcNow - dateTime;
				}
			}
			return TimeSpan.Zero;
		}

		private void CompleteRefreshProcess(bool isNewTournament)
		{
			_isRefreshingTournament = false;
			FireTournamentChangedEvent(true, isNewTournament);
		}

		private IEnumerator RefreshTournamentCoroutine()
		{
			if (_isRefreshPending)
			{
				yield break;
			}
			if (_isRefreshingTournament)
			{
				_isRefreshPending = true;
				while (_isRefreshingTournament)
				{
					yield return null;
				}
				_isRefreshPending = false;
			}
			_isRefreshingTournament = true;
			yield return CoroutineUtil.StartCoroutine(InProcessRefreshTournamentCoroutine());
		}

		private IEnumerator InProcessRefreshTournamentCoroutine()
		{
			TournamentData tour = managerData.currentTournament;
			if (tour == null || string.IsNullOrEmpty(tour.id))
			{
				while (pauseRefreshing)
				{
					yield return null;
				}
				InProcessFetchNextTournament();
				yield break;
			}
			SocialManager socialManager = SocialManager.instance;
			if (socialManager != null && !socialManager.consolidatedFriendsCompleted)
			{
				do
				{
					yield return null;
				}
				while (!socialManager.consolidatedFriendsCompleted);
			}
			bool isNewTournament = _fetchingNewTournament;
			_fetchingNewTournament = false;
			while (pauseRefreshing)
			{
				yield return null;
			}
			yield return CoroutineUtil.WaitForAll(CoroutineUtil.StartCoroutine(RequestPlayersByFacebookIds()), CoroutineUtil.StartCoroutine(RequestFindMatchedPlayers()));
			while (pauseRefreshing)
			{
				yield return null;
			}
			yield return CoroutineUtil.StartCoroutine(RequestFindMatchedPlayersByCurrentScore());
			while (pauseRefreshing)
			{
				yield return null;
			}
			PurgeSelfFromParticipants();
			PurgeUnneededIds();
			bool mustFetchNewTournament = false;
			yield return CoroutineUtil.StartCoroutine(RequestTournamentData((bool isTournamentNotFound) =>
			{
				if (isTournamentNotFound)
				{
					mustFetchNewTournament = true;
				}
			}));
			while (pauseRefreshing)
			{
				yield return null;
			}
			if (mustFetchNewTournament)
			{
				_isRefreshPending = false;
				InProcessFetchNextTournament();
				yield break;
			}
			GetMissingFacebookInfo();
			int unsubmittedScore = PropagateUnsubmittedScore();
			if (unsubmittedScore > 0)
			{
				CoroutineUtil.StartCoroutine(RequestSubmitScoreCoroutine(unsubmittedScore, false, true));
			}
			yield return CoroutineUtil.WaitForAll(CoroutineUtil.StartCoroutine(RequestRanksForPlayers()), CoroutineUtil.StartCoroutine(RequestNextTournamentFetchTime()));
			CompleteRefreshProcess(isNewTournament);
		}

		private int PropagateUnsubmittedScore()
		{
			int num = 0;
			TournamentData currentTournament = managerData.currentTournament;
			if (currentTournament != null)
			{
				num = currentTournament.unsubmittedScore;
				int unsubmittedScoreNoTournament = managerData.unsubmittedScoreNoTournament;
				if (unsubmittedScoreNoTournament > 0)
				{
					DateTime? unsubmittedScoreNoTournamentTime = managerData.unsubmittedScoreNoTournamentTime;
					DateTime startTime = currentTournament.startTime;
					DateTime endTime = currentTournament.endTime;
					if (startTime > DateTime.MinValue && endTime > DateTime.MinValue && unsubmittedScoreNoTournamentTime < endTime)
					{
						managerData.unsubmittedScoreNoTournament = 0;
						managerData.unsubmittedScoreNoTournamentTime = null;
						if (unsubmittedScoreNoTournamentTime.HasValue && unsubmittedScoreNoTournamentTime.GetValueOrDefault() >= startTime && unsubmittedScoreNoTournament > num)
						{
							num = (currentTournament.unsubmittedScore = unsubmittedScoreNoTournament);
						}
					}
				}
			}
			return num;
		}

		private void InProcessFetchNextTournament()
		{
			if (string.IsNullOrEmpty(country))
			{
				LogError("FetchNextTournament: Country must be set first");
				return;
			}
			string newTournamentCountry = country;
			KilooWebClient.CreateGet(new Uri(string.Format("{0}?Country={1}", MakeApiURL("/tournaments/getlatesttournamentidbycountry"), newTournamentCountry))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
				.WithMaxBackOffTime(requestMaxBackOffTime)
				.ThenMakeTheRequestAsync((IRequest request, bool isFromCache) =>
				{
					string text = null;
					IResponse response = request.response;
					string json = ((response != null) ? response.AsString() : null);
					if (request.isSuccessful && response != null && response.IsJson())
					{
						IDictionary dictionary;
						try
						{
							dictionary = KilooJson.Deserialize(json) as IDictionary;
						}
						catch (Exception)
						{
							dictionary = null;
						}
						if (dictionary != null)
						{
							string text2 = dictionary["TournamentId"] as string;
							if (text2 != null)
							{
								text = text2;
							}
						}
					}
					if (text == null)
					{
						LogError("FetchNextTournament: Request failed or bad response, status: " + request.statusMessage);
						CompleteRefreshProcess(false);
					}
					else
					{
						TournamentData currentTournament = managerData.currentTournament;
						string value = ((currentTournament == null) ? null : currentTournament.id);
						if (!text.Equals(value))
						{
							managerData.nextTournamentTime = null;
							TournamentData currentTournament2 = managerData.currentTournament;
							if (currentTournament2 != null && (currentTournament2.submittedScore > 0 || currentTournament2.unsubmittedScore > 0))
							{
								currentTournament2.participants = null;
								currentTournament2.scoreMatchedParticipants = null;
								managerData.previousTournament = currentTournament2;
							}
							managerData.currentTournament = new TournamentData
							{
								id = text,
								country = newTournamentCountry
							};
							_currentTournamentPlayers = new Dictionary<string, Player>();
							_fetchingNewTournament = true;
						}
						CoroutineUtil.StartCoroutine(InProcessRefreshTournamentCoroutine());
					}
				});
		}

		private IList<FacebookProfile> GetFacebookFriendsToRequestPublicKeysFor()
		{
			Dictionary<long, List<FacebookProfile>> dictionary = new Dictionary<long, List<FacebookProfile>>();
			SocialManager instance = SocialManager.instance;
			if (instance.facebookIsLoggedIn && (instance.consolidationState & 1) != 0)
			{
				Friend[] array = instance.FriendsSortedByScore();
				Friend[] array2 = array;
				foreach (Friend friend in array2)
				{
					long num = -1L;
					if (friend.fbProfile != null && !playerIdData.players.ContainsKey(friend.fbProfile.id))
					{
						playerIdData.players[friend.fbProfile.id] = new PlayerIdsData.PlayerIdData
						{
							name = friend.fbProfile.name,
							expiry = GetRandomisedPlayerIdExpiry()
						};
						num = 0L;
					}
					else if (friend.fbProfile != null)
					{
						PlayerIdsData.PlayerIdData value;
						if (playerIdData.players.TryGetValue(friend.fbProfile.id, out value) && value.expiry.HasValue)
						{
							num = value.expiry.Value.Ticks;
						}
						if (num == -1)
						{
							LogError("[Friends] Friend in _playerIdData already, but without an expiry! friend.fbProfile.id: " + friend.fbProfile.id);
						}
					}
					if (num != -1 && num < CDateTime.UtcNow.Ticks)
					{
						if (!dictionary.ContainsKey(num))
						{
							dictionary[num] = new List<FacebookProfile>();
						}
						dictionary[num].Add(friend.fbProfile);
					}
				}
			}
			IList<FacebookProfile> list = new List<FacebookProfile>();
			List<long> list2 = dictionary.Keys.ToList();
			list2.Sort();
			for (int j = 0; j < list2.Count; j++)
			{
				if (list.Count >= maxFriendPublicKeyRequestsToSend)
				{
					break;
				}
				if (list2[j] >= CDateTime.UtcNow.Ticks)
				{
					break;
				}
				list.AddRangeDistinct(dictionary[list2[j]].Take(maxFriendPublicKeyRequestsToSend - list.Count));
			}
			return list;
		}

		private IList<Player> GetFacebookFriendsWithPublicKeysAsPlayers()
		{
			List<Player> list = new List<Player>();
			SocialManager instance = SocialManager.instance;
			if (instance.facebookIsLoggedIn && (instance.consolidationState & 1) != 0)
			{
				Friend[] array = instance.FriendsSortedByScore();
				Friend[] array2 = array;
				foreach (Friend friend in array2)
				{
					FacebookProfile fbProfile = friend.fbProfile;
					PlayerIdsData.PlayerIdData value;
					if (fbProfile != null && playerIdData.players.TryGetValue(fbProfile.id, out value) && value.publicKey != null)
					{
						list.Add(new Player
						{
							publicKey = value.publicKey,
							facebookProfile = fbProfile,
							isFriend = true
						});
					}
				}
			}
			return list;
		}

		private IEnumerator RequestPlayersByFacebookIds()
		{
			IList<FacebookProfile> friendsWithNoOrOldPublicKeys = null;
			if (_lastFriendsUpdate + OnlineSettings.instance.GetValue("toprun_friends_refresh_interval", 1800) < CDateTime.UtcNow.ToUnixTime())
			{
				friendsWithNoOrOldPublicKeys = GetFacebookFriendsToRequestPublicKeysFor();
			}
			if (friendsWithNoOrOldPublicKeys == null || friendsWithNoOrOldPublicKeys.Count <= 0)
			{
				yield break;
			}
			List<object> facebookIds = new List<object>(friendsWithNoOrOldPublicKeys.Count);
			foreach (FacebookProfile item in friendsWithNoOrOldPublicKeys)
			{
				ulong result;
				if (ulong.TryParse(item.id, NumberStyles.None, CultureInfo.InvariantCulture, out result))
				{
					facebookIds.Add(result);
				}
			}
			IRequest request = KilooSerializerHTTPExtensions.WithJsonContent(jsonData: new Dictionary<string, object> { { "FacebookIds", facebookIds } }, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/facebook/getplayersbyfacebookids")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
				.WithMaxBackOffTime(requestMaxBackOffTime);
			yield return request.SendAsync();
			bool badResponse = true;
			IResponse response = request.response;
			string responseText = ((response != null) ? response.AsString() : null);
			if (request.isSuccessful && request.response.IsJson())
			{
				IDictionary dictionary;
				try
				{
					dictionary = KilooJson.Deserialize(responseText) as IDictionary;
				}
				catch (Exception)
				{
					dictionary = null;
				}
				if (dictionary != null)
				{
					IList list = dictionary["Players"] as IList;
					if (list != null)
					{
						badResponse = false;
						_lastFriendsUpdate = CDateTime.UtcNow.ToUnixTime();
						foreach (object item2 in list)
						{
							IDictionary dictionary2 = item2 as IDictionary;
							if (dictionary2 != null)
							{
								JsonObject jsonObject = new JsonObject(dictionary2);
								string text = jsonObject.Get<string>("FacebookId", null, null);
								string text2 = jsonObject.Get<string>("PublicKey", null, null);
								if (text != null && text2 != null)
								{
									playerIdData.players[text] = new PlayerIdsData.PlayerIdData
									{
										publicKey = text2,
										expiry = GetRandomisedPlayerIdExpiry()
									};
								}
							}
						}
					}
				}
			}
			if (badResponse)
			{
				LogError("LookupFriendPublicKeys: Request failed or bad response, status: " + request.statusMessage);
			}
			SocialManager social = SocialManager.instance;
			if (!social.facebookIsLoggedIn || (social.consolidationState & 1) == 0)
			{
				yield break;
			}
			Friend[] array = social.FriendsSortedByScore();
			Friend[] array2 = array;
			foreach (Friend friend in array2)
			{
				PlayerIdsData.PlayerIdData value;
				if (friend.fbProfile != null && playerIdData.players.TryGetValue(friend.fbProfile.id, out value) && value.publicKey != null)
				{
					Player value2;
					_currentTournamentPlayers.TryGetValue(value.publicKey, out value2);
					if (value2 == null)
					{
						Player player = new Player();
						player.publicKey = value.publicKey;
						value2 = player;
						_currentTournamentPlayers[value.publicKey] = value2;
					}
					value2.isFriend = true;
					value2.facebookProfile = friend.fbProfile;
				}
			}
		}

		private bool HandleFindMatchedPlayersResponse(IRequest request, ParticipantSelector participantSelector, ParticipantSetter participantSetter, TimestampSetter timestampSetter)
		{
			IResponse response = request.response;
			string json = ((response != null) ? response.AsString() : null);
			if (request.isSuccessful && response != null && response.IsJson())
			{
				try
				{
					IDictionary dictionary = KilooJson.Deserialize(json) as IDictionary;
					if (dictionary != null)
					{
						IList list = dictionary["Players"] as IList;
						if (list != null)
						{
							TournamentData currentTournament = managerData.currentTournament;
							IList list2 = participantSelector(currentTournament);
							if (list2 == null)
							{
								list2 = new List<object>();
								participantSetter(currentTournament, list2);
							}
							foreach (object item in list)
							{
								JsonObject jsonObject = new JsonObject(item);
								string text = jsonObject.Get<string>("FacebookId", null, null);
								string text2 = jsonObject.Get<string>("PublicKey", null, null);
								string text3 = jsonObject.Get<string>("PlayerName", null, null);
								if (text != null && text2 != null && text3 != null)
								{
									if (!list2.Contains(text2))
									{
										list2.Add(text2);
									}
									playerIdData.players[text] = new PlayerIdsData.PlayerIdData
									{
										publicKey = text2,
										name = text3,
										expiry = GetRandomisedPlayerIdExpiry()
									};
									if (!_currentTournamentPlayers.ContainsKey(text2))
									{
										FacebookProfile facebookProfile = new FacebookProfile();
										facebookProfile.id = text;
										facebookProfile.name = text3;
										facebookProfile.fullName = text3;
										FacebookProfile facebookProfile2 = facebookProfile;
										_currentTournamentPlayers[text2] = new Player
										{
											publicKey = text2,
											isFriend = false,
											facebookProfile = facebookProfile2
										};
									}
								}
								else
								{
									LogWarning("HandleFindMatchedPlayersResponse: Invalid Player Data: FacebookId=" + QuoteString(text) + ", PublicKey=" + QuoteString(text2) + ", PlayerName=" + QuoteString(text3));
								}
							}
							timestampSetter(currentTournament, CDateTime.UtcNow);
							return true;
						}
					}
				}
				catch (Exception)
				{
				}
			}
			return false;
		}

		private IEnumerator RequestFindMatchedPlayers()
		{
			TournamentData currTour = managerData.currentTournament;
			DateTime timeStamp = DateTime.MinValue;
			if (currTour != null)
			{
				DateTime? participantsTimeStamp = currTour.participantsTimeStamp;
				if (participantsTimeStamp.HasValue)
				{
					timeStamp = currTour.participantsTimeStamp.Value;
				}
			}
			DateTime now = CDateTime.UtcNow;
			if (timeStamp > now)
			{
				if (currTour != null)
				{
					currTour.participantsTimeStamp = null;
				}
				timeStamp = DateTime.MinValue;
			}
			int retryInterval = OnlineSettings.instance.GetValue("toprun_stranger_amount_retry_interval", 3600);
			int needStrangers;
			if (timeStamp + TimeSpan.FromSeconds(retryInterval) < now)
			{
				needStrangers = OnlineSettings.instance.GetValue("toprun_stranger_amount", 10);
				IList list = ((currTour == null) ? null : currTour.participants);
				if (list != null)
				{
					needStrangers -= list.Count;
				}
			}
			else
			{
				needStrangers = 0;
			}
			string tournamentId = ((currTour == null) ? null : currTour.id);
			if (needStrangers > 0 && !string.IsNullOrEmpty(tournamentId))
			{
				Uri uri = new Uri(string.Format("{0}?TournamentId={1}&Score={2}&Multiplier={3}&Amount={4}", MakeApiURL("/tournaments/findmatchedplayers"), tournamentId, highScore, multiplier, needStrangers));
				IRequest request = KilooWebClient.CreateGet(uri).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
					.WithMaxBackOffTime(requestMaxBackOffTime);
				yield return request.SendAsync();
				if (!HandleFindMatchedPlayersResponse(request, (TournamentData tour) => tour.participants, (TournamentData tour, IList participants) =>
				{
					tour.participants = participants;
				}, (TournamentData tour, DateTime? timestamp) =>
				{
					tour.participantsTimeStamp = timestamp;
				}))
				{
					LogError("RequestFindMatchedPlayers: Request failed or bad response, status: " + request.statusMessage);
				}
			}
		}

		private int GetAmountOfChallengersToRequest()
		{
			if (tournament == null || tournament.isFinished)
			{
				return 0;
			}
			int value = OnlineSettings.instance.GetValue("toprun_challengers_amount", 0);
			if (value <= 0)
			{
				return 0;
			}
			TournamentData currentTournament = managerData.currentTournament;
			IList list = ((currentTournament == null) ? null : currentTournament.scoreMatchedParticipants);
			if (list != null)
			{
				return 0;
			}
			long num = tournament.startTimeUTC.ToUnixTime();
			long num2 = tournament.endTimeUTC.ToUnixTime();
			long num3 = num2 - num;
			double num4 = (double)(CDateTime.UtcNow.ToUnixTime() - num) / (double)num3;
			double value2 = OnlineSettings.instance.GetValue("toprun_challengers_period_normalized_start", 0.5);
			if (num4 < value2)
			{
				return 0;
			}
			double value3 = OnlineSettings.instance.GetValue("toprun_challengers_period_normalized_end", 2925.0 / 4095.0);
			if (num4 >= value3)
			{
				return 0;
			}
			int value4 = OnlineSettings.instance.GetValue("toprun_challengers_placement_requirement", 4);
			int num5 = 1;
			foreach (IPlayer player in tournament.GetPlayers())
			{
				if (score > player.score)
				{
					break;
				}
				num5++;
			}
			if (num5 > value4)
			{
				return 0;
			}
			return value;
		}

		private IEnumerator RequestFindMatchedPlayersByCurrentScore()
		{
			TournamentData currTour = managerData.currentTournament;
			int needStrangers = GetAmountOfChallengersToRequest();
			if (needStrangers > 0)
			{
				IList list = ((currTour == null) ? null : currTour.scoreMatchedParticipants);
				if (list != null)
				{
					needStrangers -= list.Count;
				}
			}
			if (needStrangers > 0)
			{
				string tournamentId = ((currTour == null) ? string.Empty : currTour.id);
				Uri uri = new Uri(string.Format("{0}?TournamentId={1}&Score={2}&Multiplier={3}&Amount={4}", MakeApiURL("/tournaments/findmatchedplayersbycurrentscore"), tournamentId, score, multiplier, needStrangers));
				IRequest request = KilooWebClient.CreateGet(uri).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
					.WithMaxBackOffTime(requestMaxBackOffTime);
				yield return request.SendAsync();
				if (!HandleFindMatchedPlayersResponse(request, (TournamentData tour) => tour.scoreMatchedParticipants, (TournamentData tour, IList scoreMatchedParticipants) =>
				{
					tour.scoreMatchedParticipants = scoreMatchedParticipants;
				}, (TournamentData tour, DateTime? timestamp) =>
				{
					tour.scoreMatchedParticipantsTimeStamp = timestamp;
				}))
				{
					LogError("RequestFindMatchedPlayersByCurrentScore: Request failed or bad response, status: " + request.statusMessage);
				}
			}
		}

		private void GetMissingFacebookInfo()
		{
			CoroutineUtil.StartCoroutine(GetMissingFacebookInfoCoroutine());
		}

		private IEnumerator GetMissingFacebookInfoCoroutine()
		{
			List<Player> players = GetTournamentPlayersWithMissingFacebookInfo();
			foreach (Player p in players)
			{
				Player player = p;
				FacebookProfile profile = player.facebookProfile;
				if (profile == null || !(profile.image == null) || IsFacebookIDInvalid(profile.id))
				{
					continue;
				}
				while (pauseRefreshing)
				{
					yield return null;
				}
				yield return DownloadFacebookImage(profile.id, (Texture2D image) =>
				{
					if (image != null && image.width == 8 && image.height == 8)
					{
						image = null;
					}
					if (image != null)
					{
						profile.image = image;
						FirePlayerInfoChangedEvent(player);
					}
				});
			}
		}

		private bool IsFacebookIDInvalid(string facebookId)
		{
			int result;
			return int.TryParse(facebookId, out result) && result == 0;
		}

		private Coroutine DownloadFacebookImage(string facebookId, Action<Texture2D> onComplete)
		{
			Uri uri = new Uri(string.Format("http://graph.facebook.com/{0}/picture?width=64&height=64", facebookId));
			return WebClient.CreateGet(uri).WithTimeout(requestProfileImageTimeout).WithMinBackOffTime(requestMinBackOffTime)
				.WithMaxBackOffTime(requestMaxBackOffTime)
				.WithIgnoreCertificateErrors(true)
				.ThenMakeTheRequestAsync((IRequest request, bool cache) =>
				{
					Texture2D texture2D = null;
					bool flag = true;
					if (request.isSuccessful && request.response.IsTexture())
					{
						texture2D = request.response.AsTexture();
						if (texture2D != null)
						{
							flag = false;
						}
					}
					if (flag)
					{
						LogError("RequestUserInfoFromFacebook: Request failed or bad response, status: " + request.statusMessage);
					}
					onComplete(texture2D);
				});
		}

		private List<Player> GetTournamentPlayersWithMissingFacebookInfo()
		{
			List<Player> list = new List<Player>();
			foreach (KeyValuePair<string, Player> currentTournamentPlayer in _currentTournamentPlayers)
			{
				Player value = currentTournamentPlayer.Value;
				if (!value.isFriend && (value.facebookProfile == null || value.facebookProfile.image == null))
				{
					list.Add(value);
				}
			}
			return list;
		}

		private void PurgeSelfFromParticipants()
		{
			string text = managerData.localPlayerPublicKey;
			if (text == null)
			{
				SocialManager instance = SocialManager.instance;
				if (instance.facebookIsLoggedIn && (instance.consolidationState & 1) != 0)
				{
					FacebookProfile facebookProfile = instance.facebookProfile;
					PlayerIdsData.PlayerIdData value;
					if (facebookProfile != null && playerIdData.players.TryGetValue(facebookProfile.id, out value))
					{
						text = value.publicKey;
					}
				}
			}
			if (text == null || !_currentTournamentPlayers.Remove(text))
			{
				return;
			}
			TournamentData currentTournament = managerData.currentTournament;
			if (currentTournament == null)
			{
				return;
			}
			IList[] array = new IList[2] { currentTournament.participants, currentTournament.scoreMatchedParticipants };
			IList[] array2 = array;
			foreach (IList list in array2)
			{
				if (list != null)
				{
					list.Remove(text);
				}
			}
		}

		private void PurgeUnneededIds()
		{
			SocialManager instance = SocialManager.instance;
			if (!instance.facebookIsLoggedIn || (instance.consolidationState & 1) == 0)
			{
				return;
			}
			Friend[] array = instance.FriendsSortedByScore();
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, PlayerIdsData.PlayerIdData> player in playerIdData.players)
			{
				string key = player.Key;
				PlayerIdsData.PlayerIdData value = player.Value;
				string text = ((value == null) ? null : value.publicKey);
				if (text == null)
				{
					list.Add(key);
				}
				else
				{
					if (_currentTournamentPlayers.ContainsKey(text))
					{
						continue;
					}
					bool flag = false;
					Friend[] array2 = array;
					foreach (Friend friend in array2)
					{
						if (friend.fbProfile != null && friend.fbProfile.id == key)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(key);
					}
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			foreach (string item in list)
			{
				playerIdData.players.Remove(item);
			}
		}

		private IEnumerator RequestTournamentData(Action<bool> onComplete)
		{
			string tournamentId = null;
			TournamentData currentTournament = managerData.currentTournament;
			if (currentTournament != null)
			{
				tournamentId = currentTournament.id;
			}
			bool shouldUpdate = true;
			long now = CDateTime.UtcNow.ToUnixTime();
			if (this.tournament != null && tournamentId == this.tournament.id)
			{
				int num = ((!this.tournament.isFinished || this.tournament.hasResult) ? OnlineSettings.instance.GetValue("toprun_refresh_interval", 300) : OnlineSettings.instance.GetValue("toprun_refresh_interval_when_result_is_due", 30));
				if (_lastTournamentUpdate > now)
				{
					_lastTournamentUpdate = 0L;
				}
				shouldUpdate = now - _lastTournamentUpdate > num;
			}
			List<string> participantsPublicKeys = new List<string>(_currentTournamentPlayers.Keys);
			if (!shouldUpdate && _lastTournamentUpdateParticipantsPublicKeys != null)
			{
				foreach (string item in participantsPublicKeys)
				{
					if (!_lastTournamentUpdateParticipantsPublicKeys.Contains(item))
					{
						shouldUpdate = true;
						break;
					}
				}
			}
			if (shouldUpdate)
			{
				_lastTournamentUpdate = now;
				_lastTournamentUpdateParticipantsPublicKeys = participantsPublicKeys;
				IRequest request = KilooSerializerHTTPExtensions.WithJsonContent(jsonData: new Dictionary<string, object>
				{
					{ "TournamentId", tournamentId },
					{ "ParticipantsPublicKeys", participantsPublicKeys }
				}, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/tournaments/gettournamentbyid")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
					.WithMaxBackOffTime(requestMaxBackOffTime);
				yield return request.SendAsync();
				bool isTournamentNotFound = false;
				bool badResponse = true;
				IResponse response = request.response;
				string responseText = ((response != null) ? response.AsString() : null);
				if (request.isSuccessful && response != null && response.IsJson())
				{
					try
					{
						TimeSpan clientTimeOffset = GetClientTimeOffset(request);
						object json;
						try
						{
							json = KilooJson.Deserialize(responseText);
						}
						catch (Exception)
						{
							json = null;
						}
						TournamentData currentTournament2 = managerData.currentTournament;
						string text = currentTournament2.country;
						Tournament tournament = (Tournament)(this.tournament = new Tournament(new JsonObject(json), clientTimeOffset, text, this));
						currentTournament2.startTime = tournament.startTimeUTC;
						currentTournament2.endTime = tournament.endTimeUTC;
						currentTournament2.nextStartTime = tournament.nextTournamentTimeUTC;
						currentTournament2.sequenceId = tournament.sequenceId;
						if (tournament.hasResult)
						{
							currentTournament2.closed = true;
							int num2 = score;
							if (num2 > 0)
							{
								currentTournament2.rank = tournament.GetRankForScore(num2);
							}
						}
						Rank rank = null;
						TournamentData previousTournament = managerData.previousTournament;
						if (previousTournament != null)
						{
							int num3 = previousTournament.rank;
							string text2 = previousTournament.country;
							if (num3 >= 0 && text2 != null)
							{
								int sequenceId = previousTournament.sequenceId;
								if (sequenceId + 1 == tournament.sequenceId)
								{
									rank = new Rank(text2, num3);
								}
							}
						}
						this.rank = rank;
						foreach (KeyValuePair<string, Player> currentTournamentPlayer in _currentTournamentPlayers)
						{
							int num4 = tournament.FindParticipantScoreByPublicKey(currentTournamentPlayer.Key);
							currentTournamentPlayer.Value.score = num4;
						}
						badResponse = false;
					}
					catch (Exception ex2)
					{
						LogError("GetTournamentData: Error creating tournament object from received data: " + ex2);
					}
				}
				else if (response != null && response.status == 404)
				{
					isTournamentNotFound = true;
				}
				if (badResponse)
				{
					LogError("GetTournamentData: Request failed or bad response, status: " + request.statusMessage);
				}
				onComplete(isTournamentNotFound);
			}
			else
			{
				onComplete(false);
			}
		}

		private IEnumerator RequestNextTournamentFetchTime()
		{
			if (managerData.nextTournamentTime.HasValue)
			{
				yield break;
			}
			TournamentData tour = managerData.currentTournament;
			if (tour == null)
			{
				yield break;
			}
			string tourCountry = tour.country;
			if (country != tourCountry)
			{
				if (tour.submittedScore > 0)
				{
					string privateKey = managerData.localPlayerPrivateKey;
					if (string.IsNullOrEmpty(privateKey))
					{
						yield break;
					}
					Uri uri = new Uri(string.Format("{0}?PrivateKey={1}&ToCountry={2}", MakeApiURL("/players/getcountrychangedate"), Uri.EscapeDataString(privateKey), country));
					IRequest request = KilooWebClient.CreateGet(uri).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
						.WithMaxBackOffTime(requestMaxBackOffTime);
					yield return request.SendAsync();
					bool badResponse = true;
					IResponse response = request.response;
					string responseText = ((response != null) ? response.AsString() : null);
					if (request.isSuccessful && response != null && response.IsJson())
					{
						TimeSpan clientTimeOffset = GetClientTimeOffset(request);
						IDictionary dictionary;
						try
						{
							dictionary = KilooJson.Deserialize(responseText) as IDictionary;
						}
						catch (Exception)
						{
							dictionary = null;
						}
						DateTime result;
						if (dictionary != null && JsonObject.TryGet<DateTime>(dictionary, "ChangeDateUtc", out result))
						{
							result += clientTimeOffset;
							managerData.nextTournamentTime = result;
							badResponse = false;
						}
					}
					if (badResponse)
					{
						LogError("GetNextTournamentFetchTime: Request failed or bad response, status: " + request.statusMessage);
					}
				}
				else
				{
					managerData.nextTournamentTime = CDateTime.UtcNow;
				}
			}
			else
			{
				managerData.nextTournamentTime = null;
			}
		}

		private IList<string> GetPublicKeysForPlayersWeHaveNotRequestedRanksFor()
		{
			Dictionary<string, Rank> ranks = rankData.ranks;
			List<string> list = new List<string>(playerIdData.players.Count);
			foreach (KeyValuePair<string, PlayerIdsData.PlayerIdData> player in playerIdData.players)
			{
				PlayerIdsData.PlayerIdData value = player.Value;
				string text = ((value != null) ? value.publicKey : null);
				if (text != null && (ranks == null || !ranks.ContainsKey(text)))
				{
					list.Add(text);
				}
			}
			return list;
		}

		private IEnumerator RequestRanksForPlayers()
		{
			string currentTournamentId = null;
			TournamentData tour = managerData.currentTournament;
			if (tour != null)
			{
				currentTournamentId = tour.id;
			}
			string ranksUpdatedTournamentId = rankData.tournamentId;
			if (ranksUpdatedTournamentId == null || ranksUpdatedTournamentId != currentTournamentId)
			{
				rankData = new RanksData
				{
					ranks = new Dictionary<string, Rank>(playerIdData.players.Count + 1)
				};
			}
			IList<string> keysToGetRanksFor = GetPublicKeysForPlayersWeHaveNotRequestedRanksFor();
			if (keysToGetRanksFor.Count <= 0)
			{
				yield break;
			}
			yield return KilooSerializerHTTPExtensions.WithJsonContent(jsonData: new Dictionary<string, object> { { "PublicKeys", keysToGetRanksFor } }, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/tournaments/getprevioustournamentresultsforplayers")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
				.WithMaxBackOffTime(requestMaxBackOffTime)
				.ThenMakeTheRequestAsync((IRequest request, bool isFromCache) =>
				{
					bool flag = true;
					IResponse response = request.response;
					string json = ((response != null) ? response.AsString() : null);
					if (request.isSuccessful && response != null && response.IsJson())
					{
						IDictionary dictionary;
						try
						{
							dictionary = KilooJson.Deserialize(json) as IDictionary;
						}
						catch (Exception)
						{
							dictionary = null;
						}
						if (dictionary != null)
						{
							IList list = dictionary["PreviousTournamentResults"] as IList;
							if (list != null)
							{
								rankData.tournamentId = currentTournamentId;
								foreach (string item in keysToGetRanksFor)
								{
									Rank value = null;
									foreach (object item2 in list)
									{
										IDictionary dictionary2 = item2 as IDictionary;
										if (dictionary2 != null)
										{
											object obj = dictionary2["PublicKey"];
											if (obj != null)
											{
												if (item.Equals(obj))
												{
													string value2 = dictionary2["Country"] as string;
													int num = JsonObject.Get(dictionary2, "DivisionIndex", -1);
													if (num >= 0 && !string.IsNullOrEmpty(value2))
													{
														value = new Rank(value2, num);
													}
													else
													{
														LogError("GetRanksForPlayers: Invalid rank data received");
													}
													break;
												}
											}
											else
											{
												LogError("GetRanksForPlayers: Invalid rank data received");
											}
										}
										else
										{
											LogError("GetRanksForPlayers: Invalid rank data received");
										}
									}
									rankData.ranks[item] = value;
								}
								flag = false;
							}
						}
					}
					if (flag)
					{
						LogError("GetRanksForPlayers: Request failed or bad response, status: " + request.statusMessage);
					}
				});
		}

		private IEnumerator RequestSubmitScoreCoroutine(int scoreToSubmit, bool isRetry, bool previouslySavedScore)
		{
			if (!canSubmitScore)
			{
				if (!previouslySavedScore)
				{
					int unsubmittedScoreNoTournament = managerData.unsubmittedScoreNoTournament;
					if (scoreToSubmit > unsubmittedScoreNoTournament)
					{
						managerData.unsubmittedScoreNoTournament = scoreToSubmit;
						managerData.unsubmittedScoreNoTournamentTime = CDateTime.UtcNow;
					}
				}
				yield break;
			}
			TournamentData tour = managerData.currentTournament;
			int submittedScore = tour.submittedScore;
			if (scoreToSubmit <= submittedScore)
			{
				yield break;
			}
			bool scoreWasUpdated = false;
			int unsubmittedScore = tour.unsubmittedScore;
			if (scoreToSubmit > unsubmittedScore)
			{
				TournamentData tournamentData = tour;
				int unsubmittedScore2;
				unsubmittedScore = (unsubmittedScore2 = scoreToSubmit);
				tournamentData.unsubmittedScore = unsubmittedScore2;
				scoreWasUpdated = true;
			}
			if (_isRefreshingTournament)
			{
				do
				{
					yield return null;
				}
				while (_isRefreshingTournament);
			}
			_isRefreshingTournament = true;
			yield return CoroutineUtil.StartCoroutine(RefreshKeysIfNecessary());
			string privateKey = managerData.localPlayerPrivateKey;
			string tournamentId = tour.id;
			string tournamentCountry = tour.country;
			DateTime tournamentStart = tour.startTime;
			DateTime tournamentEnd = tour.endTime;
			if (privateKey != null && tournamentId != null && tournamentCountry != null && tournamentStart != DateTime.MinValue && tournamentEnd != DateTime.MinValue)
			{
				IRequest request = KilooSerializerHTTPExtensions.WithJsonContent(jsonData: new Dictionary<string, object>
				{
					{ "Score", unsubmittedScore },
					{ "Multiplier", multiplier },
					{ "PrivateKey", privateKey },
					{ "TournamentId", tournamentId },
					{
						"PlayerName",
						GetNameFromSocialManager()
					}
				}, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/scores/submitscore"))));
				yield return request.SendAsync();
				tour = managerData.currentTournament;
				if (tour != null && tournamentId == tour.id)
				{
					bool flag = true;
					IResponse response = request.response;
					string text = ((response != null) ? response.AsString() : null);
					if (request.isSuccessful && response != null && response.IsJson())
					{
						IDictionary dictionary;
						try
						{
							dictionary = KilooJson.Deserialize(text) as IDictionary;
						}
						catch (Exception)
						{
							dictionary = null;
						}
						if (dictionary != null)
						{
							if (JsonObject.Get(dictionary, "Success", false))
							{
								tour.unsubmittedScore = 0;
								tour.submittedScore = unsubmittedScore;
								managerData.lastSubmittedScoreCountry = tournamentCountry;
								managerData.lastSubmittedScoreStartTime = tournamentStart;
								managerData.lastSubmittedScoreEndTime = tournamentEnd;
							}
							else
							{
								tour.closed = true;
							}
							flag = false;
						}
					}
					else if (response != null)
					{
						if (response.status == 400)
						{
							if (text.Contains("Invalid PrivateKey"))
							{
								LogError("SubmitScore: Invalid User Info, discarding and retrying (once)");
								managerData.localPlayerFacebookId = null;
								managerData.localPlayerPrivateKey = null;
								managerData.localPlayerPublicKey = null;
								if (!isRetry)
								{
									_isRefreshingTournament = false;
									CoroutineUtil.StartCoroutine(RequestSubmitScoreCoroutine(scoreToSubmit, true, true));
									yield break;
								}
							}
						}
						else if (response.status == 404)
						{
							tour.closed = true;
						}
					}
					if (flag)
					{
						LogError("SubmitScore: Request failed or bad response, status: " + request.statusMessage);
					}
				}
				else
				{
					scoreWasUpdated = false;
				}
			}
			_isRefreshingTournament = false;
			if (scoreWasUpdated)
			{
				FireTournamentChangedEvent(true, false);
			}
		}

		private string GetFacebookIdFromSocialManager()
		{
			SocialManager instance = SocialManager.instance;
			if (instance.facebookIsLoggedIn)
			{
				FacebookProfile facebookProfile = instance.facebookProfile;
				if (facebookProfile != null)
				{
					return facebookProfile.id;
				}
			}
			return null;
		}

		private string GetNameFromSocialManager()
		{
			SocialManager instance = SocialManager.instance;
			if (instance.facebookIsLoggedIn)
			{
				FacebookProfile facebookProfile = instance.facebookProfile;
				if (facebookProfile != null)
				{
					return facebookProfile.name;
				}
			}
			return null;
		}

		private IEnumerator RefreshKeysIfNecessary()
		{
			string privateKey = managerData.localPlayerPrivateKey;
			if (privateKey != null)
			{
				string facebookId = GetFacebookIdFromSocialManager();
				if (facebookId == null)
				{
					yield break;
				}
				string privateKeyFacebookId = managerData.localPlayerFacebookId;
				if (privateKeyFacebookId != null)
				{
					if (facebookId != privateKeyFacebookId)
					{
						ClearKeys();
						yield return CoroutineUtil.StartCoroutine(LoginOrCreatePlayer());
					}
				}
				else
				{
					yield return CoroutineUtil.StartCoroutine(LoginOrRegisterFacebook());
				}
			}
			else
			{
				yield return CoroutineUtil.StartCoroutine(LoginOrCreatePlayer());
			}
		}

		private IEnumerator LoginOrRegisterFacebook()
		{
			FacebookLoginResult facebookLoginResult = FacebookLoginResult.Failed;
			yield return CoroutineUtil.StartCoroutine(RequestFacebookLogin((FacebookLoginResult result) =>
			{
				facebookLoginResult = result;
			}));
			if (facebookLoginResult == FacebookLoginResult.NotFound)
			{
				yield return CoroutineUtil.StartCoroutine(RequestFacebookRegister());
			}
		}

		private IEnumerator LoginOrCreatePlayer()
		{
			string facebookId = GetFacebookIdFromSocialManager();
			if (facebookId != null)
			{
				FacebookLoginResult facebookLoginResult = FacebookLoginResult.Failed;
				yield return CoroutineUtil.StartCoroutine(RequestFacebookLogin((FacebookLoginResult result) =>
				{
					facebookLoginResult = result;
				}));
				if (facebookLoginResult != FacebookLoginResult.NotFound)
				{
					yield break;
				}
			}
			yield return CoroutineUtil.StartCoroutine(RequestCreatePlayer());
		}

		private IEnumerator RequestFacebookLogin(Action<FacebookLoginResult> onComplete)
		{
			string facebookId = GetFacebookIdFromSocialManager();
			ulong longFbId;
			if (facebookId != null && ulong.TryParse(facebookId, NumberStyles.None, CultureInfo.InvariantCulture, out longFbId))
			{
				yield return KilooSerializerHTTPExtensions.WithJsonContent(jsonData: new Dictionary<string, object> { { "FacebookId", longFbId } }, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/facebook/login")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
					.WithMaxBackOffTime(requestMaxBackOffTime)
					.ThenMakeTheRequestAsync((IRequest request, bool isFromCache) =>
					{
						bool flag = true;
						FacebookLoginResult obj = FacebookLoginResult.Failed;
						IResponse response = request.response;
						string json = ((response != null) ? response.AsString() : null);
						if (request.isSuccessful && response != null && response.IsJson())
						{
							IDictionary dictionary;
							try
							{
								dictionary = KilooJson.Deserialize(json) as IDictionary;
							}
							catch (Exception)
							{
								dictionary = null;
							}
							if (dictionary != null)
							{
								string text = dictionary["PrivateKey"] as string;
								string text2 = dictionary["PublicKey"] as string;
								if (text != null && text2 != null)
								{
									managerData.localPlayerFacebookId = facebookId;
									managerData.localPlayerPrivateKey = text;
									managerData.localPlayerPublicKey = text2;
									obj = FacebookLoginResult.Success;
									flag = false;
								}
							}
						}
						else if (request.response != null && request.response.status == 404)
						{
							obj = FacebookLoginResult.NotFound;
							flag = false;
						}
						if (flag)
						{
							LogError("FacebookLogin: Request failed or bad response, status: " + request.statusMessage);
						}
						onComplete(obj);
					});
			}
			else
			{
				onComplete(FacebookLoginResult.Failed);
			}
		}

		private IEnumerator RequestCreatePlayer()
		{
			string facebookId = GetFacebookIdFromSocialManager();
			ulong longFbId = 0uL;
			if (facebookId != null)
			{
				ulong.TryParse(facebookId, NumberStyles.None, CultureInfo.InvariantCulture, out longFbId);
				if (longFbId == 0)
				{
					facebookId = null;
				}
			}
			yield return KilooSerializerHTTPExtensions.WithJsonContent(jsonData: new Dictionary<string, object> { { "FacebookId", longFbId } }, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/players/createplayer")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
				.WithMaxBackOffTime(requestMaxBackOffTime)
				.ThenMakeTheRequestAsync((IRequest request, bool isFromCache) =>
				{
					bool flag = true;
					IResponse response = request.response;
					string json = ((response != null) ? response.AsString() : null);
					if (request.isSuccessful && response != null && response.IsJson())
					{
						IDictionary dictionary;
						try
						{
							dictionary = KilooJson.Deserialize(json) as IDictionary;
						}
						catch (Exception)
						{
							dictionary = null;
						}
						if (dictionary != null)
						{
							string text = dictionary["PrivateKey"] as string;
							string text2 = dictionary["PublicKey"] as string;
							if (text != null && text2 != null)
							{
								managerData.localPlayerFacebookId = facebookId;
								managerData.localPlayerPrivateKey = text;
								managerData.localPlayerPublicKey = text2;
								flag = false;
							}
						}
					}
					if (flag)
					{
						LogError("CreatePlayer: Request failed or bad response, status: " + request.statusMessage);
					}
				});
		}

		private IEnumerator RequestFacebookRegister()
		{
			string privateKey = managerData.localPlayerPrivateKey;
			string facebookId = GetFacebookIdFromSocialManager();
			ulong longFbId;
			if (privateKey != null && facebookId != null && ulong.TryParse(facebookId, NumberStyles.None, CultureInfo.InvariantCulture, out longFbId))
			{
				yield return KilooSerializerHTTPExtensions.WithJsonContent(jsonData: new Dictionary<string, object>
				{
					{ "PrivateKey", privateKey },
					{ "FacebookId", longFbId }
				}, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/facebook/register")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
					.WithMaxBackOffTime(requestMaxBackOffTime)
					.ThenMakeTheRequestAsync((IRequest request, bool isFromCache) =>
					{
						bool flag = true;
						IResponse response = request.response;
						string json = ((response != null) ? response.AsString() : null);
						if (request.isSuccessful && response != null && response.IsJson())
						{
							IDictionary dictionary;
							try
							{
								dictionary = KilooJson.Deserialize(json) as IDictionary;
							}
							catch (Exception)
							{
								dictionary = null;
							}
							if (dictionary != null && JsonObject.Get(dictionary, "Success", false))
							{
								managerData.localPlayerFacebookId = facebookId;
								flag = false;
							}
						}
						if (flag)
						{
							LogError("FacebookRegister: Request failed or bad response, status: " + request.statusMessage);
						}
					});
			}
			else
			{
				LogError("FacebookRegister: Cannot register now, missing private key and/or facebook id.");
			}
		}

		private void ClearKeys()
		{
			managerData.localPlayerFacebookId = null;
			managerData.localPlayerPrivateKey = null;
			managerData.localPlayerPublicKey = null;
		}

		public static string QuoteString(string s)
		{
			if (s != null)
			{
				return '"' + s + '"';
			}
			return "null";
		}

		public static void LogError(object msg)
		{
			SDebug.LogError("[TopRun.Manager] " + msg);
		}

		public static void LogWarning(object msg)
		{
		}

		[Conditional("TOPRUN_DEBUG_LOGS")]
		public static void Log(object msg)
		{
		}
	}
}
