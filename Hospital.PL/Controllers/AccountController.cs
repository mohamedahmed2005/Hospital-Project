using Hospital.DAL.Models.Shared;
using Hospital.PL.Helper;
using Hospital.PL.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hospital.PL.Controllers
{
    public class AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, RoleManager<IdentityRole> _roleManager) : Controller
    {
        #region Register
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerView)
        {
            if (!ModelState.IsValid) return View(registerView);

            // Validate UserType
            if (string.IsNullOrWhiteSpace(registerView.UserType) || 
                (registerView.UserType != "Doctor" && registerView.UserType != "Patient"))
            {
                ModelState.AddModelError(nameof(registerView.UserType), "Please select either Doctor or Patient");
                return View(registerView);
            }

            var normalizedEmail = registerView.Email?.Trim() ?? string.Empty;
            var User = new ApplicationUser()
            {
                FirstName = registerView.FirstName,
                LastName = registerView.LastName,
                Email = normalizedEmail,
                UserName = normalizedEmail
            };
            var Result = await _userManager.CreateAsync(User, registerView.Password);
            if (Result.Succeeded)
            {
                // Ensure roles exist
                if (!await _roleManager.RoleExistsAsync("Admin"))
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!await _roleManager.RoleExistsAsync("Doctor"))
                    await _roleManager.CreateAsync(new IdentityRole("Doctor"));
                if (!await _roleManager.RoleExistsAsync("Patient"))
                    await _roleManager.CreateAsync(new IdentityRole("Patient"));

                // Assign role based on UserType
                await _userManager.AddToRoleAsync(User, registerView.UserType);
                
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in Result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerView);
            }
            // P@$$0rd ===> أى حد يجرب الباسورد ديه هتكون اسهل عليكوا
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginView, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(loginView);
            var user = await _userManager.FindByEmailAsync(loginView.Email);
            if (user is not null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginView.Password, loginView.RememberMe, false);
                if (result.Succeeded)
                {
                    // If there's a return URL, redirect there first
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Redirect to appropriate dashboard based on user role
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                        return RedirectToAction("Admin", "Dashboard");
                    else if (roles.Contains("Doctor"))
                        return RedirectToAction("Doctor", "Dashboard");
                    else if (roles.Contains("Patient"))
                        return RedirectToAction("Patient", "Dashboard");
                    else
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

                }

            }
            return View(loginView);
        }
        #endregion

        #region Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion

        #region Change Password (for logged-in users)
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction(nameof(Login));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["PasswordResetSuccess"] = "Your password has been updated successfully.";

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                    return RedirectToAction("Admin", "Dashboard");
                else if (roles.Contains("Doctor"))
                    return RedirectToAction("Doctor", "Dashboard");
                else if (roles.Contains("Patient"))
                    return RedirectToAction("Patient", "Dashboard");

                // Fallback if no specific role is found
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        #endregion

        #region Forget Password
        [HttpGet]
        public IActionResult ForgetPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendResetPasswordLink(ForgetPasswordViewModel forgetPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(ForgetPassword), forgetPassword);
            }

            var user = await _userManager.FindByEmailAsync(forgetPassword.Email);
            if (user is not null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordLink = Url.Action("ResetPassword", "Account", new { email = forgetPassword.Email, token }, Request.Scheme);
                var appBaseUrl = $"{Request.Scheme}://{Request.Host}";
                // Publicly hosted hospital icon so it loads correctly in email clients
                var logoUrl = "https://img.icons8.com/color/96/hospital-3.png";

                var htmlBody = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Reset Your Password - NovaHealth</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
            background-color: #f4f6fb;
            color: #1f2933;
        }}
        .wrapper {{
            width: 100%;
            background-color: #f4f6fb;
            padding: 24px 0;
        }}
        .email-container {{
            max-width: 560px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 16px;
            box-shadow: 0 15px 40px rgba(15, 23, 42, 0.08);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #1d4ed8, #0ea5e9);
            padding: 24px 32px;
            display: flex;
            align-items: center;
            gap: 12px;
            color: #ffffff;
        }}
        .header-logo {{
            width: 40px;
            height: 40px;
            border-radius: 12px;
            background-color: rgba(15,23,42,0.35);
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
        }}
        .header-title {{
            font-size: 20px;
            font-weight: 700;
            letter-spacing: 0.02em;
        }}
        .header-subtitle {{
            font-size: 12px;
            opacity: 0.85;
        }}
        .content {{
            padding: 28px 32px 8px 32px;
        }}
        .title {{
            font-size: 20px;
            font-weight: 600;
            color: #111827;
            margin: 0 0 8px 0;
        }}
        .pill {{
            display: inline-block;
            padding: 4px 10px;
            border-radius: 999px;
            font-size: 11px;
            letter-spacing: 0.08em;
            text-transform: uppercase;
            background-color: #e0f2fe;
            color: #0369a1;
            margin-bottom: 8px;
        }}
        .text {{
            font-size: 14px;
            line-height: 1.7;
            color: #4b5563;
            margin: 0 0 16px 0;
        }}
        .button-wrapper {{
            text-align: center;
            margin: 24px 0 20px 0;
        }}
        .btn-primary {{
            display: inline-block;
            padding: 12px 32px;
            border-radius: 999px;
            background: linear-gradient(135deg, #1d4ed8, #0ea5e9);
            color: #ffffff !important;
            font-size: 14px;
            font-weight: 600;
            text-decoration: none;
            letter-spacing: 0.04em;
            text-transform: uppercase;
            box-shadow: 0 10px 25px rgba(37, 99, 235, 0.35);
        }}
        .btn-primary:hover {{
            filter: brightness(1.03);
        }}
        .link-hint {{
            font-size: 12px;
            color: #6b7280;
            text-align: center;
            margin-bottom: 20px;
        }}
        .link-hint a {{
            color: #2563eb;
            word-break: break-all;
        }}
        .separator {{
            border-top: 1px solid #e5e7eb;
            margin: 0 32px;
        }}
        .footer {{
            padding: 16px 32px 22px 32px;
            font-size: 11px;
            color: #9ca3af;
        }}
        .footer strong {{
            color: #6b7280;
        }}
        .meta {{
            margin-top: 6px;
        }}
        .meta span {{
            display: inline-block;
            margin-right: 12px;
        }}
        @media (max-width: 600px) {{
            .email-container {{
                border-radius: 0;
            }}
            .header, .content, .footer {{
                padding-left: 20px;
                padding-right: 20px;
            }}
            .separator {{
                margin-left: 20px;
                margin-right: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""wrapper"">
        <div class=""email-container"">
            <div class=""header"">
                <div class=""header-logo"">
                    <img src=""{logoUrl}"" alt=""NovaHealth Icon"" style=""width:100%;height:100%;object-fit:contain;border-radius:10px;display:block;"" />
                </div>
                <div>
                    <div class=""header-title"">NovaHealth</div>
                    <div class=""header-subtitle"">Secure Password Reset</div>
                </div>
            </div>
            <div class=""content"">
                <div class=""pill"">Password Reset Request</div>
                <h1 class=""title"">Reset your password</h1>
                <p class=""text"">
                    We received a request to reset the password for your NovaHealth account associated with
                    <strong>{forgetPassword.Email}</strong>.
                </p>
                <p class=""text"">
                    If this was you, please click the button below to choose a new password. This link is unique to you
                    and will only be valid for a limited time.
                </p>
                <div class=""button-wrapper"">
                    <a class=""btn-primary"" style=""color:#ffffff !important;"" href=""{resetPasswordLink}"">Reset Password</a>
                </div>
                <p class=""link-hint"">
                    If the button doesn&apos;t work, copy and paste this link into your browser:<br />
                    <a href=""{resetPasswordLink}"">{resetPasswordLink}</a>
                </p>
                <p class=""text"">
                    If you did <strong>not</strong> request a password reset, you can safely ignore this email. Your
                    password will remain unchanged.
                </p>
            </div>
            <div class=""separator""></div>
            <div class=""footer"">
                <strong>NovaHealth Hospital Management System</strong>
                <div class=""meta"">
                    <span>🌐 {appBaseUrl}</span>
                    <span>🔒 Please do not share this link with anyone.</span>
                </div>
            </div>
        </div>
    </div>
</body>
</html>";

                var email = new Email
                {
                    To = forgetPassword.Email,
                    Subject = "Reset Password",
                    Body = htmlBody,
                    IsHtml = true
                };

                EmailSettings.SendEmail(email);
                return RedirectToAction(nameof(CheckYourInbox));
            }

            // To avoid revealing whether the email exists, we still redirect to CheckYourInbox
            return RedirectToAction(nameof(CheckYourInbox));
        }
        #endregion

        #region Check Your Inbox
        [HttpGet]
        public IActionResult CheckYourInbox() => View();
        #endregion

        #region Reset Password
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Invalid password reset link.");
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPassword);
            }

            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid password reset request.");
                return View(resetPassword);
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
            if (result.Succeeded)
            {
                // Automatically sign in the user with the new password
                var signInResult = await _signInManager.PasswordSignInAsync(user, resetPassword.Password, false, false);
                if (signInResult.Succeeded)
                {
                    TempData["PasswordResetSuccess"] = "Your password has been updated successfully.";

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                        return RedirectToAction("Admin", "Dashboard");
                    else if (roles.Contains("Doctor"))
                        return RedirectToAction("Doctor", "Dashboard");
                    else if (roles.Contains("Patient"))
                        return RedirectToAction("Patient", "Dashboard");
                }

                // Fallback: go home if we couldn't determine a specific dashboard
                TempData["PasswordResetSuccess"] = "Your password has been updated successfully.";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(resetPassword);
        }
        #endregion
    }
}
