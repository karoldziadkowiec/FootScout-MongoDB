using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Services.Classes;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Services
{
    public class PasswordServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private PasswordService _passwordService;

        public PasswordServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _passwordService = new PasswordService();
        }

        [Fact]
        public void HashPassword_ShouldReturnHash_WhenPasswordIsValid()
        {
            // Arrange
            var password = "Password123!";
            var service = new PasswordService();

            // Act
            var hashedPassword = service.HashPassword(password);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEqual(password, hashedPassword);
            Assert.True(hashedPassword.Length > 0);
        }

        [Fact]
        public void HashPassword_ShouldReturnHash_WhenPasswordIsEmpty()
        {
            // Arrange
            var password = string.Empty;
            var service = new PasswordService();

            // Act
            var hashedPassword = service.HashPassword(password);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEqual(password, hashedPassword);
        }

        [Fact]
        public void HashPassword_ShouldThrowArgumentNullException_WhenPasswordIsNull()
        {
            // Arrange
            string password = null;
            var service = new PasswordService();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.HashPassword(password));
        }
    }
}