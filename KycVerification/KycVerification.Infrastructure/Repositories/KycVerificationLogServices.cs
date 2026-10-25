using KycVerification.Application.Interface;
using KycVerification.Domain.Entities;
using KycVerification.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KycVerification.Infrastructure.Repositories
{
    public class KycVerificationLogServices : IKycVerificationLogServices
    {
        private readonly AppDbContext db;

        public KycVerificationLogServices(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<KycVerificationLog?> GetByIdAsync(int logId)
        {
            return await db.KycVerificationLogs
                .FirstOrDefaultAsync(x => x.log_id == logId);
        }

        public async Task<List<KycVerificationLog>> GetByKycIdAsync(int kycId)
        {
            return await db.KycVerificationLogs
                .Where(x => x.kyc_id == kycId)
                .OrderByDescending(x => x.action_date)
                .ToListAsync();
        }

        public async Task<List<KycVerificationLog>> GetAllAsync()
        {
            return await db.KycVerificationLogs
                .OrderByDescending(x => x.action_date)
                .ToListAsync();
        }

        public async Task<KycVerificationLog> AddAsync(KycVerificationLog log)
        {
            log.log_id = 0;
            log.action_date = DateTime.UtcNow;

            db.KycVerificationLogs.Add(log);
            await db.SaveChangesAsync();

            return log;
        }
    }
}