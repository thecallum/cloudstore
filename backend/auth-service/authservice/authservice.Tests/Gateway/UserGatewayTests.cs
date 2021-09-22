using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.Tests.Gateway
{
    public class UserGatewayTests
    {
    }
}

/*
[Fact]
public async Task WhenEmailAlreadyInUse_ThrowsUserWithEmailAlreadyExistsException()
{
    // Arrange
    var mockRequest = _fixture.Create<RegisterRequestObject>();
    //var mockUser = _fixture.Create<User>();

    var exception = new UserWithEmailAlreadyExistsException(mockRequest.Email);

    _mockUserGateway
        .Setup(x => x.RegisterUser(It.IsAny<User>()))
        .ThrowsAsync(exception);

    // Act

    Func<Task<Guid>> func = async () => await _registerUseCase.Execute(mockRequest)
       .ConfigureAwait(false);

    // Assert
    func.Should().Throw<UserWithEmailAlreadyExistsException>();
    //func.Should().Throw<UserWithEmailAlreadyExistsException>().WithMessage(exception.Message);
}
*/