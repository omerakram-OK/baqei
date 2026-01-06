using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Baqei.Domain.Entities;

namespace Baqei.Infrastructure.Mongo;

public class UserMongoRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserMongoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<User>("users");
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var all = await _collection.Find(_ => true).ToListAsync();
        return all;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var found = await _collection.Find(filter).FirstOrDefaultAsync();
        return found;
    }

    public async Task CreateAsync(User user)
    {
        await _collection.InsertOneAsync(user);
    }
}
