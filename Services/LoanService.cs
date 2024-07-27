using LoanGateway.Models;
using LoanGateway.Services;
using LoanGeteway.Common;
using LoanGeteway.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

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
                RequestId = request.RequestId
            };
        }

        public async Task<LoanApplicationResponse> SubmitApplication(string loanType, LoanApplication request)
        {
            var eligibility = new EligibilityCheck { AnnualIncome=request.AnnualIncome, Occupation=request.Occupation, TenureMonths=request.TenureMonths, Amount=request.Amount };
            var (interestRate, emi) = LoanCalculator.CalculateInterestRateAndEMI(loanType,eligibility);

            request.Emi = emi;
            request.InterestRate = interestRate;

            List<string> files = new List<string>();
            foreach (var file in request.Documents)
            {
                var (base64String, fileNAme)=await ConvertFileToBase64Async(file);
                files.Add($"{fileNAme}|{file.ContentType}|{base64String}");
            }
            request.DocumentsBase64 = files;

            // save to DB

            var collection = db.GetCollection<LoanApplication>(DbObjects.LoanApplicationCollection);
            await collection.InsertOneAsync(request);

            // return  the response
            return new LoanApplicationResponse { 
                Arn = request.Arn,
                Amount = request.Amount,
                InterestRate = interestRate,
                Status = LoanRequestStatus.Submitted,
                Remarks = "",
                TenureMonths = request.TenureMonths,
                UserId=request.Pan
            };
        }

        public async Task<LoanStatusResponse?> GetStatus(string userId, string arn)
        {
            var loanApplicationCollection = db.GetCollection<LoanApplication>(DbObjects.LoanApplicationCollection);
            var applicationFilter = Builders<LoanApplication>.Filter.Empty;
            var applicationProjection = Builders<LoanApplication>.Projection.Exclude("_id");
            var loanApplicationDocuments = await loanApplicationCollection.Find(applicationFilter).Project(applicationProjection).ToListAsync();
            var loanApplicationList = BsonSerializer.Deserialize<List<LoanApplication>>(loanApplicationDocuments.ToJson());

            if (loanApplicationList.Count > 0)
            {
                var (pending, completed) = GetSteps(loanApplicationList[0].Status);
                return new LoanStatusResponse
                {
                    Status = loanApplicationList[0].Status,
                    Remarks = loanApplicationList[0].Remarks,
                    Arn = arn,
                    UserId = userId,
                    PendingSteps = pending,
                    CompletedSteps = completed,
                };
            }

            return null;
        }

        public async Task<UserRequestHistory> GetHistory(string userId)
        {
            var eligibilityCollection = db.GetCollection<EligibilityCheck>(DbObjects.EligibilityCollection);
            var loanApplicationCollection = db.GetCollection<EligibilityCheck>(DbObjects.LoanApplicationCollection);

            var filter = Builders<EligibilityCheck>.Filter.Eq("Pan", userId);
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

        public async Task<UserRequestHistory> GetHistory()
        {
            var eligibilityCollection = db.GetCollection<EligibilityCheck>(DbObjects.EligibilityCollection);
            var loanApplicationCollection = db.GetCollection<LoanApplication>(DbObjects.LoanApplicationCollection);

            var eligibilityfilter = Builders<EligibilityCheck>.Filter.Empty;
            var projection = Builders<EligibilityCheck>.Projection.Exclude("_id");

            var eligibilityDocuments = await eligibilityCollection.Find(eligibilityfilter).Project(projection).ToListAsync();
            var eligibilityList = BsonSerializer.Deserialize<List<EligibilityCheck>>(eligibilityDocuments.ToJson());

            var applicationFilter = Builders<LoanApplication>.Filter.Empty;
            var applicationProjection = Builders<LoanApplication>.Projection.Exclude("_id");
            var loanApplicationDocuments = await loanApplicationCollection.Find(applicationFilter).Project(applicationProjection).ToListAsync();
            var loanApplicationList = BsonSerializer.Deserialize<List<LoanApplication>>(loanApplicationDocuments.ToJson());


            var result = new UserRequestHistory
            {
                EligibilityChecks = eligibilityList, // this will be fetched from database
                LoanApplications = loanApplicationList // this will be fetched from database
            };
            return result;
        }

        private async Task<(string,string)> ConvertFileToBase64Async(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                return (Convert.ToBase64String(fileBytes),file.FileName);
            }
        }

        public async Task<bool> UpdateStatus(StausUpdateRequest request)
        {
            var collection = db.GetCollection<LoanApplication>(DbObjects.LoanApplicationCollection);

            var filter = Builders<LoanApplication>.Filter.Eq(e => e.Arn, request.Arn);
            var update = Builders<LoanApplication>.Update
                .Set(e => e.Status, request.Status)
                .Set(e => e.Remarks, request.Remarks);
            var result = await collection.UpdateOneAsync(filter, update);
           
           return result.MatchedCount > 0;
        }

        public EligibilityCheck ConvertToEligibilityCheckDto(EligibilityCheckRequest eligibility, string productCode)
        {
            return new EligibilityCheck { 
                Aadhaar=eligibility.Aadhaar,
                Amount=eligibility.Amount,
                AnnualIncome=eligibility.AnnualIncome,
                Consent=eligibility.Consent,
                Email = eligibility.Email,
                FullName=eligibility.FullName,
                Mobile= eligibility.Mobile,
                Occupation=eligibility.Occupation,
                Pan=eligibility.Pan,
                ProductCode=productCode.ToUpper(),
                Status= LoanRequestStatus.Submitted,
                TenureMonths= eligibility.TenureMonths
            };
        }

        public LoanApplication ConvertToLoanApplicationDto(LoanApplicationRequest request, string productCode)
        {
            return new LoanApplication
            {
                Aadhaar = request.Aadhaar,
                Amount = request.Amount,
                AnnualIncome=request.AnnualIncome,
                Arn= $"{productCode.ToUpper().Substring(0, 1)}ARN{DateTime.Now.Ticks}", // generating unique ARN
                Consent= request.Consent,
                Email= request.Email,
                Documents= request.Documents,
                EligibilityRequestId= request.EligibilityRequestId,
                FullName= request.FullName,
                Mobile=request.Mobile,
                Occupation=request.Occupation,
                Pan=request.Pan,
                ProductCode=productCode.ToUpper(),
                TenureMonths=request.TenureMonths,
         };
        }

        public async Task<bool> ValidateProductCode(string productCode)
        {
            var list = await GetProductsList();
            var found = list.Exists(x=>x.Code == productCode.ToUpper());
            return found;
        }


        private (List<string>, List<string>) GetSteps(string currentStatus)
        {
            List<string> pending= new List<string>();
            List<string> completed = new List<string>();

            if (currentStatus == LoanRequestStatus.Submitted)
            {
                pending.AddRange(new List<string> { LoanRequestStatus.InReview, LoanRequestStatus.Approved, LoanRequestStatus.Disbursed });
            }
            if (currentStatus == LoanRequestStatus.InReview)
            {
                pending.AddRange(new List<string> { LoanRequestStatus.Approved, LoanRequestStatus.Disbursed });
                completed.AddRange(new List<string> { LoanRequestStatus.Submitted});
            }
            if (currentStatus == LoanRequestStatus.Approved)
            {
                pending.AddRange(new List<string> { LoanRequestStatus.Disbursed });
                completed.AddRange(new List<string> { LoanRequestStatus.Submitted, LoanRequestStatus.InReview });
            }
            if (currentStatus == LoanRequestStatus.Disbursed)
            {
                completed.AddRange(LoanRequestStatus.All);
            }
            if (currentStatus == LoanRequestStatus.Rejected)
            {
                completed.AddRange(new List<string> { LoanRequestStatus.Submitted, LoanRequestStatus.InReview });
            }

            return (pending, completed);
        }
    }
}
