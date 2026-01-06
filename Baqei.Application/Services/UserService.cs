using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baqei.Application.DTOs;
using Baqei.Domain.Entities;
using Baqei.Domain.Interfaces;

namespace Baqei.Application.Services;

public class UserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        var dtos = users.Select(u => new UserDto(u.Id, u.Name, u.Email));
        return dtos;
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        var dto = new UserDto(user.Id, user.Name, user.Email);
        return dto;
    }

    public async Task<UserDto> CreateAsync(UserDto dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email
        };

        var created = await _repository.CreateAsync(user);

        var createdDto = new UserDto(created.Id, created.Name, created.Email);
        return createdDto;
    }
}
