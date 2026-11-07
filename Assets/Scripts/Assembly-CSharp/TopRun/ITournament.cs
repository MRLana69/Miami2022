using System;
using System.Collections.Generic;

namespace TopRun
{
	public interface ITournament
	{
		string id { get; }

		int sequenceId { get; }

		bool isFinished { get; }

		bool hasResult { get; }

		string country { get; }

		DateTime startTimeUTC { get; }

		DateTime officialEndTimeUTC { get; }

		DateTime endTimeUTC { get; }

		DateTime nextTournamentTimeUTC { get; }

		int rankCount { get; }

		int GetRankForScore(int score);

		IEnumerable<IPlayer> GetPlayers();

		IEnumerable<IPlayer> GetPlayersForRank(int rank);

		int GetBestScoreForRank(int rank);

		int GetNumberOfPlayersForRank(int rank);

		int GetNumberOfPlayersForTournament();
	}
}
