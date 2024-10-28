using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;


namespace BudgetManagement.Services {
    public class TransactionsRepository: ITransactionsRepository {

        private readonly string connectionString;

        public TransactionsRepository(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("BudgetManagementDb");
        }

        public async Task Create(Transaction transaction) {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                "Transaction_Insert",
                new { transaction.UserId, transaction.TransactionDate, transaction.Amount, transaction.CategoryId, transaction.AccountId, transaction.Note },
                commandType: System.Data.CommandType.StoredProcedure);
            transaction.Id = id;
        }
    }
}
