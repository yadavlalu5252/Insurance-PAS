using AutoMapper;
using KycVerification.Application.Interface;
using KycVerification.Domain.Entities;
using KycVerification.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Infrastructure.Repositories
{
    public class CkycDetailsServices : ICkycDetailsServices
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public CkycDetailsServices(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        // Get CKYC details by ID
        public async Task<CkycDetails?> GetByIdAsync(int ckycId)
        {
            var data = await db.CkycDetails
                .FirstOrDefaultAsync(x => x.CkycId == ckycId);

            return data;
        }

        // Get CKYC details by customer ID
        public async Task<CkycDetails?> GetByCustomerIdAsync(int customerId)
        {
            var data = await db.CkycDetails
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);

            return data;
        }

        // Get all CKYC records
        public async Task<List<CkycDetails>> GetAllAsync()
        {
            var data = await db.CkycDetails.ToListAsync();

            return data;
        }

        // Save CKYC details
        public async Task<CkycDetails> AddAsync(CkycDetails details)
        {
            var map = mapper.Map<CkycDetails>(details);

            map.CkycId = 0;

            await db.CkycDetails.AddAsync(map);
            await db.SaveChangesAsync();

            return map;
        }

        // Update CKYC details
        public async Task UpdateAsync(CkycDetails details)
        {
            var data = await db.CkycDetails
                .FirstOrDefaultAsync(x => x.CkycId == details.CkycId);

            if (data == null)
            {
                throw new KeyNotFoundException(
                    $"CKYC record with ID {details.CkycId} was not found.");
            }

            data.CkycNumber = details.CkycNumber;
            data.VerificationDate = details.VerificationDate;
            data.CkycStatus = details.CkycStatus;

            await db.SaveChangesAsync();
        }
    }

}

