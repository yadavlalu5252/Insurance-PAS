
using Microsoft.AspNetCore.Mvc;
using ReportsDashboard.Application.Interfaces;
using System.Text;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ReportsDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/v1/reports/export")]
    public class reportexportcontroller : ControllerBase
    {
        private readonly Ifinancialreport service;

        public reportexportcontroller(Ifinancialreport service)
        {
            this.service = service;
        }

        [HttpGet("csv")]
        public async Task<IActionResult> exportcsv()
        {
            var data = await service.getfinancial();

            if (data == null)
            {
                return NotFound("Financial report data not found");
            }

            var csv = new StringBuilder();

            csv.AppendLine("premium_income,claim_payments,outstanding_premium,refunds,branch_revenue,monthly_profit");

            csv.AppendLine(
                $"{data.premium_income}," +
                $"{data.claim_payments}," +
                $"{data.outstanding_premium}," +
                $"{data.refunds}," +
                $"{data.branch_revenue}," +
                $"{data.monthly_profit}");

            return File(
                Encoding.UTF8.GetBytes(csv.ToString()),
                "text/csv",
                "financialreport.csv");
        }

        [HttpGet("excel")]
        public async Task<IActionResult> exportexcel()
        {
            var data = await service.getfinancial();

            if (data == null)
            {
                return NotFound("Financial report data not found");
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Financial Report");

            worksheet.Cell(1, 1).Value = "Financial Report";

            worksheet.Cell(3, 1).Value = "premium_income";
            worksheet.Cell(3, 2).Value = "claim_payments";
            worksheet.Cell(3, 3).Value = "outstanding_premium";
            worksheet.Cell(3, 4).Value = "refunds";
            worksheet.Cell(3, 5).Value = "branch_revenue";
            worksheet.Cell(3, 6).Value = "monthly_profit";

            worksheet.Cell(4, 1).Value = data.premium_income;
            worksheet.Cell(4, 2).Value = data.claim_payments;
            worksheet.Cell(4, 3).Value = data.outstanding_premium;
            worksheet.Cell(4, 4).Value = data.refunds;
            worksheet.Cell(4, 5).Value = data.branch_revenue;
            worksheet.Cell(4, 6).Value = data.monthly_profit;

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "financialreport.xlsx");
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> exportpdf()
        {
            var data = await service.getfinancial();

            if (data == null)
            {
                return NotFound("Financial report data not found");
            }

            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header()
                        .Text("Financial Report")
                        .FontSize(20)
                        .Bold();

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text(
                            $"Premium Income: {data.premium_income}");

                        column.Item().Text(
                            $"Claim Payments: {data.claim_payments}");

                        column.Item().Text(
                            $"Outstanding Premium: {data.outstanding_premium}");

                        column.Item().Text(
                            $"Refunds: {data.refunds}");

                        column.Item().Text(
                            $"Branch Revenue: {data.branch_revenue}");

                        column.Item().Text(
                            $"Monthly Profit: {data.monthly_profit}");
                    });

                    page.Footer().AlignCenter()
                        .Text("Insurance PAS - Financial Report");
                });
            }).GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                "financialreport.pdf");
        }
    }
}
