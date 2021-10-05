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
        private readonly IGetDocumentLinkUseCase _getDocumentLinkUseCase;
        private readonly IDeleteDocumentUseCase _deleteDocumentUseCase;

        public DocumentController(
            IUploadDocumentUseCase uploadDocumentUseCase,
            IGetAllDocumentsUseCase getAllDocumentsUseCase,
            IGetDocumentLinkUseCase getDocumentLinkUseCase,
            IDeleteDocumentUseCase deleteDocumentUseCase)
        {
            _uploadDocumentUseCase = uploadDocumentUseCase;
            _getAllDocumentsUseCase = getAllDocumentsUseCase;
            _getDocumentLinkUseCase = getDocumentLinkUseCase;
            _deleteDocumentUseCase = deleteDocumentUseCase;
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


        [HttpGet]
        [ProducesResponseType(typeof(GetAllDocumentsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // directory not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDocuments([FromQuery] GetAllDocumentsQuery query)
        {
            try
            {
                var response = await _getAllDocumentsUseCase.Execute(_userId, query);
                return Ok(response);

            }
            catch (DirectoryNotFoundException)
            {
                return NotFound(query.DirectoryId);
            }
        }

        [Route("{documentId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDocument([FromRoute] DeleteDocumentRequest request)
        {
            try
            {
                await _deleteDocumentUseCase.Execute(_userId, request.DocumentId);

                return NoContent();

            } catch(DocumentNotFoundException)
            {
                return NotFound(request.DocumentId);
            }
        }

        [Route("{documentId}")]
        [HttpGet]
        [ProducesResponseType(typeof(GetDocumentLinkResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDocumentLink([FromRoute] GetDocumentLinkQuery query)
        {
            try
            {
                var link = await _getDocumentLinkUseCase.Execute(_userId, query.DocumentId);

                var response = new GetDocumentLinkResponse { DocumentLink = link };

                return Ok(response);
            } 
            catch(DocumentNotFoundException)
            {
                return NotFound(query.DocumentId);
            }
        }
    }
}
