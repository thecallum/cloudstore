using System;
using authservice.Boundary.Request;
using authservice.Domain;
using authservice.Infrastructure;
using authservice.JWT;

namespace authservice.Factories
{
    public static class EntityFactories
    {
        public static Payload ToPayload(this User user)
        {
            return new Payload
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public static User ToDomain(this UserDb entity)
        {
            return new User
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Hash = entity.Hash
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

        public static UserDb ToDatabase(this User domain)
        {
            return new UserDb
            {
                Id = domain.Id,
                FirstName = domain.FirstName,
                LastName = domain.LastName,
                Email = domain.Email,
                Hash = domain.Hash
            };
        }
    }
}