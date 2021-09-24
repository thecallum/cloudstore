using System.Linq;
using authservice.Boundary.Request;
using authservice.Boundary.Request.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace authservice.Tests.Boundary.Validation
{
    public class RegisterRequestObjectValidatiorTests
    {
        private readonly RegisterRequestObjectValidation _validator;

        public RegisterRequestObjectValidatiorTests()
        {
            _validator = new RegisterRequestObjectValidation();
        }

        [Fact]
        public void FirstName_WhenTooLong_HasError()
        {
            // arrange
            var model = new RegisterRequestObject
            {
                FirstName = string.Concat(Enumerable.Repeat("a", 100))
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void FirstName_WhenContainsUppercaseLetters_HasError()
        {
            // arrange
            var model = new RegisterRequestObject
            {
                FirstName = "AAAA"
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void FirstName_WhenContainsNumbers_HasError()
        {
            // arrange
            var model = new RegisterRequestObject
            {
                FirstName = "aa33"
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void LastName_WhenTooLong_HasError()
        {
            // arrange
            var model = new RegisterRequestObject
            {
                LastName = string.Concat(Enumerable.Repeat("a", 100))
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Fact]
        public void LastName_WhenContainsUppercaseLetters_HasError()
        {
            // arrange
            var model = new RegisterRequestObject
            {
                LastName = "JJJDD"
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Fact]
        public void LastName_WhenContainsNumbers_HasError()
        {
            // arrange
            var model = new RegisterRequestObject
            {
                LastName = "d223"
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Fact]
        public void Email_WhenTooLong_HasError()
        {
            // arrange
            var model = new RegisterRequestObject
            {
                Email = string.Concat(Enumerable.Repeat("a", 100))
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Email_WhenTooShort_HasError()
        {
            // arrange
            var model = new RegisterRequestObject
            {
                Email = "j"
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Email_WhenInvalidFormat_HasError()
        {
            // arrange
            var model = new RegisterRequestObject
            {
                Email = "hasdfsfiasdfhiuasfihasf"
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
            var model = new RegisterRequestObject
            {
                Password = string.Concat(Enumerable.Repeat("a", 101))
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Password_WhenTooShort_HasError()
        {
            // arrange
            var model = new RegisterRequestObject
            {
                Password = "j"
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void WhenAllValid_HasNoError()
        {
            // arrange
            var model = new RegisterRequestObject
            {
                FirstName = "tom",
                LastName = "smith",
                Email = "tom.smith@gmail.com",
                Password = "password1234"
            };

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}