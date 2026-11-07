using System;
using System.Collections.Generic;
using Kiloo;
using Kiloo.Common;

namespace TopRun.Internal
{
	internal class Tournament : ITournament
	{
		private struct Rank
		{
			public int numberOfPlayers;

			public int bestScore;
		}

		private struct Participant
		{
			public string publicKey;

			public int highScore;
		}

		private readonly ITournamentPlayerManager _tournamentPlayerManager;

		private readonly int _totalNumberOfPlayers;

		private readonly Rank[] _ranks;

		private readonly Participant[] _participants;

		public string id { get; private set; }

		public int sequenceId { get; private set; }

		public bool isFinished
		{
			get
			{
				return hasResult || CDateTime.UtcNow > endTimeUTC;
			}
		}

		public bool hasResult { get; private set; }

		public string country { get; private set; }

		public DateTime startTimeUTC { get; private set; }

		public DateTime officialEndTimeUTC
		{
			get
			{
				return endTimeUTC;
			}
		}

		public DateTime endTimeUTC { get; private set; }

		public DateTime nextTournamentTimeUTC { get; private set; }

		public int rankCount
		{
			get
			{
				return _ranks.Length;
			}
		}

		public Tournament(JsonObject json, TimeSpan clientTimeOffset, string country, ITournamentPlayerManager tournamentPlayerManager)
		{
			this.country = country;
			_tournamentPlayerManager = tournamentPlayerManager;
			id = json.Get<string>("TournamentId");
			sequenceId = json.Get<int>("SequenceId");
			endTimeUTC = json.Get<DateTime>("CurrentTournamentEndDateUtc").ToUniversalTime() + clientTimeOffset;
			nextTournamentTimeUTC = json.Get<DateTime>("NextTournamentStartDateUtc").ToUniversalTime() + clientTimeOffset;
			startTimeUTC = json.Get<DateTime>("CurrentTournamentStartDateUtc").ToUniversalTime() + clientTimeOffset;
			hasResult = json.Get("LatestCalculationDateUtc", DateTime.MinValue).ToUniversalTime() + clientTimeOffset >= endTimeUTC;
			IList<object> list = json.Get<IList<object>>("TournamentDivisions");
			_ranks = new Rank[list.Count];
			int num = 0;
			int num2 = 0;
			foreach (object item in list)
			{
				_ranks[num2] = new Rank
				{
					bestScore = JsonObject.Get<int>(item, "BestScore"),
					numberOfPlayers = JsonObject.Get<int>(item, "NumberOfPlayers")
				};
				num += _ranks[num2].numberOfPlayers;
				num2++;
			}
			_totalNumberOfPlayers = num;
			IList<object> list2 = json.Get<IList<object>>("TournamentParticipants");
			_participants = new Participant[list2.Count];
			num2 = 0;
			foreach (object item2 in list2)
			{
				_participants[num2] = new Participant
				{
					publicKey = JsonObject.Get<string>(item2, "PublicKey"),
					highScore = JsonObject.Get<int>(item2, "HighScore")
				};
				num2++;
			}
			Array.Sort(_participants, (Participant player1, Participant player2) => player2.highScore - player1.highScore);
		}

		public int FindParticipantScoreByPublicKey(string publicKey)
		{
			Participant[] participants = _participants;
			for (int i = 0; i < participants.Length; i++)
			{
				Participant participant = participants[i];
				if (participant.publicKey == publicKey)
				{
					return participant.highScore;
				}
			}
			return 0;
		}

		public int GetRankForScore(int score)
		{
			for (int num = _ranks.Length - 1; num > 0; num--)
			{
				if (score < _ranks[num].bestScore)
				{
					return num;
				}
			}
			return 0;
		}

		public IEnumerable<IPlayer> GetPlayers()
		{
			Participant[] participants = _participants;
			for (int i = 0; i < participants.Length; i++)
			{
				Participant participant = participants[i];
				IPlayer player = _tournamentPlayerManager.GetPlayerByPublicKey(participant.publicKey);
				if (player != null)
				{
					yield return player;
				}
			}
		}

		public IEnumerable<IPlayer> GetPlayersForRank(int rank)
		{
			int lowest = ((rank != _ranks.Length - 1) ? (_ranks[rank + 1].bestScore + 1) : 0);
			int highest = _ranks[rank].bestScore;
			Participant[] participants = _participants;
			for (int i = 0; i < participants.Length; i++)
			{
				Participant participant = participants[i];
				if (participant.highScore > highest)
				{
					if (rank != 0)
					{
						continue;
					}
				}
				else if (participant.highScore < lowest)
				{
					break;
				}
				IPlayer player = _tournamentPlayerManager.GetPlayerByPublicKey(participant.publicKey);
				if (player != null)
				{
					yield return player;
				}
			}
		}

		public int GetBestScoreForRank(int rank)
		{
			return _ranks[rank].bestScore;
		}

		public int GetNumberOfPlayersForRank(int rank)
		{
			return _ranks[rank].numberOfPlayers;
		}

		public int GetNumberOfPlayersForTournament()
		{
			return _totalNumberOfPlayers;
		}
	}
}
