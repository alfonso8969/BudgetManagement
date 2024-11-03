using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;


namespace BudgetManagement.Repositories {
    public class AccountsRepository: IAccountsRepository {
        private readonly string _connectionString;

        public AccountsRepository(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString("BudgetManagementDb");
        }

        public async Task Create(Account account) {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var Id = await connection.QuerySingleAsync<int>(
                $@"INSERT INTO Accounts  (Name,AccountTypeId,Balance,Description)
                VALUES (@Name,@AccountTypeId,@Balance,@Description);
                SELECT SCOPE_IDENTITY();", account
               );
            //"Accounts_Insert", new { name = account.Name, accountTypeId = account.AccountTypeId }, commandType: CommandType.StoredProcedure
            account.Id = Id;
        }

        public async Task<IEnumerable<Account>> Search(int userId) {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Account>(
                @"SELECT Accounts.Id, Accounts.Name,Balance, at.Name as 'AccountType' 
                   FROM Accounts INNER JOIN AccountsType at on at.Id = Accounts.AccountTypeId
                   WHERE at.UserId = @UserId ORDER BY at.[Order]", new { userId });
        }

        public async Task Update(Account account) {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE Accounts SET Name = @Name, Balance = @Balance, AccountTypeId = @AccountTypeId, Description = @Description
                  WHERE Id = @Id", account);
        }

        public async Task<Account> GetForId(int id, int userId) {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Account>(
                @"SELECT Accounts.Id, Accounts.Name, Balance, Description, AccountTypeId
                  FROM Accounts INNER JOIN AccountsType at on at.Id = Accounts.AccountTypeId 
                  WHERE Accounts.Id = @Id AND userId = @UserId", new { id, userId });
        }

        public async Task Delete(int id) {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM Accounts WHERE Id = @Id", new { id });
        }
    }
}
