using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using vote.Participant;

namespace vote.Votes
{
    public class VotesRepo
    {
        private readonly TableClient _table;

        public VotesRepo()
        {
            TableServiceClient serviceClient = new(Common.DevConnectionString);
            serviceClient.CreateTableIfNotExists("votes");
            _table = serviceClient.GetTableClient("votes");
        }

        public async Task Write(string uid, ParticipantDto vote)
        {
            await _table.AddEntityAsync(VoteEntity.From(uid, vote));
        }

        public IList<VoteEntity> Read(string name, DateTimeOffset startTime, DateTimeOffset endtime)
        {
            var filter = $"Name eq '{name}'";
            var pages = _table.Query<VoteEntity>(filter).AsPages().ToImmutableList();
            if (pages.Count > 1)
                throw new Exception("Assuming we have only one page");
            return pages.First().Values.OrderBy(entity => entity.Timestamp)
                .SkipWhile(e => e.Timestamp < startTime)
                .TakeWhile(e => e.Timestamp <= endtime)
                .ToImmutableList();
        }
    }
}