using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vote.Participant;

namespace vote.Votes
{
    [ApiController]
    [Route("[controller]")]
    public class VoteController : ControllerBase
    {
        private readonly VotesRepo _votesRepo;

        public VoteController()
        {
            _votesRepo = new VotesRepo();
        }

        [HttpPost]
        public async Task<IActionResult> AddVote(ParticipantDto participantDto)
        {
            await _votesRepo.Write(participantDto);
            return Accepted();
        }
    }
}