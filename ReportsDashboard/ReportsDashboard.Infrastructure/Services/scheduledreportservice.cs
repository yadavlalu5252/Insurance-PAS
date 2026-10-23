
using Microsoft.EntityFrameworkCore;
using ReportsDashboard.Application.DTO;
using ReportsDashboard.Application.Interfaces;
using ReportsDashboard.Domain.Entities;
using ReportsDashboard.Infrastructure.Data;

namespace ReportsDashboard.Infrastructure.Services
{
    public class scheduledreportservice : Ischeduledreport
    {
        private readonly AppDbContext db;

        public scheduledreportservice(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<scheduledreportdto?> addschedule(savescheduledreportdto dto)
        {
            var report = await db.report_master.FindAsync(dto.report_id);

            if (report == null)
            {
                return null;
            }

            var schedule = new report_schedule
            {
                report_id = dto.report_id,
                schedule_type = dto.schedule_type,
                next_run_date = dto.next_run_date,
                email_to = dto.email_to,
                status = dto.status
            };

            db.report_schedule.Add(schedule);
            await db.SaveChangesAsync();

            return new scheduledreportdto
            {
                schedule_id = schedule.schedule_id,
                report_id = schedule.report_id,
                report_name = report.report_name,
                schedule_type = schedule.schedule_type,
                next_run_date = schedule.next_run_date,
                email_to = schedule.email_to,
                status = schedule.status
            };
        }

        public async Task<List<scheduledreportdto>> getschedules()
        {
            return await db.report_schedule
                .Include(s => s.report)
                .Select(s => new scheduledreportdto
                {
                    schedule_id = s.schedule_id,
                    report_id = s.report_id,
                    report_name = s.report!.report_name,
                    schedule_type = s.schedule_type,
                    next_run_date = s.next_run_date,
                    email_to = s.email_to,
                    status = s.status
                })
                .ToListAsync();
        }

        public async Task<bool> updateschedule(long id, savescheduledreportdto dto)
        {
            var schedule = await db.report_schedule.FindAsync(id);

            if (schedule == null)
            {
                return false;
            }

            var report = await db.report_master.FindAsync(dto.report_id);

            if (report == null)
            {
                return false;
            }

            schedule.report_id = dto.report_id;
            schedule.schedule_type = dto.schedule_type;
            schedule.next_run_date = dto.next_run_date;
            schedule.email_to = dto.email_to;
            schedule.status = dto.status;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> deleteschedule(long id)
        {
            var schedule = await db.report_schedule.FindAsync(id);

            if (schedule == null)
            {
                return false;
            }

            db.report_schedule.Remove(schedule);
            await db.SaveChangesAsync();

            return true;
        }
    }
}
