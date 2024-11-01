using Campus_Events.Models;
using Campus_Events.Persistence;
using MimeKit;

namespace Campus_Events.Misc
{
    public class EmailService
    {
        private ILogger<EmailService> logger;
        private IUserRepository userRepository;
        private EmailQueue emailQueue;
        PasswordHelper passwordHelper;

        public EmailService(ILogger<EmailService> logger, IUserRepository userRepository, EmailQueue emailQueue, PasswordHelper passwordHelper)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.emailQueue = emailQueue;
            this.passwordHelper = passwordHelper;
        }

        public void SendPasswortResetMail(User user)
        {
            user.PasswordResetToken = passwordHelper.GenerateToken();
            userRepository.Update(user);

            var message = new MimeMessage();
            message.To.Add(new MailboxAddress("", user.EMail));
            message.Subject = "ToDo - Reset Password";
            message.Body = new TextPart("plain")
            {
                Text = $"Reset passwort: http://localhost:5162/Authentication/ResetPassword/{user.PasswordResetToken}"
            };
            emailQueue.Enqueue(message);
        }

        public void SendRegistrationMail(User user)
        {

        }
    }
}
