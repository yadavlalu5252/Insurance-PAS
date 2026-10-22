
using Microsoft.EntityFrameworkCore;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Infrastructure.Config;
using Underwriting.Infrastructure.Data;

namespace Underwriting.Infrastructure.Repositories
{
    public class RiskScoringService : IRiskScoringService
    {
        private readonly AppDbContext db;
        private readonly CustomerRegistrationClient customerClient;

        public RiskScoringService(AppDbContext db, CustomerRegistrationClient customerClient)
        {
            this.db = db;
            this.customerClient = customerClient;
        }

        public async Task<riskscoredto?> CalculateRiskScore(int id)
        {
            var u = await db.underwriting_case.FindAsync(id);
            if (u == null) return null;

            var c = await customerClient.GetCustomerByIdAsync(u.customer_id);
            if (c == null) throw new Exception("Customer not found.");

            var m = await db.medical_underwriting.FirstOrDefaultAsync(x => x.underwriting_id == id);
            var f = await db.financial_underwriting.FirstOrDefaultAsync(x => x.underwriting_id == id);

            if (m == null || f == null) throw new Exception("Medical and financial details are required.");

            int age = DateTime.Today.Year - c.date_of_birth.Year;
            if (c.date_of_birth.Date > DateTime.Today.AddYears(-age)) age--;

            int ageScore;
            if (age < 25) ageScore = 5;
            else if (age <= 35) ageScore = 3;
            else if (age <= 45) ageScore = 7;
            else if (age <= 55) ageScore = 12;
            else if (age <= 65) ageScore = 17;
            else ageScore = 20;

            int medicalScore = 0;
            string history = (m.medical_history ?? "").ToLower();

            if (history.Contains("severe") || history.Contains("organ failure")) medicalScore = 30;
            else if (history.Contains("diabetes") || history.Contains("heart disease") || history.Contains("cancer") || history.Contains("hypertension")) medicalScore = 20;
            else if (history != "" && history != "none" && history != "no" && history != "nil") medicalScore = 10;
            else if (m.bmi < 18.5m || m.bmi >= 25m) medicalScore = 5;

            int occupationScore = 0;
            string job = (f.occupation ?? "").ToLower();

            if (job.Contains("diver") || job.Contains("explosive")) occupationScore = 20;
            else if (job.Contains("firefighter") || job.Contains("mining") || job.Contains("miner") || job.Contains("construction") || job.Contains("pilot")) occupationScore = 15;
            else if (job.Contains("driver") || job.Contains("factory") || job.Contains("electrician") || job.Contains("mechanic")) occupationScore = 10;
            else if (job.Contains("field") || job.Contains("delivery") || job.Contains("outdoor")) occupationScore = 5;

            int lifestyleScore = 0;
            if (m.smoking && m.alcohol) lifestyleScore = 15;
            else if (m.smoking) lifestyleScore = 10;
            else if (m.alcohol) lifestyleScore = 5;

            int financialScore = 0;
            if (f.annual_income <= 0 || f.loan_liability < 0) financialScore = 10;
            else
            {
                decimal ratio = f.loan_liability / f.annual_income;
                if (ratio <= 0.20m) financialScore = 0;
                else if (ratio <= 0.50m) financialScore = 3;
                else if (ratio <= 1.00m) financialScore = 7;
                else financialScore = 10;
            }

            int total = ageScore + medicalScore + occupationScore + lifestyleScore + financialScore;

            string category;
            if (total <= 30) category = "Low Risk";
            else if (total <= 60) category = "Medium Risk";
            else if (total <= 80) category = "High Risk";
            else category = "Very High Risk";

            var risk = await db.risk_score.FirstOrDefaultAsync(x => x.underwriting_id == id);
            if (risk == null)
            {
                risk = new Underwriting.Domain.Entities.risk_score();
                risk.underwriting_id = id;
                db.risk_score.Add(risk);
            }

            risk.age_score = ageScore;
            risk.medical_score = medicalScore;
            risk.occupation_score = occupationScore;
            risk.lifestyle_score = lifestyleScore;
            risk.financial_score = financialScore;
            risk.total_score = total;
            risk.risk_category = category;

            await db.SaveChangesAsync();

            return new riskscoredto
            {
                underwriting_id = id,
                age_score = ageScore,
                medical_score = medicalScore,
                occupation_score = occupationScore,
                lifestyle_score = lifestyleScore,
                financial_score = financialScore,
                total_score = total,
                risk_category = category
            };
        }
    }
}
