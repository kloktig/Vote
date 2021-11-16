using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using vote.Participant;

namespace vote.Poll
{
    public class PollRepo
    {
        private readonly TableClient _tableClient;
        
        public PollRepo()
        {
            TableServiceClient serviceClient = new(Common.DevConnectionString);
            serviceClient.CreateTableIfNotExists("poll");
            _tableClient = serviceClient.GetTableClient("poll");
        }

        public async Task<PollDto> AddEndTime(string id, DateTimeOffset endTime)
        {
            var entityToUpdate = FindEntity(id);
            var entityWithUpdate = entityToUpdate with{ EndTime = endTime};
            await _tableClient.UpdateEntityAsync(entityWithUpdate, ETag.All);
            return PollDto.From(entityWithUpdate);
        }
        
        public async Task WritePoll(IList<ParticipantDto> participants, DateTimeOffset? endTime)
        {
            PollEntity pollEntity = PollEntity.Create(participants, endTime);
            await _tableClient.AddEntityAsync(pollEntity);
        }
        
        public PollDto GetLastPoll()
        {
            var lastStartedPoll = ListPolls().FirstOrDefault();
            return lastStartedPoll == null ? PollDto.Default : PollDto.From(lastStartedPoll);
        }
        
        public ImmutableList<PollEntity> ListPolls(string? id = null)
        {
            var filter = id == null ? null : $"RowKey eq '{id}'";
            var pages = _tableClient.Query<PollEntity>(filter).AsPages().ToImmutableList();
            if (pages.Count > 1)
                throw new Exception("Assuming we have only one page");
            return pages.First().Values.OrderByDescending(p => p.Timestamp).ToImmutableList();
        }
        
        public PollEntity FindEntity(string id)
        {
            return ListPolls(id).First();
        }
        
    }
}