﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Request
{
    public class GetAllDocumentsQuery
    { 
        [FromQuery(Name = "directoryId")]
        public Guid? DirectoryId { get; set; } = null;
    }
}
