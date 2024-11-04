using BudgetManagement.Models;

namespace BudgetManagement.Interfaces {
    public interface ITypesAccountRepository {
        Task Create(AccountType accountType);
        Task Delete(int id);
        Task<AccountType> GetById(int id, int userId);
        Task<int> GetTotalRecords(int userId);
        Task<IEnumerable<AccountType>> GetTypesAccount(int userId, PaginationViewModel pagination);
        Task Sort(IEnumerable<AccountType> sorterAccountTypes);
        Task<bool> TypeAccountExitsForUserId(string name, int userId, int Id = 0);
        Task Update(AccountType accountType);
    }
}
