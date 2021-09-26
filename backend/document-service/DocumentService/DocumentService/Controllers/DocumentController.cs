using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
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
    [Route("api/document")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        // Will be taken from request token
        private readonly Guid _userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

        private readonly IUploadDocumentUseCase _uploadDocumentUseCase;
        public DocumentController(IUploadDocumentUseCase uploadDocumentUseCase)
        {
            _uploadDocumentUseCase = uploadDocumentUseCase;
        }

        [Route("upload")]
        [HttpPost]
        [ProducesResponseType(typeof(UploadDocumentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadDocument([FromBody] UploadDocumentRequest request)
        {
            try
            {
                var response = await _uploadDocumentUseCase.Execute(request, _userId);

                // not sure what this will be yet
                var documentLocation = $"{response.DocumentId}";

                return Created(documentLocation, response);
            }
            catch (Exception ex)
            {
                if (ex is InvalidFilePathException || ex is FileTooLargeException)
                {
                    return BadRequest();
                }

                // unknown exception
                throw;
            }
        }
    }
}
