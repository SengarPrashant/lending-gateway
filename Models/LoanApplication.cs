using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LoanGeteway.Models
{
    public class LoanApplicationRequest
    {
        public string? EligibilityRequestId { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Pan { get; set; }
        public string Aadhaar { get; set; }
        public double Amount { get; set; } = 0.0;
        public int TenureMonths { get; set; } = 0;
        public List<IFormFile> Documents { get; set; }
        public string Occupation { get; set; } // Self imployed / Salaried
        public double AnnualIncome { get; set; } = 0;
        public bool Consent { get; set; }
    }
    public class LoanApplication : LoanApplicationRequest
    {
        public string? Arn { get; set; } // This will not be passed from frontend. This is for backend use
       
        [BsonIgnore]
        public List<IFormFile> Documents { get; set; }
        public string? Status { get; set; }  
        public string? Remarks { get; set; }
        public double? Emi { get; set; } =0;
        public double? InterestRate { get; set; } = 0;
        public string? ProductCode { get; set; }
        public List<string>? DocumentsBase64 { get; set; }
    }
    public class LoanApplicationResponse
    {
        public string Arn { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public double Amount { get; set; }
        public int TenureMonths { get; set; }
        public double InterestRate { get; set; }
    }
    public class LoanStatusResponse
    {
        public string Arn { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public List<string> CompletedSteps { get; set; }
        public List<string> PendingSteps { get; set; }
    }
}
