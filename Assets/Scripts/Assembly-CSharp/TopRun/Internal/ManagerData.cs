using System;

namespace TopRun.Internal
{
	public class ManagerData
	{
		public string country;

		public TournamentData currentTournament;

		public string localPlayerPrivateKey;

		public string localPlayerPublicKey;

		public string localPlayerFacebookId;

		public string lastSubmittedScoreCountry;

		public DateTime lastSubmittedScoreStartTime;

		public DateTime lastSubmittedScoreEndTime;

		public DateTime? nextTournamentTime;

		public TournamentData previousTournament;

		public int unsubmittedScoreNoTournament;

		public DateTime? unsubmittedScoreNoTournamentTime;
	}
}
