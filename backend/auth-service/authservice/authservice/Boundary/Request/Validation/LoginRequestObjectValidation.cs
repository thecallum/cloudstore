﻿using FluentValidation;

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
                .NotNull()
                .Length(1, 100);
        }
    }
}