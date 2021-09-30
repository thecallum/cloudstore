using DocumentService.Boundary.Request;
using DocumentService.Boundary.Request.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace DocumentService.Tests.Request.Validation
{
    public class CreateDirectoryRequestValidatorTests
    {
        private readonly CreateDirectoryRequestValidator _validator;

        public CreateDirectoryRequestValidatorTests()
        {
            _validator = new CreateDirectoryRequestValidator();
        }

        [Fact]
        public void WhenNameTooShort_HasError()
        {
            // Arrange
            var model = new CreateDirectoryRequest
            {
                Name = ""
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void WhenNameTooLong_HasError()
        {
            // Arrange
            var model = new CreateDirectoryRequest
            {
                Name = new string('x', 200)
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void WhenNameValid_NoError()
        {
            // Arrange
            var model = new CreateDirectoryRequest
            {
                Name = new string('x', 10)
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
    }
}
