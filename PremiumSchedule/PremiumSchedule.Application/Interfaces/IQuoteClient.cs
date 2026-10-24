using PremiumSchedule.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.Interfaces
{
    public interface IQuoteClient
    {
        Task<QuoteDto?> GetQuoteByIdAsync(int quoteId);
    }
}
