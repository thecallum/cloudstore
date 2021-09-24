using authservice.Boundary.Request;
using authservice.Boundary.Request.Validation;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public void Password_WhenTooLong_HasError()
        {
            // arrange
            var model = new LoginRequestObject
            {
                Password = string.Concat(Enumerable.Repeat("a", 101))
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
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
