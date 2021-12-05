using DocumentService.Boundary.Request;
using DocumentService.Boundary.Request.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace DocumentService.Tests.Request.Validation
{
    public class RenameDirectoryRequestValidatorTests
    {
        private readonly RenameDirectoryRequestValidator _validator;

        public RenameDirectoryRequestValidatorTests()
        {
            _validator = new RenameDirectoryRequestValidator();
        }

        [Fact]
        public void WhenNameTooShort_HasError()
        {
            // Arrange
            var model = new RenameDirectoryRequest
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
            var model = new RenameDirectoryRequest
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
            var model = new RenameDirectoryRequest
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
