using LoanGateway.Services;
using LoanGeteway.Common;
using LoanGeteway.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace LoanGeteway.Services
{
    public class LoanService : ILoanService
    {
        private readonly IMongoHelper mongoHelper;
        private readonly IMongoDatabase db;
        public LoanService(IMongoHelper helper)
        {
            mongoHelper = helper;
            db= mongoHelper.GetDb();
        }
        public async Task<List<Product>> GetProductsList()
        {
            // this hardcoded for demo perpose. This will stored into database
            var products = new List<Product> {
                     new Product{Code="A0001",Type="AUTO Loan", Description="Applicable for automobile loan" },
                     new Product{Code="H0001",Type="Home Loan", Description="Applicable for home loan" },
                     new Product{Code="P0001",Type="Personal Loan", Description="Applicable for personal loan" },
                     new Product{Code="S0001",Type="Student Loan", Description="Applicable for student loan" },
                    };
            return products;
        }
        public async Task<EligibilityCheckResponse> EligibilityCheck(string loanType, EligibilityCheck request)
        {
            var (interestRate, emi) = LoanCalculator.CalculateInterestRateAndEMI(loanType,request);

            request.Emi = emi;
            request.InterestRate = interestRate;
            request.Status = "Eligible";
            request.Remarks = "Congratulations! you are eligible for the loan.";
            // save to DB
            var collection = db.GetCollection<EligibilityCheck>(DbObjects.EligibilityCollection);
            await collection.InsertOneAsync(request);
            // returning the response
            return new EligibilityCheckResponse
            {
                InterestRate = interestRate,
                Emi = emi,
                Amount = request.Amount,
                Status = request.Status,
                Remarks = request.Remarks,
                Id = (Guid)request.RequestId
            };
        }

        public async Task<LoanApplicationResponse> SubmitApplication(string loanType, LoanApplication request)
        {
            var eligibility = new EligibilityCheck { AnnualIncome=request.AnnualIncome, Occupation=request.Occupation, Amount=request.Amount, Dob=request.Dob };
            var (interestRate, emi) = LoanCalculator.CalculateInterestRateAndEMI(loanType,eligibility);

            request.Arn = $"{loanType.ToUpper().Substring(0, 1)}{DateTime.Now.Ticks}"; // generating unique ARN
            request.Emi = emi;
            request.InterestRate = interestRate;

            // save to DB


            // return  the response
            return new LoanApplicationResponse { 
                Arn = request.Arn,
                Amount = request.Amount,
                InterestRate = interestRate,
                Status =Status.Submitted,
                Remarks = "",
                TenureMonths = request.TenureMonths,
                UserId=request.Ssn
            };
        }


        public async Task<LoanStatusResponse> GetStatus(string userId, string arn)
        {
           var result = new LoanStatusResponse
           {
               Status = Status.InReview,
               Remarks = "You loan application is in review process.",
               Arn = arn,
               UserId = userId,
               PendingSteps = new List<string> { },
               CompletedSteps = new List<string> { },
           };

            return result;
        }

        public async Task<UserRequestHistory> GetHistory(string userId)
        {
            var eligibilityCollection = db.GetCollection<EligibilityCheck>(DbObjects.EligibilityCollection);
            var loanApplicationCollection = db.GetCollection<EligibilityCheck>(DbObjects.LoanApplicationCollection);

            var filter = Builders<EligibilityCheck>.Filter.Eq("SSN", userId);
            var projection = Builders<EligibilityCheck>.Projection.Exclude("_id");

            var eligibilityDocuments = await eligibilityCollection.Find(filter).Project(projection).ToListAsync();
            var eligibilityList = BsonSerializer.Deserialize<List<EligibilityCheck>>(eligibilityDocuments.ToJson());

            var loanApplicationDocuments = await loanApplicationCollection.Find(filter).Project(projection).ToListAsync();
            var loanApplicationList = BsonSerializer.Deserialize<List<LoanApplication>>(loanApplicationDocuments.ToJson());


            var result = new UserRequestHistory {
                EligibilityChecks= eligibilityList, // this will be fetched from database
                LoanApplications= loanApplicationList // this will be fetched from database
            };
            return result;
        }
    }
}
