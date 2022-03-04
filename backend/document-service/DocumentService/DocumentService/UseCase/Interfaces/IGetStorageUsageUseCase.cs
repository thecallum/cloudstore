﻿using DocumentService.Boundary.Response;
using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IGetStorageUsageUseCase
    {
        Task<StorageUsageResponse> Execute(User user);
    }
}
