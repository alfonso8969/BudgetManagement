using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
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

        public async Task Update(Transaction transaction, decimal previousAmount, int previousAccountId) {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                "Transactions_Update",
                new {
                    transaction.Id,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.CategoryId,
                    transaction.AccountId,
                    transaction.Note,
                    previousAmount,
                    previousAccountId
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Delete(int id) {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                "Transaction_Delete",
                new { id },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaction> GetById(int id, int userId) {
            using var connection = new SqlConnection(connectionString);
            var transaction = await connection.QuerySingleOrDefaultAsync<Transaction>(
                @$"SELECT Transactions.*, ct.OperationTypeId 
                   FROM Transactions INNER JOIN Categories ct ON ct.Id = Transactions.CategoryId
                   WHERE Transactions.Id = @id AND Transactions.UserId = @userId",
                new { id, userId });
            return transaction;
        }
    }
}
