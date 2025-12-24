namespace ServerCloudStore.Application.DTOs.Auth;

/// <summary>
/// DTO para solicitud de login
/// </summary>
public class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

