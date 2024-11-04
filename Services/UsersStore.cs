using BudgetManagement.Interfaces;
using BudgetManagement.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace BudgetManagement.Services {
    public class UsersStore: IUserEmailStore<User>, IUserPasswordStore<User> {
        private bool _disposed = false;
        private readonly IUsersRepository usersRepository;

        public UsersStore(IUsersRepository usersRepository) {
            this.usersRepository = usersRepository;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken) {
            user.Id = await usersRepository.CreateUser(user);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    // Dispose managed resources here.
                }
                // Dispose unmanaged resources here.
                _disposed = true;
            }
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) {
            return await usersRepository.GetUserByEmail(normalizedEmail);
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            return await usersRepository.FindByUserName(normalizedUserName);
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Email);            
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken) {
            return Task.FromResult(user.PasswordHash);            
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken) {
            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken) {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken) { 
            user.Name = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken) {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken) {
            user.Name = user.UserName.ToLower();            
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken) {
            await usersRepository.Update(user);
            return IdentityResult.Success;
            
        }
    }
}
