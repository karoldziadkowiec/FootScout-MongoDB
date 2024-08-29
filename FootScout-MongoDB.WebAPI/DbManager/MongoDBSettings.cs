namespace FootScout_MongoDB.WebAPI.DbManager
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        // Collections
        public string AchievementsCollectionName { get; set; }
        public string ChatsCollectionName { get; set; }
        public string ClubAdvertisementsCollectionName { get; set; }
        public string ClubHistoriesCollectionName { get; set; }
        public string ClubOffersCollectionName { get; set; }
        public string FavoriteClubAdvertisementsCollectionName { get; set; }
        public string FavoritePlayerAdvertisementsCollectionName { get; set; }
        public string MessagesCollectionName { get; set; }
        public string OfferStatusesCollectionName { get; set; }
        public string PlayerAdvertisementsCollectionName { get; set; }
        public string PlayerFeetCollectionName { get; set; }
        public string PlayerOffersCollectionName { get; set; }
        public string PlayerPositionsCollectionName { get; set; }
        public string ProblemsCollectionName { get; set; }
        public string RolesCollectionName { get; set; }
        public string SalaryRangesCollectionName { get; set; }
        public string UserRolesCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
    }
}