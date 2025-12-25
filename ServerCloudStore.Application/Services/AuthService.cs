using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ServerCloudStore.Application.DTOs.Auth;
using ServerCloudStore.Domain.Services;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.Application.Services;

/// <summary>
/// Servicio de autenticación
/// </summary>
public class AuthService : IAuthService
{
    private readonly IAuthDomainService _authDomainService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly IMapper _mapper;

    public AuthService(
        IAuthDomainService authDomainService,
        IConfiguration configuration,
        ILogger<AuthService> logger,
        IMapper mapper)
    {
        _authDomainService = authDomainService;
        _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Autentica un usuario y retorna un token JWT
    /// </summary>
    public async Task<Response<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            // Validar credenciales usando el servicio de dominio
            var user = await _authDomainService.ValidateCredentialsAsync(request.Username, request.Password);

            if (user == null)
            {
                return Response<LoginResponseDto>.Error("Credenciales inválidas", 401);
            }

            // Generar token JWT
            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 60));

            // Mapear usuario a DTO
            var userDto = _mapper.Map<UserDto>(user);

            var response = new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = userDto
            };

            return Response<LoginResponseDto>.Success(response, "Login exitoso", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al autenticar usuario: {Username}", request.Username);
            return Response<LoginResponseDto>.Error("Error al procesar la autenticación", 500);
        }
    }

    /// <summary>
    /// Genera un token JWT para el usuario
    /// </summary>
    private string GenerateJwtToken(Domain.Entities.User user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey no configurada");
        var issuer = _configuration["JwtSettings:Issuer"] ?? "ServerCloudStoreAPI";
        var audience = _configuration["JwtSettings:Audience"] ?? "ServerCloudStoreClient";
        var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 60);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("RoleId", user.RoleId.ToString())
        };

        // Agregar nombre del rol
        if (user.Role != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
        }

        // Agregar permisos
        if (user.Role != null && !string.IsNullOrEmpty(user.Role.Permissions))
        {
            try
            {
                var permissions = JsonSerializer.Deserialize<List<string>>(user.Role.Permissions) ?? new List<string>();
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("Permission", permission));
                }
            }
            catch
            {
                // Si no se puede parsear, continuar sin permisos
            }
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

