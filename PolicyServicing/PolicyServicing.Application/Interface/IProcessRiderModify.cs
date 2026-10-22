using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.Interface
{
    public interface IProcessRiderModify
    {
        Task<string> ProcessRiderModificationAsync(long customerId, EndorsementRequestDTO request);
    }
}
