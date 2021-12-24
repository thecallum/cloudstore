using System;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IDeleteUseCase
    {
        Task Execute(Guid id);
    }
}