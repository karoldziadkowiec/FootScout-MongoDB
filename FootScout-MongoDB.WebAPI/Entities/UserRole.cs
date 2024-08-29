using FootScout_MongoDB.WebAPI.Models.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class UserRole
    {
        [BsonId]
        public int Id { get; set; }
        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonIgnoreIfNull]
        public UserDTO User { get; set; }
        [BsonElement("RoleId")]
        public string RoleId { get; set; }

        [BsonIgnoreIfNull]
        public Role Role { get; set; }
    }
}