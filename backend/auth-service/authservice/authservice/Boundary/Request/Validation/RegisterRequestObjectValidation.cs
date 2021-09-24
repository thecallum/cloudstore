using System.Text.RegularExpressions;
using FluentValidation;

namespace authservice.Boundary.Request.Validation
{
    public class RegisterRequestObjectValidation : AbstractValidator<RegisterRequestObject>
    {
        public RegisterRequestObjectValidation()
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .Length(1, 50)
                .Must(x => IsOnlyLowerCaseAlphabetical(x))
                .WithMessage("{PropertyName} must be lowercase and only contain alphabetical characters.");

            RuleFor(x => x.LastName)
                .NotNull()
                .Length(1, 50)
                .Must(x => IsOnlyLowerCaseAlphabetical(x))
                .WithMessage("{PropertyName} must be lowercase and only contain alphabetical characters.");

            RuleFor(x => x.Email)
                .NotNull()
                .Length(3, 50)
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotNull()
                .MinimumLength(8)
                .MaximumLength(100);
        }

        private static bool IsOnlyLowerCaseAlphabetical(string? value)
        {
            if (value == null) return false;

            var pattern = @"^[a-z]*$";

            return Regex.IsMatch(value, pattern);
        }
    }
}