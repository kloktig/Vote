using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using vote.Participant;

namespace vote.Current
{
    public record CurrentEntity : ITableEntity
    {
        public static CurrentEntity Create(IList<ParticipantDto> participants)
        {
            return new CurrentEntity
            {
                PartitionKey = "Current",
                RowKey = Guid.NewGuid().ToString(),
                Participants = JsonSerializer.Serialize(participants),
            };
        }

        public string Participants { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}