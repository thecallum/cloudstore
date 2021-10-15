using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
using DocumentService.Middleware;
using DocumentService.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.Controllers
{
    [Route("api/document")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
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

        [Route("upload/{existingDocumentId?}")]
        [HttpGet]
        [ProducesResponseType(typeof(GetDocumentUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetDocumentUploadLink([FromRoute] GetDocumentUploadLinkQuery query)
        {
            LogHelper.LogController("GetDocumentUploadLink");

            var user = (User)HttpContext.Items["user"];

            var response = _getDocumentUploadLinkUseCase.Execute(user.Id, query);

            return Ok(response);
        }

        [Route("validate/{documentId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateUploadedDocument([FromRoute] ValidateUploadedDocumentQuery query, [FromBody] ValidateUploadedDocumentRequest request)
        {
            LogHelper.LogController("ValidateUploadedDocument");

            var user = (User)HttpContext.Items["user"];

            var document = await _validateUploadedDocumentUseCase.Execute(user.Id, query.DocumentId, request);
            if (document == null) return NotFound(query.DocumentId);

            return Created($"/document-service/api/document/{query.DocumentId}", document);
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetAllDocumentsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // directory not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDocuments([FromQuery] GetAllDocumentsQuery query)
        {
            LogHelper.LogController("GetAllDocuments");

            var user = (User)HttpContext.Items["user"];

            try
            {
                var response = await _getAllDocumentsUseCase.Execute(user.Id, query);
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
            LogHelper.LogController("DeleteDocument");

            var user = (User)HttpContext.Items["user"];

            try
            {
                await _deleteDocumentUseCase.Execute(user.Id, request.DocumentId);

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
            LogHelper.LogController("GetDocumentDownloadLink");

            var user = (User)HttpContext.Items["user"];

            try
            {
                var link = await _getDocumentDownloadLinkUseCase.Execute(user.Id, query.DocumentId);

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
