using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Request.Validation
{
    public class ValidateUploadedDocumentRequestValidator : AbstractValidator<ValidateUploadedDocumentRequest>
    {
        public ValidateUploadedDocumentRequestValidator()
        {
            RuleFor(x => x.FileName).NotNull().NotEmpty();
        }
    }
}
