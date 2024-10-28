using BudgetManagement.Models;

namespace BudgetManagement.Interfaces {
    public interface ITransactionsRepository {
        Task Create(Transaction transaction);
    }
}