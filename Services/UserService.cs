using Wajeb.API.Dtos;
using Wajeb.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wajeb.API.Data;

namespace Wajeb.API.Services;

public class UserService : IUserService
{
    private readonly WajebDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(WajebDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> UserExistsByUsernameAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> UserExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public Task<string> HashPasswordAsync(User user, string password)
    {
        return Task.FromResult(_passwordHasher.HashPassword(user, password));
    }

    public async Task<User> CreateUserAsync(CreateUser createUserDto, string hashedPassword)
    {
        var user = new User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            Password = hashedPassword
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public Task<UserDetails> GetUserDetailsAsync(User user)
    {
        var userDetails = new UserDetails
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };

        return Task.FromResult(userDetails);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user ?? throw new InvalidOperationException($"User with email {email} not found");
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user ?? throw new InvalidOperationException($"User with ID {id} not found");
    }

    public Task<bool> VerifyPasswordAsync(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        return Task.FromResult(result == PasswordVerificationResult.Success);
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}