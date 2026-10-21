using CustomerRegistration.Api.Wrappers;
using CustomerRegistration.Application.DTO;
using CustomerRegistration.Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRegistration.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DummyController : ControllerBase
    {
        private readonly IDummyService _dummyService;

        public DummyController(IDummyService dummyService)
        {
            _dummyService = dummyService;
        }

        [HttpGet]
        public IActionResult GetDummy()
        {
            var data = _dummyService.GetDummy();

            return Ok(
                ApiResponse<DummyDTO>.SuccessResponse(
                    data,
                    "Dummy data retrieved successfully"));
        }

        [HttpPost]
        public IActionResult CreateDummy([FromBody] DummyDTO dto)
        {
            var data = _dummyService.CreateDummy(dto);

            return Ok(
                ApiResponse<DummyDTO>.SuccessResponse(
                    data,
                    "Dummy data created successfully"));
        }

        [HttpGet("exception")]
        public IActionResult TestException()
        {
            _dummyService.TestException();

            return Ok();
        }
    }
}
