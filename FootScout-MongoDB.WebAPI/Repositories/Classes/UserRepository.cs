﻿using AutoMapper;
using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;
using System.Text;

namespace FootScout_MongoDB.WebAPI.Services.Classes
{
    public class UserRepository : IUserRepository
    {
        private readonly MongoDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly INewIdGeneratorService _newIdGeneratorService;
        private readonly IPasswordService _passwordService;

        public UserRepository(MongoDBContext dbContext, INewIdGeneratorService newIdGeneratorService, IMapper mapper, IPasswordService passwordService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _newIdGeneratorService = newIdGeneratorService;
            _passwordService = passwordService;
        }
        
        public async Task<User> FindUserByEmail(string email)
        {
            return await _dbContext.UsersCollection
                .Find(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> GetRolesForUser(string userId)
        {
            var userRoles = await _dbContext.UserRolesCollection
                .Find(ur => ur.UserId == userId)
                .ToListAsync();

            var roleIds = userRoles.Select(ur => ur.RoleId).Distinct();
            var roles = await _dbContext.RolesCollection
                .Find(r => roleIds.Contains(r.Id))
                .ToListAsync();

            return roles.Select(r => r.Name);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleName(string roleName)
        {
            var role = await _dbContext.RolesCollection
                .Find(r => r.Name == roleName)
                .FirstOrDefaultAsync();

            if (role == null)
                throw new ArgumentException($"Role {roleName} does not exist.");

            var userRoles = await _dbContext.UserRolesCollection
                .Find(ur => ur.RoleId == role.Id)
                .ToListAsync();

            var userIds = userRoles.Select(ur => ur.UserId).ToList();

            var users = await _dbContext.UsersCollection
                .Find(u => userIds.Contains(u.Id))
                .ToListAsync();

            return users;
        }

        public async Task<UserDTO> GetUser(string userId)
        {
            var user = await _dbContext.UsersCollection
                .Find(u => u.Id == userId)
                .FirstOrDefaultAsync();

            var userDTO = _mapper.Map<UserDTO>(user);
            return userDTO;
        }

        public async Task<IEnumerable<UserDTO>> GetUsers()
        {
            var users = await _dbContext.UsersCollection
                .Find(Builders<User>.Filter.Empty)
                .SortByDescending(u => u.CreationDate)
                .ToListAsync();

            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);
            return userDTOs;
        }

        public async Task<IEnumerable<UserDTO>> GetOnlyUsers()
        {
            var onlyUsers = await GetUsersByRoleName("User");
            var sortedUsers = onlyUsers.OrderByDescending(u => u.CreationDate);
            var onlyUserDTOs = _mapper.Map<IEnumerable<UserDTO>>(sortedUsers);
            return onlyUserDTOs;
        }

        public async Task<IEnumerable<UserDTO>> GetOnlyAdmins()
        {
            var onlyAdmins = await GetUsersByRoleName("Admin");
            var sortedAdmins = onlyAdmins.OrderByDescending(u => u.CreationDate);
            var onlyAdminDTOs = _mapper.Map<IEnumerable<UserDTO>>(sortedAdmins);
            return onlyAdminDTOs;
        }

        public async Task<string> GetUserRole(string userId)
        {
            var userRole = await _dbContext.UserRolesCollection
                .Find(ur => ur.UserId == userId)
                .FirstOrDefaultAsync();

            if (userRole == null)
                return null;

            var role = await _dbContext.RolesCollection
                .Find(r => r.Id == userRole.RoleId)
                .FirstOrDefaultAsync();

            return role?.Name;
        }

        public async Task<int> GetUserCount()
        {
            return (int)await _dbContext.UsersCollection.CountDocumentsAsync(FilterDefinition<User>.Empty);
        }

        public async Task UpdateUser(string userId, UserUpdateDTO dto)
        {
            var user = await _dbContext.UsersCollection
                .Find(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                _mapper.Map(dto, user);
                await _dbContext.UsersCollection.ReplaceOneAsync(u => u.Id == userId, user);
            }
        }

        public async Task ResetUserPassword(string userId, UserResetPasswordDTO dto)
        {
            if (!dto.PasswordHash.Equals(dto.ConfirmPasswordHash))
                throw new ArgumentException("Confirmed password is different.");

            var user = await _dbContext.UsersCollection
                .Find(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                if (!string.IsNullOrEmpty(dto.PasswordHash))
                    user.PasswordHash = _passwordService.HashPassword(dto.PasswordHash);

                await _dbContext.UsersCollection.ReplaceOneAsync(u => u.Id == userId, user);
            }
        }

        public async Task DeleteUser(string userId)
        {
            var user = await _dbContext.UsersCollection
                .Find(u => u.Id == userId)
                .FirstOrDefaultAsync();
            if (user == null)
                throw new Exception("User not found");

            var clubHistories = await _dbContext.ClubHistoriesCollection
                .Find(ch => ch.PlayerId == userId)
                .ToListAsync();

            foreach (var clubHistory in clubHistories)
            {
                if (clubHistory.AchievementsId != null)
                {
                    var achievements = await _dbContext.AchievementsCollection
                        .Find(a => a.Id == clubHistory.AchievementsId)
                        .FirstOrDefaultAsync();
                    if (achievements != null)
                        await _dbContext.AchievementsCollection
                            .DeleteOneAsync(a => a.Id == clubHistory.AchievementsId);
                }
            }

            await _dbContext.ClubHistoriesCollection
                .DeleteManyAsync(ch => ch.PlayerId == userId);

            var chats = await _dbContext.ChatsCollection
                .Find(c => c.User1Id == userId || c.User2Id == userId)
                .ToListAsync();

            foreach (var chat in chats)
            {
                if (chat.User1Id != null && chat.User2Id != null)
                {
                    await _dbContext.MessagesCollection
                        .DeleteManyAsync(m => m.ChatId == chat.Id);
                }
            }

            await _dbContext.ChatsCollection
                .DeleteManyAsync(c => c.User1Id == userId || c.User2Id == userId);

            await _dbContext.FavoritePlayerAdvertisementsCollection
                .DeleteManyAsync(fpa => fpa.UserId == userId);

            await _dbContext.FavoriteClubAdvertisementsCollection
                .DeleteManyAsync(fca => fca.UserId == userId);

            var unknownUser = await _dbContext.UsersCollection
                .Find(u => u.Email == "unknown@unknown.com")
                .FirstOrDefaultAsync();

            if (unknownUser == null)
                throw new InvalidOperationException("Unknown user not found");

            var unknownUserId = unknownUser.Id;

            var offeredStatus = await _dbContext.OfferStatusesCollection
                .Find(a => a.StatusName == "Offered")
                .FirstOrDefaultAsync();

            var rejectedStatus = await _dbContext.OfferStatusesCollection
                .Find(a => a.StatusName == "Rejected")
                .FirstOrDefaultAsync();

            var playerAdvertisements = await _dbContext.PlayerAdvertisementsCollection
                .Find(pa => pa.PlayerId == userId)
                .ToListAsync();

            foreach (var advertisement in playerAdvertisements)
            {
                advertisement.EndDate = DateTime.Now;
                advertisement.PlayerId = unknownUserId;

                await _dbContext.PlayerAdvertisementsCollection
                    .ReplaceOneAsync(pa => pa.Id == advertisement.Id, advertisement);
            }

            var clubOffers = await _dbContext.ClubOffersCollection
                .Find(co => co.ClubMemberId == userId)
                .ToListAsync();

            foreach (var offer in clubOffers)
            {
                if (offer.OfferStatusId == offeredStatus.Id)
                {
                    offer.OfferStatusId = rejectedStatus.Id;
                }
                offer.ClubMemberId = unknownUserId;

                await _dbContext.ClubOffersCollection
                    .ReplaceOneAsync(co => co.Id == offer.Id, offer);
            }

            var clubAdvertisements = await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.ClubMemberId == userId)
                .ToListAsync();

            foreach (var advertisement in clubAdvertisements)
            {
                advertisement.EndDate = DateTime.Now;
                advertisement.ClubMemberId = unknownUserId;

                await _dbContext.ClubAdvertisementsCollection
                    .ReplaceOneAsync(ca => ca.Id == advertisement.Id, advertisement);
            }

            var playerOffers = await _dbContext.PlayerOffersCollection
                .Find(po => po.PlayerId == userId)
                .ToListAsync();

            foreach (var offer in playerOffers)
            {
                if (offer.OfferStatusId == offeredStatus.Id)
                {
                    offer.OfferStatusId = rejectedStatus.Id;
                }
                offer.PlayerId = unknownUserId;

                await _dbContext.PlayerOffersCollection
                    .ReplaceOneAsync(po => po.Id == offer.Id, offer);
            }

            await _dbContext.UsersCollection
                .DeleteOneAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<ClubHistory>> GetUserClubHistory(string userId)
        {
            return await _dbContext.ClubHistoriesCollection
                .Find(ch => ch.PlayerId == userId)
                .SortByDescending(ch => ch.StartDate)
                .ThenByDescending(ch => ch.EndDate)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<PlayerAdvertisement>> GetUserPlayerAdvertisements(string userId)
        {
            return await _dbContext.PlayerAdvertisementsCollection
                .Find(pa => pa.PlayerId == userId)
                .SortByDescending(pa => pa.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerAdvertisement>> GetUserActivePlayerAdvertisements(string userId)
        {
            return await _dbContext.PlayerAdvertisementsCollection
                .Find(pa => pa.PlayerId == userId && pa.EndDate >= DateTime.Now)
                .SortBy(pa => pa.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerAdvertisement>> GetUserInactivePlayerAdvertisements(string userId)
        {
            return await _dbContext.PlayerAdvertisementsCollection
                .Find(pa => pa.PlayerId == userId && pa.EndDate < DateTime.Now)
                .SortByDescending(pa => pa.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserFavoritePlayerAdvertisements(string userId)
        {
            return await _dbContext.FavoritePlayerAdvertisementsCollection
                .Find(pa => pa.UserId == userId)
                .SortByDescending(pa => pa.PlayerAdvertisement.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserActiveFavoritePlayerAdvertisements(string userId)
        {
            return await _dbContext.FavoritePlayerAdvertisementsCollection
                .Find(pa => pa.UserId == userId && pa.PlayerAdvertisement.EndDate >= DateTime.Now)
                .SortBy(pa => pa.PlayerAdvertisement.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserInactiveFavoritePlayerAdvertisements(string userId)
        {
            return await _dbContext.FavoritePlayerAdvertisementsCollection
                .Find(pa => pa.UserId == userId && pa.PlayerAdvertisement.EndDate < DateTime.Now)
                .SortByDescending(pa => pa.PlayerAdvertisement.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClubOffer>> GetReceivedClubOffers(string userId)
        {
            return await _dbContext.ClubOffersCollection
                .Find(co => co.PlayerAdvertisement.PlayerId == userId)
                .SortByDescending(co => co.PlayerAdvertisement.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClubOffer>> GetSentClubOffers(string userId)
        {
            return await _dbContext.ClubOffersCollection
                .Find(co => co.ClubMemberId == userId)
                .SortByDescending(co => co.PlayerAdvertisement.CreationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetUserClubAdvertisements(string userId)
        {
            return await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.ClubMemberId == userId)
                .SortByDescending(ca => ca.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetUserActiveClubAdvertisements(string userId)
        {
            return await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.ClubMemberId == userId && ca.EndDate >= DateTime.Now)
                .SortBy(ca => ca.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetUserInactiveClubAdvertisements(string userId)
        {
            return await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.ClubMemberId == userId && ca.EndDate < DateTime.Now)
                .SortByDescending(ca => ca.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FavoriteClubAdvertisement>> GetUserFavoriteClubAdvertisements(string userId)
        {
            return await _dbContext.FavoriteClubAdvertisementsCollection
                .Find(fca => fca.UserId == userId)
                .SortByDescending(ca => ca.ClubAdvertisement.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FavoriteClubAdvertisement>> GetUserActiveFavoriteClubAdvertisements(string userId)
        {
            return await _dbContext.FavoriteClubAdvertisementsCollection
                .Find(fc => fc.UserId == userId && fc.ClubAdvertisement.EndDate >= DateTime.Now)
                .SortByDescending(ca => ca.ClubAdvertisement.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FavoriteClubAdvertisement>> GetUserInactiveFavoriteClubAdvertisements(string userId)
        {
            return await _dbContext.FavoriteClubAdvertisementsCollection
                .Find(fc => fc.UserId == userId && fc.ClubAdvertisement.EndDate < DateTime.Now)
                .SortByDescending(ca => ca.ClubAdvertisement.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerOffer>> GetReceivedPlayerOffers(string userId)
        {
            return await _dbContext.PlayerOffersCollection
                .Find(po => po.ClubAdvertisement.ClubMemberId == userId)
                .SortByDescending(ca => ca.ClubAdvertisement.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerOffer>> GetSentPlayerOffers(string userId)
        {
            return await _dbContext.PlayerOffersCollection
                .Find(po => po.PlayerId == userId)
                .SortByDescending(ca => ca.ClubAdvertisement.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Chat>> GetUserChats(string userId)
        {
            var chats = await _dbContext.ChatsCollection
                .Find(c => c.User1Id == userId || c.User2Id == userId)
                .ToListAsync();

            var chatWithLastMessageTimestamps = new List<(Chat Chat, DateTime? LastMessageTimestamp)>();

            foreach (var chat in chats)
            {
                var lastMessageTimestamp = await _dbContext.MessagesCollection
                    .Find(m => m.ChatId == chat.Id)
                    .SortByDescending(m => m.Timestamp)
                    .Project(m => (DateTime?)m.Timestamp)
                    .FirstOrDefaultAsync();

                chatWithLastMessageTimestamps.Add((chat, lastMessageTimestamp));
            }

            return chatWithLastMessageTimestamps
                .OrderByDescending(c => c.LastMessageTimestamp)
                .Select(c => c.Chat)
                .ToList();
        }

        public async Task<MemoryStream> ExportUsersToCsv()
        {
            var users = await GetUsers();
            var csv = new StringBuilder();
            csv.AppendLine("E-mail,First Name,Last Name,Phone Number,Location,Creation Date");

            foreach (var user in users)
            {
                csv.AppendLine($"{user.Email},{user.FirstName},{user.LastName},{user.PhoneNumber},{user.Location},{user.CreationDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}