using DocumentService.Boundary.Response;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Models;

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
        [ProducesResponseType(typeof(GetStorageUsageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStorageUsage()
        {
            LogHelper.LogController("GetStorageUsage");

            var user = (User)HttpContext.Items["user"];

            var response = await _getStorageUsageUseCase.Execute(user.Id);

            return Ok(response);
        }
    }
}
