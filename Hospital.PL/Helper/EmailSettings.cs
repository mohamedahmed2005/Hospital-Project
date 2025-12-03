using System.Net;
using System.Net.Mail;

namespace Hospital.PL.Helper
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            var Client = new SmtpClient("smtp.gmail.com", 587);
            Client.EnableSsl = true;
            Client.Credentials = new NetworkCredential("youssefislam217@gmail.com", "zukzskidtkvixgml");
            Client.Send("youssefislam217@gmail.com", email.To, email.Subject, email.Body);
        }
    }
}
