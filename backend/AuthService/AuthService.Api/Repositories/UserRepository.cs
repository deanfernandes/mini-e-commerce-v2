    using AuthService.Api.Data;
using AuthService.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthService.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email) =>
            await _context.Users.AnyAsync(u => u.Email == email);

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetUserByEmailAsync(string email) =>
            await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetUserByIdAsync(Guid userId) =>
            await _context.Users.FindAsync(userId);

        public async Task<IEnumerable<User>> GetAllUsersAsync() =>
            await _context.Users.ToListAsync();

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}