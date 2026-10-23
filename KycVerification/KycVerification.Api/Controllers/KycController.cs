using KycVerification.Application.DTO;
using KycVerification.Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace KycVerification.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kyc")]
    public class KycController : ControllerBase
    {
        private readonly IKycService service;

        public KycController(IKycService service)
        {
            this.service = service;
        }

        // GET: api/v1/kyc
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await service.GetAllKycAsync();
            return Ok(result);
        }

        // GET: api/v1/kyc/5
        [HttpGet("{kycId:int}")]
        public async Task<IActionResult> GetById(int kycId)
        {
            var result = await service.GetKycByIdAsync(kycId);

            if (result == null)
                return NotFound(new { message = "KYC record not found." });

            return Ok(result);
        }

        // POST: api/v1/kyc
        [HttpPost]
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
                return Conflict(new { message = ex.Message });
            }
        }

        //// POST: api/v1/kyc/documents/upload
        //[HttpPost("documents/upload")]
        //public async Task<IActionResult> UploadDocument(
        //    [FromForm] UploadKycDocumentDto dto)
        //{
        //    try
        //    {
        //        var result = await service.UploadDocumentAsync(dto);

        //        return StatusCode(201, result);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return Conflict(new { message = ex.Message });
        //    }
        //}

        // GET: api/v1/kyc/5/documents
        [HttpGet("{kycId:int}/documents")]
        public async Task<IActionResult> GetDocuments(int kycId)
        {
            var kyc = await service.GetKycByIdAsync(kycId);

            if (kyc == null)
                return NotFound(new { message = "KYC record not found." });

            var documents = await service.GetDocumentsAsync(kycId);

            return Ok(documents);
        }

        //[Authorize(Policy = "AdminOnly")]
        // PUT: api/v1/kyc/5/approve
        [HttpPut("{kycId:int}/approve")]
        public async Task<IActionResult> Approve(
            int kycId, [FromBody] ReviewKycDto dto)
        {
            try
            {
                var result = await service.ApproveKycAsync(kycId, dto);

                if (!result)
                    return NotFound(new { message = "KYC record not found." });

                return Ok(new { message = "KYC approved successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        //[Authorize(Policy = "AdminOnly")]
        // PUT: api/v1/kyc/5/reject
        [HttpPut("{kycId:int}/reject")]
        public async Task<IActionResult> Reject(int kycId, [FromBody] ReviewKycDto dto)
        {
            try
            {
                var result = await service.RejectKycAsync(kycId, dto);

                if (!result)
                    return NotFound(new { message = "KYC record not found." });

                return Ok(new { message = "KYC rejected successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize(Policy = "AdminOnly")]
        // PUT: api/v1/kyc/documents/1/verify
        [HttpPut("documents/{documentId:int}/verify")]
        public async Task<IActionResult> VerifyDocument(
            int documentId,
            [FromBody] ReviewKycDto dto)
        {
            var result = await service.VerifyDocumentAsync(documentId, dto);

            if (!result)
                return NotFound(new { message = "Document not found." });

            return Ok(new { message = "Document verified successfully." });
        }

        //[Authorize(Policy = "AdminOnly")]
        // PUT: api/v1/kyc/documents/1/reject
        [HttpPut("documents/{documentId:int}/reject")]
        public async Task<IActionResult> RejectDocument(
            int documentId,
            [FromBody] ReviewKycDto dto)
        {
            try
            {
                var result = await service.RejectDocumentAsync(documentId, dto);

                if (!result)
                    return NotFound(new { message = "Document not found." });

                return Ok(new { message = "Document rejected successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // POST: api/v1/kyc/documents/1/reupload
        [HttpPost("documents/{documentId:int}/reupload")]
        public async Task<IActionResult> ReuploadDocument(
            int documentId,
            [FromForm] UploadKycDocumentDto dto)
        {
            try
            {
                var result = await service.ReuploadDocumentAsync(
                    documentId, dto);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
