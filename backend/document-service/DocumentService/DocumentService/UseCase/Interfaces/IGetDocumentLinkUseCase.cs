﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IGetDocumentLinkUseCase
    {
        Task<string> Execute(Guid userId, Guid documentId);
    }
}
