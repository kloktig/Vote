using System;
using Azure;
using Azure.Data.Tables;
using vote.Participant;
#pragma warning disable 8618

namespace vote.Votes
{
    public record VoteEntity : ITableEntity
    {
        public static VoteEntity From(string uid, ParticipantDto participant)
        {
            return new VoteEntity
            {
                PartitionKey = uid,
                RowKey = Guid.NewGuid().ToString(),
                Name = participant.Name
            };
        }

        public string Name { get; init; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}