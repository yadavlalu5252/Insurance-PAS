using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Premium.Application.DTOs;
using PremiumPayment.Application.DTO;
using PremiumPayment.Application.Interface;
using PremiumPayment.Domain.Entities;
using PremiumPayment.Infrastructure.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PremiumPayment.Infrastructure.Repositories
{
    public class PremiumPaymentService : IPremiumPaymentService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IPremiumScheduleClient premiumScheduleClient;
        private readonly IWebHostEnvironment environment;

        public PremiumPaymentService(
            AppDbContext db,
            IMapper mapper,
            IPremiumScheduleClient premiumScheduleClient,
            IWebHostEnvironment environment)
        {
            this.db = db;
            this.mapper = mapper;
            this.premiumScheduleClient = premiumScheduleClient;
            this.environment = environment;
        }


        // =========================================================
        // CREATE PAYMENT
        // =========================================================

        public async Task<PaymentResponseDto> CreatePaymentAsync(
            CreatePaymentDto dto)
        {
            // Duplicate successful payment check
            bool duplicatePayment = await db.PremiumPayments
                .AnyAsync(x =>
                    x.schedule_id == dto.schedule_id &&
                    x.payment_status == PaymentStatus.Success);

            if (duplicatePayment)
            {
                throw new InvalidOperationException(
                    "Payment has already been completed for this schedule.");
            }

            // Create payment
            var payment = mapper.Map<PremiumPayments>(dto);

            payment.payment_date = DateTime.UtcNow;
            payment.payment_status = PaymentStatus.Initiated;

            // Online payments require gateway confirmation
            if (IsOnlinePayment(dto.payment_mode))
            {
                payment.payment_status = PaymentStatus.Pending;
            }

            // Offline payments require finance verification
            if (IsOfflinePayment(dto.payment_mode))
            {
                payment.payment_status = PaymentStatus.Pending;
            }

            db.PremiumPayments.Add(payment);

            await db.SaveChangesAsync();

            return mapper.Map<PaymentResponseDto>(payment);
        }


        // =========================================================
        // GET PAYMENT BY ID
        // =========================================================

        public async Task<PaymentResponseDto?> GetPaymentByIdAsync(
            int paymentId)
        {
            var payment = await db.PremiumPayments
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

            if (payment == null)
                return null;

            return mapper.Map<PaymentResponseDto>(payment);
        }


        // =========================================================
        // GET ALL PAYMENTS
        // =========================================================

        public async Task<IEnumerable<PaymentResponseDto>>
            GetPaymentsAsync()
        {
            var payments = await db.PremiumPayments
                .OrderByDescending(x => x.payment_date)
                .ToListAsync();

            return mapper.Map<IEnumerable<PaymentResponseDto>>(
                payments);
        }


        // =========================================================
        // VERIFY PAYMENT
        // =========================================================

        public async Task<PaymentResponseDto?> VerifyPaymentAsync(
            int paymentId,
            VerifyPaymentDto dto)
        {
            var payment = await db.PremiumPayments
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

            if (payment == null)
                return null;

            if (payment.payment_status == PaymentStatus.Success)
            {
                throw new InvalidOperationException(
                    "Payment is already successful.");
            }

            if (payment.payment_status == PaymentStatus.Refunded)
            {
                throw new InvalidOperationException(
                    "Refunded payment cannot be verified.");
            }

            payment.payment_status = dto.payment_status;

            if (!string.IsNullOrWhiteSpace(
                dto.transaction_reference))
            {
                payment.transaction_reference =
                    dto.transaction_reference;
            }

            await db.SaveChangesAsync();

            // Notify PremiumSchedule only after successful payment
            if (payment.payment_status == PaymentStatus.Success)
            {
                await premiumScheduleClient
                    .UpdatePaymentStatusAsync(
                        payment.schedule_id,
                        payment.amount_paid,
                        payment.payment_id);
            }

            return mapper.Map<PaymentResponseDto>(
                payment);
        }


        // =========================================================
        // RETRY FAILED PAYMENT
        // =========================================================

        public async Task<PaymentResponseDto?> RetryPaymentAsync(
            int paymentId)
        {
            var payment = await db.PremiumPayments
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

            if (payment == null)
                return null;

            if (payment.payment_status != PaymentStatus.Failed)
            {
                throw new InvalidOperationException(
                    "Only failed payments can be retried.");
            }

            payment.payment_status =
                PaymentStatus.Pending;

            await db.SaveChangesAsync();

            return mapper.Map<PaymentResponseDto>(
                payment);
        }


        // =========================================================
        // UPDATE PAYMENT STATUS
        // =========================================================

        public async Task<PaymentResponseDto?>
            UpdatePaymentStatusAsync(
                int paymentId,
                PaymentStatusDto dto)
        {
            var payment = await db.PremiumPayments
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

            if (payment == null)
                return null;

            payment.payment_status =
                dto.payment_status;

            if (!string.IsNullOrWhiteSpace(
                dto.transaction_reference))
            {
                payment.transaction_reference =
                    dto.transaction_reference;
            }

            await db.SaveChangesAsync();

            // Notify PremiumSchedule if payment becomes successful
            if (payment.payment_status ==
                PaymentStatus.Success)
            {
                await premiumScheduleClient
                    .UpdatePaymentStatusAsync(
                        payment.schedule_id,
                        payment.amount_paid,
                        payment.payment_id);
            }

            return mapper.Map<PaymentResponseDto>(
                payment);
        }


        // =========================================================
        // PAYMENT HISTORY BY POLICY
        // =========================================================

        public async Task<IEnumerable<PaymentResponseDto>>
            GetPaymentHistoryByPolicyAsync(
                int policyId)
        {
            var payments = await db.PremiumPayments
                .Where(x => x.policy_id == policyId)
                .OrderByDescending(x => x.payment_date)
                .ToListAsync();

            return mapper.Map<IEnumerable<PaymentResponseDto>>(
                payments);
        }


        // =========================================================
        // GET / CREATE RECEIPT
        // =========================================================

        public async Task<ReceiptResponseDto?> GetReceiptAsync(
            int paymentId)
        {
            // Find payment
            var payment = await db.PremiumPayments
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

            if (payment == null)
                return null;

            // Receipt only for successful payment
            if (payment.payment_status !=
                PaymentStatus.Success)
            {
                throw new InvalidOperationException(
                    "Receipt can only be generated for successful payments.");
            }

            // Find existing receipt
            var receipt = await db.PaymentReceipts
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

            // Create receipt if it doesn't exist
            if (receipt == null)
            {
                receipt = new PaymentReceipt
                {
                    payment_id = payment.payment_id,
                    receipt_date = DateTime.UtcNow,
                    receipt_number = string.Empty
                };

                db.PaymentReceipts.Add(receipt);

                // Save first to get receipt_id
                await db.SaveChangesAsync();

                // Generate unique receipt number
                receipt.receipt_number =
                    $"REC-{DateTime.UtcNow.Year}-" +
                    $"{receipt.receipt_id:D6}";
            }

            // Generate PDF
            string filePath =
                GenerateReceiptPdf(
                    payment,
                    receipt);

            // Save PDF path
            receipt.receipt_path =
                filePath;

            await db.SaveChangesAsync();

            return mapper.Map<ReceiptResponseDto>(
                receipt);
        }


        // =========================================================
        // REGENERATE RECEIPT
        // =========================================================

        public async Task<ReceiptResponseDto?>
            RegenerateReceiptAsync(
                int paymentId)
        {
            // Find payment
            var payment = await db.PremiumPayments
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

            if (payment == null)
                return null;

            // Only successful payments can have receipts
            if (payment.payment_status !=
                PaymentStatus.Success)
            {
                throw new InvalidOperationException(
                    "Receipt can only be regenerated for successful payments.");
            }

            // Find existing receipt
            var receipt = await db.PaymentReceipts
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

            // Create receipt if it doesn't exist
            if (receipt == null)
            {
                receipt = new PaymentReceipt
                {
                    payment_id = payment.payment_id,
                    receipt_date = DateTime.UtcNow,
                    receipt_number = string.Empty
                };

                db.PaymentReceipts.Add(receipt);

                await db.SaveChangesAsync();

                receipt.receipt_number =
                    $"REC-{DateTime.UtcNow.Year}-" +
                    $"{receipt.receipt_id:D6}";
            }
            else
            {
                // Keep same receipt number
                receipt.receipt_date =
                    DateTime.UtcNow;
            }

            // Generate new PDF
            string filePath =
                GenerateReceiptPdf(
                    payment,
                    receipt);

            // Update PDF path
            receipt.receipt_path =
                filePath;

            await db.SaveChangesAsync();

            return mapper.Map<ReceiptResponseDto>(
                receipt);
        }


        // =========================================================
        // DOWNLOAD RECEIPT
        // =========================================================

        public async Task<(byte[] FileBytes, string FileName)?>
            DownloadReceiptAsync(
                int paymentId)
        {
            var receipt = await db.PaymentReceipts
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

            if (receipt == null)
                return null;

            if (string.IsNullOrWhiteSpace(
                receipt.receipt_path))
            {
                throw new InvalidOperationException(
                    "Receipt PDF has not been generated.");
            }

            if (!File.Exists(
                receipt.receipt_path))
            {
                throw new FileNotFoundException(
                    "Receipt PDF file was not found.",
                    receipt.receipt_path);
            }

            byte[] fileBytes =
                await File.ReadAllBytesAsync(
                    receipt.receipt_path);

            string fileName =
                $"{receipt.receipt_number}.pdf";

            return (
                fileBytes,
                fileName
            );
        }


        // =========================================================
        // GENERATE RECEIPT PDF
        // =========================================================

        private string GenerateReceiptPdf(
            PremiumPayments payment,
            PaymentReceipt receipt)
        {
            // Receipts folder
            string receiptsFolder =
                Path.Combine(
                    environment.ContentRootPath,
                    "Receipts");

            if (!Directory.Exists(
                receiptsFolder))
            {
                Directory.CreateDirectory(
                    receiptsFolder);
            }

            // PDF file name
            string fileName =
                $"{receipt.receipt_number}.pdf";

            string filePath =
                Path.Combine(
                    receiptsFolder,
                    fileName);

            // Generate PDF
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);

                    page.Margin(40);

                    // Header
                    page.Header()
                        .Text(
                            "PREMIUM PAYMENT RECEIPT")
                        .Bold()
                        .FontSize(20)
                        .AlignCenter();

                    // Content
                    page.Content()
                        .PaddingTop(30)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            column.Item()
                                .Text(
                                    $"Receipt Number: " +
                                    $"{receipt.receipt_number}")
                                .FontSize(12);

                            column.Item()
                                .Text(
                                    $"Receipt Date: " +
                                    $"{receipt.receipt_date:dd-MM-yyyy HH:mm}")
                                .FontSize(12);

                            column.Item()
                                .LineHorizontal(1);

                            column.Item()
                                .Text(
                                    $"Payment ID: " +
                                    $"{payment.payment_id}")
                                .FontSize(12);

                            column.Item()
                                .Text(
                                    $"Policy ID: " +
                                    $"{payment.policy_id}")
                                .FontSize(12);

                            column.Item()
                                .Text(
                                    $"Schedule ID: " +
                                    $"{payment.schedule_id}")
                                .FontSize(12);

                            column.Item()
                                .Text(
                                    $"Amount Paid: " +
                                    $"₹{payment.amount_paid:N2}")
                                .FontSize(14)
                                .Bold();

                            column.Item()
                                .Text(
                                    $"Payment Mode: " +
                                    $"{payment.payment_mode}")
                                .FontSize(12);

                            column.Item()
                                .Text(
                                    $"Transaction Reference: " +
                                    $"{payment.transaction_reference ?? "N/A"}")
                                .FontSize(12);

                            column.Item()
                                .Text(
                                    $"Payment Status: " +
                                    $"{payment.payment_status}")
                                .FontSize(12)
                                .Bold();

                            column.Item()
                                .PaddingTop(20)
                                .Text(
                                    "Payment received successfully.")
                                .FontSize(12);

                            column.Item()
                                .PaddingTop(30)
                                .Text(
                                    "Thank you for your payment.")
                                .FontSize(11);
                        });

                    // Footer
                    page.Footer()
                        .AlignCenter()
                        .Text(
                            "Premium Payment System")
                        .FontSize(10);
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }


        // =========================================================
        // ONLINE PAYMENT MODES
        // =========================================================

        private static bool IsOnlinePayment(
            PaymentMode mode)
        {
            return mode == PaymentMode.UPI ||
                   mode == PaymentMode.CreditCard ||
                   mode == PaymentMode.DebitCard ||
                   mode == PaymentMode.NetBanking ||
                   mode == PaymentMode.Wallet;
        }


        // =========================================================
        // OFFLINE PAYMENT MODES
        // =========================================================

        private static bool IsOfflinePayment(
            PaymentMode mode)
        {
            return mode == PaymentMode.Cash ||
                   mode == PaymentMode.Cheque ||
                   mode == PaymentMode.DD;
        }

        
    }
}