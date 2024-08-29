using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Models.DTOs
{
    public class PlayerAdvertisementCreateDTO
    {
        public int PlayerPositionId { get; set; }
        public string League { get; set; }
        public string Region { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public int PlayerFootId { get; set; }
        public SalaryRangeDTO SalaryRangeDTO { get; set; }
        public string PlayerId { get; set; }
    }
}