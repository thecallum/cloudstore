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
        private readonly IGetAllDocumentsUseCase _getAllDocumentsUseCase;

        public DocumentController(IUploadDocumentUseCase uploadDocumentUseCase, IGetAllDocumentsUseCase getAllDocumentsUseCase)
        {
            _uploadDocumentUseCase = uploadDocumentUseCase;
            _getAllDocumentsUseCase = getAllDocumentsUseCase;
        }

        [Route("upload")]
        [HttpPost]
        [ProducesResponseType(typeof(UploadDocumentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // directory not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadDocument([FromBody] UploadDocumentRequest request)
        {
            try
            {
                var response = await _uploadDocumentUseCase.Execute(request, _userId);

                // not sure what this will be yet
                var documentLocation = response.DocumentId.ToString();

                return Created(documentLocation, response);
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException)
                {
                    return NotFound((Guid)request.DirectoryId);
                }    

                if (ex is InvalidFilePathException || ex is FileTooLargeException)
                {
                    return BadRequest();
                }

                // unknown exception
                throw;
            }
        }

        // new tests

        // Upload document e2e tests


        // ( Need Factory Tests )

        [HttpGet]
        [ProducesResponseType(typeof(GetAllDocumentsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDocuments()
        {
            var documents = await _getAllDocumentsUseCase.Execute(_userId);

            var response = new GetAllDocumentsResponse { Documents = documents.ToList() };

            return Ok(response);
        }
     }
}
