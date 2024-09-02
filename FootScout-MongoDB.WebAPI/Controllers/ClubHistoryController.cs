using AutoMapper;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [Route("api/club-history")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class ClubHistoryController : ControllerBase
    {
        private readonly IClubHistoryRepository _clubHistoryRepository;
        private readonly IAchievementsRepository _achievementsRepository;
        private readonly IPlayerPositionRepository _playerPositionRepository;
        private readonly IUserRepository _userRepository;
        private readonly INewIdGeneratorService _newIdGeneratorService;
        private readonly IMapper _mapper;

        public ClubHistoryController(IClubHistoryRepository clubHistoryRepository, IAchievementsRepository achievementsRepository, IPlayerPositionRepository playerPositionRepository, IUserRepository userRepository, INewIdGeneratorService newIdGeneratorService, IMapper mapper)
        {
            _clubHistoryRepository = clubHistoryRepository;
            _achievementsRepository = achievementsRepository;
            _playerPositionRepository = playerPositionRepository;
            _userRepository = userRepository;
            _newIdGeneratorService = newIdGeneratorService;
            _mapper = mapper;
        }

        // GET: api/club-history/:clubHistoryId
        [HttpGet("{clubHistoryId}")]
        public async Task<ActionResult<ClubHistory>> GetClubHistory(int clubHistoryId)
        {
            var clubHistory = await _clubHistoryRepository.GetClubHistory(clubHistoryId);
            if (clubHistory == null)
                return NotFound();

            return Ok(clubHistory);
        }

        // GET: api/club-history
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClubHistory>>> GetAllClubHistory()
        {
            var clubHistories = await _clubHistoryRepository.GetAllClubHistory();
            return Ok(clubHistories);
        }

        // GET: api/club-history/count
        [HttpGet("count")]
        public async Task<IActionResult> GetClubHistoryCount()
        {
            int count = await _clubHistoryRepository.GetClubHistoryCount();
            return Ok(count);
        }

        // POST: api/club-history
        [HttpPost]
        public async Task<ActionResult> CreateClubHistory([FromBody] ClubHistoryCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            var achievements = _mapper.Map<Achievements>(dto.Achievements);
            achievements.Id = await _newIdGeneratorService.GenerateNewAchievementsId();
            await _achievementsRepository.CreateAchievements(achievements);

            var clubHistory = _mapper.Map<ClubHistory>(dto);
            clubHistory.Id = await _newIdGeneratorService.GenerateNewClubHistoryId();
            clubHistory.Achievements = achievements;
            clubHistory.AchievementsId = achievements.Id;

            if (clubHistory.PlayerPositionId != 0)
                clubHistory.PlayerPosition = await _playerPositionRepository.GetPlayerPosition(clubHistory.PlayerPositionId);

            if (clubHistory.PlayerId is not null)
            {
                var player = await _userRepository.GetUser(clubHistory.PlayerId);
                clubHistory.Player = _mapper.Map<User>(player);
            }

            await _clubHistoryRepository.CreateClubHistory(clubHistory);
            return Ok(clubHistory);
        }

        // PUT: api/club-history/:clubHistoryId
        [HttpPut("{clubHistoryId}")]
        public async Task<ActionResult> UpdateClubHistory(int clubHistoryId, [FromBody] ClubHistory clubHistory)
        {
            if (clubHistoryId != clubHistory.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (clubHistory.PlayerPositionId != 0)
                clubHistory.PlayerPosition = await _playerPositionRepository.GetPlayerPosition(clubHistory.PlayerPositionId);

            if (clubHistory.PlayerId is not null)
            {
                var player = await _userRepository.GetUser(clubHistory.PlayerId);
                clubHistory.Player = _mapper.Map<User>(player);
            }

            await _clubHistoryRepository.UpdateClubHistory(clubHistory);
            return NoContent();
        }

        // DELETE: api/club-history/:clubHistoryId
        [HttpDelete("{clubHistoryId}")]
        public async Task<ActionResult> DeleteClubHistory(int clubHistoryId)
        {
            var clubHistory = await _clubHistoryRepository.GetClubHistory(clubHistoryId);
            if (clubHistory == null)
                return NotFound();

            await _clubHistoryRepository.DeleteClubHistory(clubHistoryId);
            return NoContent();
        }
    }
}