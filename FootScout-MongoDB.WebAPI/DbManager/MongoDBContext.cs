using FootScout_MongoDB.WebAPI.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.DbManager
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDBSettings _mongoDBSettings;

        public MongoDBContext(IOptions<MongoDBSettings> mongoDBSettings)
        {
            _mongoDBSettings = mongoDBSettings.Value;
            var mongoClient = new MongoClient(_mongoDBSettings.ConnectionString);
            _database = mongoClient.GetDatabase(_mongoDBSettings.DatabaseName);
        }

        public IMongoCollection<Achievements> AchievementsCollection =>
            _database.GetCollection<Achievements>(_mongoDBSettings.AchievementsCollectionName);

        public IMongoCollection<Chat> ChatsCollection =>
            _database.GetCollection<Chat>(_mongoDBSettings.ChatsCollectionName);

        public IMongoCollection<ClubAdvertisement> ClubAdvertisementsCollection =>
            _database.GetCollection<ClubAdvertisement>(_mongoDBSettings.ClubAdvertisementsCollectionName);

        public IMongoCollection<ClubHistory> ClubHistoriesCollection =>
            _database.GetCollection<ClubHistory>(_mongoDBSettings.ClubHistoriesCollectionName);

        public IMongoCollection<ClubOffer> ClubOffersCollection =>
            _database.GetCollection<ClubOffer>(_mongoDBSettings.ClubOffersCollectionName);

        public IMongoCollection<FavoriteClubAdvertisement> FavoriteClubAdvertisementsCollection =>
            _database.GetCollection<FavoriteClubAdvertisement>(_mongoDBSettings.FavoriteClubAdvertisementsCollectionName);

        public IMongoCollection<FavoritePlayerAdvertisement> FavoritePlayerAdvertisementsCollection =>
            _database.GetCollection<FavoritePlayerAdvertisement>(_mongoDBSettings.FavoritePlayerAdvertisementsCollectionName);

        public IMongoCollection<Message> MessagesCollection =>
            _database.GetCollection<Message>(_mongoDBSettings.MessagesCollectionName);

        public IMongoCollection<OfferStatus> OfferStatusesCollection =>
            _database.GetCollection<OfferStatus>(_mongoDBSettings.OfferStatusesCollectionName);

        public IMongoCollection<PlayerAdvertisement> PlayerAdvertisementsCollection =>
            _database.GetCollection<PlayerAdvertisement>(_mongoDBSettings.PlayerAdvertisementsCollectionName);

        public IMongoCollection<PlayerFoot> PlayerFeetCollection =>
            _database.GetCollection<PlayerFoot>(_mongoDBSettings.PlayerFeetCollectionName);

        public IMongoCollection<PlayerOffer> PlayerOffersCollection =>
            _database.GetCollection<PlayerOffer>(_mongoDBSettings.PlayerOffersCollectionName);

        public IMongoCollection<PlayerPosition> PlayerPositionsCollection =>
            _database.GetCollection<PlayerPosition>(_mongoDBSettings.PlayerPositionsCollectionName);

        public IMongoCollection<Problem> ProblemsCollection =>
            _database.GetCollection<Problem>(_mongoDBSettings.ProblemsCollectionName);

        public IMongoCollection<Role> RolesCollection =>
            _database.GetCollection<Role>(_mongoDBSettings.RolesCollectionName);

        public IMongoCollection<SalaryRange> SalaryRangesCollection =>
            _database.GetCollection<SalaryRange>(_mongoDBSettings.SalaryRangesCollectionName);

        public IMongoCollection<UserRole> UserRolesCollection =>
            _database.GetCollection<UserRole>(_mongoDBSettings.UserRolesCollectionName);

        public IMongoCollection<User> UsersCollection =>
            _database.GetCollection<User>(_mongoDBSettings.UsersCollectionName);
    }
}