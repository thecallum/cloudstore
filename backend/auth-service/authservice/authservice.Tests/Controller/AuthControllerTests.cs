using System.Threading.Tasks;
using authservice.Boundary.Request;
using authservice.Controllers;
using authservice.Domain;
using authservice.Encryption;
using authservice.Infrastructure.Exceptions;
using authservice.UseCase.Interfaces;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TokenService;
using TokenService.Models;
using Xunit;

namespace authservice.Tests.Controller
{
    public class AuthControllerTests
    {
        private readonly AuthController _authController;

        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IDeleteUseCase> _mockDeleteUseCase;
        private readonly Mock<IPasswordHasher> _mockHashService;

        private readonly Mock<ITokenService> _mockJWTService;
        private readonly Mock<ILoginUseCase> _mockLoginUseCase;
        private readonly Mock<IRegisterUseCase> _mockRegisterUseCase;

        public AuthControllerTests()
        {
            _mockLoginUseCase = new Mock<ILoginUseCase>();
            _mockRegisterUseCase = new Mock<IRegisterUseCase>();
            _mockDeleteUseCase = new Mock<IDeleteUseCase>();

            _mockJWTService = new Mock<ITokenService>();
            _mockHashService = new Mock<IPasswordHasher>();

            _authController = new AuthController(
                _mockLoginUseCase.Object,
                _mockRegisterUseCase.Object,
                _mockDeleteUseCase.Object,
                _mockJWTService.Object,
                _mockHashService.Object);

            // Ensure the controller can add response headers
            _authController.ControllerContext = new ControllerContext();
            _authController.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task Login_WhenUserDoestExist_Returns404()
        {
            // Arrange
            var requestObject = _fixture.Create<LoginRequestObject>();

            _mockLoginUseCase.Setup(x => x.Execute(It.IsAny<string>())).ReturnsAsync((User) null);

            // Act
            var result = await _authController.Login(requestObject);

            // Assert
            result.Should().BeOfType(typeof(NotFoundObjectResult));
            _authController.Response.Headers.Should().NotContainKey(Constants.AuthToken);
        }

        [Fact]
        public async Task Login_WhenPasswordIsIncorrect_Returns401()
        {
            // Arrange
            var requestObject = _fixture.Create<LoginRequestObject>();
            var mockUser = _fixture.Create<User>();

            _mockLoginUseCase.Setup(x => x.Execute(It.IsAny<string>())).ReturnsAsync(mockUser);

            // setup hashservice to return password is bad
            _mockHashService.Setup(x => x.Check(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            var result = await _authController.Login(requestObject);

            // Assert
            result.Should().BeOfType(typeof(UnauthorizedResult));
            _authController.Response.Headers.Should().NotContainKey(Constants.AuthToken);
        }

        [Fact]
        public async Task Login_WhenUserExists_Returns200WithToken()
        {
            // Arrange
            var requestObject = _fixture.Create<LoginRequestObject>();
            var mockUser = _fixture.Create<User>();

            _mockLoginUseCase.Setup(x => x.Execute(It.IsAny<string>())).ReturnsAsync(mockUser);

            // setup hashservice to return password is valid
            _mockHashService.Setup(x => x.Check(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // setup jwtService to return custom token
            var tokenToReturn = _fixture.Create<string>();
            _mockJWTService.Setup(x => x.CreateToken(It.IsAny<Payload>())).Returns(tokenToReturn);

            // Act
            var result = await _authController.Login(requestObject);

            // Assert
            result.Should().BeOfType(typeof(OkResult));
            _authController.Response.Headers[Constants.AuthToken].Should().Equal(tokenToReturn);
        }

        [Fact]
        public async Task Register_WhenEmailAlreadyUsed_Returns409Conflict()
        {
            // Arrange    
            var requestObject = _fixture.Create<RegisterRequestObject>();
            var mockHash = _fixture.Create<string>();

            // setup usecase to throw exception
            var exception = new UserWithEmailAlreadyExistsException(requestObject.Email);

            _mockRegisterUseCase
                .Setup(x => x.Execute(It.IsAny<RegisterRequestObject>(), It.IsAny<string>()))
                .ThrowsAsync(exception);

            _mockHashService
                .Setup(x => x.Hash(It.IsAny<string>()))
                .Returns(mockHash);

            // Act
            var result = await _authController.Register(requestObject);

            // Assert
            result.Should().BeOfType(typeof(ConflictObjectResult));
            (result as ConflictObjectResult).Value.Should().BeEquivalentTo(requestObject.Email);
        }

        [Fact]
        public async Task Register_WhenValid_Return200Ok()
        {
            // Arrange    
            var requestObject = _fixture.Create<RegisterRequestObject>();
            var mockHash = _fixture.Create<string>();

            _mockRegisterUseCase
                .Setup(x => x.Execute(It.IsAny<RegisterRequestObject>(), It.IsAny<string>()));

            _mockHashService
                .Setup(x => x.Hash(It.IsAny<string>()))
                .Returns(mockHash);

            // Act
            var result = await _authController.Register(requestObject);

            // Assert
            result.Should().BeOfType(typeof(OkResult));
        }

        [Fact]
        public async Task Delete_WhenInvalidToken_Returns401Unauthorized()
        {
            // Arrange    
            var mockToken = _fixture.Create<string>();

            // setup hashservice to return password is bad
            _mockHashService.Setup(x => x.Check(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            var result = await _authController.DeleteAccount(mockToken);

            // Assert
            result.Should().BeOfType(typeof(UnauthorizedResult));
        }

        [Fact]
        public async Task Delete_WhenUserDoesntExist_Returns404NotFound()
        {
            // Arrange    
            var mockPayload = _fixture.Create<Payload>();
            var mockToken = _fixture.Create<string>();

            // setup hashservice to return password is valid
            _mockHashService.Setup(x => x.Check(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // setup jwtService to return user
            var tokenToReturn = _fixture.Create<string>();
            _mockJWTService.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(mockPayload);

            // setup usecase to throw not found exception
            var exception = new UserNotFoundException(mockPayload.Email);
            _mockDeleteUseCase
                .Setup(x => x.Execute(It.IsAny<string>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _authController.DeleteAccount(mockToken);

            // Assert
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task Delete_WhenCalled_Returns201NoContent()
        {
            // Arrange    
            var mockPayload = _fixture.Create<Payload>();
            var mockToken = _fixture.Create<string>();

            // setup hashservice to return password is valid
            _mockHashService.Setup(x => x.Check(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // setup jwtService to return user
            var tokenToReturn = _fixture.Create<string>();
            _mockJWTService.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(mockPayload);

            // Act
            var result = await _authController.DeleteAccount(mockToken);

            // Assert
            result.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public async Task Check_WhenTokenIsInvalid_Returns401Unauthorized()
        {
            // Arrange    
            var mockToken = _fixture.Create<string>();

            // setup jwtService to return user
            var tokenToReturn = _fixture.Create<string>();
            _mockJWTService.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns((Payload) null);

            // Act
            var result = await _authController.CheckAuth(mockToken);

            // Assert
            result.Should().BeOfType(typeof(UnauthorizedResult));
        }

        [Fact]
        public async Task Check_WhenTokenIsValid_Returns200Ok()
        {
            // Arrange    
            var mockPayload = _fixture.Create<Payload>();
            var mockToken = _fixture.Create<string>();

            // setup jwtService to return user
            var tokenToReturn = _fixture.Create<string>();
            _mockJWTService.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(mockPayload);

            // Act
            var result = await _authController.CheckAuth(mockToken);

            // Assert
            result.Should().BeOfType(typeof(OkResult));
        }
    }
}