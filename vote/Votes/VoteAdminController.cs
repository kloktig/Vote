using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
        [Route("voteCounts")]
        public IActionResult GetVoteCounts([FromQuery] DateTime? from, [FromQuery] DateTime? to,
            [FromQuery] bool? current)
        {
            var participants = current == true ? _currentRepo.GetCurrent() : _participantRepo.GetParticipants();
            var votes = participants.SelectMany(part => GetVotesInRange(part.Name, from, to)).ToImmutableList();
            var totalCount = votes.Count;
            var counts = participants.Select(p =>
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

        private IImmutableList<VoteEntity> GetVotesInRange(string name, DateTime? from, DateTime? to)
        {
            var allVotes = _votesStorage.Read(name);
            var votes = (from.HasValue, to.HasValue) switch
            {
                (true, true) => allVotes.SkipWhile(v => v.Timestamp < from).TakeWhile(v => v.Timestamp <= to),
                (false, true) => allVotes.TakeWhile(v => v.Timestamp <= to),
                (true, false) => allVotes.SkipWhile(v => v.Timestamp < from),
                (false, false) => allVotes
            };
            return votes.ToImmutableList();
        }
    }
}