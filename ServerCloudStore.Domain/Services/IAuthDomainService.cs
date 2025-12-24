using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Domain.Services;

/// <summary>
/// Interfaz para el servicio de dominio de autenticación
/// </summary>
public interface IAuthDomainService
{
    /// <summary>
    /// Valida las credenciales del usuario
    /// </summary>
    Task<User?> ValidateCredentialsAsync(string username, string password);
}

