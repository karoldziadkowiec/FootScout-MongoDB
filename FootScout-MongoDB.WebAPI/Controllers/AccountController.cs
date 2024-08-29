using FootScout_MongoDB.WebAPI.Models.Constants;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService) =>
            _accountService = accountService;

        // POST: api/account/register
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (registerDTO == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _accountService.Register(registerDTO);
                return Ok(registerDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during the registration of account: {ex.Message}");
            }
        }

        // POST: api/account/login
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (loginDTO == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = await _accountService.Login(loginDTO);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during the login: {ex.Message}");
            }
        }

        // GET: api/account/roles
        [Authorize(Roles = RoleName.Admin)]
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetRoles()
        {
            var roles = await _accountService.GetRoles();
            return Ok(roles);
        }

        // GET: api/account/roles/make-admin/:userId
        [Authorize(Roles = RoleName.Admin)]
        [HttpPost("roles/make-admin/{userId}")]
        public async Task<IActionResult> MakeAnAdmin(string userId)
        {
            await _accountService.MakeAnAdmin(userId);
            return NoContent();
        }

        // GET: api/account/roles/make-user/:userId
        [Authorize(Roles = RoleName.Admin)]
        [HttpPost("roles/make-user/{userId}")]
        public async Task<IActionResult> DemoteFromAdmin(string userId)
        {
            await _accountService.MakeAnUser(userId);
            return NoContent();
        }
    }
}
