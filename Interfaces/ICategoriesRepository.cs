using BudgetManagement.Models;

namespace BudgetManagement.Interfaces {
    public interface ICategoriesRepository {
        Task Create(Category category);
        Task Delete(int id);
        Task<IEnumerable<Category>> GetAllForUser(int userId);
        Task<Category> GetById(int id, int userId);
        Task<IEnumerable<Category>> GetForUserAndOperationType(int userId, int operationTypeId);
        Task Update(Category category);
    }
}