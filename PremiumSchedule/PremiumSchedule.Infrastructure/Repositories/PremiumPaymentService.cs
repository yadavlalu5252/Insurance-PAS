using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Premium.Application.DTOs;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Application.Interface;
using PremiumSchedule.Domain.Entities;
using PremiumSchedule.Infrastructure.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PremiumSchedule.Infrastructure.Repositories
{
    public class PremiumPaymentService : IPremiumPaymentService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment environment;

        public PremiumPaymentService(
            AppDbContext db,
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            this.db = db;
            this.mapper = mapper;
            this.environment = environment;
        }



        // CREATE PAYMENT
        

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

            
            var payment = mapper.Map<PremiumPayments>(dto);

            payment.payment_date = DateTime.UtcNow;
            payment.payment_status = PaymentStatus.Initiated;

         
            if (IsOnlinePayment(dto.payment_mode))
            {
                payment.payment_status = PaymentStatus.Pending;
            }

          
            if (IsOfflinePayment(dto.payment_mode))
            {
                payment.payment_status = PaymentStatus.Pending;
            }

            db.PremiumPayments.Add(payment);

            await db.SaveChangesAsync();

            return mapper.Map<PaymentResponseDto>(payment);
        }


      
        // GET PAYMENT BY ID
 

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


        
        // GET ALL PAYMENTS
        

        public async Task<IEnumerable<PaymentResponseDto>>
            GetPaymentsAsync()
        {
            var payments = await db.PremiumPayments
                .OrderByDescending(x => x.payment_date)
                .ToListAsync();

            return mapper.Map<IEnumerable<PaymentResponseDto>>(
                payments);
        }


        
        // VERIFY PAYMENT
       

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

       
            if (payment.payment_status == PaymentStatus.Success)
            {
                await UpdatePremiumScheduleAfterPaymentAsync(
                    payment.schedule_id,
                    payment.amount_paid);
            }

            return mapper.Map<PaymentResponseDto>(
                payment);
        }


        
        // RETRY FAILED PAYMENT
        
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


 
        // UPDATE PAYMENT STATUS
       

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

       
            if (payment.payment_status ==
                PaymentStatus.Success)
            {
                await UpdatePremiumScheduleAfterPaymentAsync(
                    payment.schedule_id,
                    payment.amount_paid);
            }

            return mapper.Map<PaymentResponseDto>(
                payment);
        }


       
        // UPDATE PREMIUM SCHEDULE AFTER SUCCESSFUL PAYMENT
        

        private async Task UpdatePremiumScheduleAfterPaymentAsync(
            int scheduleId,
            decimal amountPaid)
        {
            var schedule = await db.PremiumSchedule
                .FirstOrDefaultAsync(x =>
                    x.schedule_id == scheduleId);

            if (schedule == null)
            {
                throw new InvalidOperationException(
                    "Premium schedule not found.");
            }

            
            decimal totalPaid = await db.PremiumPayments
                .Where(x =>
                    x.schedule_id == scheduleId &&
                    x.payment_status == PaymentStatus.Success)
                .SumAsync(x => x.amount_paid);

           
            if (totalPaid < amountPaid)
            {
                totalPaid += amountPaid;
            }

            decimal totalDue =
                schedule.premium_amount +
                schedule.late_fee;

            if (totalPaid >= totalDue)
            {
                schedule.status = "Paid";
            }
            else
            {
                schedule.status = "Partial Paid";
            }

            await db.SaveChangesAsync();
        }


        
        // PAYMENT HISTORY BY POLICY
        

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


        
        // GET / CREATE RECEIPT
       

        public async Task<ReceiptResponseDto?> GetReceiptAsync(
    int paymentId)
        {
           
            var payment = await db.PremiumPayments
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

            if (payment == null)
                return null;

            if (payment.payment_status !=
                PaymentStatus.Success)
            {
                throw new InvalidOperationException(
                    "Receipt can only be generated for successful payments.");
            }

           
            var receipt = await db.PaymentReceipts
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

        
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
                    $"REC-{DateTime.UtcNow.Year}-{receipt.receipt_id:D6}";

                
                await db.SaveChangesAsync();
            }

           
            string filePath =
                GenerateReceiptPdf(
                    payment,
                    receipt);

            
            receipt.receipt_path = filePath;

            await db.SaveChangesAsync();

            return mapper.Map<ReceiptResponseDto>(
                receipt);
        }


     

        public async Task<ReceiptResponseDto?>
            RegenerateReceiptAsync(
                int paymentId)
        {
         
            var payment = await db.PremiumPayments
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

            if (payment == null)
                return null;

  
            if (payment.payment_status !=
                PaymentStatus.Success)
            {
                throw new InvalidOperationException(
                    "Receipt can only be regenerated for successful payments.");
            }

 
            var receipt = await db.PaymentReceipts
                .FirstOrDefaultAsync(x =>
                    x.payment_id == paymentId);

        
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
              
                receipt.receipt_date =
                    DateTime.UtcNow;
            }

            
            string filePath =
                GenerateReceiptPdf(
                    payment,
                    receipt);

           
            receipt.receipt_path =
                filePath;

            await db.SaveChangesAsync();

            return mapper.Map<ReceiptResponseDto>(
                receipt);
        }


       
        // DOWNLOAD RECEIPT
      

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


        
        // GENERATE RECEIPT PDF
        

        private string GenerateReceiptPdf(
            PremiumPayments payment,
            PaymentReceipt receipt)
        {
           
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

        
            string fileName =
                $"{receipt.receipt_number}.pdf";

            string filePath =
                Path.Combine(
                    receiptsFolder,
                    fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);

                    page.Margin(40);

                    page.Header()
                        .Text(
                            "PREMIUM PAYMENT RECEIPT")
                        .Bold()
                        .FontSize(20)
                        .AlignCenter();

                    
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


        
        // ONLINE PAYMENT MODES
        

        private static bool IsOnlinePayment(
            PaymentMode mode)
        {
            return mode == PaymentMode.UPI ||
                   mode == PaymentMode.CreditCard ||
                   mode == PaymentMode.DebitCard ||
                   mode == PaymentMode.NetBanking ||
                   mode == PaymentMode.Wallet;
        }


        
        // OFFLINE PAYMENT MODES


        private static bool IsOfflinePayment(
            PaymentMode mode)
        {
            return mode == PaymentMode.Cash ||
                   mode == PaymentMode.Cheque ||
                   mode == PaymentMode.DD;
        }
    }
}