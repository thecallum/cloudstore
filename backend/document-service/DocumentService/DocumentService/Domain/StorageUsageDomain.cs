using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Domain
{
    public class StorageUsageDomain
    {
        public long StorageUsage { get; set; }
        public long Capacity { get; set; }
    }
}
