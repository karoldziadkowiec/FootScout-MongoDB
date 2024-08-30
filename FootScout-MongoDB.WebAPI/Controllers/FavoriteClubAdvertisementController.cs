using AutoMapper;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [Route("api/club-advertisements/favorites")]
    [Authorize(Policy = "UserRights")]
    [ApiController]
    public class FavoriteClubAdvertisementController : ControllerBase
    {
        private readonly IFavoriteClubAdvertisementRepository _favoriteClubAdvertisementRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClubAdvertisementRepository _clubAdvertisementRepository;
        private readonly INewIdGeneratorService _newIdGeneratorService;
        private readonly IMapper _mapper;

        public FavoriteClubAdvertisementController(IFavoriteClubAdvertisementRepository favoriteClubAdvertisementRepository, INewIdGeneratorService newIdGeneratorService, IUserRepository userRepository, IClubAdvertisementRepository clubAdvertisementRepository, IMapper mapper)
        {
            _favoriteClubAdvertisementRepository = favoriteClubAdvertisementRepository;
            _userRepository = userRepository;
            _clubAdvertisementRepository = clubAdvertisementRepository;
            _newIdGeneratorService = newIdGeneratorService;
            _mapper = mapper;
        }

        // POST: api/club-advertisements/favorites
        [HttpPost]
        public async Task<ActionResult> AddToFavorites([FromBody] FavoriteClubAdvertisementCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid dto data.");

            var favoriteClubAdvertisement = _mapper.Map<FavoriteClubAdvertisement>(dto);
            favoriteClubAdvertisement.Id = await _newIdGeneratorService.GenerateNewFavoriteClubAdvertisementId();

            if (favoriteClubAdvertisement.ClubAdvertisementId != 0)
                favoriteClubAdvertisement.ClubAdvertisement = await _clubAdvertisementRepository.GetClubAdvertisement(favoriteClubAdvertisement.ClubAdvertisementId);

            if (favoriteClubAdvertisement.UserId is not null)
            {
                var user = await _userRepository.GetUser(favoriteClubAdvertisement.UserId);
                favoriteClubAdvertisement.User = _mapper.Map<User>(user);
            }

            await _favoriteClubAdvertisementRepository.AddToFavorites(favoriteClubAdvertisement);
            return Ok(favoriteClubAdvertisement);
        }

        // DELETE: api/club-advertisements/favorites/:favoriteClubAdvertisementId
        [HttpDelete("{favoriteClubAdvertisementId}")]
        public async Task<ActionResult> DeleteFromFavorites(int favoriteClubAdvertisementId)
        {
            await _favoriteClubAdvertisementRepository.DeleteFromFavorites(favoriteClubAdvertisementId);
            return NoContent();
        }

        // GET: api/club-advertisements/favorites/check/:clubAdvertisementId/:userId
        [HttpGet("check/{clubAdvertisementId}/{userId}")]
        public async Task<IActionResult> CheckClubAdvertisementIsFavorite(int clubAdvertisementId, string userId)
        {
            var favoriteId = await _favoriteClubAdvertisementRepository.CheckClubAdvertisementIsFavorite(clubAdvertisementId, userId);
            return Ok(favoriteId);
        }
    }
}