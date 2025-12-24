using Microsoft.AspNetCore.Mvc;
using ServerCloudStore.Application.DTOs.Auth;
using ServerCloudStore.Application.Services;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.API.Controllers;

/// <summary>
/// Controller para autenticación
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Autentica un usuario y retorna un token JWT
    /// </summary>
    /// <param name="request">Credenciales de login</param>
    /// <returns>Token JWT y información del usuario</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(Response<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(Response<LoginResponseDto>), 401)]
    [ProducesResponseType(typeof(Response<LoginResponseDto>), 500)]
    public async Task<ActionResult<Response<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(Response<LoginResponseDto>.Error("Datos de entrada inválidos", 400));
        }

        var result = await _authService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return Ok(result);
    }
}

