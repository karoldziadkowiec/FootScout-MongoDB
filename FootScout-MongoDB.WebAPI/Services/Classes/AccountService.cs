using AutoMapper;
using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.Constants;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;

namespace FootScout_MongoDB.WebAPI.Services.Classes
{
    public class AccountService : IAccountService
    {
        private readonly MongoDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IRoleService _roleService;
        private readonly ITokenService _tokenService;
        private readonly ICookieService _cookieService;

        public AccountService(MongoDBContext dbContext, IMapper mapper, IUserRepository userRepository, IPasswordService passworService, IRoleService roleService, ITokenService tokenService, ICookieService cookieService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userRepository = userRepository;
            _passwordService = passworService;
            _roleService = roleService;
            _tokenService = tokenService;
            _cookieService = cookieService;
        }

        public async Task Register(RegisterDTO registerDTO)
        {
            var userByEmail = await _dbContext.UsersCollection
                .Find(u => u.Email == registerDTO.Email)
                .FirstOrDefaultAsync();

            if (userByEmail != null)
                throw new ArgumentException($"User with email {registerDTO.Email} already exists.");

            if (!registerDTO.Password.Equals(registerDTO.ConfirmPassword))
                throw new ArgumentException($"Confirmed password is different.");

            var passwordHash = _passwordService.HashPassword(registerDTO.Password);
            var user = _mapper.Map<User>(registerDTO);
            user.PasswordHash =  passwordHash;

            await _dbContext.UsersCollection.InsertOneAsync(user);
            await _roleService.AddRoleToUser(user.Id, RoleName.User);
        }

        public async Task<string> Login(LoginDTO loginDTO)
        {
            var user = await _userRepository.FindUserByEmail(loginDTO.Email);
            if (user == null)
            {
                throw new ArgumentException($"User {loginDTO.Email} does not exist.");
            }

            var passwordHash = _passwordService.HashPassword(loginDTO.Password);
            if (passwordHash != user.PasswordHash)
            {
                throw new ArgumentException($"Unable to authenticate user {loginDTO.Email} - wrong password.");
            }

            var token = await _tokenService.CreateTokenJWT(user);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            await _cookieService.SetCookies(token, tokenString);

            return tokenString;
        }

        public async Task<IEnumerable<string>> GetRoles()
        {
            var roles = await _dbContext.RolesCollection.Find(_ => true).ToListAsync();
            return roles.Select(r => r.Name);
        }

        public async Task MakeAnAdmin(string userId)
        {
            var user = await _userRepository.GetUser(userId);
            if (user == null)
                throw new ArgumentException($"User {userId} does not exist.");

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
                        await _dbContext.AchievementsCollection.DeleteOneAsync(a => a.Id == clubHistory.AchievementsId);
                }
            }
            await _dbContext.ClubHistoriesCollection
                .DeleteManyAsync(ch => ch.PlayerId == userId);

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
                await _dbContext.PlayerAdvertisementsCollection.ReplaceOneAsync(pa => pa.Id == advertisement.Id, advertisement);
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
                await _dbContext.ClubOffersCollection.ReplaceOneAsync(co => co.Id == offer.Id, offer);
            }

            var clubAdvertisements = await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.ClubMemberId == userId)
                .ToListAsync();

            foreach (var advertisement in clubAdvertisements)
            {
                advertisement.EndDate = DateTime.Now;
                advertisement.ClubMemberId = unknownUserId;
                await _dbContext.ClubAdvertisementsCollection.ReplaceOneAsync(ca => ca.Id == advertisement.Id, advertisement);
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
                await _dbContext.PlayerOffersCollection.ReplaceOneAsync(po => po.Id == offer.Id, offer);
            }

            await _roleService.RemoveRoleFromUser(userId, RoleName.User);
            await _roleService.AddRoleToUser(userId, RoleName.Admin);
        }

        public async Task MakeAnUser(string userId)
        {
            var user = await _userRepository.GetUser(userId);
            if (user == null)
                throw new ArgumentException($"User {userId} does not exist.");

            await _roleService.RemoveRoleFromUser(userId, RoleName.Admin);
            await _roleService.AddRoleToUser(userId, RoleName.User);
        }

        private string GetRegisterError(IEnumerable<IdentityError> errors)
        {
            return string.Join(", ", errors.Select(error => error.Description).ToArray());
        }
    }
}