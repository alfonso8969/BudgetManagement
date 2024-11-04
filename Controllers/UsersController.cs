using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.ClientModel.Primitives;
using System.Text;
using System.Text.RegularExpressions;

namespace BudgetManagement.Controllers {
    public class UsersController: Controller {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IEmailService emailService;

        public UsersController(UserManager<User> userManager,
                               SignInManager<User> signInManager,
                               IEmailService emailService) {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() {
            return View("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded) {
                return RedirectToAction("Index", "Transactions");
            } else {
                ModelState.AddModelError(String.Empty, "Invalid login attempt.");
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var user = new User {
                Email = model.Email,
                UserName = model.UserName
            };

            var result = await userManager.CreateAsync(user, password: model.Password);

            if (result.Succeeded) {
                await signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Transactions");
            } else {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgetMyPassword(string message = "") {
            if (!ModelState.IsValid) {
                return View(ModelState);
            }
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetMyPassword(ForgetMyPasswordViewModel model) {
            if (!ModelState.IsValid) {
                return View(ModelState);
            }

            var message = "Process completed. If the email address provided corresponds to one of our users, you will find instructions on how to recover your password in your inbox.";
            ViewBag.Message = message;
            ModelState.Clear();

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null) {
                return RedirectToAction("ForgetMyPassword", new { message = "The email informed does not exist in our database." });
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var tokenBase64 = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(token));
            var link = Url.Action("ResetPassword", "Users", new { token = tokenBase64 }, Request.Scheme);
            await emailService.SendEmailChangePassword(model.Email, link);
            return RedirectToAction("ForgetMyPassword", new { message = "An email was sent to you with instructions to reset your password." });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token) {
            if (!ModelState.IsValid) {
                return View(ModelState);
            }

            if(token is null) {
                var message = "Invalid token.";
                return RedirectToAction("ForgetMyPassword", new { message });
            }

            var model = new RecoverPasswordViewModel {
                Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token))
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(RecoverPasswordViewModel model) {
            if (!ModelState.IsValid) {
                return View(ModelState);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null) {
                return RedirectToAction("ChangedPassword", new { message = "The email informed does not exist in our database." });
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

            var result = await userManager.ResetPasswordAsync(user, token, model.Password);
            if (result.Succeeded) {
                return RedirectToAction("ChangedPassword");
            } else {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChangedPassword(string message = "") {
            if (!ModelState.IsValid) {
                return View(ModelState);
            }
            ViewBag.Message = message;
            return View("ChangedPassword");
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Users");
        }


    }
}
