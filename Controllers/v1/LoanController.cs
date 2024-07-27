using LoanGateway.Models;
using LoanGeteway.Common;
using LoanGeteway.Models;
using LoanGeteway.Services;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

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
        public async Task<IActionResult> Eligibility(string productCode, [FromBody] EligibilityCheckRequest request)
        {
            try
            {
                var validationResult = await ValidatEligibilityCheckRequest(request,productCode);
                if (validationResult.Count>0)
                {
                    return BadRequest(ApiResponse<string, List<ErrorDetail>>.ErrorObject(validationResult));
                }

                var eligibilityRequest = _loanService.ConvertToEligibilityCheckDto(request, productCode);
                eligibilityRequest.RequestId = $"{productCode.ToUpper().Substring(0, 1)}ELG{DateTime.Now.Ticks}";

                var result = await _loanService.EligibilityCheck(productCode, eligibilityRequest);

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
        public async Task<IActionResult> Apply(string productCode, [FromForm] LoanApplicationRequest request)
        {
            try
            {

                var validationResult = await ValidateApplicationRequest(request, productCode);
                if (validationResult.Count>0)
                {
                    return BadRequest(ApiResponse<string, List<ErrorDetail>>.ErrorObject(validationResult));
                }

                var appliactionRequest = _loanService.ConvertToLoanApplicationDto(request, productCode);

                var result =await _loanService.SubmitApplication(productCode, appliactionRequest);

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


        private async Task<List<ErrorDetail>> ValidateApplicationRequest(LoanApplicationRequest request, string productCode)
        {
            var isValidProductCode = await _loanService.ValidateProductCode(productCode);
            var error = new List<ErrorDetail>();

            if (!isValidProductCode)
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidProductCode });

            if (!Occupation.All.Contains(request.Occupation))
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidOccupation });
            }

            if (double.IsNaN(request.AnnualIncome) || double.IsInfinity(request.AnnualIncome) || request.AnnualIncome ==0 )
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidAnualIncome });
            }

            if (request.TenureMonths <= 0)
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidTenure });
            }

            if (string.IsNullOrEmpty(request.Aadhaar) || request.Aadhaar.Length !=12 || !BigInteger.TryParse(request.Aadhaar,out _))
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidAadhaar });
            }
            if (string.IsNullOrEmpty(request.Pan) || request.Pan.Length != 10)
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidPan });
            }
            if (!request.Consent)
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.Concent });
            }

            if (string.IsNullOrEmpty(request.FullName))
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.FullNameReq });
            }
            if (string.IsNullOrEmpty(request.Mobile))
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.MobileReq });
            }

            if (request.Documents.Count==0)
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.DocumentsReq });
            }

            return error;

        }

        private async Task<List<ErrorDetail>> ValidatEligibilityCheckRequest(EligibilityCheckRequest request, string productCode)
        {
            var isValidProductCode = await _loanService.ValidateProductCode(productCode);
            var error = new List<ErrorDetail>();

            if (!isValidProductCode)
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidProductCode });

            if (!Occupation.All.Contains(request.Occupation))
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidOccupation });
            }

            if (double.IsNaN(request.AnnualIncome) || double.IsInfinity(request.AnnualIncome) || request.AnnualIncome == 0)
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidAnualIncome });
            }

            if (request.TenureMonths <= 0)
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidTenure });
            }

            if (string.IsNullOrEmpty(request.Aadhaar) || request.Aadhaar.Length != 12 || !BigInteger.TryParse(request.Aadhaar, out _))
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidAadhaar });
            }
            if (string.IsNullOrEmpty(request.Pan) || request.Pan.Length != 10)
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.InvalidPan });
            }
            if (!request.Consent)
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.Concent });
            }

            if (string.IsNullOrEmpty(request.FullName))
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.FullNameReq });
            }
            if (string.IsNullOrEmpty(request.Mobile))
            {
                error.Add(new ErrorDetail { ErrorCode = "400", Message = Message.MobileReq });
            }

           
            return error;

        }
    }
}
