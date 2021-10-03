using DocumentService.Boundary.Request;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryController : ControllerBase
    {
        // Will be taken from request token
        private readonly Guid _userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

        private readonly ICreateDirectoryUseCase _createDirectoryUseCase;
        private readonly IRenameDirectoryUseCase _renameDirectoryUseCase;
        private readonly IDeleteDirectoryUseCase _delteDirectoryUseCase;

        public DirectoryController(
            ICreateDirectoryUseCase createDirectoryUseCase,
            IRenameDirectoryUseCase renameDirectoryUseCase,
            IDeleteDirectoryUseCase delteDirectoryUseCase)
        {
            _createDirectoryUseCase = createDirectoryUseCase;
            _renameDirectoryUseCase = renameDirectoryUseCase;
            _delteDirectoryUseCase = delteDirectoryUseCase;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDirectory([FromBody] CreateDirectoryRequest request)
        {
            var directoryId = await _createDirectoryUseCase.Execute(request, _userId);

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
            try
            {
                await _renameDirectoryUseCase.Execute(query, request, _userId);
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
            try
            {
                await _delteDirectoryUseCase.Execute(query, _userId);
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
    }
}
