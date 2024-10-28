using System.Diagnostics;
using BudgetManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManagement.Controllers {
    public class HomeController: Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Privacy() {
            _logger.LogInformation("Privacy page visited");
            return View();
        }

        public IActionResult MineNotFound() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode = 500) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (statusCode == 404) {
                return View("MineNotFound");
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
