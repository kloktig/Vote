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

        public async Task<CurrentDto> WriteCurrent(IList<ParticipantDto> currentParticipants)
        {
            
            CurrentEntity currentEntity = CurrentEntity.Create(currentParticipants);
            await _currentTableClient.AddEntityAsync(currentEntity);
            return new CurrentDto {Participants = currentParticipants, Id = currentEntity.RowKey};
        }
        
        public CurrentDto GetCurrent()
        {
            var currentEntity = ListCurrents().FirstOrDefault();
            if (currentEntity == null)
            {
                throw new NotSupportedException("Should not be null");
            }
            var participants = JsonSerializer.Deserialize<IList<ParticipantDto>>(currentEntity.Participants) ?? ImmutableList<ParticipantDto>.Empty;
            return new CurrentDto
            {
                Participants = participants,
                Id = currentEntity.RowKey,
                StartTime = currentEntity.Timestamp.Value
            };
        }
        
        public ImmutableList<CurrentEntity> ListCurrents()
        {
            var pages = _currentTableClient.Query<CurrentEntity>().AsPages().ToImmutableList();
            if (pages.Count > 1)
                throw new Exception("Assuming we have only one page");
            return pages.First().Values.OrderByDescending(p => p.Timestamp).ToImmutableList();
        }
        
        public (CurrentDto dto, DateTimeOffset? endTime) FindEntry(string id)
        {
            var pages = _currentTableClient.Query<CurrentEntity>().AsPages().ToImmutableList();
            if (pages.Count > 1)
                throw new Exception("Assuming we have only one page");

            var currentEntities = pages
                .First().Values
                .OrderByDescending(p => p.Timestamp)
                .ToImmutableList();
            
            var index = currentEntities.FindIndex(p => p.RowKey == id);
            
            if (index < 0)
            {
                throw new NotSupportedException("Could not find entity");
            }

            var currentEntity = currentEntities[index];
            if (currentEntity.Timestamp == null)
            {
                throw new NotSupportedException("Timestamp must be set");
            }
            var participants = JsonSerializer.Deserialize<IList<ParticipantDto>>(currentEntity.Participants) ?? ImmutableList<ParticipantDto>.Empty;
            var currentDto = new CurrentDto
            {
                Participants = participants,
                Id = currentEntity.RowKey,
                StartTime = currentEntity.Timestamp.Value
            };
            var endTime = index > 0 ? currentEntities[index - 1].Timestamp : null;
            return (currentDto, endTime);
        }

    }
}