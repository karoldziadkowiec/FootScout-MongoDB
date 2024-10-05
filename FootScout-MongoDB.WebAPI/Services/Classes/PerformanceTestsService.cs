using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.Constants;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Services.Classes
{
    public class PerformanceTestsService : IPerformanceTestsService
    {
        private readonly MongoDBContext _dbContext;
        private readonly IPasswordService _passwordService;
        private readonly INewIdGeneratorService _newIdGeneratorService;

        public PerformanceTestsService(MongoDBContext dbContext, IPasswordService passwordService, INewIdGeneratorService newIdGeneratorService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _newIdGeneratorService = newIdGeneratorService;
        }

        public async Task SeedComponents(int testCounter)
        {
            await SeedUsers(testCounter);
            await SeedClubHistories(testCounter);
            await SeedProblems(testCounter);
            await SeedChats(testCounter);
            await SeedMessages(testCounter);
            await SeedPlayerAdvertisements(testCounter);
            await SeedClubOffers(testCounter);
            await SeedClubAdvertisements(testCounter);
            await SeedPlayerOffers(testCounter);
        }

        public async Task ClearDatabaseOfSeededComponents()
        {
            await ClearAchievements();
            await ClearClubHistories();
            await ClearProblems();
            await ClearMessages();
            await ClearChats();
            await ClearSalaryRanges();
            await ClearPlayerAdvertisements();
            await ClearClubOffers();
            await ClearClubAdvertisements();
            await ClearPlayerOffers();
            await ClearUsers();
        }

        // Seeding
        private async Task SeedUsers(int testCounter)
        {
            var users = new List<User>();
            var userRoles = new List<UserRole>();

            var firstUserRoleId = await _newIdGeneratorService.GenerateNewUserRoleId();
            var userRole = await _dbContext.RolesCollection.Find(r => r.Name == RoleName.User).FirstOrDefaultAsync();

            // users
            for (int i = 1; i <= testCounter; i++)
            {
                string userId = $"user{i}";

                var user = new User
                {
                    Id = userId,
                    Email = $"user{i}@mail.com",
                    FirstName = $"FirstName {i}",
                    LastName = $"LastName {i}",
                    Location = $"Location {i}",
                    PhoneNumber = $"123456789",
                    CreationDate = DateTime.Now
                };
                user.PasswordHash = _passwordService.HashPassword($"Password{i}!");
                users.Add(user);

                // user roles
                userRoles.Add(new UserRole
                {
                    Id = firstUserRoleId + i - 1,
                    UserId = userId,
                    User = user,
                    RoleId = userRole.Id,
                    Role = userRole,
                });
            }
            await _dbContext.UsersCollection.InsertManyAsync(users);
            await _dbContext.UserRolesCollection.InsertManyAsync(userRoles);
        }

        private async Task SeedClubHistories(int testCounter)
        {
            var achievements = new List<Achievements>();
            var clubHistories = new List<ClubHistory>();

            // achievements
            for (int i = 1; i <= testCounter; i++)
            {
                achievements.Add(new Achievements
                {
                    Id = i,
                    NumberOfMatches = i,
                    Goals = i,
                    Assists = i,
                    AdditionalAchievements = $"Achievement {i}"
                });
            }
            await _dbContext.AchievementsCollection.InsertManyAsync(achievements);

            // club histories
            var achievementIds = achievements.Select(a => a.Id).ToList();
            var userIds = Enumerable.Range(1, testCounter).Select(i => $"user{i}").ToList();
            var players = await _dbContext.UsersCollection.Find(u => userIds.Contains(u.Id)).ToListAsync();
            for (int i = 1; i <= testCounter; i++)
            {
                var player = players.FirstOrDefault(u => u.Id == $"user{i}");

                clubHistories.Add(new ClubHistory
                {
                    Id = i,
                    PlayerPositionId = 1,
                    PlayerPosition = new PlayerPosition { Id = 1, PositionName = "Goalkeeper" },
                    ClubName = $"ClubName {i}",
                    League = $"League {i}",
                    Region = $"Region {i}",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(150),
                    AchievementsId = achievementIds[i - 1],
                    Achievements = achievements[i - 1],
                    PlayerId = player.Id,
                    Player = player
                });
            }
            await _dbContext.ClubHistoriesCollection.InsertManyAsync(clubHistories);
        }

        private async Task SeedProblems(int testCounter)
        {
            var problems = new List<Problem>();

            // problems
            var userIds = Enumerable.Range(1, testCounter).Select(i => $"user{i}").ToList();
            var requesters = await _dbContext.UsersCollection.Find(u => userIds.Contains(u.Id)).ToListAsync();
            for (int i = 1; i <= testCounter; i++)
            {
                var isSolvedParameter = (i % 2 == 0) ? true : false;
                var requester = requesters.FirstOrDefault(u => u.Id == $"user{i}");

                problems.Add(new Problem
                {
                    Id = i,
                    Title = $"Title {i}",
                    Description = $"Description {i}",
                    CreationDate = DateTime.Now,
                    IsSolved = isSolvedParameter,
                    RequesterId = $"user{i}",
                    Requester = requester
                });
            }
            await _dbContext.ProblemsCollection.InsertManyAsync(problems);
        }

        private async Task SeedChats(int testCounter)
        {
            var chats = new List<Chat>();

            // chats
            var userIds = Enumerable.Range(1, testCounter).Select(i => $"user{i}").ToList();
            var users = await _dbContext.UsersCollection.Find(u => userIds.Contains(u.Id)).ToListAsync();
            var userDictionary = users.ToDictionary(u => u.Id, u => u);
            for (int i = 1; i <= testCounter; i++)
            {
                var user1Id = (i == 1) ? "user2" : "user1";
                var user2Id = (i == 1) ? "user3" : $"user{i}";
                userDictionary.TryGetValue(user1Id, out var user1);
                userDictionary.TryGetValue(user2Id, out var user2);

                chats.Add(new Chat
                {
                    Id = i,
                    User1Id = user1Id,
                    User1 = user1,
                    User2Id = user2Id,
                    User2 = user2
                });
            }
            await _dbContext.ChatsCollection.InsertManyAsync(chats);
        }

        private async Task SeedMessages(int testCounter)
        {
            var messages = new List<Message>();

            // messages
            var chats = await _dbContext.ChatsCollection.Find(_ => true).Limit(testCounter).ToListAsync();
            var chatDictionary = chats.ToDictionary(c => c.Id, c => c);

            var userIds = Enumerable.Range(1, testCounter).Select(i => $"user{i}").ToList();
            var users = await _dbContext.UsersCollection.Find(u => userIds.Contains(u.Id)).ToListAsync();
            var userDictionary = users.ToDictionary(u => u.Id, u => u);

            for (int i = 1; i <= testCounter; i++)
            {
                chatDictionary.TryGetValue(chats[i - 1].Id, out var chat);
                var senderId = (i == 1) ? "user2" : "user1";
                var receiverId = (i == 1) ? "user3" : $"user{i}";
                userDictionary.TryGetValue(senderId, out var sender);
                userDictionary.TryGetValue(receiverId, out var receiver);

                messages.Add(new Message
                {
                    Id = i,
                    ChatId = chat.Id,
                    Chat = chat,
                    Content = $"Message {i}",
                    SenderId = senderId,
                    Sender = sender,
                    ReceiverId = receiverId,
                    Receiver = receiver,
                    Timestamp = DateTime.Now
                });
            }
            await _dbContext.MessagesCollection.InsertManyAsync(messages);
        }

        private async Task SeedPlayerAdvertisements(int testCounter)
        {
            var salaryRanges = new List<SalaryRange>();
            var playerAdvertisements = new List<PlayerAdvertisement>();
            var favoritePlayerAdvertisements = new List<FavoritePlayerAdvertisement>();

            // salary ranges
            for (int i = 1; i <= testCounter; i++)
            {
                salaryRanges.Add(new SalaryRange
                {
                    Id = i,
                    Min = i,
                    Max = i + 1
                });
            }
            await _dbContext.SalaryRangesCollection.InsertManyAsync(salaryRanges);

            // player advertisements
            var playerIds = Enumerable.Range(1, testCounter).Select(i => $"user{i}").ToList();
            var players = await _dbContext.UsersCollection.Find(u => playerIds.Contains(u.Id)).ToListAsync();
            var playerDictionary = players.ToDictionary(p => p.Id, p => p);
            for (int i = 1; i <= testCounter; i++)
            {
                playerDictionary.TryGetValue($"user{i}", out var player);

                playerAdvertisements.Add(new PlayerAdvertisement
                {
                    Id = i,
                    PlayerPositionId = 1,
                    PlayerPosition = new PlayerPosition { Id = 1, PositionName = "Goalkeeper" },
                    League = $"League {i}",
                    Region = $"Region {i}",
                    Age = i,
                    Height = i,
                    PlayerFootId = 1,
                    PlayerFoot = new PlayerFoot { Id = 1, FootName = "Left" },
                    SalaryRangeId = salaryRanges[i - 1].Id,
                    SalaryRange = salaryRanges[i - 1],
                    CreationDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(30),
                    PlayerId = $"user{i}",
                    Player = player
                });
            }
            await _dbContext.PlayerAdvertisementsCollection.InsertManyAsync(playerAdvertisements);

            // favorite player advertisements
            for (int i = 1; i <= testCounter; i++)
            {
                var userId = (i == testCounter) ? $"user1" : $"user{i + 1}";
                playerDictionary.TryGetValue(userId, out var user);

                favoritePlayerAdvertisements.Add(new FavoritePlayerAdvertisement
                {
                    Id = i,
                    PlayerAdvertisementId = playerAdvertisements[i - 1].Id,
                    PlayerAdvertisement = playerAdvertisements[i - 1],
                    UserId = userId,
                    User = user,
                });
            }
            await _dbContext.FavoritePlayerAdvertisementsCollection.InsertManyAsync(favoritePlayerAdvertisements);
        }

        private async Task SeedClubOffers(int testCounter)
        {
            var clubOffers = new List<ClubOffer>();

            // club offers
            var playerAdvertisements = await _dbContext.PlayerAdvertisementsCollection.Find(_ => true).Limit(testCounter).ToListAsync();
            var clubMemberIds = Enumerable.Range(1, testCounter).Select(i => $"user{i}").ToList();
            var clubMembers = await _dbContext.UsersCollection.Find(u => clubMemberIds.Contains(u.Id)).ToListAsync();
            var clubMemberDictionary = clubMembers.ToDictionary(cm => cm.Id, cm => cm);
            for (int i = 1; i <= testCounter; i++)
            {
                var counter = (i == testCounter) ? 1 : i+1;
                clubMemberDictionary.TryGetValue($"user{counter}", out var clubMember);

                clubOffers.Add(new ClubOffer
                {
                    Id = i,
                    PlayerAdvertisementId = playerAdvertisements[i - 1].Id,
                    PlayerAdvertisement = playerAdvertisements[i - 1],
                    OfferStatusId = 1,
                    OfferStatus = new OfferStatus { Id = 1, StatusName = "Offered" },
                    PlayerPositionId = 1,
                    PlayerPosition = new PlayerPosition { Id = 1, PositionName = "Goalkeeper" },
                    ClubName = $"ClubName {counter}",
                    League = $"League {counter}",
                    Region = $"Region {counter}",
                    Salary = counter,
                    AdditionalInformation = $"Info {counter}",
                    CreationDate = DateTime.Now,
                    ClubMemberId = $"user{counter}",
                    ClubMember = clubMember,
                });
            }
            await _dbContext.ClubOffersCollection.InsertManyAsync(clubOffers);
        }

        private async Task SeedClubAdvertisements(int testCounter)
        {
            var salaryRanges = new List<SalaryRange>();
            var clubAdvertisements = new List<ClubAdvertisement>();
            var favoriteClubAdvertisements = new List<FavoriteClubAdvertisement>();

            // salary ranges
            for (int i = 1; i <= testCounter; i++)
            {
                salaryRanges.Add(new SalaryRange
                {
                    Id = testCounter + i,
                    Min = i,
                    Max = i + 1
                });
            }
            await _dbContext.SalaryRangesCollection.InsertManyAsync(salaryRanges);

            // club advertisements
            var clubMemberIds = Enumerable.Range(1, testCounter).Select(i => $"user{i}").ToList();
            var clubMembers = await _dbContext.UsersCollection.Find(u => clubMemberIds.Contains(u.Id)).ToListAsync();
            var clubMemberDictionary = clubMembers.ToDictionary(cm => cm.Id, cm => cm);
            for (int i = 1; i <= testCounter; i++)
            {
                clubMemberDictionary.TryGetValue($"user{i}", out var clubMember);

                clubAdvertisements.Add(new ClubAdvertisement
                {
                    Id = i,
                    PlayerPositionId = 1,
                    PlayerPosition = new PlayerPosition { Id = 1, PositionName = "Goalkeeper" },
                    ClubName = $"ClubName {i}",
                    League = $"League {i}",
                    Region = $"Region {i}",
                    SalaryRangeId = salaryRanges[i - 1].Id,
                    SalaryRange = salaryRanges[i - 1],
                    CreationDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(30),
                    ClubMemberId = $"user{i}",
                    ClubMember = clubMember
                });
            }
            await _dbContext.ClubAdvertisementsCollection.InsertManyAsync(clubAdvertisements);

            // favorite club advertisements
            for (int i = 1; i <= testCounter; i++)
            {
                var userId = (i == testCounter) ? $"user1" : $"user{i + 1}";
                clubMemberDictionary.TryGetValue(userId, out var user);

                favoriteClubAdvertisements.Add(new FavoriteClubAdvertisement
                {
                    Id = i,
                    ClubAdvertisementId = clubAdvertisements[i - 1].Id,
                    ClubAdvertisement = clubAdvertisements[i - 1],
                    UserId = userId,
                    User = user
                });
            }
            await _dbContext.FavoriteClubAdvertisementsCollection.InsertManyAsync(favoriteClubAdvertisements);
        }

        private async Task SeedPlayerOffers(int testCounter)
        {
            var playerOffers = new List<PlayerOffer>();

            // club offers
            var clubAdvertisements = await _dbContext.ClubAdvertisementsCollection.Find(_ => true).Limit(testCounter).ToListAsync();
            var playerIds = Enumerable.Range(1, testCounter).Select(i => $"user{i}").ToList();
            var players = await _dbContext.UsersCollection.Find(u => playerIds.Contains(u.Id)).ToListAsync();
            var playerDictionary = players.ToDictionary(p => p.Id, p => p);
            for (int i = 1; i <= testCounter; i++)
            {
                var counter = (i == testCounter) ? 1 : i + 1;
                playerDictionary.TryGetValue($"user{counter}", out var player);

                playerOffers.Add(new PlayerOffer
                {
                    Id = i,
                    ClubAdvertisementId = clubAdvertisements[i - 1].Id,
                    ClubAdvertisement = clubAdvertisements[i - 1],
                    OfferStatusId = 1,
                    OfferStatus = new OfferStatus { Id = 1, StatusName = "Offered" },
                    PlayerPositionId = 1,
                    PlayerPosition = new PlayerPosition { Id = 1, PositionName = "Goalkeeper" },
                    Age = counter,
                    Height = counter,
                    PlayerFootId = 1,
                    PlayerFoot = new PlayerFoot { Id = 1, FootName = "Left" },
                    Salary = counter,
                    AdditionalInformation = $"Info {counter}",
                    CreationDate = DateTime.Now,
                    PlayerId = $"user{counter}",
                    Player = player
                });
            }
            await _dbContext.PlayerOffersCollection.InsertManyAsync(playerOffers);
        }

        // Clearing
        private async Task ClearAchievements()
        {
            await _dbContext.AchievementsCollection.DeleteManyAsync(_ => true);
        }

        private async Task ClearClubHistories()
        {
            await _dbContext.ClubHistoriesCollection.DeleteManyAsync(_ => true);
        }

        private async Task ClearProblems()
        {
            await _dbContext.ProblemsCollection.DeleteManyAsync(_ => true);
        }

        private async Task ClearMessages()
        {
            await _dbContext.MessagesCollection.DeleteManyAsync(_ => true);
        }

        private async Task ClearChats()
        {
            await _dbContext.ChatsCollection.DeleteManyAsync(_ => true);
        }

        private async Task ClearSalaryRanges()
        {
            await _dbContext.SalaryRangesCollection.DeleteManyAsync(_ => true);
        }

        private async Task ClearPlayerAdvertisements()
        {
            await _dbContext.FavoritePlayerAdvertisementsCollection.DeleteManyAsync(_ => true);
            await _dbContext.PlayerAdvertisementsCollection.DeleteManyAsync(_ => true);
        }

        private async Task ClearClubOffers()
        {
            await _dbContext.ClubOffersCollection.DeleteManyAsync(_ => true);
        }

        private async Task ClearClubAdvertisements()
        {
            await _dbContext.FavoriteClubAdvertisementsCollection.DeleteManyAsync(_ => true);
            await _dbContext.ClubAdvertisementsCollection.DeleteManyAsync(_ => true);
        }

        private async Task ClearPlayerOffers()
        {
            await _dbContext.PlayerOffersCollection.DeleteManyAsync(_ => true);
        }

        private async Task ClearUsers()
        {
            var usersToRemove = await _dbContext.UsersCollection.Find(u => u.Email != "admin@admin.com" && u.Email != "unknown@unknown.com").ToListAsync();
            var userIdsToRemove = usersToRemove.Select(u => u.Id).ToList();

            await _dbContext.UserRolesCollection.DeleteManyAsync(ur => userIdsToRemove.Contains(ur.UserId));
            await _dbContext.UsersCollection.DeleteManyAsync(u => userIdsToRemove.Contains(u.Id));
        }
    }
}