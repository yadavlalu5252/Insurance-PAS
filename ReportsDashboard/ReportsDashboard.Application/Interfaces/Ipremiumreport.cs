using ReportsDashboard.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportsDashboard.Application.Interfaces
{
    public interface Ipremiumreport
    {
        Task<List<PremiumDTO>?> getpremiums();
    }
}
