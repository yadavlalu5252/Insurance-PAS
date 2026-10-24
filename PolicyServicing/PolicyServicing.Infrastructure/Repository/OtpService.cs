using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PolicyServicing.Infrastructure.Repository
{
    public class OtpService
    {
        private readonly IMemoryCache cache;
        private readonly IConfiguration config;

        public OtpService(IMemoryCache cache, IConfiguration config)
        {
            this.cache = cache;
            this.config = config;
        }

        public async Task SendAsync(string newEmail)
        {
            var code = Random.Shared.Next(100000, 999999).ToString();
            cache.Set($"otp:{newEmail}", code, TimeSpan.FromMinutes(10));

            var fromEmail = config["Email:From"];
            var appPassword = config["Email:AppPassword"];

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(fromEmail, newEmail, "Your OTP", $"Your verification code is {code}." +
                $"It expires in 10 minutes");
        }


        public Task<bool> VerifyAsync(string email, string code) 
        {
            var key = $"otp:{email}";
            if(!cache.TryGetValue(key, out string stored)) return Task.FromResult(false);

            if(stored != code) return Task.FromResult(false);

            cache.Remove(key);

            return Task.FromResult(true);
        
        
        
        }
    }
}
