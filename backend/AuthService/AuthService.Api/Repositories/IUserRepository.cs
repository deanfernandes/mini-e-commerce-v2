using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthService.Api.Models;

namespace AuthService.Api.Repositories
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task AddUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task SaveChangesAsync();
    }
}