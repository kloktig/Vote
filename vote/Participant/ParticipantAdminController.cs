using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace vote.Participant
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ParticipantRepo _participantRepo;

        public AdminController(ParticipantRepo participantRepo)
        {
            _participantRepo = participantRepo;
        }
        
        [HttpPost]
        [Route("Participant")]
        public IActionResult Post(Participant participant)
        {
            try
            {
                _participantRepo.AddParticipant(participant);
                return Ok(_participantRepo.GetParticipants());
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        
        
        [HttpPost]
        [Route("Current")]
        public IActionResult SetCurrentParticipants(List<Participant> current)
        {
            try
            {
                _participantRepo.SetCurrent(current);
                return Ok(current);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}