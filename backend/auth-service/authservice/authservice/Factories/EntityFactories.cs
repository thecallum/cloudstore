using authservice.Domain;
using authservice.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.Factories
{
    public static class EntityFactories
    {
        public static Payload ToPayload(this User user) {
            return new Payload
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
