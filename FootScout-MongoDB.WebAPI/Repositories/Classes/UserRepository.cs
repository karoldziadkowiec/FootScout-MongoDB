using AutoMapper;
using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;
using System;
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
            var clubHistories = await _dbContext.ClubHistoriesCollection
                .Find(ch => ch.PlayerId == userId)
                .SortByDescending(ch => ch.StartDate)
                .ThenByDescending(ch => ch.EndDate)
                .ToListAsync();

            foreach (var clubHistory in clubHistories)
            {
                if (clubHistory.AchievementsId != null)
                {
                    clubHistory.Achievements = await _dbContext.AchievementsCollection
                        .Find(a => a.Id == clubHistory.AchievementsId)
                        .FirstOrDefaultAsync();
                }

                if (clubHistory.PlayerPositionId != null)
                {
                    clubHistory.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == clubHistory.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (clubHistory.PlayerId != null)
                {
                    clubHistory.Player = await _dbContext.UsersCollection
                        .Find(u => u.Id == clubHistory.PlayerId)
                        .FirstOrDefaultAsync();
                }
            }

            return clubHistories;
        }
        
        public async Task<IEnumerable<PlayerAdvertisement>> GetUserPlayerAdvertisements(string userId)
        {
            var playerAdvertisements = await _dbContext.PlayerAdvertisementsCollection
                .Find(pa => pa.PlayerId == userId)
                .SortByDescending(pa => pa.EndDate)
                .ToListAsync();

            foreach (var advertisement in playerAdvertisements)
            {
                if (advertisement.PlayerPositionId != null)
                {
                    advertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == advertisement.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.PlayerFootId != null)
                {
                    advertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                        .Find(pf => pf.Id == advertisement.PlayerFootId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.SalaryRangeId != null)
                {
                    advertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                        .Find(sr => sr.Id == advertisement.SalaryRangeId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.PlayerId != null)
                {
                    advertisement.Player = await _dbContext.UsersCollection
                        .Find(u => u.Id == advertisement.PlayerId)
                        .FirstOrDefaultAsync();
                }
            }

            return playerAdvertisements;
        }

        public async Task<IEnumerable<PlayerAdvertisement>> GetUserActivePlayerAdvertisements(string userId)
        {
            var activeAdvertisements = await _dbContext.PlayerAdvertisementsCollection
                .Find(pa => pa.PlayerId == userId && pa.EndDate >= DateTime.Now)
                .SortBy(pa => pa.EndDate)
                .ToListAsync();

            foreach (var advertisement in activeAdvertisements)
            {
                if (advertisement.PlayerPositionId != null)
                {
                    advertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == advertisement.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.PlayerFootId != null)
                {
                    advertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                        .Find(pf => pf.Id == advertisement.PlayerFootId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.SalaryRangeId != null)
                {
                    advertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                        .Find(sr => sr.Id == advertisement.SalaryRangeId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.PlayerId != null)
                {
                    advertisement.Player = await _dbContext.UsersCollection
                        .Find(u => u.Id == advertisement.PlayerId)
                        .FirstOrDefaultAsync();
                }
            }

            return activeAdvertisements;
        }

        public async Task<IEnumerable<PlayerAdvertisement>> GetUserInactivePlayerAdvertisements(string userId)
        {
            var inactiveAdvertisements = await _dbContext.PlayerAdvertisementsCollection
                .Find(pa => pa.PlayerId == userId && pa.EndDate < DateTime.Now)
                .SortByDescending(pa => pa.EndDate)
                .ToListAsync();

            foreach (var advertisement in inactiveAdvertisements)
            {
                if (advertisement.PlayerPositionId != null)
                {
                    advertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == advertisement.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.PlayerFootId != null)
                {
                    advertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                        .Find(pf => pf.Id == advertisement.PlayerFootId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.SalaryRangeId != null)
                {
                    advertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                        .Find(sr => sr.Id == advertisement.SalaryRangeId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.PlayerId != null)
                {
                    advertisement.Player = await _dbContext.UsersCollection
                        .Find(u => u.Id == advertisement.PlayerId)
                        .FirstOrDefaultAsync();
                }
            }

            return inactiveAdvertisements;
        }

        public async Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserFavoritePlayerAdvertisements(string userId)
        {
            var favoriteAdvertisements = await _dbContext.FavoritePlayerAdvertisementsCollection
                .Find(pa => pa.UserId == userId)
                .SortByDescending(pa => pa.PlayerAdvertisement.EndDate)
                .ToListAsync();

            foreach (var favorite in favoriteAdvertisements)
            {
                if (favorite.PlayerAdvertisementId != null)
                {
                    favorite.PlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection
                        .Find(pa => pa.Id == favorite.PlayerAdvertisementId)
                        .FirstOrDefaultAsync();

                    if (favorite.PlayerAdvertisement.PlayerPositionId != null)
                    {
                        favorite.PlayerAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                            .Find(pp => pp.Id == favorite.PlayerAdvertisement.PlayerPositionId)
                            .FirstOrDefaultAsync();
                    }

                    if (favorite.PlayerAdvertisement.PlayerFootId != null)
                    {
                        favorite.PlayerAdvertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                            .Find(pf => pf.Id == favorite.PlayerAdvertisement.PlayerFootId)
                            .FirstOrDefaultAsync();
                    }

                    if (favorite.PlayerAdvertisement.SalaryRangeId != null)
                    {
                        favorite.PlayerAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                            .Find(sr => sr.Id == favorite.PlayerAdvertisement.SalaryRangeId)
                            .FirstOrDefaultAsync();
                    }

                    if (favorite.PlayerAdvertisement.PlayerId != null)
                    {
                        favorite.PlayerAdvertisement.Player = await _dbContext.UsersCollection
                            .Find(u => u.Id == favorite.PlayerAdvertisement.PlayerId)
                            .FirstOrDefaultAsync();
                    }
                }

                if (favorite.UserId != null)
                {
                    favorite.User = await _dbContext.UsersCollection
                        .Find(u => u.Id == favorite.UserId)
                        .FirstOrDefaultAsync();
                }
            }

            return favoriteAdvertisements;
        }

        public async Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserActiveFavoritePlayerAdvertisements(string userId)
        {
            var activeFavorites = await _dbContext.FavoritePlayerAdvertisementsCollection
                .Find(pa => pa.UserId == userId && pa.PlayerAdvertisement.EndDate >= DateTime.Now)
                .SortBy(pa => pa.PlayerAdvertisement.EndDate)
                .ToListAsync();

            foreach (var favorite in activeFavorites)
            {
                if (favorite.PlayerAdvertisementId != null)
                {
                    favorite.PlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection
                        .Find(pa => pa.Id == favorite.PlayerAdvertisementId)
                        .FirstOrDefaultAsync();

                    if (favorite.PlayerAdvertisement.PlayerPositionId != null)
                    {
                        favorite.PlayerAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                            .Find(pp => pp.Id == favorite.PlayerAdvertisement.PlayerPositionId)
                            .FirstOrDefaultAsync();
                    }

                    if (favorite.PlayerAdvertisement.PlayerFootId != null)
                    {
                        favorite.PlayerAdvertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                            .Find(pf => pf.Id == favorite.PlayerAdvertisement.PlayerFootId)
                            .FirstOrDefaultAsync();
                    }

                    if (favorite.PlayerAdvertisement.SalaryRangeId != null)
                    {
                        favorite.PlayerAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                            .Find(sr => sr.Id == favorite.PlayerAdvertisement.SalaryRangeId)
                            .FirstOrDefaultAsync();
                    }

                    if (favorite.PlayerAdvertisement.PlayerId != null)
                    {
                        favorite.PlayerAdvertisement.Player = await _dbContext.UsersCollection
                            .Find(u => u.Id == favorite.PlayerAdvertisement.PlayerId)
                            .FirstOrDefaultAsync();
                    }
                }

                if (favorite.UserId != null)
                {
                    favorite.User = await _dbContext.UsersCollection
                        .Find(u => u.Id == favorite.UserId)
                        .FirstOrDefaultAsync();
                }
            }

            return activeFavorites;
        }

        public async Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserInactiveFavoritePlayerAdvertisements(string userId)
        {
            var inactiveFavorites = await _dbContext.FavoritePlayerAdvertisementsCollection
                .Find(pa => pa.UserId == userId && pa.PlayerAdvertisement.EndDate < DateTime.Now)
                .SortByDescending(pa => pa.PlayerAdvertisement.EndDate)
                .ToListAsync();

            foreach (var favorite in inactiveFavorites)
            {
                if (favorite.PlayerAdvertisementId != null)
                {
                    favorite.PlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection
                        .Find(pa => pa.Id == favorite.PlayerAdvertisementId)
                        .FirstOrDefaultAsync();

                    if (favorite.PlayerAdvertisement.PlayerPositionId != null)
                    {
                        favorite.PlayerAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                            .Find(pp => pp.Id == favorite.PlayerAdvertisement.PlayerPositionId)
                            .FirstOrDefaultAsync();
                    }

                    if (favorite.PlayerAdvertisement.PlayerFootId != null)
                    {
                        favorite.PlayerAdvertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                            .Find(pf => pf.Id == favorite.PlayerAdvertisement.PlayerFootId)
                            .FirstOrDefaultAsync();
                    }

                    if (favorite.PlayerAdvertisement.SalaryRangeId != null)
                    {
                        favorite.PlayerAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                            .Find(sr => sr.Id == favorite.PlayerAdvertisement.SalaryRangeId)
                            .FirstOrDefaultAsync();
                    }

                    if (favorite.PlayerAdvertisement.PlayerId != null)
                    {
                        favorite.PlayerAdvertisement.Player = await _dbContext.UsersCollection
                            .Find(u => u.Id == favorite.PlayerAdvertisement.PlayerId)
                            .FirstOrDefaultAsync();
                    }
                }

                if (favorite.UserId != null)
                {
                    favorite.User = await _dbContext.UsersCollection
                        .Find(u => u.Id == favorite.UserId)
                        .FirstOrDefaultAsync();
                }
            }

            return inactiveFavorites;
        }

        public async Task<IEnumerable<ClubOffer>> GetReceivedClubOffers(string userId)
        {
            var receivedOffers = await _dbContext.ClubOffersCollection
                .Find(co => co.PlayerAdvertisement.PlayerId == userId)
                .SortByDescending(co => co.PlayerAdvertisement.EndDate)
                .ToListAsync();

            foreach (var offer in receivedOffers)
            {
                if (offer.PlayerAdvertisementId != null)
                {
                    offer.PlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection
                        .Find(pa => pa.Id == offer.PlayerAdvertisementId)
                        .FirstOrDefaultAsync();

                    if (offer.PlayerAdvertisement != null)
                    {
                        if (offer.PlayerAdvertisement.PlayerPositionId != null)
                        {
                            offer.PlayerAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                                .Find(pp => pp.Id == offer.PlayerAdvertisement.PlayerPositionId)
                                .FirstOrDefaultAsync();
                        }

                        if (offer.PlayerAdvertisement.PlayerFootId != null)
                        {
                            offer.PlayerAdvertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                                .Find(pf => pf.Id == offer.PlayerAdvertisement.PlayerFootId)
                                .FirstOrDefaultAsync();
                        }

                        if (offer.PlayerAdvertisement.SalaryRangeId != null)
                        {
                            offer.PlayerAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                                .Find(sr => sr.Id == offer.PlayerAdvertisement.SalaryRangeId)
                                .FirstOrDefaultAsync();
                        }

                        if (offer.PlayerAdvertisement.PlayerId != null)
                        {
                            offer.PlayerAdvertisement.Player = await _dbContext.UsersCollection
                                .Find(u => u.Id == offer.PlayerAdvertisement.PlayerId)
                                .FirstOrDefaultAsync();
                        }
                    }
                }

                if (offer.OfferStatusId != null)
                {
                    offer.OfferStatus = await _dbContext.OfferStatusesCollection
                        .Find(os => os.Id == offer.OfferStatusId)
                        .FirstOrDefaultAsync();
                }

                if (offer.PlayerPositionId != null)
                {
                    offer.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == offer.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (offer.ClubMemberId != null)
                {
                    offer.ClubMember = await _dbContext.UsersCollection
                        .Find(u => u.Id == offer.ClubMemberId)
                        .FirstOrDefaultAsync();
                }
            }

            return receivedOffers;
        }

        public async Task<IEnumerable<ClubOffer>> GetSentClubOffers(string userId)
        {
            var sentOffers = await _dbContext.ClubOffersCollection
                .Find(co => co.ClubMemberId == userId)
                .SortByDescending(co => co.PlayerAdvertisement.CreationDate)
                .ToListAsync();

            foreach (var offer in sentOffers)
            {
                if (offer.PlayerAdvertisementId != null)
                {
                    offer.PlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection
                        .Find(pa => pa.Id == offer.PlayerAdvertisementId)
                        .FirstOrDefaultAsync();

                    if (offer.PlayerAdvertisement != null)
                    {
                        if (offer.PlayerAdvertisement.PlayerPositionId != null)
                        {
                            offer.PlayerAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                                .Find(pp => pp.Id == offer.PlayerAdvertisement.PlayerPositionId)
                                .FirstOrDefaultAsync();
                        }

                        if (offer.PlayerAdvertisement.PlayerFootId != null)
                        {
                            offer.PlayerAdvertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                                .Find(pf => pf.Id == offer.PlayerAdvertisement.PlayerFootId)
                                .FirstOrDefaultAsync();
                        }

                        if (offer.PlayerAdvertisement.SalaryRangeId != null)
                        {
                            offer.PlayerAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                                .Find(sr => sr.Id == offer.PlayerAdvertisement.SalaryRangeId)
                                .FirstOrDefaultAsync();
                        }

                        if (offer.PlayerAdvertisement.PlayerId != null)
                        {
                            offer.PlayerAdvertisement.Player = await _dbContext.UsersCollection
                                .Find(u => u.Id == offer.PlayerAdvertisement.PlayerId)
                                .FirstOrDefaultAsync();
                        }
                    }
                }

                if (offer.OfferStatusId != null)
                {
                    offer.OfferStatus = await _dbContext.OfferStatusesCollection
                        .Find(os => os.Id == offer.OfferStatusId)
                        .FirstOrDefaultAsync();
                }

                if (offer.PlayerPositionId != null)
                {
                    offer.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == offer.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (offer.ClubMemberId != null)
                {
                    offer.ClubMember = await _dbContext.UsersCollection
                        .Find(u => u.Id == offer.ClubMemberId)
                        .FirstOrDefaultAsync();
                }
            }

            return sentOffers;
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetUserClubAdvertisements(string userId)
        {
            var clubAdvertisements = await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.ClubMemberId == userId)
                .SortByDescending(ca => ca.EndDate)
                .ToListAsync();

            foreach (var advertisement in clubAdvertisements)
            {
                if (advertisement.PlayerPositionId != null)
                {
                    advertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == advertisement.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.SalaryRangeId != null)
                {
                    advertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                        .Find(sr => sr.Id == advertisement.SalaryRangeId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.ClubMemberId != null)
                {
                    advertisement.ClubMember = await _dbContext.UsersCollection
                        .Find(u => u.Id == advertisement.ClubMemberId)
                        .FirstOrDefaultAsync();
                }
            }

            return clubAdvertisements;
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetUserActiveClubAdvertisements(string userId)
        {
            var activeAdvertisements = await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.ClubMemberId == userId && ca.EndDate >= DateTime.Now)
                .SortBy(ca => ca.EndDate)
                .ToListAsync();

            foreach (var advertisement in activeAdvertisements)
            {
                if (advertisement.PlayerPositionId != null)
                {
                    advertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == advertisement.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.SalaryRangeId != null)
                {
                    advertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                        .Find(sr => sr.Id == advertisement.SalaryRangeId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.ClubMemberId != null)
                {
                    advertisement.ClubMember = await _dbContext.UsersCollection
                        .Find(u => u.Id == advertisement.ClubMemberId)
                        .FirstOrDefaultAsync();
                }
            }

            return activeAdvertisements;
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetUserInactiveClubAdvertisements(string userId)
        {
            var inactiveAdvertisements = await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.ClubMemberId == userId && ca.EndDate < DateTime.Now)
                .SortByDescending(ca => ca.EndDate)
                .ToListAsync();

            foreach (var advertisement in inactiveAdvertisements)
            {
                if (advertisement.PlayerPositionId != null)
                {
                    advertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == advertisement.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.SalaryRangeId != null)
                {
                    advertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                        .Find(sr => sr.Id == advertisement.SalaryRangeId)
                        .FirstOrDefaultAsync();
                }

                if (advertisement.ClubMemberId != null)
                {
                    advertisement.ClubMember = await _dbContext.UsersCollection
                        .Find(u => u.Id == advertisement.ClubMemberId)
                        .FirstOrDefaultAsync();
                }
            }

            return inactiveAdvertisements;
        }

        public async Task<IEnumerable<FavoriteClubAdvertisement>> GetUserFavoriteClubAdvertisements(string userId)
        {
            var favoriteAdvertisements = await _dbContext.FavoriteClubAdvertisementsCollection
                .Find(fca => fca.UserId == userId)
                .ToListAsync();

            foreach (var favorite in favoriteAdvertisements)
            {
                if (favorite.ClubAdvertisementId != null)
                {
                    favorite.ClubAdvertisement = await _dbContext.ClubAdvertisementsCollection
                        .Find(ca => ca.Id == favorite.ClubAdvertisementId)
                        .FirstOrDefaultAsync();

                    if (favorite.ClubAdvertisement != null)
                    {
                        if (favorite.ClubAdvertisement.PlayerPositionId != null)
                        {
                            favorite.ClubAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                                .Find(pp => pp.Id == favorite.ClubAdvertisement.PlayerPositionId)
                                .FirstOrDefaultAsync();
                        }

                        if (favorite.ClubAdvertisement.SalaryRangeId != null)
                        {
                            favorite.ClubAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                                .Find(sr => sr.Id == favorite.ClubAdvertisement.SalaryRangeId)
                                .FirstOrDefaultAsync();
                        }

                        if (favorite.ClubAdvertisement.ClubMemberId != null)
                        {
                            favorite.ClubAdvertisement.ClubMember = await _dbContext.UsersCollection
                                .Find(u => u.Id == favorite.ClubAdvertisement.ClubMemberId)
                                .FirstOrDefaultAsync();
                        }
                    }
                }

                if (favorite.UserId != null)
                {
                    favorite.User = await _dbContext.UsersCollection
                        .Find(u => u.Id == favorite.UserId)
                        .FirstOrDefaultAsync();
                }
            }

            return favoriteAdvertisements.OrderByDescending(fca => fca.ClubAdvertisement.EndDate);
        }

        public async Task<IEnumerable<FavoriteClubAdvertisement>> GetUserActiveFavoriteClubAdvertisements(string userId)
        {
            var favorites = await _dbContext.FavoriteClubAdvertisementsCollection
                .Find(fc => fc.UserId == userId)
                .ToListAsync();

            foreach (var favorite in favorites)
            {
                if (favorite.ClubAdvertisementId != null)
                {
                    favorite.ClubAdvertisement = await _dbContext.ClubAdvertisementsCollection
                        .Find(ca => ca.Id == favorite.ClubAdvertisementId && ca.EndDate >= DateTime.Now)
                        .FirstOrDefaultAsync();

                    if (favorite.ClubAdvertisement != null)
                    {
                        if (favorite.ClubAdvertisement.PlayerPositionId != null)
                        {
                            favorite.ClubAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                                .Find(pp => pp.Id == favorite.ClubAdvertisement.PlayerPositionId)
                                .FirstOrDefaultAsync();
                        }

                        if (favorite.ClubAdvertisement.SalaryRangeId != null)
                        {
                            favorite.ClubAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                                .Find(sr => sr.Id == favorite.ClubAdvertisement.SalaryRangeId)
                                .FirstOrDefaultAsync();
                        }

                        if (favorite.ClubAdvertisement.ClubMemberId != null)
                        {
                            favorite.ClubAdvertisement.ClubMember = await _dbContext.UsersCollection
                                .Find(u => u.Id == favorite.ClubAdvertisement.ClubMemberId)
                                .FirstOrDefaultAsync();
                        }
                    }
                }

                if (favorite.UserId != null)
                {
                    favorite.User = await _dbContext.UsersCollection
                        .Find(u => u.Id == favorite.UserId)
                        .FirstOrDefaultAsync();
                }
            }

            return favorites.OrderBy(fc => fc.ClubAdvertisement.EndDate);
        }

        public async Task<IEnumerable<FavoriteClubAdvertisement>> GetUserInactiveFavoriteClubAdvertisements(string userId)
        {
            var favorites = await _dbContext.FavoriteClubAdvertisementsCollection
                .Find(fc => fc.UserId == userId)
                .ToListAsync();

            foreach (var favorite in favorites)
            {
                if (favorite.ClubAdvertisementId != null)
                {
                    favorite.ClubAdvertisement = await _dbContext.ClubAdvertisementsCollection
                        .Find(ca => ca.Id == favorite.ClubAdvertisementId && ca.EndDate < DateTime.Now)
                        .FirstOrDefaultAsync();

                    if (favorite.ClubAdvertisement != null)
                    {
                        if (favorite.ClubAdvertisement.PlayerPositionId != null)
                        {
                            favorite.ClubAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                                .Find(pp => pp.Id == favorite.ClubAdvertisement.PlayerPositionId)
                                .FirstOrDefaultAsync();
                        }

                        if (favorite.ClubAdvertisement.SalaryRangeId != null)
                        {
                            favorite.ClubAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                                .Find(sr => sr.Id == favorite.ClubAdvertisement.SalaryRangeId)
                                .FirstOrDefaultAsync();
                        }

                        if (favorite.ClubAdvertisement.ClubMemberId != null)
                        {
                            favorite.ClubAdvertisement.ClubMember = await _dbContext.UsersCollection
                                .Find(u => u.Id == favorite.ClubAdvertisement.ClubMemberId)
                                .FirstOrDefaultAsync();
                        }
                    }
                }

                if (favorite.UserId != null)
                {
                    favorite.User = await _dbContext.UsersCollection
                        .Find(u => u.Id == favorite.UserId)
                        .FirstOrDefaultAsync();
                }
            }

            return favorites.OrderByDescending(fc => fc.ClubAdvertisement.EndDate);
        }

        public async Task<IEnumerable<PlayerOffer>> GetReceivedPlayerOffers(string userId)
        {
            var offers = await _dbContext.PlayerOffersCollection
                .Find(po => po.ClubAdvertisement.ClubMemberId == userId)
                .ToListAsync();

            foreach (var offer in offers)
            {
                if (offer.ClubAdvertisementId != null)
                {
                    offer.ClubAdvertisement = await _dbContext.ClubAdvertisementsCollection
                        .Find(ca => ca.Id == offer.ClubAdvertisementId)
                        .FirstOrDefaultAsync();

                    if (offer.ClubAdvertisement != null)
                    {
                        if (offer.ClubAdvertisement.PlayerPositionId != null)
                        {
                            offer.ClubAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                                .Find(pp => pp.Id == offer.ClubAdvertisement.PlayerPositionId)
                                .FirstOrDefaultAsync();
                        }

                        if (offer.ClubAdvertisement.SalaryRangeId != null)
                        {
                            offer.ClubAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                                .Find(sr => sr.Id == offer.ClubAdvertisement.SalaryRangeId)
                                .FirstOrDefaultAsync();
                        }

                        if (offer.ClubAdvertisement.ClubMemberId != null)
                        {
                            offer.ClubAdvertisement.ClubMember = await _dbContext.UsersCollection
                                .Find(u => u.Id == offer.ClubAdvertisement.ClubMemberId)
                                .FirstOrDefaultAsync();
                        }
                    }
                }

                if (offer.OfferStatusId != null)
                {
                    offer.OfferStatus = await _dbContext.OfferStatusesCollection
                        .Find(os => os.Id == offer.OfferStatusId)
                        .FirstOrDefaultAsync();
                }

                if (offer.PlayerPositionId != null)
                {
                    offer.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == offer.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (offer.PlayerFootId != null)
                {
                    offer.PlayerFoot = await _dbContext.PlayerFeetCollection
                        .Find(pf => pf.Id == offer.PlayerFootId)
                        .FirstOrDefaultAsync();
                }

                if (offer.PlayerId != null)
                {
                    offer.Player = await _dbContext.UsersCollection
                        .Find(u => u.Id == offer.PlayerId)
                        .FirstOrDefaultAsync();
                }
            }

            return offers.OrderByDescending(po => po.ClubAdvertisement.EndDate);
        }

        public async Task<IEnumerable<PlayerOffer>> GetSentPlayerOffers(string userId)
        {
            var offers = await _dbContext.PlayerOffersCollection
                .Find(po => po.PlayerId == userId)
                .ToListAsync();

            foreach (var offer in offers)
            {
                if (offer.ClubAdvertisementId != null)
                {
                    offer.ClubAdvertisement = await _dbContext.ClubAdvertisementsCollection
                        .Find(ca => ca.Id == offer.ClubAdvertisementId)
                        .FirstOrDefaultAsync();

                    if (offer.ClubAdvertisement != null)
                    {
                        if (offer.ClubAdvertisement.PlayerPositionId != null)
                        {
                            offer.ClubAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                                .Find(pp => pp.Id == offer.ClubAdvertisement.PlayerPositionId)
                                .FirstOrDefaultAsync();
                        }

                        if (offer.ClubAdvertisement.SalaryRangeId != null)
                        {
                            offer.ClubAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                                .Find(sr => sr.Id == offer.ClubAdvertisement.SalaryRangeId)
                                .FirstOrDefaultAsync();
                        }

                        if (offer.ClubAdvertisement.ClubMemberId != null)
                        {
                            offer.ClubAdvertisement.ClubMember = await _dbContext.UsersCollection
                                .Find(u => u.Id == offer.ClubAdvertisement.ClubMemberId)
                                .FirstOrDefaultAsync();
                        }
                    }
                }

                if (offer.OfferStatusId != null)
                {
                    offer.OfferStatus = await _dbContext.OfferStatusesCollection
                        .Find(os => os.Id == offer.OfferStatusId)
                        .FirstOrDefaultAsync();
                }

                if (offer.PlayerPositionId != null)
                {
                    offer.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == offer.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (offer.PlayerFootId != null)
                {
                    offer.PlayerFoot = await _dbContext.PlayerFeetCollection
                        .Find(pf => pf.Id == offer.PlayerFootId)
                        .FirstOrDefaultAsync();
                }

                if (offer.PlayerId != null)
                {
                    offer.Player = await _dbContext.UsersCollection
                        .Find(u => u.Id == offer.PlayerId)
                        .FirstOrDefaultAsync();
                }
            }

            return offers.OrderByDescending(po => po.ClubAdvertisement.CreationDate);
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
            var users = await _dbContext.UsersCollection.Find(FilterDefinition<User>.Empty).ToListAsync();

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