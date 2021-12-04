using System;
using authservice.Boundary.Request;
using User = authservice.Infrastructure.User;

namespace authservice.Factories
{
    public static class EntityFactories
    {
        public static TokenService.Models.User ToPayload(this User user)
        {
            return new TokenService.Models.User
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                StorageCapacity = Constants.AccountStorageCapacity
            };
        }

        public static User ToDomain(this RegisterRequestObject requestObject)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                FirstName = requestObject.FirstName,
                LastName = requestObject.LastName,
                Email = requestObject.Email
            };
        }
    }
}