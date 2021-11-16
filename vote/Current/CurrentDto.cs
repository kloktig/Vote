using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text.Json;
using vote.Participant;

namespace vote.Current
{
    public record CurrentDto
    {
        public string Id { get; init; }
        public IList<ParticipantDto> Participants { get; init; }
        public DateTimeOffset? StartTime { get; init; }
        public DateTimeOffset? EndTime { get; init; }

        public static CurrentDto From(CurrentEntity entity)
        {
            return new CurrentDto
            {
                Id = entity.RowKey,
                Participants = JsonSerializer.Deserialize<IList<ParticipantDto>>(entity.Participants) ??
                               ImmutableList<ParticipantDto>.Empty,
                StartTime = entity.Timestamp,
                EndTime = entity.EndTime,
            };
        } 

    }
}