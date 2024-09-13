using AutoMapper;
using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.Constants;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.UnitTests.TestManager
{
    public class TestBase
    {
        public IMapper CreateMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            return configuration.CreateMapper();
        }

        public IConfiguration CreateConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"JWT:Secret", "keyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy"},
                {"JWT:ValidAudience", "http://localhost:3000"},
                {"JWT:ValidIssuer", "http://localhost:7272"},
                {"JWT:ExpireDays", "1"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return configuration;
        }

        // Scenario

        public async Task SeedRoleTestBase(MongoDBContext dbContext, IRoleService roleService)
        {
            var roles = new List<string> { RoleName.Admin, RoleName.User };

            foreach (var role in roles)
            {
                if (!await roleService.CheckRoleExists(role))
                    await roleService.CreateNewRole(role);
            }
        }

        public async Task SeedUserTestBase(MongoDBContext dbContext, IRoleService roleService)
        {
            // roles
            await SeedRoleTestBase(dbContext, roleService);

            // users
            await dbContext.UsersCollection.InsertManyAsync(new List<User>
            {
                new User { Id = "admin0", Email = "admin@admin.com", PasswordHash = "Admin1!", FirstName = "Admin F.", LastName = "Admin L.", Location = "Admin Loc.", PhoneNumber = "000000000" },
                new User { Id = "unknown9", Email = "unknown@unknown.com", PasswordHash = "Uuuuu1!",FirstName = "Unknown F.", LastName = "Unknown L.", Location = "Unknown Loc.", PhoneNumber = "999999999" },
                new User { Id = "leomessi", Email = "lm10@gmail.com", PasswordHash = "Leooo1!",FirstName = "Leo", LastName = "Messi", Location = "Miami", PhoneNumber = "101010101" },
                new User { Id = "pepguardiola", Email = "pg8@gmail.com", PasswordHash = "Peppp1!",FirstName = "Pep", LastName = "Guardiola", Location = "Manchester", PhoneNumber = "868686868" }
            });

            // user roles
            await dbContext.UserRolesCollection.InsertManyAsync(new List<UserRole>
            {
                new UserRole { Id = 1, UserId = "admin0", RoleId = RoleName.Admin },
                new UserRole { Id = 2, UserId = "unknown9", RoleId = RoleName.User },
                new UserRole { Id = 3, UserId = "leomessi", RoleId = RoleName.User },
                new UserRole { Id = 4, UserId = "pepguardiola", RoleId = RoleName.User }
            });
        }

        public async Task SeedOfferStatusTestBase(MongoDBContext dbContext, INewIdGeneratorService newIdGeneratorService)
        {
            var statuses = new List<string> { OfferStatusName.Offered, OfferStatusName.Accepted, OfferStatusName.Rejected };

            foreach (var status in statuses)
            {
                var existingStatus = await dbContext.OfferStatusesCollection
                    .Find(os => os.StatusName == status)
                    .FirstOrDefaultAsync();

                if (existingStatus == null)
                {
                    var newStatus = new OfferStatus
                    {
                        Id = await newIdGeneratorService.GenerateNewOfferStatusId(),
                        StatusName = status
                    };

                    await dbContext.OfferStatusesCollection.InsertOneAsync(newStatus);
                }
            }
        }

        public async Task SeedPlayerPositionTestBase(MongoDBContext dbContext, INewIdGeneratorService newIdGeneratorService)
        {
            var positions = new List<string>
            {
                Position.Goalkeeper, Position.RightBack, Position.CenterBack, Position.LeftBack, Position.RightWingBack, Position.LeftWingBack, Position.CentralDefensiveMidfield, Position.CentralMidfield, Position.CentralAttackingMidfield, Position.RightMidfield, Position.RightWing, Position.LeftMidfield, Position.LeftWing, Position.CentreForward, Position.Striker
            };

            foreach (var position in positions)
            {
                var existingPosition = await dbContext.PlayerPositionsCollection
                    .Find(p => p.PositionName == position)
                    .FirstOrDefaultAsync();

                if (existingPosition == null)
                {
                    var newPosition = new PlayerPosition
                    {
                        Id = await newIdGeneratorService.GenerateNewPlayerPositionId(),
                        PositionName = position
                    };

                    await dbContext.PlayerPositionsCollection.InsertOneAsync(newPosition);
                }
            }
        }

        public async Task SeedPlayerFootTestBase(MongoDBContext dbContext, INewIdGeneratorService newIdGeneratorService)
        {
            var feet = new List<string> { Foot.Left, Foot.Right, Foot.TwoFooted };

            foreach (var foot in feet)
            {
                var existingFoot = await dbContext.PlayerFeetCollection
                    .Find(p => p.FootName == foot)
                    .FirstOrDefaultAsync();

                if (existingFoot == null)
                {
                    var newFoot = new PlayerFoot
                    {
                        Id = await newIdGeneratorService.GenerateNewPlayerFootId(),
                        FootName = foot
                    };

                    await dbContext.PlayerFeetCollection.InsertOneAsync(newFoot);
                }
            }
        }

        public async Task SeedClubHistoryTestBase(MongoDBContext dbContext)
        {
            // club history
            await dbContext.ClubHistoriesCollection.InsertManyAsync(new List<ClubHistory>
            {
                new ClubHistory { Id = 1, PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, ClubName = "FC Barcelona", League = "La Liga", Region = "Spain", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(150), AchievementsId = 1, PlayerId = "leomessi"},
                new ClubHistory { Id = 2, PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, ClubName = "PSG", League = "Ligue1", Region = "France", StartDate = DateTime.UtcNow.AddDays(150), EndDate = DateTime.UtcNow.AddDays(300), AchievementsId = 2, PlayerId = "leomessi"},
            });

            // achievements
            await dbContext.AchievementsCollection.InsertManyAsync(new List<Achievements>
            {
                new Achievements { Id = 1, NumberOfMatches = 750, Goals = 678, Assists = 544, AdditionalAchievements = "LM" },
                new Achievements { Id = 2, NumberOfMatches = 40, Goals = 27, Assists = 24, AdditionalAchievements = "No info" },
            });
        }

        public async Task SeedProblemTestBase(MongoDBContext dbContext)
        {
            // club history
            await dbContext.ProblemsCollection.InsertManyAsync(new List<Problem>
            {
                new Problem { Id = 1, Title = "Problem 1", Description = "Desc 1", CreationDate = DateTime.UtcNow, IsSolved = true, RequesterId = "leomessi" },
                new Problem { Id = 2, Title = "Problem 2", Description = "Desc 2", CreationDate = DateTime.UtcNow.AddDays(150), IsSolved = false, RequesterId = "pepguardiola" },
            });
        }

        public async Task SeedChatTestBase(MongoDBContext dbContext)
        {
            // chat
            await dbContext.ChatsCollection.InsertManyAsync(new List<Chat>
            {
                new Chat { Id = 1, User1Id = "leomessi", User2Id = "pepguardiola" },
                new Chat { Id = 2, User1Id = "admin0", User2Id = "leomessi" },
            });
        }

        public async Task SeedMessageTestBase(MongoDBContext dbContext)
        {
            // messages
            await dbContext.MessagesCollection.InsertManyAsync(new List<Message>
            {
                new Message { Id = 1, ChatId = 1, Content = "Hey", SenderId = "pepguardiola", ReceiverId = "leomessi" , Timestamp = DateTime.UtcNow },
                new Message { Id = 2, ChatId = 1, Content = "Hello", SenderId = "leomessi", ReceiverId = "pepguardiola" , Timestamp = DateTime.UtcNow },
                new Message { Id = 3, ChatId = 2, Content = "a b c", SenderId = "admin0", ReceiverId = "leomessi" , Timestamp = new DateTime(2024, 09, 09) },
            });
        }

        public async Task SeedPlayerAdvertisementTestBase(MongoDBContext dbContext)
        {
            // salary range
            await dbContext.SalaryRangesCollection.InsertManyAsync(new List<SalaryRange>
            {
                new SalaryRange { Id = 1, Min = 150, Max = 200 },
                new SalaryRange { Id = 2, Min = 145, Max = 195 },
            });

            // player advertisement
            await dbContext.PlayerAdvertisementsCollection.InsertManyAsync(new List<PlayerAdvertisement>
            {
                new PlayerAdvertisement { Id = 1, PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, League = "Premier League", Region = "England", Age = 37, Height = 167, PlayerFootId = 3, PlayerFoot = new PlayerFoot { Id = 3, FootName = "Right" }, SalaryRangeId = 1, CreationDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), PlayerId = "leomessi" },
                new PlayerAdvertisement { Id = 2, PlayerPositionId = 14, PlayerPosition = new PlayerPosition { Id = 14, PositionName = "Centre-Forward" }, League = "La Liga", Region = "Spain", Age = 37, Height = 167, PlayerFootId = 3, PlayerFoot = new PlayerFoot { Id = 3, FootName = "Right" }, SalaryRangeId = 2, CreationDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), PlayerId = "leomessi" },
            });

            // favorite player advertisement
            await dbContext.FavoritePlayerAdvertisementsCollection.InsertManyAsync(new List<FavoritePlayerAdvertisement>
            {
                new FavoritePlayerAdvertisement { Id = 1, PlayerAdvertisementId = 1, UserId = "pepguardiola" },
                new FavoritePlayerAdvertisement { Id = 2, PlayerAdvertisementId = 2, UserId = "pepguardiola" },
            });
        }

        public async Task SeedClubOfferTestBase(MongoDBContext dbContext)
        {

            // club offer
            await dbContext.ClubOffersCollection.InsertManyAsync(new List<ClubOffer>
            {
                new ClubOffer { Id = 1, PlayerAdvertisementId = 1, OfferStatusId = 1, OfferStatus = new OfferStatus { Id = 1, StatusName = "Offered" }, PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, ClubName = "Manchester City", League = "Premier League", Region = "England", Salary = 160, AdditionalInformation = "no info", CreationDate = DateTime.UtcNow, ClubMemberId = "pepguardiola" },
                new ClubOffer { Id = 2, PlayerAdvertisementId = 2, OfferStatusId = 2, OfferStatus = new OfferStatus { Id = 2, StatusName = "Accepted" }, PlayerPositionId = 14, PlayerPosition = new PlayerPosition { Id = 14, PositionName = "Centre-Forward" }, ClubName = "Real Madrid", League = "La Liga", Region = "Spain", Salary = 155, AdditionalInformation = "no info", CreationDate = DateTime.UtcNow, ClubMemberId = "pepguardiola" },
            });
        }

        public async Task SeedClubAdvertisementTestBase(MongoDBContext dbContext)
        {
            // salary range
            await dbContext.SalaryRangesCollection.InsertManyAsync(new List<SalaryRange>
            {
                new SalaryRange { Id = 3, Min = 150, Max = 200 },
                new SalaryRange { Id = 4, Min = 145, Max = 195 },
            });

            // club advertisement
            await dbContext.ClubAdvertisementsCollection.InsertManyAsync(new List<ClubAdvertisement>
            {
                new ClubAdvertisement { Id = 1, PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, ClubName = "Manchester City", League = "Premier League", Region = "England", SalaryRangeId = 3, CreationDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), ClubMemberId = "pepguardiola" },
                new ClubAdvertisement { Id = 2, PlayerPositionId = 14, PlayerPosition = new PlayerPosition { Id = 14, PositionName = "Centre-Forward" }, ClubName = "Manchester City", League = "Premier League", Region = "England", SalaryRangeId = 4, CreationDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), ClubMemberId = "pepguardiola" },
            });

            // favorite club advertisement
            await dbContext.FavoriteClubAdvertisementsCollection.InsertManyAsync(new List<FavoriteClubAdvertisement>
            {
                new FavoriteClubAdvertisement { Id = 1, ClubAdvertisementId = 1, UserId = "leomessi" },
                new FavoriteClubAdvertisement { Id = 2, ClubAdvertisementId = 2, UserId = "leomessi" },
            });
        }

        public async Task SeedPlayerOfferTestBase(MongoDBContext dbContext)
        {
            // player offer
            await dbContext.PlayerOffersCollection.InsertManyAsync(new List<PlayerOffer>
            {
                new PlayerOffer { Id = 1, ClubAdvertisementId = 1, OfferStatusId = 1, OfferStatus = new OfferStatus { Id = 1, StatusName = "Offered" }, PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, Age = 37, Height = 167, PlayerFootId = 1, PlayerFoot = new PlayerFoot { Id = 1, FootName = "Left" }, Salary = 160, AdditionalInformation = "no info", CreationDate = DateTime.UtcNow, PlayerId = "leomessi" },
                new PlayerOffer { Id = 2, ClubAdvertisementId = 2, OfferStatusId = 2, OfferStatus = new OfferStatus { Id = 2, StatusName = "Accepted" }, PlayerPositionId = 14, PlayerPosition = new PlayerPosition { Id = 14, PositionName = "Centre-Forward" }, Age = 37, Height = 167, PlayerFootId = 1, PlayerFoot = new PlayerFoot { Id = 1, FootName = "Left" }, Salary = 155, AdditionalInformation = "no info", CreationDate = DateTime.UtcNow, PlayerId = "leomessi" },
            });
        }
    }
}