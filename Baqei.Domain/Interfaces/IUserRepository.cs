using System.Collections.Generic;
using System.Threading.Tasks;
using Baqei.Domain.Entities;

namespace Baqei.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(User user);
}
