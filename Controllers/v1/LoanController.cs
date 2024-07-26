using LoanGateway.Models;
using LoanGeteway.Common;
using LoanGeteway.Models;
using LoanGeteway.Services;
using Microsoft.AspNetCore.Mvc;


namespace LoanGeteway.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        public LoanController(ILoanService loanService) {
        _loanService = loanService;
        }
        
        [HttpGet("products")]
        [ProducesResponseType(typeof(ApiResponse<List<Product>, string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 500)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var products = await _loanService.GetProductsList();

                return Ok(ApiResponse<List<Product>, string>.SuccessObject(products, Message.Success));
            }
            catch (Exception ex)
            {
                var error = new List<ErrorDetail>
                    {
                        new ErrorDetail { ErrorCode = "500", Message =Message.UnknownError }
                    };
                return StatusCode(500,ApiResponse<string, List<ErrorDetail>>.ErrorObject(error));
            }
 
        }

      
        [HttpPost("products/{productCode}/eligibility")]
        [ProducesResponseType(typeof(ApiResponse<EligibilityCheckResponse, string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 500)]
        public async Task<IActionResult> Eligibility(string productCode, [FromBody] EligibilityCheck request)
        {
            try
            {
                request.ProductCode = productCode;
                request.RequestId = $"{productCode.Substring(0, 1)}ELG{DateTime.Now.Ticks}";

                var result = await _loanService.EligibilityCheck(productCode,request);

                return Ok(ApiResponse<EligibilityCheckResponse, string>.SuccessObject(result, Message.Success));
            }
            catch (Exception ex)
            {
                var error = new List<ErrorDetail>
                {
                   new ErrorDetail { ErrorCode = "500", Message = Message.UnknownError }
                };
                return StatusCode(500, ApiResponse<string, List<ErrorDetail>>.ErrorObject(error));
            }
        }


        [HttpPost("products/{productCode}/apply")]
        [ProducesResponseType(typeof(ApiResponse<LoanApplicationResponse, string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 500)]
        public async Task<IActionResult> Apply(string productCode, [FromForm] LoanApplication request)
        {
            try
            {
                request.ProductCode = productCode;
                var result =await _loanService.SubmitApplication(productCode, request);

                return Ok(ApiResponse<LoanApplicationResponse, string>.SuccessObject(result, Message.Success));
            }
            catch (Exception ex)
            {
                var error = new List<ErrorDetail>
                {
                   new ErrorDetail { ErrorCode = "500", Message = Message.UnknownError }
                };
                return StatusCode(500, ApiResponse<string, List<ErrorDetail>>.ErrorObject(error));
            }
        }

        [HttpGet("{userid}/{arn}/Status")]
        [ProducesResponseType(typeof(ApiResponse<LoanStatusResponse, string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 500)]
        public async Task<IActionResult> Status(string userid, string arn)
        {
            try
            {
                var result = await _loanService.GetStatus(userid, arn);

                return Ok(ApiResponse<LoanStatusResponse, string>.SuccessObject(result, Message.Success));
            }
            catch (Exception ex)
            {
                var error = new List<ErrorDetail>
                {
                   new ErrorDetail { ErrorCode = "500", Message = Message.UnknownError }
                };
                return StatusCode(500, ApiResponse<string, List<ErrorDetail>>.ErrorObject(error));
            }
        }

        [HttpGet("{userid}/history")]
        [ProducesResponseType(typeof(ApiResponse<LoanStatusResponse, string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 500)]
        public async Task<IActionResult> History(string userid)
        {
            try
            {
                var result = await _loanService.GetHistory(userid);

                return Ok(ApiResponse<UserRequestHistory, string>.SuccessObject(result, Message.Success));
            }
            catch (Exception ex)
            {
                var error = new List<ErrorDetail>
                {
                   new ErrorDetail { ErrorCode = "500", Message = Message.UnknownError }
                };
                return StatusCode(500, ApiResponse<string, List<ErrorDetail>>.ErrorObject(error));
            }
        }

        [HttpGet("history")]
        [ProducesResponseType(typeof(ApiResponse<LoanStatusResponse, string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 500)]
        public async Task<IActionResult> History()
        {
            try
            {
                var result = await _loanService.GetHistory();

                return Ok(ApiResponse<UserRequestHistory, string>.SuccessObject(result, Message.Success));
            }
            catch (Exception ex)
            {
                var error = new List<ErrorDetail>
                {
                   new ErrorDetail { ErrorCode = "500", Message = Message.UnknownError }
                };
                return StatusCode(500, ApiResponse<string, List<ErrorDetail>>.ErrorObject(error));
            }
        }

        [HttpPut("updateStauts")]
        [ProducesResponseType(typeof(ApiResponse<LoanStatusResponse, string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string, List<ErrorDetail>>), 500)]
        public async Task<IActionResult> StautsUpdate(StausUpdateRequest request)
        {
            try
            {
                if (!LoanRequestStatus.All.Contains(request.Status))
                {
                    var error = new List<ErrorDetail>
                    {
                       new ErrorDetail { ErrorCode = "400", Message = Message.InvalidStatus }
                    };
                    return BadRequest(ApiResponse<string, List<ErrorDetail>>.ErrorObject(error));
                }
                var result = await _loanService.UpdateStatus(request);

                return Ok(ApiResponse<bool, string>.SuccessObject(result, Message.Success));
            }
            catch (Exception ex)
            {
                var error = new List<ErrorDetail>
                {
                   new ErrorDetail { ErrorCode = "500", Message = Message.UnknownError }
                };
                return StatusCode(500, ApiResponse<string, List<ErrorDetail>>.ErrorObject(error));
            }
        }
    }
}
