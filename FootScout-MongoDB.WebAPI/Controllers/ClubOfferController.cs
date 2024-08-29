using AutoMapper;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [Route("api/club-offers")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class ClubOfferController : ControllerBase
    {
        private readonly IClubOfferRepository _clubOfferRepository;
        private readonly INewIdGeneratorService _newIdGeneratorService;
        private readonly IMapper _mapper;

        public ClubOfferController(IClubOfferRepository clubOfferRepository, INewIdGeneratorService newIdGeneratorService, IMapper mapper)
        {
            _clubOfferRepository = clubOfferRepository;
            _newIdGeneratorService = newIdGeneratorService;
            _mapper = mapper;
        }

        // GET: api/club-offers/:clubOfferId
        [HttpGet("{clubOfferId}")]
        public async Task<ActionResult<ClubOffer>> GetClubOffer(int clubOfferId)
        {
            var clubOffer = await _clubOfferRepository.GetClubOffer(clubOfferId);
            if (clubOffer == null)
                return NotFound();

            return Ok(clubOffer);
        }

        // GET: api/club-offers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClubOffer>>> GetClubOffers()
        {
            var clubOffers = await _clubOfferRepository.GetClubOffers();
            return Ok(clubOffers);
        }

        // GET: api/club-offers/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ClubOffer>>> GetActiveClubOffers()
        {
            var activeClubOffers = await _clubOfferRepository.GetActiveClubOffers();
            return Ok(activeClubOffers);
        }

        // GET: api/club-offers/active/count
        [HttpGet("active/count")]
        public async Task<IActionResult> GetActiveClubOfferCount()
        {
            int count = await _clubOfferRepository.GetActiveClubOfferCount();
            return Ok(count);
        }

        // GET: api/club-offers/inactive
        [HttpGet("inactive")]
        public async Task<ActionResult<IEnumerable<ClubOffer>>> GetInactiveClubOffers()
        {
            var inactiveClubOffers = await _clubOfferRepository.GetInactiveClubOffers();
            return Ok(inactiveClubOffers);
        }

        // POST: api/club-offers
        [HttpPost]
        public async Task<ActionResult> CreateClubOffer([FromBody] ClubOfferCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid dto data.");

            var clubOffer = _mapper.Map<ClubOffer>(dto);
            clubOffer.Id = await _newIdGeneratorService.GenerateNewClubOfferId();
            await _clubOfferRepository.CreateClubOffer(clubOffer);

            return Ok(clubOffer);
        }

        // PUT: api/club-offers/:clubOfferId
        [HttpPut("{clubOfferId}")]
        public async Task<ActionResult> UpdateClubOffer(int clubOfferId, [FromBody] ClubOffer clubOffer)
        {
            if (clubOfferId != clubOffer.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _clubOfferRepository.UpdateClubOffer(clubOffer);
            return NoContent();
        }

        // DELETE: api/club-offers/:clubOfferId
        [HttpDelete("{clubOfferId}")]
        public async Task<IActionResult> DeleteClubOffer(int clubOfferId)
        {
            try
            {
                if (await _clubOfferRepository.GetClubOffer(clubOfferId) == null)
                    return NotFound($"Club offer : {clubOfferId} not found");

                await _clubOfferRepository.DeleteClubOffer(clubOfferId);
            }
            catch (Exception)
            {
                return NotFound();
            }
            return NoContent();
        }

        // PUT: api/club-offers/accept/:clubOfferId
        [HttpPut("accept/{clubOfferId}")]
        public async Task<ActionResult> AcceptClubOffer(int clubOfferId, [FromBody] ClubOffer clubOffer)
        {
            if (clubOfferId != clubOffer.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _clubOfferRepository.AcceptClubOffer(clubOffer);
            return NoContent();
        }

        // PUT: api/club-offers/reject/:clubOfferId
        [HttpPut("reject/{clubOfferId}")]
        public async Task<ActionResult> RejectClubOffer(int clubOfferId, [FromBody] ClubOffer clubOffer)
        {
            if (clubOfferId != clubOffer.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _clubOfferRepository.RejectClubOffer(clubOffer);
            return NoContent();
        }

        // GET: api/club-offers/status/:playerAdvertisementId/:userId
        [HttpGet("status/{playerAdvertisementId}/{userId}")]
        public async Task<IActionResult> GetClubOfferStatusId(int playerAdvertisementId, string userId)
        {
            var clubOfferStatusId = await _clubOfferRepository.GetClubOfferStatusId(playerAdvertisementId, userId);
            return Ok(clubOfferStatusId);
        }

        // GET: api/club-offers/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportClubOffersToCsv()
        {
            var csvStream = await _clubOfferRepository.ExportClubOffersToCsv();
            return File(csvStream, "text/csv", "club-offers.csv");
        }
    }
}
