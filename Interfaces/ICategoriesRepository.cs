using BudgetManagement.Models;

namespace BudgetManagement.Interfaces {
    public interface ICategoriesRepository {
        Task Create(Category category);
        Task Delete(int id);
        Task<IEnumerable<Category>> GetAllForUser(int userId, PaginationViewModel pagination);
        Task<Category> GetById(int id, int userId);
        Task<IEnumerable<Category>> GetForUserAndOperationType(int userId, int operationTypeId);
        Task<int> GetTotalRecords(int userId);
        Task Update(Category category);
    }
}