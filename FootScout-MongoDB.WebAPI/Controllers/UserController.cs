using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [ApiController]
    [Authorize(Policy = "AdminOrUserRights")]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository) =>
            _userRepository = userRepository;

        // GET: api/users/email/:email
        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> FindUserByEmail(string email)
        {
            var user = await _userRepository.FindUserByEmail(email);
            return Ok(user);
        }

        // GET: api/users/:userId/roles
        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetRolesForUser(string userId)
        {
            var roles = await _userRepository.GetRolesForUser(userId);
            return Ok(roles);
        }

        // GET: api/users/:userId/roles/:roleName
        [HttpGet("{userId}/roles/{roleName}")]
        public async Task<IActionResult> GetUsersByRoleName(string roleName)
        {
            var usersByRoleName = await _userRepository.GetRolesForUser(roleName);
            return Ok(usersByRoleName);
        }

        // GET: api/users/:userId
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var userDTO = await _userRepository.GetUser(userId);
            if (userDTO == null)
                return NotFound();

            return Ok(userDTO);
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var userDTOs = await _userRepository.GetUsers();
            return Ok(userDTOs);
        }

        // GET: api/users/role/user
        [HttpGet("role/user")]
        public async Task<IActionResult> GetOnlyUsers()
        {
            var onlyUserDTOs = await _userRepository.GetOnlyUsers();
            return Ok(onlyUserDTOs);
        }

        // GET: api/users/role/admin
        [HttpGet("role/admin")]
        public async Task<IActionResult> GetOnlyAdmins()
        {
            var onlyAdminDTOs = await _userRepository.GetOnlyAdmins();
            return Ok(onlyAdminDTOs);
        }

        // GET: api/users/:userId/role
        [HttpGet("{userId}/role")]
        public async Task<IActionResult> GetUserRole(string userId)
        {
            string role = await _userRepository.GetUserRole(userId);
            return Ok(role);
        }

        // GET: api/users/count
        [HttpGet("count")]
        public async Task<IActionResult> GetUserCount()
        {
            int count = await _userRepository.GetUserCount();
            return Ok(count);
        }

        // PUT: api/users/:userId
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserUpdateDTO dto)
        {
            await _userRepository.UpdateUser(userId, dto);
            return NoContent();
        }

        // PUT: api/users/reset-password/:userId
        [HttpPut("reset-password/{userId}")]
        public async Task<IActionResult> ResetUserPassword(string userId, [FromBody] UserResetPasswordDTO dto)
        {
            await _userRepository.ResetUserPassword(userId, dto);
            return NoContent();
        }
        
        // DELETE: api/users/:userId
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                if (await _userRepository.GetUser(userId) == null)
                    return NotFound($"User {userId} not found");

                await _userRepository.DeleteUser(userId);
            }
            catch (Exception)
            {
                return NotFound();
            }
            return NoContent();
        }

        // GET: api/users/:userId/club-history
        [HttpGet("{userId}/club-history")]
        public async Task<ActionResult<IEnumerable<ClubHistory>>> GetUserClubHistory(string userId)
        {
            var userClubHistories = await _userRepository.GetUserClubHistory(userId);
            return Ok(userClubHistories);
        }

        // GET: api/users/:userId/player-advertisements
        [HttpGet("{userId}/player-advertisements")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetUserPlayerAdvertisements(string userId)
        {
            var userPlayerAdvertisements = await _userRepository.GetUserPlayerAdvertisements(userId);
            return Ok(userPlayerAdvertisements);
        }

        // GET: api/users/:userId/player-advertisements/active
        [HttpGet("{userId}/player-advertisements/active")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetUserActivePlayerAdvertisements(string userId)
        {
            var userActivePlayerAdvertisements = await _userRepository.GetUserActivePlayerAdvertisements(userId);
            return Ok(userActivePlayerAdvertisements);
        }

        // GET: api/users/:userId/player-advertisements/inactive
        [HttpGet("{userId}/player-advertisements/inactive")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetUserInactivePlayerAdvertisements(string userId)
        {
            var userInactivePlayerAdvertisements = await _userRepository.GetUserInactivePlayerAdvertisements(userId);
            return Ok(userInactivePlayerAdvertisements);
        }

        // GET: api/users/:userId/player-advertisements/favorites
        [HttpGet("{userId}/player-advertisements/favorites")]
        public async Task<ActionResult<IEnumerable<FavoritePlayerAdvertisement>>> GetUserFavoritePlayerAdvertisements(string userId)
        {
            var userFavoritePlayerAdvertisements = await _userRepository.GetUserFavoritePlayerAdvertisements(userId);
            return Ok(userFavoritePlayerAdvertisements);
        }

        // GET: api/users/:userId/player-advertisements/favorites/active
        [HttpGet("{userId}/player-advertisements/favorites/active")]
        public async Task<ActionResult<IEnumerable<FavoritePlayerAdvertisement>>> GetUserActiveFavoritePlayerAdvertisements(string userId)
        {
            var userActiveFavoritePlayerAdvertisements = await _userRepository.GetUserActiveFavoritePlayerAdvertisements(userId);
            return Ok(userActiveFavoritePlayerAdvertisements);
        }

        // GET: api/users/:userId/player-advertisements/favorites/inactive
        [HttpGet("{userId}/player-advertisements/favorites/inactive")]
        public async Task<ActionResult<IEnumerable<FavoritePlayerAdvertisement>>> GetUserInactiveFavoritePlayerAdvertisements(string userId)
        {
            var userInactiveFavoritePlayerAdvertisements = await _userRepository.GetUserInactiveFavoritePlayerAdvertisements(userId);
            return Ok(userInactiveFavoritePlayerAdvertisements);
        }

        // GET: api/users/:userId/club-offers/received
        [HttpGet("{userId}/club-offers/received")]
        public async Task<ActionResult<IEnumerable<ClubOffer>>> GetReceivedClubOffers(string userId)
        {
            var receivedClubOffers = await _userRepository.GetReceivedClubOffers(userId);
            return Ok(receivedClubOffers);
        }

        // GET: api/users/:userId/club-offers/sent
        [HttpGet("{userId}/club-offers/sent")]
        public async Task<ActionResult<IEnumerable<ClubOffer>>> GetSentClubOffers(string userId)
        {
            var sentClubOffers = await _userRepository.GetSentClubOffers(userId);
            return Ok(sentClubOffers);
        }

        // GET: api/users/:userId/club-advertisements
        [HttpGet("{userId}/club-advertisements")]
        public async Task<ActionResult<IEnumerable<ClubAdvertisement>>> GetUserClubAdvertisements(string userId)
        {
            var userClubAdvertisements = await _userRepository.GetUserClubAdvertisements(userId);
            return Ok(userClubAdvertisements);
        }

        // GET: api/users/:userId/club-advertisements/active
        [HttpGet("{userId}/club-advertisements/active")]
        public async Task<ActionResult<IEnumerable<ClubAdvertisement>>> GetUserActiveClubAdvertisements(string userId)
        {
            var userActiveClubAdvertisements = await _userRepository.GetUserActiveClubAdvertisements(userId);
            return Ok(userActiveClubAdvertisements);
        }

        // GET: api/users/:userId/club-advertisements/inactive
        [HttpGet("{userId}/club-advertisements/inactive")]
        public async Task<ActionResult<IEnumerable<ClubAdvertisement>>> GetUserInactiveClubAdvertisements(string userId)
        {
            var userInactiveClubAdvertisements = await _userRepository.GetUserInactiveClubAdvertisements(userId);
            return Ok(userInactiveClubAdvertisements);
        }

        // GET: api/users/:userId/club-advertisements/favorites
        [HttpGet("{userId}/club-advertisements/favorites")]
        public async Task<ActionResult<IEnumerable<FavoriteClubAdvertisement>>> GetUserFavoriteClubAdvertisements(string userId)
        {
            var userFavoriteClubAdvertisements = await _userRepository.GetUserFavoriteClubAdvertisements(userId);
            return Ok(userFavoriteClubAdvertisements);
        }

        // GET: api/users/:userId/club-advertisements/favorites/active
        [HttpGet("{userId}/club-advertisements/favorites/active")]
        public async Task<ActionResult<IEnumerable<FavoriteClubAdvertisement>>> GetUserActiveFavoriteClubAdvertisements(string userId)
        {
            var userActiveFavoriteClubAdvertisements = await _userRepository.GetUserActiveFavoriteClubAdvertisements(userId);
            return Ok(userActiveFavoriteClubAdvertisements);
        }

        // GET: api/users/:userId/club-advertisements/favorites/inactive
        [HttpGet("{userId}/club-advertisements/favorites/inactive")]
        public async Task<ActionResult<IEnumerable<FavoriteClubAdvertisement>>> GetUserInactiveFavoriteClubAdvertisements(string userId)
        {
            var userInactiveFavoriteClubAdvertisements = await _userRepository.GetUserInactiveFavoriteClubAdvertisements(userId);
            return Ok(userInactiveFavoriteClubAdvertisements);
        }

        // GET: api/users/:userId/player-offers/received
        [HttpGet("{userId}/player-offers/received")]
        public async Task<ActionResult<IEnumerable<PlayerOffer>>> GetReceivedPlayerOffers(string userId)
        {
            var receivedPlayerOffers = await _userRepository.GetReceivedPlayerOffers(userId);
            return Ok(receivedPlayerOffers);
        }

        // GET: api/users/:userId/player-offers/sent
        [HttpGet("{userId}/player-offers/sent")]
        public async Task<ActionResult<IEnumerable<PlayerOffer>>> GetSentPlayerOffers(string userId)
        {
            var sentPlayerOffers = await _userRepository.GetSentPlayerOffers(userId);
            return Ok(sentPlayerOffers);
        }

        // GET: api/users/:userId/chats
        [HttpGet("{userId}/chats")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetUserChats(string userId)
        {
            var userChats = await _userRepository.GetUserChats(userId);
            return Ok(userChats);
        }

        // GET: api/users/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportUsersToCsv()
        {
            var csvStream = await _userRepository.ExportUsersToCsv();
            return File(csvStream, "text/csv", "users.csv");
        }
    }
}
