using System;
using JetBrains.Annotations;

namespace TopRun
{
	public interface IManager
	{
		[NotNull]
		string country { get; set; }

		int multiplier { get; }

		int highScore { get; }

		bool canSubmitScore { get; }

		bool isRefreshing { get; }

		bool pauseRefreshing { get; set; }

		bool hasFetchedTournament { get; }

		int score { get; }

		[CanBeNull]
		IRank rank { get; }

		DateTime nextTournamentFetchTime { get; }

		[CanBeNull]
		ITournament tournament { get; }

		event EventHandler<TournamentChangedEventArgs> tournamentChanged;

		event EventHandler<PlayerInfoChangedEventArgs> playerInfoChanged;

		[CanBeNull]
		IRank GetPlayerRank(string facebookId);

		void FetchNextTournament();

		void RefreshTournament();

		void SubmitScore(int scoreToSubmit, DateTime gameStartedTime);
	}
}
