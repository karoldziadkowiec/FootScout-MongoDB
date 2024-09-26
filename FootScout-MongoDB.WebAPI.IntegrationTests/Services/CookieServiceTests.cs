using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Services.Classes;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Services
{
    public class CookieServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly CookieService _cookieService;

        public CookieServiceTests()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _cookieService = new CookieService(_httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task SetCookies_ShouldSetCookie_WithExpectedProperties()
        {
            // Arrange
            var token = new JwtSecurityToken
            (
                issuer: "issuer",
                audience: "audience",
                expires: DateTime.UtcNow.AddHours(1)
            );
            var tokenString = "testAuthToken";

            var responseCookiesMock = new Mock<IResponseCookies>();
            var httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.Setup(r => r.Cookies).Returns(responseCookiesMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);

            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);

            // Act
            await _cookieService.SetCookies(token, tokenString);

            // Assert
            responseCookiesMock.Verify(c => c.Append(
                "AuthToken",
                tokenString,
                It.Is<CookieOptions>(opts =>
                    opts.HttpOnly == true &&
                    opts.Secure == true &&
                    opts.SameSite == SameSiteMode.Strict &&
                    opts.Expires == token.ValidTo
                )),
                Times.Once
            );
        }
    }
}