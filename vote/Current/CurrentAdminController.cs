using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vote.Participant;

namespace vote.Current
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
        public async Task<IActionResult> SetCurrentParticipants(List<ParticipantDto> currentParticipants)
        {
            var participants = _participantRepo.GetParticipants();
            if (!currentParticipants.All(participants.Contains))
            {
                return BadRequest("Can only set participants as current");
            }

            return Ok(await _currentRepo.WriteCurrent(currentParticipants, null));
        }

        [HttpPost]
        [Route("close/{id}")]
        public async Task<IActionResult> CloseCurrent(string id)
        {
            return Ok(await _currentRepo.AddEndTime(id, DateTimeOffset.Now));
        }

        [HttpGet]
        [Route("all")]
        public IActionResult ListAll()
        {
            return Ok(_currentRepo.ListCurrents());
        }
    }
}