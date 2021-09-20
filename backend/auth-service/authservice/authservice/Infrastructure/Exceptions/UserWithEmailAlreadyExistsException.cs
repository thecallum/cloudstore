using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.Infrastructure.Exceptions
{
    public class UserWithEmailAlreadyExistsException : Exception
    {
        public UserWithEmailAlreadyExistsException(string email) 
            : base($"An account already exists with the email address \"{email}\"")
        {
         
        }
    }
}
