using System;
using Microsoft.AspNetCore.Mvc;

namespace vote.Participant
{
    [ApiController]
    [Route("[controller]")]
    public class ParticipantsController : ControllerBase
    {
        private readonly ParticipantRepo _participantRepo;

        public ParticipantsController(ParticipantRepo participantRepo)
        {
            _participantRepo = participantRepo;
        }

        [HttpGet]
        [Route("All")]
        public IActionResult GetParticipants()
        {
            return Ok(_participantRepo.GetParticipants());
        }
        
        [HttpGet]
        [Route("Current")]
        public IActionResult GetCurrentParticipants()
        {
            return Ok(_participantRepo.GetCurrent());
        }
    }
}