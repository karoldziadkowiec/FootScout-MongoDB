using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [Route("api/offer-statuses")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class OfferStatusController : ControllerBase
    {
        private readonly IOfferStatusRepository _offerStatusRepository;

        public OfferStatusController(IOfferStatusRepository offerStatusRepository)
        {
            _offerStatusRepository = offerStatusRepository;
        }

        // GET: api/offer-statuses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OfferStatus>>> GetOfferStatuses()
        {
            var offerStatuses = await _offerStatusRepository.GetOfferStatuses();
            return Ok(offerStatuses);
        }

        // GET: api/offer-statuses/:offerStatusId
        [HttpGet("{offerStatusId}")]
        public async Task<IActionResult> GetOfferStatus(int offerStatusId)
        {
            var offerStatus = await _offerStatusRepository.GetOfferStatus(offerStatusId);
            if (offerStatus == null)
                return NotFound();

            return Ok(offerStatus);
        }

        // GET: api/offer-statuses/name/:statusId
        [HttpGet("name/{statusId}")]
        public async Task<IActionResult> GetAdvertisementStatusName(int statusId)
        {
            var statusName = await _offerStatusRepository.GetOfferStatusName(statusId);
            if (statusName == null)
                return NotFound();

            return Ok(statusName);
        }

        // GET: api/offer-statuses/id/:statusName
        [HttpGet("id/{statusName}")]
        public async Task<IActionResult> GetOfferStatusId(string statusName)
        {
            var statusId = await _offerStatusRepository.GetOfferStatusId(statusName);
            if (statusId == 0)
                return NotFound();

            return Ok(statusId);
        }
    }
}
