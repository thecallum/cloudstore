﻿using DocumentService.Domain;
using DocumentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentService.Gateways.Interfaces
{
    public interface IDirectoryGateway
    {
        Task CreateDirectory(DirectoryDomain directory);
        Task DeleteDirectory(Guid directoryId, Guid userId);
        Task RenameDirectory(string newName, Guid directoryId, Guid userId);
        Task<IEnumerable<DirectoryDomain>> GetAllDirectories(Guid userId);
        Task<bool> CheckDirectoryExists(Guid directoryId, Guid userId);
        Task<bool> ContainsChildDirectories(Guid directoryId, Guid userId);
    }
}
