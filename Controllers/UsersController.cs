using BudgetManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManagement.Controllers {
    public class UsersController: Controller {

        public IActionResult Index() {
            return View("Register");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }
            return RedirectToAction("Index", "Transactions");
        }
    }
}
