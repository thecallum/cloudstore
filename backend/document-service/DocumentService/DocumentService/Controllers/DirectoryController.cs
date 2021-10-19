using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
using DocumentService.Middleware;
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
    [Route("api/directory")]
    [ApiController]
    public class DirectoryController : ControllerBase
    {
        private readonly ICreateDirectoryUseCase _createDirectoryUseCase;
        private readonly IRenameDirectoryUseCase _renameDirectoryUseCase;
        private readonly IDeleteDirectoryUseCase _delteDirectoryUseCase;
        private readonly IGetAllDirectoriesUseCase _getAllDirectoriesUseCase;

        public DirectoryController(
            ICreateDirectoryUseCase createDirectoryUseCase,
            IRenameDirectoryUseCase renameDirectoryUseCase,
            IDeleteDirectoryUseCase delteDirectoryUseCase,
            IGetAllDirectoriesUseCase getAllDirectoriesUseCase)
        {
            _createDirectoryUseCase = createDirectoryUseCase;
            _renameDirectoryUseCase = renameDirectoryUseCase;
            _delteDirectoryUseCase = delteDirectoryUseCase;
            _getAllDirectoriesUseCase = getAllDirectoriesUseCase;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDirectory([FromBody] CreateDirectoryRequest request)
        {
            LogHelper.LogController("CreateDirectory");

            var user = (User)HttpContext.Items["user"];

            var directoryId = await _createDirectoryUseCase.Execute(request, user.Id);

            return Created(directoryId.ToString(), null);
        }

        [Route("{id}")]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RenameDirectory([FromRoute] RenameDirectoryQuery query, [FromBody] RenameDirectoryRequest request)
        {
            LogHelper.LogController("RenameDirectory");

            var user = (User)HttpContext.Items["user"];

            try
            {
                await _renameDirectoryUseCase.Execute(query, request, user.Id);
                return Ok();
            } catch(DirectoryNotFoundException)
            {
                return NotFound(query.Id);
            }
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDirectory([FromRoute] DeleteDirectoryQuery query)
        {
            LogHelper.LogController("DeleteDirectory");

            var user = (User)HttpContext.Items["user"];

            try
            {
                await _delteDirectoryUseCase.Execute(query, user.Id);
                return Ok();
            }
            catch(Exception e)
            {
                if (e is DirectoryNotFoundException)
                {
                    return NotFound(query.DirectoryId);

                }

                if (e is DirectoryContainsDocumentsException || e is DirectoryContainsChildDirectoriesException)
                {
                    return BadRequest();
                }

                // any unknown exception
                throw e;
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetAllDirectoriesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDirectories()
        {
            LogHelper.LogController("GetAllDirectories");

            var user = (User)HttpContext.Items["user"];

            var response = await _getAllDirectoriesUseCase.Execute(user.Id);
            return Ok(response);
        }
    }
}
