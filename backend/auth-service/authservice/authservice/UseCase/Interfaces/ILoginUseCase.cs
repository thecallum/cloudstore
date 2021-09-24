using System.Threading.Tasks;
using authservice.Domain;

namespace authservice.UseCase.Interfaces
{
    public interface ILoginUseCase
    {
        Task<User> Execute(string email);
    }
}