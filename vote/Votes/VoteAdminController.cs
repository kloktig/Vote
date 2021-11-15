using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using vote.Current;
using vote.Participant;

namespace vote.Votes
{
    [ApiController]
    [Route("admin/votes")]
    public class VoteAdminController : ControllerBase
    {
        private readonly ParticipantRepo _participantRepo;
        private readonly CurrentRepo _currentRepo;
        private readonly VotesRepo _votesStorage;

        public VoteAdminController(ParticipantRepo participantRepo, CurrentRepo currentRepo)
        {
            _participantRepo = participantRepo;
            _currentRepo = currentRepo;
            _votesStorage = new VotesRepo();
        }

        [HttpGet]
        [Route("voteCounts/{id}")]
        public IActionResult GetVoteCounts(string id)
        {
            var (currentDto, end) = _currentRepo.FindEntry(id);
            
            var votes = currentDto.Participants.SelectMany(part => GetVotesInRange(part.Name, currentDto.StartTime, end)).ToImmutableList();
            var totalCount = votes.Count;
            var counts = currentDto.Participants.Select(p =>
            {
                var count = votes.Count(v => v.Name == p.Name);
                var pct = totalCount == 0
                    ? decimal.Zero
                    : 100 * Convert.ToDecimal(count) / Convert.ToDecimal(totalCount);
                return new VoteCount
                {
                    Name = p.Name,
                    Count = count,
                    Percentage = pct
                };
            });
            return Ok(counts);
        }

        private IImmutableList<VoteEntity> GetVotesInRange(string name, DateTimeOffset from, DateTimeOffset? to)
        {
            var allVotes = _votesStorage.Read(name, from);
            var votes = to.HasValue switch
            {
                 true => allVotes.TakeWhile(v => v.Timestamp <= to),
                 false => allVotes
            };
            return votes.ToImmutableList();
        }
    }
}