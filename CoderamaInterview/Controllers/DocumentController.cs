using CoderamaInterview.Models;
using CoderamaInterview.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoderamaInterview.Controllers
{
    [ApiController]
    [Route("documents")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody] Document document)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _logger.LogInformation("Creating document with ID: {DocumentId}", document.Id);
                await _documentService.SaveDocumentAsync(document);
                return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document with ID: {DocumentId}", document.Id);
                return StatusCode(500, "An error occurred while creating the document.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDocument([FromBody] Document document)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _logger.LogInformation("Updating document with ID: {DocumentId}", document.Id);
                await _documentService.UpdateDocumentAsync(document);
                return Ok(new { Message = $"Document with ID {document.Id} updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document with ID: {DocumentId}", document.Id);
                return StatusCode(500, "An error occurred while updating the document.");
            }
        }

        [HttpGet("{id}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept")]
        public async Task<IActionResult> GetDocument(string id)
        {
            try
            {
                var acceptHeader = Request.Headers["Accept"].FirstOrDefault() ?? "application/json";
                _logger.LogInformation("Retrieving document with ID: {DocumentId}", id);
                var serializedDocument = await _documentService.GetDocumentAsync(id, acceptHeader);
                return Content(serializedDocument, acceptHeader);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Document with ID: {DocumentId} not found", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document with ID: {DocumentId}", id);
                return StatusCode(500, "An error occurred while retrieving the document.");
            }
        }
    }
}
