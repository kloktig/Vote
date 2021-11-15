using System;
using System.Collections.Immutable;
using System.Linq;
using Azure.Data.Tables;

namespace vote.Participant
{
    public class ParticipantRepo
    {
        private readonly TableClient _participantsTableClient;
       
        public ParticipantRepo()
        {
            TableServiceClient serviceClient = new(CommonPaths.DevConnectionString);

            serviceClient.CreateTableIfNotExists("participants");
            serviceClient.CreateTableIfNotExists("current");

            _participantsTableClient = serviceClient.GetTableClient("participants");
        }

        private ImmutableList<ParticipantEntity> ReadParticipants()
        {
            var pages = _participantsTableClient.Query<ParticipantEntity>().AsPages().ToImmutableList();
            if (pages.Count > 1)
                throw new Exception("Assuming we have only one page");
            return pages.First().Values.ToImmutableList();
        }

        public ImmutableList<ParticipantDto> GetParticipants()
        {
            return ReadParticipants()
                .Select(participantEntity => new ParticipantDto()
                {
                    Name = participantEntity.Name,
                }).ToImmutableList();
        }

        public void AddParticipant(ParticipantDto participantDto)
        {
            if (ReadParticipants().Any(participantEntity => participantEntity.Name == participantDto.Name))
            {
                throw new NotSupportedException("Cannot add same participant serveral times");
            }

            var entity = ParticipantEntity.Create(participantDto.Name);
            _participantsTableClient.UpsertEntityAsync(entity);
        }

        public void RemoveParticipant(ParticipantDto participantDto)
        {
            var participantToDelete = ReadParticipants().FirstOrDefault(p => p.Name == participantDto.Name);
            if (participantToDelete == null)
            {
                throw new NotSupportedException("Not able to delete participant - It does not exist");
            }

            _participantsTableClient.DeleteEntityAsync(participantToDelete.PartitionKey, participantToDelete.RowKey,
                participantToDelete.ETag);
        }
    }
}