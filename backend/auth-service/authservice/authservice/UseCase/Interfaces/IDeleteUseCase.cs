using System.Threading.Tasks;

namespace authservice.UseCase.Interfaces
{
    public interface IDeleteUseCase
    {
        Task Execute(string email);
    }
}