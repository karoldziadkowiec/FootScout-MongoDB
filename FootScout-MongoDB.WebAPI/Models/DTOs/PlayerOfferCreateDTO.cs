namespace FootScout_MongoDB.WebAPI.Models.DTOs
{
    public class PlayerOfferCreateDTO
    {
        public int ClubAdvertisementId { get; set; }
        public int PlayerPositionId { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public int PlayerFootId { get; set; }
        public double Salary { get; set; }
        public string AdditionalInformation { get; set; }
        public string PlayerId { get; set; }
    }
}