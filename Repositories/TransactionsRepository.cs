﻿using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;


namespace BudgetManagement.Repositories {
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

        public async Task<IEnumerable<Transaction>> GetByAccountId(GetTransactionByAccountViewModel model) {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaction>(
                @"SELECT t.Id, t.Amount, t.TransactionDate,ac.Name AS Account, ct.Name AS Category, ct.OperationTypeId
                  FROM Transactions t
                  INNER JOIN Categories ct ON ct.Id = t.CategoryId
                  INNER JOIN Accounts ac ON ac.Id = t.AccountId
                  WHERE t.AccountId = @AccountId AND t.UserId = @UserId AND t.TransactionDate BETWEEN @DateInit AND @DateEnd
                  ORDER BY t.TransactionDate DESC",
                model);
        }


        public async Task<IEnumerable<Transaction>> GetByUserId(ParametersGetTransactionsByUser model) {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaction>(
                @"SELECT t.Id, t.Amount, t.TransactionDate, Note, ac.Name AS Account, ct.Name AS Category, ct.OperationTypeId
                  FROM Transactions t
                  INNER JOIN Categories ct ON ct.Id = t.CategoryId
                  INNER JOIN Accounts ac ON ac.Id = t.AccountId
                  WHERE t.UserId = @UserId AND t.TransactionDate BETWEEN @DateInit AND @DateEnd
                  ORDER BY t.TransactionDate DESC",
                model);
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

        #region Reports

        public async Task<IEnumerable<ResultReportWeekly>> GetByWeek(ParametersGetTransactionsByUser model) {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultReportWeekly>(
                @"SELECT DATEDIFF(D, @dateInit, TransactionDate) / 7 +1 as week,
                    SUM(Amount) as Amount,ct.OperationTypeId
                    from Transactions
                    INNER JOIN Categories ct
                    ON ct.Id = Transactions.CategoryId
                    WHERE  Transactions.UserId = @userId AND
                    TransactionDate BETWEEN @dateInit AND @dateEnd
                    GROUP BY DATEDIFF(D, @dateInit, TransactionDate) / 7, ct.OperationTypeId",
                model);
        }

        public async Task<IEnumerable<ResultReportMonthly>> GetByMonth(int userId, int year) {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultReportMonthly>(
                @"SELECT MONTH(TransactionDate) as Month,
                SUM(Amount) as Amount, ct.OperationTypeId
                FROM Transactions
                INNER JOIN Categories ct
                ON ct.Id = Transactions.CategoryId
                WHERE Transactions.UserId = @userId AND YEAR(TransactionDate) = @year
                GROUP BY Month(TransactionDate), ct.OperationTypeId",
                new { userId, year });
        }

        #endregion
    }
}
