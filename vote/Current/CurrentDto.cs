using System;
using System.Collections.Generic;
using vote.Participant;

namespace vote.Current
{
    public record CurrentDto
    {
        public string Id { get; init; }
        public IList<ParticipantDto> Participants { get; init; }
        public DateTimeOffset StartTime { get; init; }
        public DateTimeOffset? EndTime { get; init; }

    }
}