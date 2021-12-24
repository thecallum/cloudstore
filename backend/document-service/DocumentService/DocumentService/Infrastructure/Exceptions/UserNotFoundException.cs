using System;

namespace DocumentService.Infrastructure.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string email)
            : base($"No user could be found with the email address \"{email}\"")
        {
        }
    }
}