using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DocumentService.Controllers
{
    [Route("api/document")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        // Will be taken from request token
        private readonly Guid _userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

        private readonly IGetAllDocumentsUseCase _getAllDocumentsUseCase;
        private readonly IGetDocumentDownloadLinkUseCase _getDocumentDownloadLinkUseCase;
        private readonly IDeleteDocumentUseCase _deleteDocumentUseCase;
        private readonly IGetDocumentUploadLinkUseCase _getDocumentUploadLinkUseCase;
        private readonly IValidateUploadedDocumentUseCase _validateUploadedDocumentUseCase;

        public DocumentController(
            IGetAllDocumentsUseCase getAllDocumentsUseCase,
            IGetDocumentDownloadLinkUseCase getDocumentDownloadLinkUseCase,
            IGetDocumentUploadLinkUseCase getDocumentUploadLinkUseCase,
            IValidateUploadedDocumentUseCase validateUploadedDocumentUseCase,
            IDeleteDocumentUseCase deleteDocumentUseCase)
        {
            _getAllDocumentsUseCase = getAllDocumentsUseCase;
            _getDocumentDownloadLinkUseCase = getDocumentDownloadLinkUseCase;
            _deleteDocumentUseCase = deleteDocumentUseCase;
            _getDocumentUploadLinkUseCase = getDocumentUploadLinkUseCase;
            _validateUploadedDocumentUseCase = validateUploadedDocumentUseCase;
        }

        [Route("upload")]
        [HttpGet]
        [ProducesResponseType(typeof(GetDocumentUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetDocumentUploadLink()
        {
            var response = _getDocumentUploadLinkUseCase.Execute(_userId);

            return Ok(response);
        }

        [Route("validate/{documentId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateUploadedDocument([FromRoute] ValidateUploadedDocumentQuery query, [FromBody] ValidateUploadedDocumentRequest request)
        {
            var document = await _validateUploadedDocumentUseCase.Execute(_userId, query.DocumentId, request);
            if (document == null) return NotFound(query.DocumentId);

            return Created($"/document-service/api/document/{query.DocumentId}", document);
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

            }
            catch (DocumentNotFoundException)
            {
                return NotFound(request.DocumentId);
            }
        }

        [Route("download/{documentId}")]
        [HttpGet]
        [ProducesResponseType(typeof(GetDocumentLinkResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDocumentDownloadLink([FromRoute] GetDocumentLinkQuery query)
        {
            try
            {
                var link = await _getDocumentDownloadLinkUseCase.Execute(_userId, query.DocumentId);

                var response = new GetDocumentLinkResponse { DocumentLink = link };

                return Ok(response);
            }
            catch (DocumentNotFoundException)
            {
                return NotFound(query.DocumentId);
            }
        }
    }
}
