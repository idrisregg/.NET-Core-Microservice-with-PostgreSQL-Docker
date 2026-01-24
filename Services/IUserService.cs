using Wajeb.API.Dtos;
using Wajeb.API.Models;

namespace Wajeb.API.Services;

public interface IUserService
{
    Task<bool> UserExistsByUsernameAsync(string username);
    Task<bool> UserExistsByEmailAsync(string email);
    Task<string> HashPasswordAsync(User user, string password);
    Task<User> CreateUserAsync(CreateUser createUserDto, string hashedPassword);
    Task<UserDetails> GetUserDetailsAsync(User user);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByIdAsync(int id);
    Task<bool> VerifyPasswordAsync(User user, string password);
    Task UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);

    Task<User> GetUserByUsernameAsync(string username);
}

