﻿using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace BudgetManagement.Repositories {
    public class CategoriesRepository: ICategoriesRepository {

        private readonly string connectionString;

        public CategoriesRepository(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("BudgetManagementDb");
        }

        public async Task Create(Category category) {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categories (Name, OperationTypeId, UserId) 
            VALUES (@Name, @OperationTypeId, @UserId);
            SELECT SCOPE_IDENTITY();", category);
            category.Id = id;
        }

        public async Task<IEnumerable<Category>> GetAllForUser(int userId, PaginationViewModel pagination) {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(
                @$"SELECT * FROM Categories
                WHERE UserId = @UserId
                ORDER BY Name
                OFFSET {pagination.RecordsToSkip} ROWS
                FETCH NEXT {pagination.RecordsPerPage} ROWS ONLY", new { userId });
        }

        public async Task<int> GetTotalRecords(int userId) {
            using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(*) FROM Categories WHERE UserId = @UserId", new { userId });
        }

        public async Task<IEnumerable<Category>> GetForUserAndOperationType(int userId, int operationTypeId) {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(
                @$"SELECT * FROM Categories 
                WHERE UserId = @UserId AND OperationTypeId = @operationTypeId",
                 new { userId, operationTypeId });
        }

        public async Task<Category> GetById(int id, int userId) {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Category>(
                @"SELECT * FROM Categories WHERE Id = @Id AND userId = @UserId", new { id, userId });
        }

        public async Task Update(Category category) {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categories SET Name = @Name, OperationTypeId = @OperationTypeId WHERE Id = @Id", category);
        }

        public async Task Delete(int id) {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM Categories WHERE Id = @Id", new { id });
        }

    }
}
