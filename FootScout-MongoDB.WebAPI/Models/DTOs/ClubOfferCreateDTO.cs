using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Models.DTOs
{
    public class ClubOfferCreateDTO
    {
        public int PlayerAdvertisementId { get; set; }
        public int PlayerPositionId { get; set; }
        public string ClubName { get; set; }
        public string League { get; set; }
        public string Region { get; set; }
        public double Salary { get; set; }
        public string AdditionalInformation { get; set; }
        public string ClubMemberId { get; set; }
    }
}