using DocumentService.Infrastructure;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface ILoginUseCase
    {
        Task<UserDb> Execute(string email);
    }
}