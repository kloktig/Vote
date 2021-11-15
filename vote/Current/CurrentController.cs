using Microsoft.AspNetCore.Mvc;

namespace vote.Current
{
    [ApiController]
    [Route("current")]
    public class CurrentController : ControllerBase
    {
        private readonly CurrentRepo _currentRepo;

        public CurrentController(CurrentRepo currentRepo)
        {
            _currentRepo = currentRepo;
        }
        
        [HttpGet]
        public IActionResult GetCurrentParticipants()
        {
            return Ok(_currentRepo.GetCurrent());
        }
    }
}