using BudgetManagement.Models;

namespace BudgetManagement.Interfaces {
    public interface IUsersRepository {
        Task<int> CreateUser(User user);
        Task<User> FindByUserName(string userName);
        Task<User> GetUserByEmail(string normalizedEmail);
        Task Update(User user);
    }
}