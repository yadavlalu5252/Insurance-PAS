using System;
using System.Collections.Generic;
using System.Text;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Domain.Entities;
using Underwriting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Underwriting.Infrastructure.Repositories
{
    public class savemedicalServices : Isavemedical
    {
        AppDbContext db;

        public savemedicalServices(AppDbContext db)
        {
            this.db = db;
        }

        public async Task addmedical(long id, savemedicaldto dto)
        {
            var mdata = await db.underwriting_case.FindAsync(id);

            if (mdata != null)
            {
                var data = new medical_underwriting()
                {
                    underwriting_id = id,
                    height = dto.height,
                    weight = dto.weight,
                    bmi = dto.bmi,
                    smoking = dto.smoking,
                    alcohol = dto.alcohol,
                    medical_history = dto.medical_history,
                    medical_report_path = dto.medical_report_path
                };
                await db.medical_underwriting.AddAsync(data);
                await db.SaveChangesAsync();
            }
        }

        public async Task<medical_underwriting?> fetchmedical(long id)
        {
            var data = await db.medical_underwriting.FirstOrDefaultAsync(x => x.underwriting_id == id);
            return data;
        }

        public async Task updatemedical(long id, savemedicaldto dto)
        {
            var data = await db.medical_underwriting.FirstOrDefaultAsync(x => x.underwriting_id == id);
            if (data != null)
            { 
               data.height = dto.height;
               data.weight = dto.weight;
               data.bmi = dto.bmi;
               data.smoking = dto.smoking;
                data.alcohol= dto.alcohol;
                data.medical_history= dto.medical_history;
                data.medical_report_path= dto.medical_report_path;

            }
            await db.SaveChangesAsync();
        }
    }
}
