using authservice.Infrastructure;
using System.Threading.Tasks;

namespace authservice.UseCase.Interfaces
{
    public interface ILoginUseCase
    {
        Task<User> Execute(string email);
    }
}