using Microsoft.Extensions.Options;
using MimeKit;

namespace Campus_Events.Misc
{
    public class EmailQueue
    {
        private Queue<MimeMessage> messages = new Queue<MimeMessage>();
        private MailSettings settings;

        public EmailQueue(IOptions<MailSettings> settings)
        {
            this.settings = settings.Value;
        }

        public void Enqueue(MimeMessage message)
        {
            message.From.Add(new MailboxAddress("ToDo", settings.FromMailAddress));
            messages.Enqueue(message);
        }

        public MimeMessage Dequeue()
        {
            return messages.Dequeue();
        }

        public bool HasMessages
        {
            get { return messages.Count > 0; }
        }
    }
}
