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

        #region Forget Password
        [HttpGet]
        public IActionResult ForgetPassword() => View();

        [HttpPost]
        public IActionResult SendResetPasswordLink(ForgetPasswordViewModel forgetPassword)
        {
            if (ModelState.IsValid)
            {
                var User = _userManager.FindByEmailAsync(forgetPassword.Email).Result;
                if (User is not null)
                {
                    var Token = _userManager.GeneratePasswordResetTokenAsync(User).Result;
                    var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { email = forgetPassword.Email, Token }, Request.Scheme);
                    var email = new Email()
                    {
                        To = forgetPassword.Email,
                        Subject = "Reset Password",
                        Body = ResetPasswordLink
                    };
                    EmailSettings.SendEmail(email);
                    return RedirectToAction("CheckYourInbox");
                }

            }
            ModelState.AddModelError(string.Empty, "Invalid Operation");
            return View(nameof(ForgetPassword), forgetPassword);
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
            TempData["Email"] = email;
            TempData["Token"] = token;
            return View();
        }
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel resetPassword)
        {
            if (!ModelState.IsValid) return View(resetPassword);
            string email = TempData["Email"]?.ToString() ?? string.Empty;
            string token = TempData["Token"]?.ToString() ?? string.Empty;
            var User = _userManager.FindByEmailAsync(email).Result;
            if (User is not null)
            {
                var result = _userManager.ResetPasswordAsync(User, token, resetPassword.Password).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return View(nameof(ResetPassword), resetPassword);
        }
        #endregion
    }
}
