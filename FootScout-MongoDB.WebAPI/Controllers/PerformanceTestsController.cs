using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [Route("api/performance-tests")]
    [Authorize(Policy = "AdminRights")]
    [ApiController]
    public class PerformanceTestsController : ControllerBase
    {
        private readonly IPerformanceTestsService _performanceTestsService;

        public PerformanceTestsController(IPerformanceTestsService performanceTestsService)
        {
            _performanceTestsService = performanceTestsService;
        }

        // POST: api/performance-tests/seed/:testCounter
        [HttpPost("seed/{testCounter}")]
        public async Task<ActionResult> SeedComponents(int testCounter)
        {
            try
            {
                if(testCounter <= 0)
                    return BadRequest("Invalid test counter.");

                await _performanceTestsService.SeedComponents(testCounter);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during seeding components: {ex.Message}");
            }
        }

        // DELETE: api/performance-tests/clear
        [HttpDelete("clear")]
        public async Task<ActionResult> ClearDatabaseOfSeededComponents()
        {
            try
            {
                await _performanceTestsService.ClearDatabaseOfSeededComponents();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during clearing components: {ex.Message}");
            }
        }
    }
}