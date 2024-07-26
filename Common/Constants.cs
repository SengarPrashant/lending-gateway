namespace LoanGeteway.Common
{
    public class Message
    {
        public static string UnknownError { get; set; } = "Unable to process the request at the moment.";
        public static string Success { get; set; } = "Request processed successfully";
    }
    public class Status
    {
        public static string Submitted { get; set; } = "SUBMITTED";
        public static string InReview { get; set; } = "IN-REVIEW";
        public static string Approved { get; set; } = "APPROVED";
        public static string PartialApproved { get; set; } = "PARTIALAPPROVED";
        public static string Rejected { get; set; } = "REJECTED";
    }
    public class DbObjects
    {
        public static string Uri { get; set; } = "mongo";
        public static string DbName { get; set; } = "loan-gateway";
        public static string EligibilityCollection { get; set; } = "eligibility";
        public static string LoanApplicationCollection { get; set; } = "loanapplication";
    }
}
