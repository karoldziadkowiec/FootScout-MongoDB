using AutoMapper;
using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Services.Classes;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class UserRepositoryTests : IAsyncLifetime
    {
        private IMongoClient _mongoClient;
        private IMongoDatabase _database;
        private UserRepository _userRepository;
        private IMapper _mapper;
        private INewIdGeneratorService _newIdGeneratorService;
        private IPasswordService _passwordService;

        public UserRepositoryTests()
        {
            // Konfiguracja klienta MongoDB
            _mongoClient = new MongoClient("mongodb://localhost:27017");
            _database = _mongoClient.GetDatabase("FootScout");

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<UserDTO, User>();
            });

            _mapper = mapperConfig.CreateMapper();

            var mongoDBSettings = Options.Create(new MongoDBSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "FootScout",
                UsersCollectionName = "Users"
            });

            var dbContext = new MongoDBContext(mongoDBSettings);
            _userRepository = new UserRepository(
                dbContext,
                _newIdGeneratorService,
                _mapper,
                _passwordService
            );
        }

        public async Task DisposeAsync()
        {
            // Czyszczenie bazy danych
            await _database.DropCollectionAsync("Users");
        }

        public Task InitializeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetUser_ShouldReturnUserDTO_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Id = "userId",
                Email = "example@example.com",
                PasswordHash = "hashedPassword",
                FirstName = "John",
                LastName = "Doe",
                Location = "New York",
                PhoneNumber = "1234567890",
                CreationDate = DateTime.UtcNow
            };

            var usersCollection = _database.GetCollection<User>("Users");
            await usersCollection.InsertOneAsync(user);

            var userDto = new UserDTO
            {
                Id = "userId",
                Email = "example@example.com",
                FirstName = "John",
                LastName = "Doe",
                Location = "New York",
                PhoneNumber = "1234567890"
            };

            // Act
            var result = await _userRepository.GetUser("userId");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Location, result.Location);
            Assert.Equal(user.PhoneNumber, result.PhoneNumber);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnListOfUserDTOs()
        {
            // Arrange
            var user1 = new User
            {
                Id = "userId1",
                Email = "user1@example.com",
                PasswordHash = "hashedPassword1",
                FirstName = "Alice",
                LastName = "Smith",
                Location = "Los Angeles",
                PhoneNumber = "1234567890",
                CreationDate = DateTime.UtcNow.AddDays(-1)
            };

            var user2 = new User
            {
                Id = "userId2",
                Email = "user2@example.com",
                PasswordHash = "hashedPassword2",
                FirstName = "Bob",
                LastName = "Johnson",
                Location = "San Francisco",
                PhoneNumber = "0987654321",
                CreationDate = DateTime.UtcNow
            };

            var usersCollection = _database.GetCollection<User>("Users");
            await usersCollection.InsertManyAsync(new[] { user1, user2 });

            // Act
            var result = await _userRepository.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, dto => dto.Id == "userId1");
            Assert.Contains(result, dto => dto.Id == "userId2");
        }
    }
}