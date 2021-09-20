using authservice.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.UseCase.Interfaces
{
    public interface ILoginUseCase
    {
        Task<User> Execute(string email);
    }
}
