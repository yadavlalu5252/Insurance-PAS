using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.Interface
{
    public interface IProcessNominee
    {
        Task<string> ProcessNomineeChangeAsync(long customerId, EndorsementRequestDTO request);
    }
}
