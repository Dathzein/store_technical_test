using ServerCloudStore.Application.DTOs.Auth;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.Application.Services;

/// <summary>
/// Interfaz para el servicio de autenticación
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Autentica un usuario y retorna un token JWT
    /// </summary>
    Task<Response<LoginResponseDto>> LoginAsync(LoginRequestDto request);
}

