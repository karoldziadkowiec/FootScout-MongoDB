using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [Route("api/player-feet")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class PlayerFootController : ControllerBase
    {
        private readonly IPlayerFootRepository _playerFootRepository;

        public PlayerFootController(IPlayerFootRepository playerFootRepository)
        {
            _playerFootRepository = playerFootRepository;
        }

        // GET: api/player-feet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerFoot>>> GetPlayerFeet()
        {
            var playerFeet = await _playerFootRepository.GetPlayerFeet();
            return Ok(playerFeet);
        }

        // GET: api/player-feet/:footId
        [HttpGet("{footId}")]
        public async Task<IActionResult> GetPlayerFootName(int footId)
        {
            var footName = await _playerFootRepository.GetPlayerFootName(footId);
            if (footName == null)
                return NotFound();

            return Ok(footName);
        }
    }
}
