using BudgetManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BudgetManagement.Controllers {
    public class UsersController: Controller {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UsersController(UserManager<User> userManager,
                               SignInManager<User> signInManager) {
            this.userManager = userManager;
            this.signInManager = signInManager;
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

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Users");
        }
    }
}
