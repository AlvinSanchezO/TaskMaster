using BCrypt.Net;

namespace TaskMaster.CLI.Services;

public class AuthService
{
    //Tranforma la contraseña en un hash utilizando BCrypt
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    // Compara la contraseña ingresada con el hash almacenado
    public bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
