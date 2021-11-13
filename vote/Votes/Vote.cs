using System;

namespace vote.Votes
{
    public record Vote
    {
        public Participant.Participant Participant { get; init; }
        public DateTime TimeOfVote { get; init; }

        public static Vote From(Participant.Participant participant)
        {
            return new Vote
            {
                Participant = participant, 
                TimeOfVote = DateTime.Now
            };
        }
    }
}