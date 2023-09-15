using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace VibrantInfoTask.Models
{
    public class EmailService
    {
        public async Task SendForgotPasswordEmailAsync(string recipientEmail, string resetLink)
        {
            string FromEmail = "";
            string Pwd = "";
            var smtpClient = new SmtpClient()
            {
                Port = 587,
                Credentials = new NetworkCredential(FromEmail, Pwd),
                EnableSsl = true,
                Host = "smtp.gmail.com"
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(FromEmail),
                Subject = "Password Reset",
                Body = $"Click the following link to reset your password: {resetLink}",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(recipientEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
