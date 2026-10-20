
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;
using QuoteGeneration.Infrastructure.Data;

namespace QuoteGeneration.Infrastructure.Services
{
    public class RiskAssessmentService : IRiskAssessmentService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public RiskAssessmentService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<RiskAssessmentDTO> AssessRiskAsync(RiskAssessmentDTO dto)
        {
            var risk = mapper.Map<QuoteGeneration.Domain.Entities.RiskAssessment>(dto);

          
            risk.total_score =(risk.age_score ?? 0) + (risk.occupation_score ?? 0) +(risk.medical_score ?? 0) +(risk.lifestyle_score ?? 0);

            db.RiskAssessments.Add(risk);
            await db.SaveChangesAsync();

            return mapper.Map<RiskAssessmentDTO>(risk);
        }

        public async Task<RiskAssessmentDTO?> GetRiskByQuoteIdAsync(int quoteId)
        {
            var risk = await db.RiskAssessments
                .FirstOrDefaultAsync(r => r.quote_id == quoteId);

            if (risk == null)
            {
                return null;
            }

            return mapper.Map<RiskAssessmentDTO>(risk);
        }
    }
}
