using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Request.Validation
{
    public class CreateDirectoryRequestValidator : AbstractValidator<CreateDirectoryRequest>
    {
        public CreateDirectoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1, 100);
        }
    }
}
