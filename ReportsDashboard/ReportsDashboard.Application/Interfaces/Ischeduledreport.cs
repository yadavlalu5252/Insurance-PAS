
using ReportsDashboard.Application.DTO;

namespace ReportsDashboard.Application.Interfaces
{
    public interface Ischeduledreport
    {
        Task<scheduledreportdto?> addschedule(savescheduledreportdto dto);

        Task<List<scheduledreportdto>> getschedules();

        Task<bool> updateschedule(long id, savescheduledreportdto dto);

        Task<bool> deleteschedule(long id);
    }
}
