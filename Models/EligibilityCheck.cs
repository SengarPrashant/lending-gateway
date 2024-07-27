using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LoanGeteway.Models
{
    public class EligibilityCheckRequest
    {
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Pan { get; set; }
        public string Aadhaar { get; set; }
        public double Amount { get; set; }
        public int TenureMonths { get; set; }
        public bool Consent { get; set; }
        public string Occupation { get; set; } // Self imployed / Salaried
        public double AnnualIncome { get; set; }
     
    }

    public class EligibilityCheck : EligibilityCheckRequest
    {
        public string RequestId { get; set; }
        // thi is backend use start
        public double? InterestRate { get; set; } = 0;
        public double? Emi { get; set; } = 0;
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public string? ProductCode { get; set; }
        // thi is backend use end
    }

    public class EligibilityCheck1
    {
        public string RequestId { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Pan { get; set; }
        public string Aadhaar { get; set; }
        public double Amount { get; set; }
        public int TenureMonths { get; set; }
        public bool Consent { get; set; }
        public string Occupation { get; set; } // Self imployed / Salaried
        public double AnnualIncome { get; set; }

        // thi is backend use start
        public double? InterestRate { get; set; } =0;
        public double? Emi { get; set; } = 0;
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public string? ProductCode { get; set; }
        // thi is backend use end
    }
    public class EligibilityCheckResponse
    {
        public string RequestId { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public double InterestRate { get; set; }
        public double Emi { get; set; }
    }
}
