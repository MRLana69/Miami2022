using System;
using System.Collections;

namespace TopRun.Internal
{
	public class TournamentData
	{
		public string id;

		public string country;

		public IList participants;

		public IList scoreMatchedParticipants;

		public DateTime? participantsTimeStamp;

		public DateTime? scoreMatchedParticipantsTimeStamp;

		public DateTime startTime;

		public DateTime endTime;

		public DateTime nextStartTime;

		public int sequenceId = -1;

		public int unsubmittedScore;

		public int submittedScore;

		public bool closed;

		public int rank = -1;
	}
}
