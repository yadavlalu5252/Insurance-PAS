using System;
using System.Collections.Generic;
using System.Text;
using Underwriting.Application.DTO;
using Underwriting.Domain.Entities;

namespace Underwriting.Application.Interfaces
{
    public interface Iunderwriting
    {
        Task addunerwriting(createunderwritingdto dto);

        Task<List<underwriting_case>> fetchunderwriting();

        Task<underwriting_case?> fetchunderwritingbyid(long id);

        Task updateunderwritingbyid(long id, createunderwritingdto dto);

        Task approveunderwriting(long id);

        Task rejectunderwriting(long id, string reason);
        Task loadingunderwriting(long id);
        Task<underwriting_decision?> fetchdecision(long id);
    }
}