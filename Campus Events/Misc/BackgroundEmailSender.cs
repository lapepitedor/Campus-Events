using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace Campus_Events.Misc
{
    public class BackgroundEmailSender: BackgroundService
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

        private bool SendMail(MimeMessage message, CancellationToken stoppingToken)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(settings.Host, settings.Port, SecureSocketOptions.Auto, stoppingToken);
                    client.Authenticate(settings.Username, settings.Password);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not send mail!");
                return false;
            }

            return true;
        }
    }
}
