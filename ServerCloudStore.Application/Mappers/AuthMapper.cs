using System.Text.Json;
using ServerCloudStore.Application.DTOs.Auth;
using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades de autenticación a DTOs
/// </summary>
public static class AuthMapper
{
    /// <summary>
    /// Mapea una entidad User a UserDto
    /// </summary>
    public static UserDto MapToUserDto(User user)
    {
        var permissions = new List<string>();

        if (user.Role != null && !string.IsNullOrEmpty(user.Role.Permissions))
        {
            try
            {
                permissions = JsonSerializer.Deserialize<List<string>>(user.Role.Permissions) ?? new List<string>();
            }
            catch
            {
                // Si no se puede parsear, dejar lista vacía
            }
        }

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            RoleName = user.Role?.Name ?? string.Empty,
            Permissions = permissions
        };
    }
}

