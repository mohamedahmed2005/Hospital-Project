using System.Diagnostics;
using Hospital.PL.Models;
using Hospital.PL.Helper;
using Hospital.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.PL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() => View();

        public IActionResult About() => View();

        [HttpGet]
        public IActionResult Contact() => View(new ContactViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Email to hospital/admin (HTML styled similar to reset-password email)
            var fullName = $"{model.FirstName} {model.LastName}".Trim();

            var adminEmailBody = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"" />
    <title>New Contact Message - NovaHealth</title>
    <style>
        body {{
            margin:0;
            padding:0;
            font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;
            background-color:#f4f6fb;
        }}
        .wrap {{padding:24px 0;}}
        .card {{
            max-width:560px;
            margin:0 auto;
            background:#fff;
            border-radius:16px;
            box-shadow:0 12px 32px rgba(15,23,42,.12);
            overflow:hidden;
        }}
        .hdr {{
            background:linear-gradient(135deg,#1d4ed8,#0ea5e9);
            color:#fff;
            padding:20px 28px;
        }}
        .hdr-title {{font-size:20px;font-weight:700;margin:0 0 4px;}}
        .hdr-sub {{font-size:13px;opacity:.9;margin:0;}}
        .body {{padding:22px 28px 16px;font-size:14px;color:#374151;line-height:1.6;}}
        .label {{color:#6b7280;width:110px;vertical-align:top;font-size:13px;}}
        .val {{color:#111827;font-size:13px;}}
        .row td {{padding:3px 0;}}
        .section-title {{margin:16px 0 6px;font-weight:600;color:#111827;font-size:14px;}}
        .msg-box {{
            margin-top:6px;
            padding:10px 12px;
            border-radius:10px;
            background:#f3f4ff;
            border:1px solid #e5e7eb;
            white-space:pre-wrap;
        }}
        .ftr {{border-top:1px solid #e5e7eb;padding:10px 28px 16px;font-size:11px;color:#9ca3af;}}
    </style>
</head>
<body>
    <div class=""wrap"">
        <div class=""card"">
            <div class=""hdr"">
                <h1 class=""hdr-title"">New Contact Message</h1>
                <p class=""hdr-sub"">A new inquiry was submitted from the NovaHealth website.</p>
            </div>
            <div class=""body"">
                <div class=""section-title"">Contact Details</div>
                <table cellspacing=""0"" cellpadding=""0"">
                    <tr class=""row"">
                        <td class=""label"">Name</td>
                        <td class=""val"">{fullName}</td>
                    </tr>
                    <tr class=""row"">
                        <td class=""label"">Email</td>
                        <td class=""val"">{model.Email}</td>
                    </tr>
                    <tr class=""row"">
                        <td class=""label"">Phone</td>
                        <td class=""val"">{model.PhoneNumber}</td>
                    </tr>
                    <tr class=""row"">
                        <td class=""label"">Department</td>
                        <td class=""val"">{model.Department}</td>
                    </tr>
                    <tr class=""row"">
                        <td class=""label"">Urgent</td>
                        <td class=""val"">{(model.IsUrgent ? "Yes" : "No")}</td>
                    </tr>
                </table>

                <div class=""section-title"">Subject</div>
                <p>{model.Subject}</p>

                <div class=""section-title"">Message</div>
                <div class=""msg-box"">{model.Message}</div>
            </div>
            <div class=""ftr"">
                This message was generated automatically from the NovaHealth contact form.
            </div>
        </div>
    </div>
</body>
</html>";

            var adminEmail = new Email
            {
                To = "appointments@novahealth.com",
                Subject = $"New contact request: {model.Subject}",
                Body = adminEmailBody,
                IsHtml = true
            };

            EmailSettings.SendEmail(adminEmail);

            // Copy to the user (HTML styled confirmation)
            var userCopyBody = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"" />
    <title>Your Message to NovaHealth</title>
    <style>
        body {{
            margin:0;
            padding:0;
            font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;
            background-color:#f4f6fb;
        }}
        .wrap {{padding:24px 0;}}
        .card {{
            max-width:560px;
            margin:0 auto;
            background:#fff;
            border-radius:16px;
            box-shadow:0 12px 32px rgba(15,23,42,.12);
            overflow:hidden;
        }}
        .hdr {{
            background:linear-gradient(135deg,#1d4ed8,#0ea5e9);
            color:#fff;
            padding:20px 28px;
        }}
        .hdr-title {{font-size:20px;font-weight:700;margin:0 0 4px;}}
        .hdr-sub {{font-size:13px;opacity:.9;margin:0;}}
        .body {{padding:22px 28px 16px;font-size:14px;color:#374151;line-height:1.6;}}
        .section-title {{margin:16px 0 6px;font-weight:600;color:#111827;font-size:14px;}}
        .msg-box {{
            margin-top:6px;
            padding:10px 12px;
            border-radius:10px;
            background:#f3f4ff;
            border:1px solid #e5e7eb;
            white-space:pre-wrap;
        }}
        .ftr {{border-top:1px solid #e5e7eb;padding:10px 28px 16px;font-size:11px;color:#9ca3af;}}
    </style>
</head>
<body>
    <div class=""wrap"">
        <div class=""card"">
            <div class=""hdr"">
                <h1 class=""hdr-title"">Thank You for Contacting NovaHealth</h1>
                <p class=""hdr-sub"">We have received your message and will get back to you within 24 hours.</p>
            </div>
            <div class=""body"">
                <p>Hello {fullName},</p>
                <p>Here is a copy of the message you sent us:</p>

                <div class=""section-title"">Subject</div>
                <p>{model.Subject}</p>

                <div class=""section-title"">Message</div>
                <div class=""msg-box"">{model.Message}</div>

                <p style=""margin-top:18px;"">
                    Best regards,<br />
                    <strong>NovaHealth Team</strong>
                </p>
            </div>
            <div class=""ftr"">
                If you did not submit this request, you can safely ignore this email.
            </div>
        </div>
    </div>
</body>
</html>";

            var userCopyEmail = new Email
            {
                To = model.Email,
                Subject = $"Copy of your message to NovaHealth: {model.Subject}",
                Body = userCopyBody,
                IsHtml = true
            };

            EmailSettings.SendEmail(userCopyEmail);

            TempData["ContactSuccess"] = "Your message has been sent. A copy has been emailed to you.";

            // Clear the form after successful send
            return RedirectToAction(nameof(Contact));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
