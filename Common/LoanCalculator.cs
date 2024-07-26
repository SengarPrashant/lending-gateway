using LoanGeteway.Models;

namespace LoanGeteway.Common
{
    public class LoanCalculator
    {
        public static (double interestRate, double emi) CalculateInterestRateAndEMI(string loanType, EligibilityCheck eligibility)
        {
            double interestRate = CalculateInterestRate(eligibility.AnnualIncome, eligibility.Amount, eligibility.Occupation, loanType);
            double emi = CalculateEMI(eligibility.Amount, interestRate, eligibility.TenureMonths);
            return (interestRate, emi);
        }

        private static double CalculateInterestRate(double annualIncome, double amount, string occupation, string loanType)
        {
            double baseRate = occupation == "Salaried" ? 5.0 : 6.0; // Different base rate for occupation
            double incomeFactor = 0.1 * (annualIncome / 100000); // Adjust the income factor to be proportional
            double amountFactor = 0.001 * (amount / 100000); // Adjust the amount factor to be proportional
           // int age = CalculateAge(dob);
            // double ageFactor = age < 25 ? 0.5 : (age > 60 ? 0.7 : 0.0); // Age factor adjustment
            // double ageFactor = age < 25 ? 0.5 : (age > 60 ? 0.7 : 0.0); // Age factor adjustment
            double ageFactor = 0; // Age factor adjustment
            double loanTypeFactor = GetLoanTypeFactor(loanType);

            // Calculate the interest rate ensuring it stays positive
            double interestRate = baseRate + amountFactor - incomeFactor + ageFactor + loanTypeFactor;

            // Ensure interest rate is within a reasonable range
            interestRate = Math.Max(interestRate, 1.0);
            interestRate = Math.Min(interestRate, 15.0);
            return interestRate;
        }

      
        private static int CalculateAge(DateTime dob)
        {
            int age = DateTime.Now.Year - dob.Year;
            if (DateTime.Now.DayOfYear < dob.DayOfYear)
                age--;
            return age;
        }

        private static double GetLoanTypeFactor(string loanType)
        {
            switch (loanType.ToUpper())
            {
                case "P0001":
                    return 6.5;
                case "A0001":
                    return 3.5;
                case "H0001":
                    return 2.5;
                case "S0001":
                    return -0.5;
                default:
                    return 4.0;
            }
        }

       
        private static double CalculateEMI(double amount, double annualInterestRate, int tenureMonths)
        {
            double monthlyInterestRate = annualInterestRate / 12 / 100;
            double emi = (amount * monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, tenureMonths)) /
                         (Math.Pow(1 + monthlyInterestRate, tenureMonths) - 1);
            return emi;
        }
    }
}
