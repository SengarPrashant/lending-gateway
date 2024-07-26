using LoanGateway.Models;
using LoanGeteway.Models;

namespace LoanGeteway.Services
{
    public interface ILoanService
    {
        Task<List<Product>> GetProductsList();
        Task<EligibilityCheckResponse> EligibilityCheck(string productCode, EligibilityCheck request);
        Task<LoanApplicationResponse> SubmitApplication(string productCode, LoanApplication request);
        Task<LoanStatusResponse> GetStatus(string userId, string arn);
        Task<UserRequestHistory> GetHistory(string userId);
        Task<UserRequestHistory> GetHistory();
        Task<bool> UpdateStatus(StausUpdateRequest request);
    }
}
