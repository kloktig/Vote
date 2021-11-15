using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;
using vote.Participant;

namespace vote.Current
{
    public class CurrentRepo
    {
        private readonly TableClient _currentTableClient;
        public CurrentRepo()
        {
            TableServiceClient serviceClient = new(CommonPaths.DevConnectionString);
            serviceClient.CreateTableIfNotExists("current");
            _currentTableClient = serviceClient.GetTableClient("current");
        }

        public async Task WriteCurrent(IList<ParticipantDto> currentParticipants)
        {
            CurrentEntity currentEntity = CurrentEntity.Create(currentParticipants);
            await _currentTableClient.AddEntityAsync(currentEntity);
        }
        
        public IList<ParticipantDto> GetCurrent()
        {
            var pages = _currentTableClient.Query<CurrentEntity>().AsPages().ToImmutableList();
            if (pages.Count > 1)
                throw new Exception("Assuming we have only one page");
            var participants = pages.First().Values.OrderByDescending(p => p.Timestamp).FirstOrDefault()?.Participants;
            if (participants == null)
            {
                return ImmutableList<ParticipantDto>.Empty;
            }
            return JsonSerializer.Deserialize<IList<ParticipantDto>>(participants) ?? ImmutableList<ParticipantDto>.Empty;
        }
    }
}