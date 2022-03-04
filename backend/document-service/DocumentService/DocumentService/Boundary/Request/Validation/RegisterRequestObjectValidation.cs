using System.Text.RegularExpressions;
using FluentValidation;

namespace DocumentService.Boundary.Request.Validation
{
    public class RegisterRequestObjectValidation : AbstractValidator<RegisterRequestObject>
    {
        public RegisterRequestObjectValidation()
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .Length(1, 50)
                .Must(x => IsOnlyAlphabetical(x))
                .WithMessage("{PropertyName} must be lowercase and only contain alphabetical characters.");

            RuleFor(x => x.LastName)
                .NotNull()
                .Length(1, 50)
                .Must(x => IsOnlyAlphabetical(x))
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

        private static bool IsOnlyAlphabetical(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            var pattern = @"^[a-zA-Z]*$";

            return Regex.IsMatch(value, pattern);
        }
    }
}