namespace LoanGeteway.Models
{
    public class UserRequestHistory
    {
        public List<EligibilityCheck> EligibilityChecks { get; set; }
        public List<LoanApplication> LoanApplications { get; set; }
    }
}
