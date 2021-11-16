using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
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

        public async Task<CurrentDto> AddEndTime(string id, DateTimeOffset endTime)
        {
            var entityToUpdate = FindEntity(id);
            var entityWithUpdate = entityToUpdate with{ EndTime = endTime};
            await _currentTableClient.UpdateEntityAsync(entityWithUpdate, ETag.All);
            return CurrentDto.From(entityWithUpdate);
        }
        
        public async Task<CurrentDto> WriteCurrent(IList<ParticipantDto> currentParticipants, DateTimeOffset? endTime)
        {
            CurrentEntity currentEntity = CurrentEntity.Create(currentParticipants, endTime);
            await _currentTableClient.AddEntityAsync(currentEntity);
            return CurrentDto.From(currentEntity);
        }
        
        public CurrentDto GetCurrent()
        {
            var currentEntity = ListCurrents().FirstOrDefault();
            if (currentEntity == null)
            {
                throw new NotSupportedException("Should not be null");
            }
            return CurrentDto.From(currentEntity);
        }
        
        public ImmutableList<CurrentEntity> ListCurrents()
        {
            var pages = _currentTableClient.Query<CurrentEntity>().AsPages().ToImmutableList();
            if (pages.Count > 1)
                throw new Exception("Assuming we have only one page");
            return pages.First().Values.OrderByDescending(p => p.Timestamp).ToImmutableList();
        }
        
        public CurrentEntity FindEntity(string id)
        {
            // TODO: Filter
            var currentEntities = ListCurrents();
            
            var index = currentEntities.FindIndex(p => p.RowKey == id);
            if (index < 0)
            {
                throw new NotSupportedException("Could not find entity");
            }
            
            return currentEntities[index];
        }
        
    }
}