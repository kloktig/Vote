using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using vote.Poll;

namespace vote.Votes
{
    [ApiController]
    [Route("admin/votes")]
    public class VoteAdminController : ControllerBase
    {
        private readonly PollRepo _pollRepo;
        private readonly VotesRepo _votesStorage;

        public VoteAdminController(PollRepo pollRepo)
        {
            _pollRepo = pollRepo;
            _votesStorage = new VotesRepo();
        }

        [HttpGet]
        [Route("voteCounts/{id}")]
        public IActionResult GetVoteCounts(string id)
        {
            var pollDto = PollDto.From(_pollRepo.FindEntity(id));
            
            var votes = pollDto.Participants.SelectMany(part => GetVotesInRange(part.Name, pollDto.StartTime!.Value, pollDto.EndTime)).ToImmutableList();
            var totalCount = votes.Count;
            var counts = pollDto.Participants.Select(p =>
            {
                var count = votes.Count(v => v.Name == p.Name);
                var pct = totalCount == 0
                    ? decimal.Zero
                    : 100 * Convert.ToDecimal(count) / Convert.ToDecimal(totalCount);
                return new VoteCount(p.Name, count, pct);
            });
            return Ok(counts);
        }

        private IImmutableList<VoteEntity> GetVotesInRange(string name, DateTimeOffset from, DateTimeOffset? to)
        {
            var allVotes = _votesStorage.Read(name, from, to ?? DateTimeOffset.MaxValue);
            var votes = to.HasValue switch
            {
                 true => allVotes.TakeWhile(v => v.Timestamp <= to),
                 false => allVotes
            };
            return votes.ToImmutableList();
        }
    }
}