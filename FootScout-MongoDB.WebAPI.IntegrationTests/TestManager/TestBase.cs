using AutoMapper;
using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;
using Message = FootScout_MongoDB.WebAPI.Entities.Message;
using FootScout_MongoDB.WebAPI.Services.Classes;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.TestManager
{
    public class TestBase
    {
        public static IMapper CreateMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            return configuration.CreateMapper();
        }

        public static IPasswordHasher<User> CreatePasswordHasher()
        {
            return new PasswordHasher<User>();
        }

        public static IConfiguration CreateConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"JWT:Secret", "keyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy"},
                {"JWT:ValidAudience", "http://localhost:3000"},
                {"JWT:ValidIssuer", "https://localhost:7220"},
                {"JWT:ExpireDays", "1"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return configuration;
        }

        // Scenario

        public static async Task SeedRoleTestBase(MongoDBContext dbContext)
        {
            var roles = new List<Role> 
            {
                new Role { Name = RoleName.Admin },
                new Role { Name = RoleName.User }
            };

            foreach (var role in roles)
            {
                role.Id = Guid.NewGuid().ToString();
                try
                {
                    await dbContext.RolesCollection.InsertOneAsync(role);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for role with ID: {role.Id}");
                }
            }
        }

        public static async Task SeedUserTestBase(MongoDBContext dbContext)
        {
            // users
            await dbContext.UsersCollection.InsertManyAsync(new List<User>
            {
                new User { Id = "admin0", Email = "admin@admin.com", PasswordHash = "Admin1!", FirstName = "Admin F.", LastName = "Admin L.", Location = "Admin Loc.", PhoneNumber = "000000000", CreationDate = DateTime.Now },
                new User { Id = "unknown9", Email = "unknown@unknown.com", PasswordHash = "Uuuuu1!",FirstName = "Unknown F.", LastName = "Unknown L.", Location = "Unknown Loc.", PhoneNumber = "999999999", CreationDate = DateTime.Now },
                new User { Id = "leomessi", Email = "lm10@gmail.com",  PasswordHash = "Leooo1!",FirstName = "Leo", LastName = "Messi", Location = "Miami", PhoneNumber = "101010101", CreationDate = DateTime.Now },
                new User { Id = "pepguardiola", Email = "pg8@gmail.com", PasswordHash = "Peppp1!",FirstName = "Pep", LastName = "Guardiola", Location = "Manchester", PhoneNumber = "868686868", CreationDate = DateTime.Now }
            });

            // user roles
            var adminRole = await dbContext.RolesCollection.Find(r => r.Name == RoleName.Admin).FirstOrDefaultAsync();
            var userRole = await dbContext.RolesCollection.Find(r => r.Name == RoleName.User).FirstOrDefaultAsync();

            var usersWithRoles = new List<UserRole>
            {
                new UserRole { Id = 1, UserId = "admin0", RoleId = adminRole.Id, Role = adminRole },
                new UserRole { Id = 2, UserId = "unknown9", RoleId = userRole.Id, Role = userRole },
                new UserRole { Id = 3, UserId = "leomessi", RoleId = userRole.Id, Role = userRole },
                new UserRole { Id = 4, UserId = "pepguardiola", RoleId = userRole.Id, Role = userRole }
            };

            foreach (var userWithRole in usersWithRoles)
            {
                try
                {
                    userWithRole.User = await dbContext.UsersCollection.Find(u => u.Id == userWithRole.UserId).FirstOrDefaultAsync();
                    await dbContext.UserRolesCollection.InsertOneAsync(userWithRole);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for user role with ID: {userWithRole.Id}");
                }
            }
        }

        public static async Task SeedOfferStatusTestBase(MongoDBContext dbContext)
        {
            var statuses = new List<string> { OfferStatusName.Offered, OfferStatusName.Accepted, OfferStatusName.Rejected };

            var existingStatuses = await dbContext.OfferStatusesCollection
                .Find(_ => true)
                .Project(os => os.StatusName)
                .ToListAsync();

            var newStatuses = new List<OfferStatus>();

            for (int i = 0; i < statuses.Count; i++)
            {
                var status = statuses[i];

                if (!existingStatuses.Contains(status))
                {
                    var newStatus = new OfferStatus
                    {
                        Id = i + 1,
                        StatusName = status
                    };

                    newStatuses.Add(newStatus);
                }
            }

            if (newStatuses.Any())
            {
                try
                {
                    await dbContext.OfferStatusesCollection.InsertManyAsync(newStatuses);
                }
                catch (MongoBulkWriteException ex)
                {
                    Console.WriteLine($"Error occurred while seeding offer statuses: {ex.Message}");
                }
            }
        }

        public static async Task SeedPlayerPositionTestBase(MongoDBContext dbContext)
        {
            var positions = new List<string>
            {
                Position.Goalkeeper, Position.RightBack, Position.CenterBack, Position.LeftBack, Position.RightWingBack,
                Position.LeftWingBack, Position.CentralDefensiveMidfield, Position.CentralMidfield,
                Position.CentralAttackingMidfield, Position.RightMidfield, Position.RightWing, Position.LeftMidfield,
                Position.LeftWing, Position.CentreForward, Position.Striker
            };

            var existingPositions = await dbContext.PlayerPositionsCollection
                .Find(_ => true)
                .Project(p => p.PositionName)
                .ToListAsync();

            var newPositions = new List<PlayerPosition>();

            for (int i = 0; i < positions.Count; i++)
            {
                var position = positions[i];

                if (!existingPositions.Contains(position))
                {
                    var newPosition = new PlayerPosition
                    {
                        Id = i + 1,
                        PositionName = position
                    };

                    newPositions.Add(newPosition);
                }
            }

            if (newPositions.Any())
            {
                try
                {
                    await dbContext.PlayerPositionsCollection.InsertManyAsync(newPositions);
                }
                catch (MongoBulkWriteException ex)
                {
                    Console.WriteLine($"Error occurred while seeding player positions: {ex.Message}");
                }
            }
        }

        public static async Task SeedPlayerFootTestBase(MongoDBContext dbContext)
        {
            var feet = new List<string> { Foot.Left, Foot.Right, Foot.TwoFooted };

            var existingFeet = await dbContext.PlayerFeetCollection
                .Find(_ => true)
                .Project(p => p.FootName)
                .ToListAsync();

            var newFeet = new List<PlayerFoot>();

            for (int i = 0; i < feet.Count; i++)
            {
                var foot = feet[i];

                if (!existingFeet.Contains(foot))
                {
                    var newFoot = new PlayerFoot
                    {
                        Id = i + 1,
                        FootName = foot
                    };

                    newFeet.Add(newFoot);
                }
            }

            if (newFeet.Any())
            {
                try
                {
                    await dbContext.PlayerFeetCollection.InsertManyAsync(newFeet);
                }
                catch (MongoBulkWriteException ex)
                {
                    Console.WriteLine($"Error occurred while seeding player feet: {ex.Message}");
                }
            }
        }

        public static async Task SeedClubHistoryTestBase(MongoDBContext dbContext)
        {
            // achievements
            var achievements = new List<Achievements>
            {
                new Achievements { Id = 1, NumberOfMatches = 750, Goals = 678, Assists = 544, AdditionalAchievements = "LM" },
                new Achievements { Id = 2, NumberOfMatches = 40, Goals = 27, Assists = 24, AdditionalAchievements = "No info" },
            };

            foreach (var achievement in achievements)
            {
                try
                {
                    await dbContext.AchievementsCollection.InsertOneAsync(achievement);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for achievement with ID: {achievement.Id}");
                }
            }

            // club history
            var clubHistories = new List<ClubHistory>
            {
                new ClubHistory { Id = 1, PlayerPositionId = 15, ClubName = "FC Barcelona", League = "La Liga", Region = "Spain", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(150), AchievementsId = 1, PlayerId = "leomessi"},
                new ClubHistory { Id = 2, PlayerPositionId = 15, ClubName = "PSG", League = "Ligue1", Region = "France", StartDate = DateTime.Now.AddDays(150), EndDate = DateTime.Now.AddDays(300), AchievementsId = 2, PlayerId = "leomessi"},
            };

            foreach (var clubHistory in clubHistories)
            {
                clubHistory.PlayerPosition = await dbContext.PlayerPositionsCollection.Find(pp => pp.Id == clubHistory.PlayerPositionId).FirstOrDefaultAsync();
                clubHistory.Achievements = await dbContext.AchievementsCollection.Find(a => a.Id == clubHistory.AchievementsId).FirstOrDefaultAsync();
                clubHistory.Player = await dbContext.UsersCollection.Find(u => u.Id == clubHistory.PlayerId).FirstOrDefaultAsync();

                try
                {
                    await dbContext.ClubHistoriesCollection.InsertOneAsync(clubHistory);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for club history with ID: {clubHistory.Id}");
                }
            }
        }

        public static async Task SeedProblemTestBase(MongoDBContext dbContext)
        {
            // problems
            var problems = new List<Problem>
            {
                new Problem { Id = 1, Title = "Problem 1", Description = "Desc 1", CreationDate = DateTime.Now, IsSolved = true, RequesterId = "leomessi" },
                new Problem { Id = 2, Title = "Problem 2", Description = "Desc 2", CreationDate = DateTime.Now.AddDays(150), IsSolved = false, RequesterId = "pepguardiola" },
            };

            foreach (var problem in problems)
            {
                problem.Requester = await dbContext.UsersCollection.Find(u => u.Id == problem.RequesterId).FirstOrDefaultAsync();
                try
                {
                    await dbContext.ProblemsCollection.InsertOneAsync(problem);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for problem with ID: {problem.Id}");
                }
            }
        }

        public static async Task SeedChatTestBase(MongoDBContext dbContext)
        {
            // chats
            var chats = new List<Chat>
            {
                new Chat { Id = 1, User1Id = "leomessi", User2Id = "pepguardiola" },
                new Chat { Id = 2, User1Id = "admin0", User2Id = "leomessi" },
            };

            foreach (var chat in chats)
            {
                chat.User1 = await dbContext.UsersCollection.Find(u => u.Id == chat.User1Id).FirstOrDefaultAsync();
                chat.User2 = await dbContext.UsersCollection.Find(u => u.Id == chat.User2Id).FirstOrDefaultAsync();
                try
                {
                    await dbContext.ChatsCollection.InsertOneAsync(chat);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for chat with ID: {chat.Id}");
                }
            }
        }

        public static async Task SeedMessageTestBase(MongoDBContext dbContext)
        {
            // messages
            var messages = new List<Message>
            {
                new Message { Id = 1, ChatId = 1, Content = "Hey", SenderId = "pepguardiola", ReceiverId = "leomessi", Timestamp = DateTime.Now },
                new Message { Id = 2, ChatId = 1, Content = "Hello", SenderId = "leomessi", ReceiverId = "pepguardiola", Timestamp = DateTime.Now },
                new Message { Id = 3, ChatId = 2, Content = "a b c", SenderId = "admin0", ReceiverId = "leomessi", Timestamp = DateTime.Now },
            };

            foreach (var message in messages)
            {
                message.Chat = await dbContext.ChatsCollection.Find(c => c.Id == message.ChatId).FirstOrDefaultAsync();
                message.Sender = await dbContext.UsersCollection.Find(u => u.Id == message.SenderId).FirstOrDefaultAsync();
                message.Receiver = await dbContext.UsersCollection.Find(u => u.Id == message.ReceiverId).FirstOrDefaultAsync();
                try
                {
                    await dbContext.MessagesCollection.InsertOneAsync(message);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for message with ID: {message.Id}");
                }
            }
        }

        public static async Task SeedPlayerAdvertisementTestBase(MongoDBContext dbContext)
        {
            // Salary range
            var salaryRanges = new List<SalaryRange>
            {
                new SalaryRange { Id = 1, Min = 150, Max = 200 },
                new SalaryRange { Id = 2, Min = 145, Max = 195 },
            };

            foreach (var salaryRange in salaryRanges)
            {
                try
                {
                    await dbContext.SalaryRangesCollection.InsertOneAsync(salaryRange);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for salary range with ID: {salaryRange.Id}");
                }
            }

            // Player advertisement
            var playerAdvertisements = new List<PlayerAdvertisement>
            {
                new PlayerAdvertisement { Id = 1, PlayerPositionId = 15, League = "Premier League", Region = "England", Age = 37, Height = 167, PlayerFootId = 3, SalaryRangeId = 1, CreationDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), PlayerId = "leomessi" },
                new PlayerAdvertisement { Id = 2, PlayerPositionId = 14, League = "La Liga", Region = "Spain", Age = 37, Height = 167, PlayerFootId = 3, SalaryRangeId = 2, CreationDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), PlayerId = "leomessi" },
            };

            foreach (var advertisement in playerAdvertisements)
            {
                advertisement.PlayerPosition = await dbContext.PlayerPositionsCollection.Find(pp => pp.Id == advertisement.PlayerPositionId).FirstOrDefaultAsync();
                advertisement.PlayerFoot = await dbContext.PlayerFeetCollection.Find(pf => pf.Id == advertisement.PlayerFootId).FirstOrDefaultAsync();
                advertisement.SalaryRange = await dbContext.SalaryRangesCollection.Find(sr => sr.Id == advertisement.SalaryRangeId).FirstOrDefaultAsync();
                advertisement.Player = await dbContext.UsersCollection.Find(u => u.Id == advertisement.PlayerId).FirstOrDefaultAsync();
                try
                {
                    await dbContext.PlayerAdvertisementsCollection.InsertOneAsync(advertisement);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for player advertisement with ID: {advertisement.Id}");
                }
            }

            // Favorite player advertisement
            var favoriteAdvertisements = new List<FavoritePlayerAdvertisement>
            {
                new FavoritePlayerAdvertisement { Id = 1, PlayerAdvertisementId = 1, UserId = "pepguardiola" },
                new FavoritePlayerAdvertisement { Id = 2, PlayerAdvertisementId = 2, UserId = "pepguardiola" },
            };

            foreach (var favAdvertisement in favoriteAdvertisements)
            {
                favAdvertisement.PlayerAdvertisement = await dbContext.PlayerAdvertisementsCollection.Find(pa => pa.Id == favAdvertisement.PlayerAdvertisementId).FirstOrDefaultAsync();
                favAdvertisement.User = await dbContext.UsersCollection.Find(u => u.Id == favAdvertisement.UserId).FirstOrDefaultAsync();
                try
                {
                    await dbContext.FavoritePlayerAdvertisementsCollection.InsertOneAsync(favAdvertisement);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for favorite player advertisement with ID: {favAdvertisement.Id}");
                }
            }
        }

        public static async Task SeedClubOfferTestBase(MongoDBContext dbContext)
        {

            // club offer
            var clubOffers = new List<ClubOffer>
            {
                new ClubOffer { Id = 1, PlayerAdvertisementId = 1, OfferStatusId = 1, PlayerPositionId = 15, ClubName = "Manchester City", League = "Premier League", Region = "England", Salary = 160, AdditionalInformation = "no info", CreationDate = DateTime.Now, ClubMemberId = "pepguardiola" },
                new ClubOffer { Id = 2, PlayerAdvertisementId = 2, OfferStatusId = 2, PlayerPositionId = 14, ClubName = "Real Madrid", League = "La Liga", Region = "Spain", Salary = 155, AdditionalInformation = "no info", CreationDate = DateTime.Now, ClubMemberId = "pepguardiola" },
            };

            foreach (var clubOffer in clubOffers)
            {
                clubOffer.PlayerAdvertisement = await dbContext.PlayerAdvertisementsCollection.Find(pa => pa.Id == clubOffer.PlayerAdvertisementId).FirstOrDefaultAsync();
                clubOffer.OfferStatus = await dbContext.OfferStatusesCollection.Find(os => os.Id == clubOffer.OfferStatusId).FirstOrDefaultAsync();
                clubOffer.PlayerPosition = await dbContext.PlayerPositionsCollection.Find(pp => pp.Id == clubOffer.PlayerPositionId).FirstOrDefaultAsync();
                clubOffer.ClubMember = await dbContext.UsersCollection.Find(u => u.Id == clubOffer.ClubMemberId).FirstOrDefaultAsync();
                try
                {
                    await dbContext.ClubOffersCollection.InsertOneAsync(clubOffer);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for club offer with ID: {clubOffer.Id}");
                }
            }
        }

        public static async Task SeedClubAdvertisementTestBase(MongoDBContext dbContext)
        {
            // salary range
            var salaryRanges = new List<SalaryRange>
            {
                new SalaryRange { Id = 3, Min = 150, Max = 200 },
                new SalaryRange { Id = 4, Min = 145, Max = 195 },
            };

            foreach (var salaryRange in salaryRanges)
            {
                try
                {
                    await dbContext.SalaryRangesCollection.InsertOneAsync(salaryRange);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for salary range with ID: {salaryRange.Id}");
                }
            }

            // Club advertisement
            var clubAdvertisements = new List<ClubAdvertisement>
            {
                new ClubAdvertisement { Id = 1, PlayerPositionId = 15, ClubName = "Manchester City", League = "Premier League", Region = "England", SalaryRangeId = 3, CreationDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), ClubMemberId = "pepguardiola" },
                new ClubAdvertisement { Id = 2, PlayerPositionId = 14, ClubName = "Manchester City", League = "Premier League", Region = "England", SalaryRangeId = 4, CreationDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), ClubMemberId = "pepguardiola" },
            };

            foreach (var clubAdvertisement in clubAdvertisements)
            {
                clubAdvertisement.PlayerPosition = await dbContext.PlayerPositionsCollection.Find(pp => pp.Id == clubAdvertisement.PlayerPositionId).FirstOrDefaultAsync();
                clubAdvertisement.SalaryRange = await dbContext.SalaryRangesCollection.Find(sr => sr.Id == clubAdvertisement.SalaryRangeId).FirstOrDefaultAsync();
                clubAdvertisement.ClubMember = await dbContext.UsersCollection.Find(u => u.Id == clubAdvertisement.ClubMemberId).FirstOrDefaultAsync();
                try
                {
                    await dbContext.ClubAdvertisementsCollection.InsertOneAsync(clubAdvertisement);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for club advertisement with ID: {clubAdvertisement.Id}");
                }
            }

            // Favorite club advertisement
            var favoriteClubAdvertisements = new List<FavoriteClubAdvertisement>
            {
                new FavoriteClubAdvertisement { Id = 1, ClubAdvertisementId = 1, UserId = "leomessi" },
                new FavoriteClubAdvertisement { Id = 2, ClubAdvertisementId = 2, UserId = "leomessi" },
            };

            foreach (var favClubAdvertisement in favoriteClubAdvertisements)
            {
                favClubAdvertisement.ClubAdvertisement = await dbContext.ClubAdvertisementsCollection.Find(ca => ca.Id == favClubAdvertisement.ClubAdvertisementId).FirstOrDefaultAsync();
                favClubAdvertisement.User = await dbContext.UsersCollection.Find(u => u.Id == favClubAdvertisement.UserId).FirstOrDefaultAsync();
                try
                {
                    await dbContext.FavoriteClubAdvertisementsCollection.InsertOneAsync(favClubAdvertisement);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for favorite club advertisement with ID: {favClubAdvertisement.Id}");
                }
            }
        }

        public static async Task SeedPlayerOfferTestBase(MongoDBContext dbContext)
        {
            // player offer
            var playerOffers = new List<PlayerOffer>
            {
                new PlayerOffer { Id = 1, ClubAdvertisementId = 1, OfferStatusId = 1, PlayerPositionId = 15, Age = 37, Height = 167, PlayerFootId = 1, Salary = 160, AdditionalInformation = "no info", CreationDate = DateTime.Now, PlayerId = "leomessi" },
                new PlayerOffer { Id = 2, ClubAdvertisementId = 2, OfferStatusId = 2, PlayerPositionId = 14, Age = 37, Height = 167, PlayerFootId = 1, Salary = 155, AdditionalInformation = "no info", CreationDate = DateTime.Now, PlayerId = "leomessi" },
            };

            foreach (var playerOffer in playerOffers)
            {
                playerOffer.ClubAdvertisement = await dbContext.ClubAdvertisementsCollection.Find(ca => ca.Id == playerOffer.ClubAdvertisementId).FirstOrDefaultAsync();
                playerOffer.OfferStatus = await dbContext.OfferStatusesCollection.Find(os => os.Id == playerOffer.OfferStatusId).FirstOrDefaultAsync();
                playerOffer.PlayerPosition = await dbContext.PlayerPositionsCollection.Find(pp => pp.Id == playerOffer.PlayerPositionId).FirstOrDefaultAsync();
                playerOffer.PlayerFoot = await dbContext.PlayerFeetCollection.Find(pp => pp.Id == playerOffer.PlayerFootId).FirstOrDefaultAsync();
                playerOffer.Player = await dbContext.UsersCollection.Find(u => u.Id == playerOffer.PlayerId).FirstOrDefaultAsync();
                try
                {
                    await dbContext.PlayerOffersCollection.InsertOneAsync(playerOffer);
                }
                catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    Console.WriteLine($"Duplicate key error for player offer with ID: {playerOffer.Id}");
                }
            }
        }
    }
}