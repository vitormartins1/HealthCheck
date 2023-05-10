using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldCities.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        public SendGridEmailSenderOptions Options { get; set; }

        public SendGridEmailSender(
                IOptions<SendGridEmailSenderOptions> options
            )
        {
            this.Options = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await Execute(Options.ApiKey, subject, htmlMessage, email);
        }

        private async Task<Response> Execute(string apiKey, string subject, string htmlMessage, string email)
        {
            SendGridClient client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Options.Sender_Email, Options.Sender_Name),
                Subject = subject,
                PlainTextContent = htmlMessage,
                HtmlContent = htmlMessage
            };

            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);
            msg.SetOpenTracking(false);
            msg.SetGoogleAnalytics(false);
            msg.SetSubscriptionTracking(false);

            return await client.SendEmailAsync(msg);
        }
    }
}
