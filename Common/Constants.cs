namespace LoanGeteway.Common
{
    public class Message
    {
        public static string UnknownError { get; set; } = "Unable to process the request at the moment.";
        public static string Success { get; set; } = "Request processed successfully";
        public static string InvalidStatus { get; set; } = "Invalid status";
        public static string InvalidProductCode { get; set; } = "Invalid product code.";
        public static string InvalidOccupation { get; set; } = "Invalid occupation.";
        public static string InvalidAnualIncome { get; set; } = "Invalid annual income.";
        public static string InvalidTenure { get; set; } = "Tenure should be minimum 6 month.";
        public static string InvalidAadhaar { get; set; } = "Invalid aadhaar number.";
        public static string InvalidPan { get; set; } = "Invalid PAN number.";
        public static string Concent { get; set; } = "User concent is required.";
        public static string FullNameReq { get; set; } = "Full Name is required.";
        public static string MobileReq { get; set; } = "Mobile number is required.";
        public static string DocumentsReq { get; set; } = "Documents are required.";
    }
    public class LoanRequestStatus
    {
        public static string Submitted { get; set; } = "SUBMITTED";
        public static string InReview { get; set; } = "IN-REVIEW";
        public static string Approved { get; set; } = "APPROVED";
        public static string PartialApproved { get; set; } = "PARTIAL-APPROVED";
        public static string Rejected { get; set; } = "REJECTED";
        public static string Disbursed { get; set; } = "DISBURSED";

        public static List<string> All { get; set; } =new List<string> { Submitted, InReview, Approved, PartialApproved, Rejected, Disbursed };
    }
    public class DbObjects
    {
        public static string Uri { get; set; } = "mongo";
        public static string DbName { get; set; } = "loan-gateway";
        public static string EligibilityCollection { get; set; } = "eligibility";
        public static string LoanApplicationCollection { get; set; } = "loanappliaction";
    }

    public class Occupation
    {
        public static string Salaried { get; set; } = "Salaried";
        public static string Self { get; set; } = "Self employed";
        public static List<string> All { get; set; } = new List<string> { Salaried, Self };
    }
}
