using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using vote.Participant;

namespace vote.Votes
{
    [ApiController]
    [Route("[controller]")]
    public class VoteController : ControllerBase
    {
        private readonly ParticipantRepo _participantRepo;
        private readonly ListFileRepo<Vote> _votesFileRepo;
        
        public VoteController(ParticipantRepo participantRepo)
        {
            _participantRepo = participantRepo;
            _votesFileRepo = new ListFileRepo<Vote>(Path.Join(CommonPaths.BasePath, "votes.json"));
        }

        [HttpPost]
        public IActionResult AddVote(Participant.Participant participant)
        {
            if (!_participantRepo.GetCurrent().Contains(participant))
            {
                return BadRequest();
            }
            _votesFileRepo.Append(Vote.From(participant));
            return Accepted();
        }

        [HttpGet]
        [Route("votes")] 
        public IActionResult GetVotes([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var votes = GetVotesInRange(from, to);
            return Ok(votes);
        }
        
        [HttpGet]
        [Route("voteCounts")] 
        public IActionResult GetVoteCounts([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] bool? current)
        {
            var participants = current == true ? _participantRepo.GetCurrent() : _participantRepo.GetParticipants();
            var votes = GetVotesInRange(from, to).Where(v => participants.Contains(v.Participant)).ToImmutableList();
            var totalCount = votes.Count;
            var counts  = participants.Select(p =>
            {
                var count = votes.Count(v => v.Participant == p);
                var pct = 100 * Convert.ToDecimal(count) / Convert.ToDecimal(totalCount);
                return new VoteCount
                {
                    Name = p.Name,
                    Count = count,
                    Percentage = pct
                };
            });
            return Ok(counts);
        }

        private IEnumerable<Vote> GetVotesInRange(DateTime? from, DateTime? to)
        {
            var allVotes = _votesFileRepo.Read().OrderBy(v => v.TimeOfVote);
            var votes = (from.HasValue, to.HasValue) switch
            {
                (true, true) => allVotes.SkipWhile(v => v.TimeOfVote < from).TakeWhile(v => v.TimeOfVote <= to),
                (false, true) => allVotes.TakeWhile(v => v.TimeOfVote <= to),
                (true, false) => allVotes.SkipWhile(v => v.TimeOfVote < from),
                (false, false) => allVotes
            };
            return votes;
        }
    }
}