using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace vote.Participant
{
    public record ParticipantRepo
    {
        private readonly FileRepo<IList<Participant>> _participantFileRepo;
        private readonly FileRepo<IList<Participant>> _currentFileRepo;

        public ParticipantRepo()
        {
            _participantFileRepo = new FileRepo<IList<Participant>>(Path.Join(CommonPaths.BasePath,"participants.json"), ImmutableList<Participant>.Empty);
            _currentFileRepo = new FileRepo<IList<Participant>>( Path.Join(CommonPaths.BasePath, "current.json"), ImmutableList<Participant>.Empty);
        }
        
        public ImmutableList<Participant> GetParticipants()
        {
            return _participantFileRepo.Read().ToImmutableList();
        }

        public void AddParticipant(Participant participant)
        {
            if (GetParticipants().Contains(participant))
            {
                throw new Exception("Participant already exist");
            }
            _participantFileRepo.Write(GetParticipants().Add(participant));
        }
        
        public void RemoveParticipant(Participant participant)
        {
            var participants = GetParticipants();
            if (!participants.Contains(participant))
            {
                throw new ArgumentOutOfRangeException($"Cannot delete {participant}. It is not in the list");
            }
            _participantFileRepo.Write(participants.Remove(participant));
        }
        
        public IList<Participant> GetCurrent()
        {
            return _currentFileRepo.Read();
        }
        
        public void SetCurrent(IList<Participant> participant)
        {
            var participants = GetParticipants();
            var foundParticipants = participants.FindAll(participant.Contains);
            if (foundParticipants.Count != participant.Count)
            {
                throw new Exception($"Not all participants {participant} found, in {foundParticipants}");
            }
            _currentFileRepo.Write(foundParticipants);
        }
    }
}