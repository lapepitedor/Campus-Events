using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace Campus_Events.Misc
{
    public class BackgroundEmailSender : BackgroundService
    {
        private ILogger<BackgroundEmailSender> logger;
        private EmailQueue mailQueue;
        private MailSettings settings;

        public BackgroundEmailSender(ILogger<BackgroundEmailSender> logger, EmailQueue mailQueue, IOptions<MailSettings> settings)
        {
            this.logger = logger;
            this.mailQueue = mailQueue;
            this.settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (mailQueue.HasMessages)
                {
                    var message = mailQueue.Dequeue();
                    if (!SendMail(message, stoppingToken))
                        mailQueue.Enqueue(message);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        //public void SendEmail(string email, string subject, string messageBody)
        //{
        //    var message = new MimeMessage();
        //    message.From.Add(new MailboxAddress("FromName", "fromAddress@gmail.com"));
        //    message.To.Add(new MailboxAddress("", email));
        //    message.Subject = subject;
        //    message.Body = new TextPart("plain") { Text = messageBody };

        //    using var client = new SmtpClient();
        //    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        //    client.Authenticate("yourEmail@gmail.com", "yourGeneratedPassword");
        //    client.Send(message);
        //    client.Disconnect(true);
        //}


        private bool SendMail(MimeMessage message, CancellationToken stoppingToken)
        {
            try
            {
                using (var client = new SmtpClient() )
                {
                    Console.WriteLine("Connecting to SMTP server...");

                    client.Connect(settings.Host, settings.Port, SecureSocketOptions.SslOnConnect, stoppingToken);
                    Console.WriteLine("Connected to SMTP server.");

                    client.Authenticate(settings.Username, settings.Password);
                    Console.WriteLine("Authenticated successfully.");

                    client.Send(message);
                    Console.WriteLine("Email sent successfully to {0}.", message.To.ToString());

                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not send mail! Error: {0}", ex.Message);
                return false;
            }

            return true;
        }
    }
}
