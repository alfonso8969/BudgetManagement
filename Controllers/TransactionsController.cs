using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace BudgetManagement.Controllers {
    public class TransactionsController: Controller {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IAccountsRepository _accountsRepository;
        private readonly ICategoriesRepository categoriesRepository;
        private readonly IUsersService _usersService;

        public TransactionsController(ITransactionsRepository transactionsRepository,
                                      IUsersService usersService, 
                                      IAccountsRepository accountsRepository,
                                      ICategoriesRepository categoriesRepository) {
            _transactionsRepository = transactionsRepository;
            _accountsRepository = accountsRepository;
            this.categoriesRepository = categoriesRepository;
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> Create() {
            var userId = _usersService.GetUserId();
            var model = new TransactionCreateViewModel {
                Accounts = await GetAccounts(userId),
            };
            model.Categories = await GetCategories((int)OperationType.Income, userId);
               
            return View(model);
        }

        public async Task<IEnumerable<SelectListItem>> GetAccounts(int userId) {
            if (!ModelState.IsValid) {
                return new List<SelectListItem>();
            }
            var accounts = await _accountsRepository.Search(userId);
            return accounts.Select(a => new SelectListItem(a.Name, a.Id.ToString()));
        }

        [HttpGet]
        private async Task<IEnumerable<SelectListItem>> GetCategories(int operationType, int userId) {
            var categories = await categoriesRepository.GetForUserAndOperationType(userId, operationType);
            return categories.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> GetCategories([FromBody] int operationType) {
            if (!ModelState.IsValid) {
                return View(ModelState);
            }
            var userId = _usersService.GetUserId();
            var categories = await GetCategories(operationType, userId);
            return Ok(categories);
            
        }
    }
}
