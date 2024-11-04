using BudgetManagement.Models;

namespace BudgetManagement.Interfaces {
    public interface IPersonRepository {
        Task<Address> GetAddressByPersonId(int personId);
        Task<Person> GetPersonById(int id);
    }
}
