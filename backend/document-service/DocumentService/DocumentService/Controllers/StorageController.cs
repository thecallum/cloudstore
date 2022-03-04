﻿using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Controllers
{
    [Route("api/storage")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IGetStorageUsageUseCase _getStorageUsageUseCase;

        public StorageController(IGetStorageUsageUseCase getStorageUsageUseCase)
        {
            _getStorageUsageUseCase = getStorageUsageUseCase;
        }

        [Route("usage")]
        [HttpGet]
        [ProducesResponseType(typeof(StorageUsageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStorageUsage()
        {
            LogHelper.LogController("GetStorageUsage");

            var user = (User)HttpContext.Items["user"];

            var response = await _getStorageUsageUseCase.Execute(user);

            return Ok(response);
        }
    }
}
