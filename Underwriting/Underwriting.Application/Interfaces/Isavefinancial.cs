using System;
using System.Collections.Generic;
using System.Text;
using Underwriting.Application.DTO;
using Underwriting.Domain.Entities;

namespace Underwriting.Application.Interfaces
{
    public interface Isavefinancial
    {
        Task addfinancial(long id, savefinancialdto dto);

        Task<financial_underwriting?> fetchfinancial(long id);

        Task updatefinancial(long id, savefinancialdto dto);
    }
}
