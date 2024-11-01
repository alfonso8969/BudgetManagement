using BudgetManagement.Interfaces;
using BudgetManagement.Models;

namespace BudgetManagement.Services {
    public class ReportService: IReportService {
        private readonly ITransactionsRepository transactionsRepository;
        private readonly HttpContext httpContext;

        public ReportService(ITransactionsRepository transactionsRepository, IHttpContextAccessor httpContextAccessor) {
            this.transactionsRepository = transactionsRepository;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<ResultReportWeekly>> GetReportWeekly(int userId, int month, int year, dynamic ViewBag) {
            (DateTime dateInit, DateTime dateEnd) = SetDatesInitAndEnd(month, year);
            var parameter = new ParametersGetTransactionsByUser {
                DateInit = dateInit,
                DateEnd = dateEnd,
                UserId = userId
            };

            SetViewBagValues(ViewBag, dateInit);
            var model = await transactionsRepository.GetByWeek(parameter);
           
            return model;
        }

        public async Task<DetailTransactionReportViewModel> GetDetailTransactionReportByAccount(int userId, int accountId, int month, int year, dynamic ViewBag) {
            (DateTime dateInit, DateTime dateEnd) = SetDatesInitAndEnd(month, year);

            var getTransactionsByAccount = new GetTransactionByAccountViewModel() {

                AccountId = accountId,
                UserId = userId,
                DateInit = dateInit,
                DateEnd = dateEnd
            };

            var transactions = await transactionsRepository.GetByAccountId(getTransactionsByAccount);
            DetailTransactionReportViewModel model = GenerateTransactionReport(dateInit, dateEnd, transactions);

            SetViewBagValues(ViewBag, dateInit);

            return model;

        }

        private void SetViewBagValues(dynamic ViewBag, DateTime dateInit) {
            ViewBag.previousMonth = dateInit.AddMonths(-1).Month;
            ViewBag.previousYear = dateInit.AddMonths(-1).Year;
            ViewBag.nextMonth = dateInit.AddMonths(1).Month;
            ViewBag.nextYear = dateInit.AddMonths(1).Year;
            ViewBag.returnUrl = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private static DetailTransactionReportViewModel GenerateTransactionReport(DateTime dateInit, DateTime dateEnd, IEnumerable<Transaction> transactions) {
            var model = new DetailTransactionReportViewModel();

            var transactionsByDate = transactions
                .OrderByDescending(x => x.TransactionDate)
                .GroupBy(x => x.TransactionDate)
                .Select(group => new DetailTransactionReportViewModel.TransactionByDate {
                    DateTransaction = group.Key,
                    Transactions = group.AsEnumerable()
                });

            model.TransactionsGrouped = transactionsByDate;
            model.DateInit = dateInit;
            model.DateEnd = dateEnd;
            return model;
        }

        public async Task<DetailTransactionReportViewModel> GetDetailTransactionReport(int userId, int month, int year, dynamic ViewBag) {
            (DateTime dateInit, DateTime dateEnd) = SetDatesInitAndEnd(month, year);

            var parameter = new ParametersGetTransactionsByUser {
                DateInit = dateInit,
                DateEnd = dateEnd,
                UserId = userId
            };

            var transactions = await transactionsRepository.GetByUserId(parameter);

            DetailTransactionReportViewModel model = GenerateTransactionReport(dateInit, dateEnd, transactions);

            SetViewBagValues(ViewBag, dateInit);

            return model;

        }

        private static (DateTime DateInit, DateTime DateEnd) SetDatesInitAndEnd(int month, int year) {

            DateTime dateInit;
            DateTime dateEnd;
            if (month <= 0 || month > 12 || year <= 1900) {
                var today = DateTime.Today;
                dateInit = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            } else {
                dateInit = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            dateEnd = dateInit.AddMonths(1).AddDays(-1);

            return (dateInit, dateEnd);
        }

    }
}
