using AutoMapper;
using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
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

            if (model.OperationTypeId.Equals(OperationType.Spent)) {
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

            if (string.IsNullOrEmpty(model.ReturnUrl)) {
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

        public async Task<IActionResult> WeeklyReport(int month, int year) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = _usersService.GetUserId();

            IEnumerable<ResultReportWeekly> transactionsByWeek = await reportService.GetReportWeekly(userId, month, year, ViewBag);

            var grouped = transactionsByWeek
                .GroupBy(x => x.Week)
                .Select(x => new ResultReportWeekly {
                    Week = x.Key,
                    Incomes = x.Where(y => y.OperationTypeId == OperationType.Income)
                    .Select(y => y.Amount).FirstOrDefault(),
                    Expenses = x.Where(y => y.OperationTypeId == OperationType.Spent)
                    .Select(y => y.Amount).FirstOrDefault(),
                }).ToList();

            if (month == 0 || year == 0) {
                var today = DateTime.Today;
                month = today.Month;
                year = today.Year;
            }

            var dateRef = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthDays = Enumerable.Range(1, dateRef.AddMonths(1).AddDays(-1).Day);
            var daysSegment = monthDays.Chunk(7).ToList();

            for (int i = 0; i < daysSegment.Count; i++) {
                var week = i + 1;
                var dateInit = new DateTime(year, month, daysSegment[i].First(), 0, 0, 0, DateTimeKind.Utc);
                var dateEnd = new DateTime(year, month, daysSegment[i].Last(), 0, 0, 0, DateTimeKind.Utc);
                var groupWeek = grouped.Find(x => x.Week == week);

                if (groupWeek == null) {
                    grouped.Add(new ResultReportWeekly {
                        Week = week,
                        DateInit = dateInit,
                        DateEnd = dateEnd
                    });
                } else {
                    groupWeek.DateInit = dateInit;
                    groupWeek.DateEnd = dateEnd;
                }
            }

            grouped = grouped.OrderByDescending(x => x.Week).ToList();

            var model = new ReportWeeklyModelView {
                ResultTransactionsReportWeeklies = grouped,
                ReferenceDate = dateRef,
            };

            return View(model);
        }

        public async Task<IActionResult> MonthlyReport(int year) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = _usersService.GetUserId();

            if(year == 0) {
                year = DateTime.Today.Year;
            }

            IEnumerable<ResultReportMonthly> transactionsByMonth = await _transactionsRepository.GetByMonth(userId, year);

            var transactionsGrouped = transactionsByMonth
                .GroupBy(x => x.Month)
                .Select(x => new ResultReportMonthly {
                    Month = x.Key,
                    Incomes = x.Where(y => y.OperationTypeId == OperationType.Income)
                    .Select(y => y.Amount).FirstOrDefault(),
                    Expenses = x.Where(y => y.OperationTypeId == OperationType.Spent)
                    .Select(y => y.Amount).FirstOrDefault(),
                }).ToList();


            for (int month = 1; month <= 12; month++) {
                var transaction = transactionsGrouped.Find(x => x.Month == month);
                var referenceDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
                if (transaction == null) {
                    transactionsGrouped.Add(new ResultReportMonthly {
                        Month = month,
                        ReferenceDate = referenceDate
                    });
                } else {
                    transaction.ReferenceDate = referenceDate;
                }
            }

            transactionsGrouped = transactionsGrouped.OrderByDescending(x => x.Month).ToList();

            var model = new ReportMonthlyViewModel {
                ResultTransactionsReportMonthlies = transactionsGrouped,
                Year = year
            };



            return View(model);
        }

        public IActionResult ExportToExcel() {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            return View("ExcelReport");
        }

        [HttpGet]
        public async Task<FileResult> ExportToExcelFile(int month, int year) {
            if (!ModelState.IsValid) {
                return null;
            }

            var dateInit = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var dateEnd = dateInit.AddMonths(1).AddDays(-1);

            var userId = _usersService.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(new ParametersGetTransactionsByUser {
                UserId = userId,
                DateInit = dateInit,
                DateEnd = dateEnd
            });

            var fileName = $"Transactions_{dateInit:MMM yyyy}.xlsx";            

            return GenerateExcel(fileName, transactions);
        }

        [HttpGet]
        public async Task<FileResult> ExportToExcelFileByYear(int year) {
            if (!ModelState.IsValid) {
                return null;
            }

            var dateInit = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var dateEnd = dateInit.AddYears(1).AddDays(-1);

            var userId = _usersService.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(new ParametersGetTransactionsByUser {
                UserId = userId,
                DateInit = dateInit,
                DateEnd = dateEnd
            });

            var fileName = $"Transactions_{dateInit:yyyy}.xlsx";

            return GenerateExcel(fileName, transactions);
        }

        [HttpGet]
        public async Task<FileResult> ExportToExcelFileAll() {
            if (!ModelState.IsValid) {
                return null;
            }

            var userId = _usersService.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(new ParametersGetTransactionsByUser {
                UserId = userId,
                DateInit = DateTime.Today.AddYears(-100),
                DateEnd = DateTime.Today.AddYears(1000)
            });

            var fileName = $"Transactions_{DateTime.Today:dd-MM-yyyy}.xlsx";

            return GenerateExcel(fileName, transactions);
        }

        private FileResult GenerateExcel(String fileName, IEnumerable<Transaction> transactions) {
            DataTable dt = new DataTable("Transactions");
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("Date", typeof(string)),
                new DataColumn("Account", typeof(string)),
                new DataColumn("Category", typeof(string)),
                new DataColumn("Note", typeof(string)),
                new DataColumn("Amount", typeof(decimal)),
                new DataColumn("Incomes/Expenses", typeof(decimal)),
            });
            foreach (var transaction in transactions) {
                dt.Rows.Add(
                    transaction.TransactionDate.ToString("dd/MM/yyyy"),
                    transaction.Account,
                    transaction.Category,
                    transaction.Note,
                    transaction.Amount,
                    transaction.OperationTypeId == OperationType.Income ? transaction.Amount : transaction.Amount * -1
                );
            }

            using XLWorkbook wb = new();
            wb.Worksheets.Add(dt);
            using MemoryStream stream = new();
            wb.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        public IActionResult ShowInCalendar() {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            return View("Calendar");
        }

        #endregion




    }
}
