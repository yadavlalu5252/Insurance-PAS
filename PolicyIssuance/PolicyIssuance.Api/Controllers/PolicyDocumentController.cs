
using Microsoft.AspNetCore.Mvc;
using PolicyIssuance.Application.Interface;

namespace PolicyIssuance.Api.Controllers
{
    [ApiController]
    [Route("api/v1/policies/{id:long}/document")]
    public class PolicyDocumentController : ControllerBase
    {
        private readonly IPolicyDocumentService service;

        public PolicyDocumentController(IPolicyDocumentService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Download(long id)
        {
            var document = await service.GetDocumentAsync(id);

            if (document == null)
                return NotFound("Policy document not found.");

            if (!System.IO.File.Exists(document.document_path))
                return NotFound("PDF file not found on the server.");

            return PhysicalFile(
                document.document_path,
                "application/pdf",
                document.document_name);
        }

        [HttpPost("regenerate")]
        public async Task<IActionResult> Regenerate(long id)
        {
            var document = await service.RegenerateDocumentAsync(id);

            if (document == null)
            {
                return BadRequest(
                    "PDF regeneration has not been implemented yet.");
            }

            return Ok(document);
        }
    }
}
