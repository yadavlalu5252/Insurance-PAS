using KycVerification.Application.DTO;
using KycVerification.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KycVerifiaction.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kyc")]
    [Authorize]
    public class KycController : ControllerBase
    {
        private readonly IKycService service;

        public KycController(IKycService service)
        {
            this.service = service;
        }

        // GET: api/v1/kyc
        // Agent/Admin
        [HttpGet]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await service.GetAllKycAsync();

            return Ok(result);
        }

        // GET: api/v1/kyc/5
        // Customer/Agent/Admin
        [HttpGet("{kycId:int}")]
        [Authorize(Roles = "Customer,Agent,Admin")]
        public async Task<IActionResult> GetById(int kycId)
        {
            var result = await service.GetKycByIdAsync(kycId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "KYC record not found."
                });
            }

            return Ok(result);
        }

        // POST: api/v1/kyc
        // Agent/Admin
        [HttpPost]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateCustomerKycDto dto)
        {
            try
            {
                var result = await service.CreateKycAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { kycId = result.KycId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // GET: api/v1/kyc/5/documents
        // Customer/Agent/Admin
        [HttpGet("{kycId:int}/documents")]
        [Authorize(Roles = "Customer,Agent,Admin")]
        public async Task<IActionResult> GetDocuments(int kycId)
        {
            var kyc = await service.GetKycByIdAsync(kycId);

            if (kyc == null)
            {
                return NotFound(new
                {
                    message = "KYC record not found."
                });
            }

            var documents = await service.GetDocumentsAsync(kycId);

            return Ok(documents);
        }

        // PUT: api/v1/kyc/5/approve
        // Admin only
        [HttpPut("{kycId:int}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(
            int kycId,
            [FromBody] ReviewKycDto dto)
        {
            try
            {
                var result = await service.ApproveKycAsync(
                    kycId,
                    dto);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "KYC record not found."
                    });
                }

                return Ok(new
                {
                    message = "KYC approved successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/v1/kyc/5/reject
        // Admin only
        [HttpPut("{kycId:int}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(
            int kycId,
            [FromBody] ReviewKycDto dto)
        {
            try
            {
                var result = await service.RejectKycAsync(
                    kycId,
                    dto);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "KYC record not found."
                    });
                }

                return Ok(new
                {
                    message = "KYC rejected successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/v1/kyc/documents/1/verify
        // Admin only
        [HttpPut("documents/{documentId:int}/verify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyDocument(
            int documentId,
            [FromBody] ReviewKycDto dto)
        {
            var result = await service.VerifyDocumentAsync(
                documentId,
                dto);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Document not found."
                });
            }

            return Ok(new
            {
                message = "Document verified successfully."
            });
        }

        // PUT: api/v1/kyc/documents/1/reject
        // Admin only
        [HttpPut("documents/{documentId:int}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectDocument(
            int documentId,
            [FromBody] ReviewKycDto dto)
        {
            try
            {
                var result = await service.RejectDocumentAsync(
                    documentId,
                    dto);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Document not found."
                    });
                }

                return Ok(new
                {
                    message = "Document rejected successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // POST: api/v1/kyc/documents/1/reupload
        // Customer/Agent
        [HttpPost("documents/{documentId:int}/reupload")]
        [Authorize(Roles = "Customer,Agent")]
        public async Task<IActionResult> ReuploadDocument(
            int documentId,
            [FromForm] UploadKycDocumentDto dto)
        {
            try
            {
                var result = await service.ReuploadDocumentAsync(
                    documentId,
                    dto);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}