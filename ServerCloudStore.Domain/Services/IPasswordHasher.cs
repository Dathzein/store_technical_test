namespace ServerCloudStore.Domain.Services;

/// <summary>
/// Interfaz para el servicio de hash de contraseñas
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Genera un hash de la contraseña
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifica si la contraseña coincide con el hash
    /// </summary>
    bool VerifyPassword(string password, string hashedPassword);
}

