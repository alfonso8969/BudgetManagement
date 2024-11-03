using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Microsoft.Data.SqlClient;
using Dapper;
namespace BudgetManagement.Repositories {
    public class UsersRepository: IUsersRepository {
        private readonly string connectionString;

        public UsersRepository(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("BudgetManagementDb");
        }

        public async Task<int> CreateUser(User user) {
            using var connection = new SqlConnection(connectionString);
            user.PersonId = await connection.QuerySingleOrDefaultAsync<int>("SELECT MAX(PersonId) as PersonId FROM Person");
            // The User Store is supposed to do this.
            // user.NormalizedEmail = user.Email.ToUpper();
            var sql = "INSERT INTO Users VALUES (@Email, @NormalizedEmail, @PasswordHash, @PersonId, @userName); SELECT SCOPE_IDENTITY();";
            user.PersonId++;
            var userId = await connection.QuerySingleAsync<int>(sql, new { user.Email, user.NormalizedEmail, user.PasswordHash, user.PersonId, user.UserName });
            await connection.ExecuteAsync("INSERT INTO Person (Email) VALUES (@Email)", new { user.Email });

            await connection.ExecuteAsync("CreateNewUserData", new { userId }, commandType: System.Data.CommandType.StoredProcedure);

            return userId;
        }

        public async Task<User> GetUserByEmail(string normalizedEmail) {
            using var connection = new SqlConnection(connectionString);
            normalizedEmail = normalizedEmail.ToLower();
            var sql = @"SELECT u.UserName, p.* FROM Users u INNER JOIN Person p ON u.PersonId = p.PersonId WHERE NormalizedEmail = @NormalizedEmail";
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { normalizedEmail });
        }
        
        public async Task<User> FindByUserName(string userName) {
            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT * FROM Users WHERE NormalizedEmail = @UserName";
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { userName });
        }
  
    }
}
