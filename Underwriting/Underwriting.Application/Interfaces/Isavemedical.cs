using System;
using System.Collections.Generic;
using System.Text;
using Underwriting.Application.DTO;
using Underwriting.Domain.Entities;

namespace Underwriting.Application.Interfaces
{
    public interface Isavemedical
    {
        Task addmedical(int id, savemedicaldto dto);

        Task<medical_underwriting?> fetchmedical(int id);

        Task updatemedical(int id, savemedicaldto dto);
    }
}
