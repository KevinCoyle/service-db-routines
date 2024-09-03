using RoutinesDbService.Models;

namespace RoutinesDbService.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllUsers();
    Task<User> GetUserById(string id);
    Task<bool> CreateUser(User user);
    Task<bool> UpdateUser(User user);
    Task<bool> DeleteUser(string id);
}