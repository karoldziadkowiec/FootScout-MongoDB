using FootScout_MongoDB.WebAPI.Controllers;
using FootScout_MongoDB.WebAPI.Models.Constants;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootScout_MongoDB.WebAPI.UnitTests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly AccountController _accountController;

        public AccountControllerTests()
        {
            _mockAccountService = new Mock<IAccountService>();
            _accountController = new AccountController(_mockAccountService.Object);
        }

        [Fact]
        public async Task Register_ValidDto_ReturnsOkResult()
        {
            // Arrange
            var registerDTO = new RegisterDTO { Email = "lm10@gmail.com", Password = "Password123!" };
            _mockAccountService.Setup(service => service.Register(registerDTO)).Returns(Task.CompletedTask);

            // Act
            var result = await _accountController.Register(registerDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(registerDTO, okResult.Value);
        }

        [Fact]
        public async Task Register_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            var registerDTO = new RegisterDTO { Email = "lm10@gmail.com", Password = "Password123!", ConfirmPassword = "Password123!" };
            _accountController.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _accountController.Register(registerDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Register_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var registerDTO = new RegisterDTO { Email = "lm10@gmail.com", Password = "Password123!", ConfirmPassword = "Password123!" };
            _mockAccountService.Setup(service => service.Register(registerDTO))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _accountController.Register(registerDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Error during the registration of account: Service error", objectResult.Value);
        }

        [Fact]
        public async Task Login_ValidDto_ReturnsOkResultWithToken()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "lm10@gmail.com", Password = "Password123!" };
            var token = "token";
            _mockAccountService.Setup(service => service.Login(loginDTO)).ReturnsAsync(token);

            // Act
            var result = await _accountController.Login(loginDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(token, okResult.Value);
        }

        [Fact]
        public async Task Login_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "lm10@gmail.com", Password = "Password123!" };
            _accountController.ModelState.AddModelError("Username", "Username is required");

            // Act
            var result = await _accountController.Login(loginDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "lm10@gmail.com", Password = "Password123!" };
            _mockAccountService.Setup(service => service.Login(loginDTO))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _accountController.Login(loginDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Error during the login: Service error", objectResult.Value);
        }

        [Fact]
        public async Task GetRoles_ReturnsOkResultWithRoles()
        {
            // Arrange
            var roles = new List<string> { RoleName.Admin, RoleName.User };
            _mockAccountService.Setup(service => service.GetRoles()).ReturnsAsync(roles);

            // Act
            var result = await _accountController.GetRoles();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<string>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedRoles = Assert.IsType<List<string>>(okResult.Value);
            Assert.Equal(roles.Count, returnedRoles.Count);
        }

        [Fact]
        public async Task MakeAnAdmin_ValidUserId_ReturnsNoContent()
        {
            // Arrange
            var userId = "leomessi";
            _mockAccountService.Setup(service => service.MakeAnAdmin(userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _accountController.MakeAnAdmin(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DemoteFromAdmin_ValidUserId_ReturnsNoContent()
        {
            // Arrange
            var userId = "leomessi";
            _mockAccountService.Setup(service => service.MakeAnUser(userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _accountController.DemoteFromAdmin(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}