using AutoMapper;
using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.Constants;
using FootScout_MongoDB.WebAPI.Services.Classes;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using FootScout_MongoDB.WebAPI.UnitTests.TestManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using Moq;
using System;

namespace FootScout_MongoDB.WebAPI.UnitTests.Repositories
{
    public class UserRepositoryTests : IAsyncLifetime
    {
        private MongoDbRunner _mongoRunner;
        private MongoDBContext _dbContext;
        private UserRepository _userRepository;
        private IMapper _mapper;
        private INewIdGeneratorService _newIdGeneratorService;
        private IRoleService _roleService;
        private IPasswordService _passwordService;
        private TestBase _testBase;

        public UserRepositoryTests()
        {
            _mongoRunner = MongoDbRunner.Start();
            var mongoClient = new MongoClient(_mongoRunner.ConnectionString);

            var mongoDBSettings = Options.Create(new MongoDBSettings
            {
                ConnectionString = _mongoRunner.ConnectionString,
                DatabaseName = "TestDatabase",
                RolesCollectionName = "Roles",
                UserRolesCollectionName = "UserRoles",
                UsersCollectionName = "Users"
            });

            _roleService = Mock.Of<IRoleService>();
            _testBase = new TestBase();

            _dbContext = new MongoDBContext(mongoDBSettings);
            _newIdGeneratorService = Mock.Of<INewIdGeneratorService>();
            _passwordService = Mock.Of<IPasswordService>();
            _mapper = _testBase.CreateMapper();

            _userRepository = new UserRepository(_dbContext, _newIdGeneratorService, _mapper, _passwordService);
        }

        public async Task InitializeAsync()
        {
            await _testBase.SeedUserTestBase(_dbContext, _roleService);
        }

        public async Task DisposeAsync()
        {
            _mongoRunner.Dispose();
            await Task.CompletedTask;
        }

        [Fact]
        public async Task GetUser_ShouldReturnUserDTO_WhenUserExists()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("leomessi", result.Id);
            Assert.Equal("Leo", result.FirstName);
            Assert.Equal("Messi", result.LastName);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnListOfUserDTOs()
        {
            // Arrange & Act
            var result = await _userRepository.GetUsers();

            // Assert
            Assert.Equal(4, result.Count());
        }
    }
}