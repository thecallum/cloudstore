using DocumentService.Boundary.Request;
using DocumentService.Boundary.Request.Validation;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.Boundary.Request.Validation
{
    public class ValidateUploadedDocumentRequestValidatorTests
    {
        private readonly ValidateUploadedDocumentRequestValidator _validator;

        public ValidateUploadedDocumentRequestValidatorTests()
        {
            _validator = new ValidateUploadedDocumentRequestValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFileNameInvalid_HasError(string fileName)
        {
            // Arrange
            var model = new ValidateUploadedDocumentRequest
            {
                FileName = fileName
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }
    }
}
