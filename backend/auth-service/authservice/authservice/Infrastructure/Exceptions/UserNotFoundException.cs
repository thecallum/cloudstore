using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.Infrastructure.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string email) 
            : base($"No user could be found with the email address \"{email}\"")
        {

        }
    }
}
