using TaskMaster.CLI.Data;
using TaskMaster.CLI.Models;

namespace TaskMaster.CLI.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;

    public UserService(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public User Register(string username, string password)
    {
        var newUser = new User
        {
            Username = username,
            PasswordHash = _authService.HashPassword(password) // Usa tu AuthService
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();
        return newUser;
    }

    public User? Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) return null;

        // Verifica usando tu AuthService
        return _authService.VerifyPassword(password, user.PasswordHash) ? user : null;
    }
}