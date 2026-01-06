using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Baqei.Domain.Entities;
using Baqei.Domain.Interfaces;
using Baqei.Infrastructure.Data;

namespace Baqei.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = await _context.Users.ToListAsync();
        return users;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user;
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
