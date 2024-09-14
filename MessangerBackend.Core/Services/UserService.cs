using System.Security.Cryptography;
using System.Text;
using MessangerBackend.Core.Interfaces;
using MessangerBackend.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MessangerBackend.Core.Services;

public class UserService : IUserService
{
    private readonly IRepository _repository;

    public UserService(IRepository repository)
    {
        _repository = repository;
    }

    public Task<User> Login(string nickname, string password)
    {
        if (nickname == null || string.IsNullOrEmpty(nickname.Trim()) || 
            password == null || string.IsNullOrEmpty(password.Trim()))
        {
            throw new ArgumentNullException();
        }
        var user = _repository.GetAll<User>()
            .SingleOrDefault(x => x.Nickname == nickname && x.Password == ComputeHash(password));

        if (user == null)
        {
            throw new InvalidOperationException("User not found or incorrect password.");
        }
        
        return Task.FromResult(user);
        
    }

    public async Task<User> Register(string nickname, string password)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            throw new ArgumentException("Nickname cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or whitespace.");
        }

        if (nickname.Length < 3) 
        {
            throw new ArgumentException("Nickname is too short.");
        }

        if (password.Length < 6) 
        {
            throw new ArgumentException("Password is too short.");
        }

        if (password.All(char.IsLetter))
        {
            throw new ArgumentException("You can`t use only letters in password.");
        }
        var user = new User()
        {
            Nickname = nickname,
            Password = ComputeHash(password),
            CreatedAt = DateTime.UtcNow,
            LastSeenOnline = DateTime.UtcNow,
        };

        return await _repository.Add(user);
    }

    public async Task AddStats(string nickname)
    {
        var stats = _repository.GetAll<Stats>().FirstOrDefault(x => x.Name == nickname);
        if (stats == null)
            await _repository.Add(new Stats() { Name = nickname, Count = 1 });
        else
        {
            stats.Count++;
            await _repository.Update(stats);
        }
    }

    public Task<User> GetUserById(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> GetUsers(int page, int size)
    {
        return _repository.GetAll<User>().Skip((page - 1) * size).Take(size).ToList();
    }
    
   

    public IEnumerable<User> SearchUsers(string nickname)
    {
        return _repository.GetAll<User>().Where(x => x.Nickname.ToLower().Contains(nickname.ToLower()));
    }

   


    private string ComputeHash(string data)
    {
        var sha = SHA256.Create();
        var salt = "t5Y_@d:d";
        
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(data + salt));

        return Convert.ToBase64String(bytes);
    }
    
    // якщо нікнейм коректний 
    // якщо нікнейм пустий 
    // якщо немає такого нікнейму
    // якщо нікнейм є частиною чиєгось нікнейму (User { Nickname = "TestDevUser" }, nickname = "Dev")
    // різні регістри (User { Nickname = "User" }, nickname = "user")
}