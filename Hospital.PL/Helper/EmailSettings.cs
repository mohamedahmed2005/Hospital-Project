using System.Net;
using System.Net.Mail;

namespace Hospital.PL.Helper
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential("youssefislam217@gmail.com", "zukzskidtkvixgml")
            };

            using var message = new MailMessage("youssefislam217@gmail.com", email.To)
            {
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = email.IsHtml
            };

            client.Send(message);
        }
    }
}
