using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;

namespace ServerCloudStore.Domain.Services;

/// <summary>
/// Servicio de dominio para lógica de autenticación
/// </summary>
public class AuthDomainService : IAuthDomainService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthDomainService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Valida las credenciales del usuario
    /// </summary>
    public async Task<User?> ValidateCredentialsAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _userRepository.GetByUsernameAsync(username);
        
        if (user == null)
            return null;

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }
}

