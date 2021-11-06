using System.Linq;
using authservice.Boundary.Request;
using authservice.Boundary.Request.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace authservice.Tests.Boundary.Validation
{
    public class LoginRequestObjectValidatorTests
    {
        private readonly LoginRequestObjectValidation _validator;

        public LoginRequestObjectValidatorTests()
        {
            _validator = new LoginRequestObjectValidation();
        }

        [Fact]
        public void Email_WhenInvalidFormat_HasError()
        {
            // arrange
            var model = new LoginRequestObject
            {
                Email = "asjldhajskldhjasdh"
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void WhenValid_HasNoErrors()
        {
            // arrange
            var model = new LoginRequestObject
            {
                Email = "email@email.com",
                Password = "jjjj"
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}