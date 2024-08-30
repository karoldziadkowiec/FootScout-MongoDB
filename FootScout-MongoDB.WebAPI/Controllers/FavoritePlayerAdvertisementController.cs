using AutoMapper;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [Route("api/player-advertisements/favorites")]
    [Authorize(Policy = "UserRights")]
    [ApiController]
    public class FavoritePlayerAdvertisementController : ControllerBase
    {
        private readonly IFavoritePlayerAdvertisementRepository _favoritePlayerAdvertisementRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPlayerAdvertisementRepository _playerAdvertisementRepository;
        private readonly INewIdGeneratorService _newIdGeneratorService;
        private readonly IMapper _mapper;

        public FavoritePlayerAdvertisementController(IFavoritePlayerAdvertisementRepository favoritePlayerAdvertisementRepository, INewIdGeneratorService newIdGeneratorService, IUserRepository userRepository, IPlayerAdvertisementRepository playerAdvertisementRepository, IMapper mapper)
        {
            _favoritePlayerAdvertisementRepository = favoritePlayerAdvertisementRepository;
            _userRepository = userRepository;
            _playerAdvertisementRepository = playerAdvertisementRepository;
            _newIdGeneratorService = newIdGeneratorService;
            _mapper = mapper;
        }

        // POST: api/player-advertisements/favorites
        [HttpPost]
        public async Task<ActionResult> AddToFavorites([FromBody] FavoritePlayerAdvertisementCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid dto data.");

            var favoritePlayerAdvertisement = _mapper.Map<FavoritePlayerAdvertisement>(dto);
            favoritePlayerAdvertisement.Id = await _newIdGeneratorService.GenerateNewFavoritePlayerAdvertisementId();

            if (favoritePlayerAdvertisement.PlayerAdvertisementId != 0)
                favoritePlayerAdvertisement.PlayerAdvertisement = await _playerAdvertisementRepository.GetPlayerAdvertisement(favoritePlayerAdvertisement.PlayerAdvertisementId);

            if (favoritePlayerAdvertisement.UserId is not null)
            {
                var user = await _userRepository.GetUser(favoritePlayerAdvertisement.UserId);
                favoritePlayerAdvertisement.User = _mapper.Map<User>(user);
            }

            await _favoritePlayerAdvertisementRepository.AddToFavorites(favoritePlayerAdvertisement);
            return Ok(favoritePlayerAdvertisement);
        }

        // DELETE: api/player-advertisements/favorites/:favoritePlayerAdvertisementId
        [HttpDelete("{favoritePlayerAdvertisementId}")]
        public async Task<ActionResult> DeleteFromFavorites(int favoritePlayerAdvertisementId)
        {
            await _favoritePlayerAdvertisementRepository.DeleteFromFavorites(favoritePlayerAdvertisementId);
            return NoContent();
        }

        // GET: api/player-advertisements/favorites/check/:playerAdvertisementId/:userId
        [HttpGet("check/{playerAdvertisementId}/{userId}")]
        public async Task<IActionResult> CheckPlayerAdvertisementIsFavorite(int playerAdvertisementId, string userId)
        {
            var favoriteId = await _favoritePlayerAdvertisementRepository.CheckPlayerAdvertisementIsFavorite(playerAdvertisementId, userId);
            return Ok(favoriteId);
        }
    }
}