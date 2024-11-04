using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagement.Repositories {
    public class PersonRepository: IPersonRepository {

        private readonly string connectionString;

        public PersonRepository(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("BudgetManagementDb");
        }

        public async Task<Person> GetPersonById(int id) {
            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT * FROM Person WHERE PersonId = (SELECT PersonId FROM Users Where Id = @Id)";
            return await connection.QuerySingleOrDefaultAsync<Person>(sql, new { id });
        }

        public async Task<Address> GetAddressByPersonId(int personId) {
            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT * FROM Address WHERE AddressId = (SELECT AddressId From person where personId = @PersonId)";
            return await connection.QuerySingleOrDefaultAsync<Address>(sql, new { personId });
        }

    }
}