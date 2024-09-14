using System.Security.Cryptography;
using System.Text;
using MessangerBackend.Core.Interfaces;
using MessangerBackend.Core.Models;
using MessangerBackend.Core.Services;
using MessangerBackend.Tests.Fakes;

namespace MessangerBackend.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task UserService_Login_CorrectInput()
    {
        // AAA Assign, Act, Assert
        var userService = CreateUserService();
        var expectedUser = new User()
        {
            Nickname = CorrectNickname,
            Password = ComputeHash(CorrectPassword),
            
        };

        await userService.Register(CorrectNickname, CorrectPassword);
        var user = await userService.Login(CorrectNickname, CorrectPassword);
        
        Assert.Equal(expectedUser, user, new UserComparer());
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData(null)]
    public async Task UserService_Login_ThrowsExceptionWhenEmptyField(string data)
    {
        // Assign
        var service = CreateUserService();
        
        // Act
        var exceptionNicknameHandler = async () =>
        {
            await service.Login(data, CorrectPassword);
        };
        var exceptionPasswordHandler = async () =>
        {
            await service.Login(CorrectNickname, data);
        };
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(exceptionNicknameHandler);
        await Assert.ThrowsAsync<ArgumentNullException>(exceptionPasswordHandler);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("1")]
    [InlineData("@")]
    public async Task UserService_Register_ThrowsExceptionWhenIncorrectNickname(string nickname)
    {
        var service = CreateUserService();
        
        var exceptionHandler = async () =>
        {
            await service.Register(nickname, CorrectPassword);
        };

        await Assert.ThrowsAsync<ArgumentException>(exceptionHandler);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("1")]
    [InlineData("9999")]
    [InlineData("ghsfghsegfsheg")]
    public async Task UserService_Register_ThrowsExceptionWhenIncorrectPassword(string password)
    {
        var service = CreateUserService();
        
        var exceptionHandler = async () =>
        {
            await service.Register(CorrectNickname, password);
        };

        await Assert.ThrowsAsync<ArgumentException>(exceptionHandler);
    }

    private const string CorrectNickname = "TestUser2";

    private const string CorrectPassword = "rRT56TGV!_gTr2";

    private IUserService CreateUserService()
    {
        
        return new UserService(new FakeUserRepository());
    }

    private string ComputeHash(string data)
    {
        var sha = SHA256.Create();
        var salt = "t5Y_@d:d";
        
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(data + salt));

        return Convert.ToBase64String(bytes);
    }
    
    [Fact]
    public void UserService_GetUsers_ReturnsCorrectNumberOfUsers()
    {
        // Arrange
        var userService = CreateUserService();
        userService.Register("User1", "Password1").Wait();
        userService.Register("User2", "Password2").Wait();
        userService.Register("User3", "Password3").Wait();
    
        // Act
        var users = userService.GetUsers(1, 2); 

        // Assert
        Assert.Equal(2, users.Count());
    }

    [Fact]
    public void UserService_SearchUsers_ReturnsCorrectUsersIgnoringCase()
    {
        // Arrange
        var userService = CreateUserService();
        userService.Register("User1", "Password1").Wait();
        userService.Register("user2", "Password2").Wait();
        userService.Register("AnotherUser", "Password3").Wait();

        // Act
        var searchResult = userService.SearchUsers("user"); 

        // Assert
        Assert.Equal(3, searchResult.Count()); 
    }

   

}

class UserComparer : IEqualityComparer<User>
{
    public bool Equals(User x, User y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Nickname == y.Nickname && x.Password == y.Password;
    }

    public int GetHashCode(User obj)
    {
        return HashCode.Combine(obj.Nickname, obj.Password);
    }
    
   
}