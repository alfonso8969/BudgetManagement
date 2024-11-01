using BudgetManagement.Models;

namespace BudgetManagement.Interfaces {
    public interface ITransactionsRepository {
        Task Create(Transaction transaction);
        Task Delete(int id);
        Task<Transaction> GetById(int id, int userId);
        Task<IEnumerable<Transaction>> GetByAccountId(GetTransactionByAccountViewModel model);
        Task Update(Transaction transaction, decimal previousAmount, int previousAccountId);
        Task<IEnumerable<Transaction>> GetByUserId(ParametersGetTransactionsByUser model);
        Task<IEnumerable<ResultReportWeekly>> GetByWeek(ParametersGetTransactionsByUser model);
        Task<IEnumerable<ResultReportMonthly>> GetByMonth(int userId, int year);
    }
}