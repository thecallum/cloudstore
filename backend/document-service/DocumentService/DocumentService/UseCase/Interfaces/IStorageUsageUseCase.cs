using DocumentService.Domain;
using System.Threading.Tasks;

namespace DocumentService.UseCase
{
    public interface IStorageUsageUseCase
    {
        Task<long> GetUsage(User user);
        Task UpdateUsage(User user, long difference);
    }
}
