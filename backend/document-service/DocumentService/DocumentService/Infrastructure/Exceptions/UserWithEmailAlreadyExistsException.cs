using System;

namespace DocumentService.Infrastructure.Exceptions
{
    public class UserWithEmailAlreadyExistsException : Exception
    {
        public UserWithEmailAlreadyExistsException(string email)
            : base($"An account already exists with the email address \"{email}\"")
        {
        }
    }
}