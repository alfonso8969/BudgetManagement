using BudgetManagement.Models;

namespace BudgetManagement.Interfaces {
    public interface IReportService {
        Task<DetailTransactionReportViewModel> GetDetailTransactionReport(int userId, int month, int year, dynamic ViewBag);
        Task<DetailTransactionReportViewModel> GetDetailTransactionReportByAccount(int userId, int accountId, int month, int year, dynamic ViewBag);
    }
}
