using System;
using System.Collections.Generic;
using System.Text;
using Underwriting.Application.DTO;
using Underwriting.Domain.Entities;

namespace Underwriting.Application.Interfaces
{
    public interface Isavefinancial
    {
        Task addfinancial(int id, savefinancialdto dto);

        Task<financial_underwriting?> fetchfinancial(int id);

        Task updatefinancial(int id, savefinancialdto dto);
    }
}
