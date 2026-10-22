using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.Interface
{
    public interface IUpdateContact
    {
        Task<string> ProcessContactUpdateAsync(EndorsementRequestDTO request);
    }
}
