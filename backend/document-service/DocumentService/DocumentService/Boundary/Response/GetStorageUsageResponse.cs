﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Response
{
    public class GetStorageUsageResponse
    {
        public long StorageUsage { get; set; }
        public long Capacity { get; set; }
    }
}
