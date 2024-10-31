using AutoMapper;
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
        private readonly IMapper mapper;
        private readonly IReportService reportService;
        private readonly IUsersService _usersService;

        public TransactionsController(ITransactionsRepository transactionsRepository,
                                      IUsersService usersService, 
                                      IAccountsRepository accountsRepository,
                                      ICategoriesRepository categoriesRepository,
                                      IMapper mapper,
                                      IReportService reportService) {
            _transactionsRepository = transactionsRepository;
            _accountsRepository = accountsRepository;
            this.categoriesRepository = categoriesRepository;
            this.mapper = mapper;
            this.reportService = reportService;
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int month, int year) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = _usersService.GetUserId();

            var model = await reportService.GetDetailTransactionReport(userId, month, year, ViewBag);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create() {
            var userId = _usersService.GetUserId();
            var model = new TransactionCreateViewModel {
                Accounts = await GetAccounts(userId),
                Categories = await GetCategories((int)OperationType.Income, userId)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreateViewModel model) {
            var userId = _usersService.GetUserId();
            if (!ModelState.IsValid) {
                model.Accounts = await GetAccounts(userId);
                model.Categories = await GetCategories((int)OperationType.Income, userId);                
                return View(model);
            }

            var account = await _accountsRepository.GetForId(model.AccountId, userId);

            if (account == null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            var category = await categoriesRepository.GetById(model.CategoryId, userId);

            if (category == null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            model.UserId = userId;

            if(model.OperationTypeId.Equals(OperationType.Spent)){
                model.Amount = -model.Amount;
            }

            await _transactionsRepository.Create(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string returnUrl = null) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = _usersService.GetUserId();
            var transaction = await _transactionsRepository.GetById(id, userId);
            if (transaction == null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            var model = mapper.Map<TransactionsUpdateViewModel>(transaction);

            if (model.OperationTypeId.Equals(OperationType.Spent)) {
                model.PreviousAmount = model.Amount * -1;
            }

            model.PreviousAccountId = transaction.AccountId;
            model.Categories = await GetCategories((int)transaction.OperationTypeId, userId);
            model.Accounts = await GetAccounts(userId);
            model.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TransactionsUpdateViewModel model) {
            var userId = _usersService.GetUserId();

            if (!ModelState.IsValid) {
                model.Accounts = await GetAccounts(userId);
                model.Categories = await GetCategories((int)model.OperationTypeId, userId);
                return View(model);
            }

            var account = await _accountsRepository.GetForId(model.AccountId, userId);

            if (account == null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            var category = await categoriesRepository.GetById(model.CategoryId, userId);

            if (category == null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            var transaction = mapper.Map<TransactionsUpdateViewModel>(model);

            model.PreviousAmount = model.Amount;

            if (model.OperationTypeId.Equals(OperationType.Spent)) {
                transaction.Amount *= -1;
            }

            await _transactionsRepository.Update(transaction, model.PreviousAmount, model.PreviousAccountId);

            if(string.IsNullOrEmpty(model.ReturnUrl)) {
                return RedirectToAction("Index");
            } else {
                return LocalRedirect(model.ReturnUrl);
            }


        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id, string returnUrl = null) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = _usersService.GetUserId();
            var transaction = await _transactionsRepository.GetById(id, userId);

            if (transaction is null) {
                return RedirectToAction("MineNotFound", "Home");
            }

            await _transactionsRepository.Delete(id);

            if (string.IsNullOrEmpty(returnUrl)) {
                return RedirectToAction("Index");
            } else {
                return LocalRedirect(returnUrl);
            }
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

        #region Reports

        public IActionResult WeeklyReport() {
            return View();
        }

        public IActionResult MonthlyReport() {
            return View();
        }

        public IActionResult ExportToExcel() {
            return View("ExcelReport");
        }

        public IActionResult ShowInCalendar() {
            return View("Calendar");
        }

        #endregion




    }
}
