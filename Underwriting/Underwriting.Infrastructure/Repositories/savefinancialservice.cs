
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Domain.Entities;
using Underwriting.Infrastructure.Data;

namespace Underwriting.Infrastructure.Repositories
{
    public class savefinancialservice : Isavefinancial
    {
        AppDbContext db;

        public savefinancialservice(AppDbContext db)
        {
            this.db = db;
        }


        public async Task addfinancial(long id, savefinancialdto dto)
        {
            var mdata = await db.underwriting_case.FindAsync(id);
            if (mdata != null)
            {
                var data = new financial_underwriting()
                {
                    underwriting_id = id,
                    annual_income = dto.annual_income,
                    employer_name = dto.employer_name,
                    occupation = dto.occupation,
                    existing_insurance = dto.existing_insurance,
                    loan_liability = dto.loan_liability
                };

                await db.financial_underwriting.AddAsync(data);
                await db.SaveChangesAsync();
            }
        }

        public async Task<financial_underwriting?> fetchfinancial(long id)
        {
            var data = await db.financial_underwriting.FirstOrDefaultAsync(x => x.underwriting_id == id);
            return data;
        }
        public async Task updatefinancial(long id, savefinancialdto dto)
        {
            var data = await db.financial_underwriting.FirstOrDefaultAsync(x => x.underwriting_id == id);

            if (data != null)
            {
                data.annual_income = dto.annual_income;
                data.employer_name = dto.employer_name;
                data.occupation = dto.occupation;
                data.existing_insurance = dto.existing_insurance;
                data.loan_liability = dto.loan_liability;

                await db.SaveChangesAsync();
            }
        }
    }

}
