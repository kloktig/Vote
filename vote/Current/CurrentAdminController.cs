using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace vote.Participant
{
    [ApiController]
    [Route("admin/current")]
    public class CurrentAdminController : ControllerBase
    {
        private readonly CurrentRepo _currentRepo;
        private readonly ParticipantRepo _participantRepo;

        public CurrentAdminController(CurrentRepo currentRepo, ParticipantRepo participantRepo)
        {
            _currentRepo = currentRepo;
            _participantRepo = participantRepo;
        }
        
        [HttpPost]
        public async Task<IActionResult> SetCurrentParticipants(List<ParticipantDto> current)
        {
            var participants = _participantRepo.GetParticipants();
            if (!current.All(participants.Contains))
            {
                return BadRequest("Can only set participants as current");
            }
            await _currentRepo.WriteCurrent(current);
            return Ok(current);
        }
    }
}