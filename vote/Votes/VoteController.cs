using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddVote(ParticipantDto participantDto)
        {
            var uid = HttpContext.User.Claims.ToImmutableList()[0].Value;
            await _votesRepo.Write(uid, participantDto);
            return Accepted();
        }
    }
}