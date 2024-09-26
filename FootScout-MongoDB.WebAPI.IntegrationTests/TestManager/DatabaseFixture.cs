using FootScout_MongoDB.WebAPI.DbManager;
using Microsoft.Extensions.Options;
using Moq;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.TestManager
{
    public class DatabaseFixture : IDisposable
    {
        public MongoDBContext DbContext { get; private set; }
        public string UserTokenJWT { get; private set; }
        public string AdminTokenJWT { get; private set; }

        public DatabaseFixture()
        {
            var mongoDBSettings = new MongoDBSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = $"FootScoutTests_{Guid.NewGuid()}",

                // Collections
                AchievementsCollectionName = "Achievements",
                ChatsCollectionName = "Chats",
                ClubAdvertisementsCollectionName = "ClubAdvertisements",
                ClubHistoriesCollectionName = "ClubHistories",
                ClubOffersCollectionName = "ClubOffers",
                FavoriteClubAdvertisementsCollectionName = "FavoriteClubAdvertisements",
                FavoritePlayerAdvertisementsCollectionName = "FavoritePlayerAdvertisements",
                MessagesCollectionName = "Messages",
                OfferStatusesCollectionName = "OfferStatuses",
                PlayerAdvertisementsCollectionName = "PlayerAdvertisements",
                PlayerFeetCollectionName = "PlayerFeet",
                PlayerOffersCollectionName = "PlayerOffers",
                PlayerPositionsCollectionName = "PlayerPositions",
                ProblemsCollectionName = "Problems",
                RolesCollectionName = "Roles",
                SalaryRangesCollectionName = "SalaryRanges",
                UserRolesCollectionName = "UserRoles",
                UsersCollectionName = "Users"
            };
            var options = Options.Create(mongoDBSettings);
            DbContext = new MongoDBContext(options);

            UserTokenJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjY2ZjUzOTQ4Mjk3MWExNTRhYmEyMzRlOSIsImp0aSI6IjNmNGMyOWMyLTkxZDAtNGI1ZS1hN2JmLWViYTc1YzJkMGYwNyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3Mjc0MzM0MjgsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcyNzIiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjMwMDAifQ.6wX3Z1h5cSAfGY0l6XzgIKnu1vpRG0huBdYi6m77qzg";
            AdminTokenJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjY2ZjQ1ZjdlM2U3NTMxYzc0MGU1NGQzNiIsImp0aSI6IjNhNjlmMjJjLTBkMzktNGIzOS04MzhkLTQ3MjFkNDg0MTVjOCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzI3NDMzNDYzLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjcyIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDozMDAwIn0.ZkYrFo1F-1DCsplFpfvmqVOAGzVTrD2k26m948kd7zU";

            InitializeDatabase().GetAwaiter().GetResult();
        }

        private async Task InitializeDatabase()
        {
            await TestBase.SeedRoleTestBase(DbContext);
            await TestBase.SeedPlayerPositionTestBase(DbContext);
            await TestBase.SeedPlayerFootTestBase(DbContext);
            await TestBase.SeedOfferStatusTestBase(DbContext);
            await TestBase.SeedUserTestBase(DbContext);
            await TestBase.SeedClubHistoryTestBase(DbContext);
            await TestBase.SeedProblemTestBase(DbContext);
            await TestBase.SeedChatTestBase(DbContext);
            await TestBase.SeedMessageTestBase(DbContext);
            await TestBase.SeedPlayerAdvertisementTestBase(DbContext);
            await TestBase.SeedClubOfferTestBase(DbContext);
            await TestBase.SeedClubAdvertisementTestBase(DbContext);
            await TestBase.SeedPlayerOfferTestBase(DbContext);
        }

        public void Dispose()
        {
            DbContext.AchievementsCollection.Database.DropCollection("Achievements");
            DbContext.ChatsCollection.Database.DropCollection("Chats");
            DbContext.ClubAdvertisementsCollection.Database.DropCollection("ClubAdvertisements");
            DbContext.ClubHistoriesCollection.Database.DropCollection("ClubHistories");
            DbContext.ClubOffersCollection.Database.DropCollection("ClubOffers");
            DbContext.FavoriteClubAdvertisementsCollection.Database.DropCollection("FavoriteClubAdvertisements");
            DbContext.FavoritePlayerAdvertisementsCollection.Database.DropCollection("FavoritePlayerAdvertisements");
            DbContext.MessagesCollection.Database.DropCollection("Messages");
            DbContext.OfferStatusesCollection.Database.DropCollection("OfferStatuses");
            DbContext.PlayerAdvertisementsCollection.Database.DropCollection("PlayerAdvertisements");
            DbContext.PlayerFeetCollection.Database.DropCollection("PlayerFeet");
            DbContext.PlayerOffersCollection.Database.DropCollection("PlayerOffers");
            DbContext.PlayerPositionsCollection.Database.DropCollection("PlayerPositions");
            DbContext.ProblemsCollection.Database.DropCollection("Problems");
            DbContext.RolesCollection.Database.DropCollection("Roles");
            DbContext.SalaryRangesCollection.Database.DropCollection("SalaryRanges");
            DbContext.UserRolesCollection.Database.DropCollection("UserRoles");
            DbContext.UsersCollection.Database.DropCollection("Users");

            DbContext.AchievementsCollection.Database.Client.DropDatabase(DbContext.AchievementsCollection.Database.DatabaseNamespace.DatabaseName);
        }
    }
}