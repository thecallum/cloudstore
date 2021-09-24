using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.Boundary.Request.Validation
{
    public class LoginRequestObjectValidation : AbstractValidator<LoginRequestObject>
    {
        public LoginRequestObjectValidation()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .EmailAddress();

            RuleFor(x => x.Password)
                .Length(1, 100);
        }
    }
}
