using BudgetManagement.Models;

namespace BudgetManagement.Interfaces {
    public interface ITypesAccountRepository {
        Task Create(AccountType accountType);
        Task Delete(int id);
        Task<AccountType> GetById(int id, int userId);
        Task<IEnumerable<AccountType>> GetTypesAccount(int userId);
        Task Sort(IEnumerable<AccountType> sorterAccountTypes);
        Task<bool> TypeAccountExitsForUserId(string name, int userId);
        Task Update(AccountType accountType);
    }
}
