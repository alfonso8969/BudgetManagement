using BudgetManagement.Interfaces;
using System.Net;
using System.Net.Mail;

namespace BudgetManagement.Services {
    public class EmailService: IEmailService {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration) {
            this.configuration = configuration;
        }

        public async Task SendEmailChangePassword(string receptor, string link) {
            var email = configuration.GetValue<string>("CONFIGURATIONS_EMAIL:EMAIL");
            var password = configuration.GetValue<string>("CONFIGURATIONS_EMAIL:PASSWORD");
            var host = configuration.GetValue<string>("CONFIGURATIONS_EMAIL:HOST");
            var port = configuration.GetValue<int>("CONFIGURATIONS_EMAIL:PORT");

            var smtpClient = new SmtpClient(host, port) {
                EnableSsl = true,
                UseDefaultCredentials = false,

                Credentials = new NetworkCredential(email, password)
            };

            var transmitter = email;
            var subject = "Forgot your password?";

            var htmlContent = $@"Greetings,

This message is being sent to you because you have requested a password change. If this request was not made by you, you can ignore this message.

To change your password, click on the following link:

{link}

Attentively,
Budget Management Team";

            var emailMessage = new MailMessage(transmitter, receptor, subject, htmlContent);
            await smtpClient.SendMailAsync(emailMessage);
        }
    }
}
