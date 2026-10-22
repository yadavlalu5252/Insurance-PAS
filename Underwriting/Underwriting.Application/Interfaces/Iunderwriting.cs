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

        Task<underwriting_case?> fetchunderwritingbyid(int id);

        Task updateunderwritingbyid(int id, createunderwritingdto dto);

        Task approveunderwriting(int id);

        Task rejectunderwriting(int id, string reason);
        Task loadingunderwriting(int id);
        Task<underwriting_decision?> fetchdecision(int id);
        Task<underwriting_case?> fetchunderwritingbyquoteid(int quoteId);
    }
}