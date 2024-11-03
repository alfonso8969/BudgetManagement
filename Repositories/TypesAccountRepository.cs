using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BudgetManagement.Repositories {

    public class TypesAccountRepository: ITypesAccountRepository {
        private readonly string _connectionString;

        public TypesAccountRepository(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString("BudgetManagementDb");
        }

        public async Task Create(AccountType accountType) {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var Id = await connection.QuerySingleAsync<int>(
                "AccountsType_Insert", new { userId = accountType.UserId, name = accountType.Name }, commandType: CommandType.StoredProcedure);

            accountType.Id = Id;
        }

        public async Task<bool> TypeAccountExitsForUserId(string name, int userId) {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryFirstOrDefaultAsync<int>(
                $@"SELECT COUNT(1) FROM AccountsType WHERE Name = @Name AND UserId = @UserId", new { Name = name, UserId = userId });
            return result > 0;
        }

        public async Task<IEnumerable<AccountType>> GetTypesAccount(int userId) {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryAsync<AccountType>(
                $@"SELECT DISTINCT Id, Name, [Order] FROM AccountsType WHERE UserId = @UserId ORDER BY [Order]", new { UserId = userId });
        }

        public async Task Update(AccountType accountType) {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                $@"UPDATE AccountsType SET Name = @Name WHERE Id = @Id", accountType);
        }

        public async Task<AccountType> GetById(int id, int userId) {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<AccountType>(
                $@"SELECT Id, Name, [Order] FROM AccountsType WHERE Id = @Id AND UserId = @UserId", new { Id = id, UserId = userId });
        }

        public async Task Delete(int id) {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                $@"DELETE FROM AccountsType WHERE Id = @Id", new { Id = id });
        }

        public async Task Sort(IEnumerable<AccountType> sorterAccountTypes) {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                $@"UPDATE AccountsType SET [Order] = @Order WHERE Id = @Id", sorterAccountTypes);
        }
    }
}
