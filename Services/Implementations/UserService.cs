using RoutinesDbService.Models;
using RoutinesDbService.Repositories;
using RoutinesDbService.Services.Interfaces;

namespace RoutinesDbService.Services.Implementations;

public class UserService : IUserService
{
    public async Task<List<User>> GetAllUsers()
    {
        var results = await _usersRepository.GetAll();
        
        return results;
    }

    public async Task<User> GetUserById(string id)
    {
        var result = await _usersRepository.GetById(id);
        
        return result;
    }

    public async Task<bool> CreateUser(User user)
    {
        var result = await _usersRepository.Create(user);
        
        return result;
    }

    public async Task<bool> UpdateUser(User user)
    {
        var result = await _usersRepository.Update(user);
        
        return result;
    }

    public async Task<bool> DeleteUser(string id)
    {
        var result = await _usersRepository.Delete(id);
        
        return result;
    }
    
#region Constructor

    private readonly UsersRepository _usersRepository;

    public UserService(UsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

#endregion
}