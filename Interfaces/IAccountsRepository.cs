using BudgetManagement.Models;

namespace BudgetManagement.Interfaces {
    public interface IAccountsRepository {
        Task Create(Account account);
        Task Delete(int id);
        Task<Account> GetForId(int id, int userId);
        Task<IEnumerable<Account>> Search(int userId);
        Task Update(Account account);
    }
}
