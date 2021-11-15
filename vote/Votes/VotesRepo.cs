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
            TableServiceClient serviceClient = new(CommonPaths.DevConnectionString);
            serviceClient.CreateTableIfNotExists("votes");
            _table = serviceClient.GetTableClient("votes");
        }

        public async Task Write(ParticipantDto vote)
        {
            await _table.AddEntityAsync(VoteEntity.From(vote));
        }

        public IList<VoteEntity> Read(string name, DateTimeOffset startTime)
        {
            var partitionKey = name;
            var pages = _table.Query<VoteEntity>($"PartitionKey eq '{partitionKey}'").AsPages().ToImmutableList();
            if (pages.Count > 1)
                throw new Exception("Assuming we have only one page");
            return pages.First().Values.OrderBy(entity => entity.Timestamp).SkipWhile(v => v.Timestamp < startTime).ToImmutableList();
        }
    }
}