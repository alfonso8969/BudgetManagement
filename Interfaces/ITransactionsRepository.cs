using BudgetManagement.Models;

namespace BudgetManagement.Interfaces {
    public interface ITransactionsRepository {
        Task Create(Transaction transaction);
        Task Delete(int id);
        Task<Transaction> GetById(int id, int userId);
        Task Update(Transaction transaction, decimal previousAmount, int previousAccountId);
    }
}