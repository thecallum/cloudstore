using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentServiceListener.Services
{
    public interface IStorageUsageCache
    {
        Task DeleteCache(User user);
    }
}
