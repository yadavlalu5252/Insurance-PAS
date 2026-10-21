using AutoMapper;
using InvestigationService.Application.DTOs;
using InvestigationService.Application.Interface;
using InvestigationService.Infrastructure.Data;

namespace InvestigationService.Application.Service
{
    public class InvestigationService : IInvestigationService
    {
        AppDbContext db;
        IMapper mapper;

        public InvestigationService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddInvestigation( long claimId,CreateInvestigationDto dto)
        {
            var data = mapper.Map<global::InvestigationService.Domain.Entities.Investigation>(dto);

            data.ClaimId = claimId;
            data.InvestigationStatus =  investigation.Domain.Entities.Enums.InvestigationStatus.Pending;

            await db.Investigations.AddAsync(data);
            await db.SaveChangesAsync();
        }

        public async Task SubmitInvestigation(long investigationId,SubmitInvestigationDto dto)
        {
            var data = await db.Investigations.FindAsync(investigationId);

            if (data == null)
            {
                throw new Exception("Investigation not found");
            }

            if (dto.ReportFile == null || dto.ReportFile.Length == 0)
            {
                throw new Exception("Investigation report is required");
            }

            if (Path.GetExtension(dto.ReportFile.FileName).ToLower() != ".pdf")
            {
                throw new Exception("Only PDF files are allowed");
            }

            var folder = Path.Combine(Directory.GetCurrentDirectory(),"InvestigationReports");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = Guid.NewGuid().ToString() + ".pdf";

            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ReportFile.CopyToAsync(stream);
            }

            data.ReportPath = filePath;
            data.InvestigationStatus =investigation.Domain.Entities.Enums.InvestigationStatus.Completed;

            await db.SaveChangesAsync();
        }
    }
}