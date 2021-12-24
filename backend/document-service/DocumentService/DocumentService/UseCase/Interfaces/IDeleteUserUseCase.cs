using System;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IDeleteUserUseCase
    {
        Task Execute(Guid id);
    }
}