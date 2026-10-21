using System;
using System.Collections.Generic;
using System.Text;
using Underwriting.Application.DTO;
using Underwriting.Domain.Entities;

namespace Underwriting.Application.Interfaces
{
    public interface Isavemedical
    {
        Task addmedical(long id, savemedicaldto dto);

        Task<medical_underwriting?> fetchmedical(long id);

        Task updatemedical(long id, savemedicaldto dto);
    }
}
