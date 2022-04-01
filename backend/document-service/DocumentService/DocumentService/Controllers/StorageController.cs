using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Logging;
using DocumentService.UseCase;
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
        private readonly IStorageUsageUseCase _storageUsageUseCase;

        public StorageController(IStorageUsageUseCase storageUsageUseCase)
        {
            _storageUsageUseCase = storageUsageUseCase;
        }

        [Route("usage")]
        [HttpGet]
        [ProducesResponseType(typeof(StorageUsageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStorageUsage()
        {
            LogHelper.LogController("GetStorageUsage");

            var user = (User)HttpContext.Items["user"];

            var storageUsage = await _storageUsageUseCase.GetUsage(user);

            var response = new StorageUsageResponse
            {
                StorageUsage = storageUsage,
                Capacity = user.StorageCapacity
            };

            return Ok(response);
        }
    }
}
