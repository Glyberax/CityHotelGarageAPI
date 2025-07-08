// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;

namespace CityHotelGarageAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Yeni kullanıcı kaydı - Bearer Token döner
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { 
            message = result.Message,
            data = new {
                accessToken = result.Data!.AccessToken,
                refreshToken = result.Data.RefreshToken,
                expiryDate = result.Data.ExpiryDate,
                user = result.Data.User,
                tokenType = "Bearer" // Bearer token olduğunu belirt
            }
        });
    }

    /// <summary>
    /// Kullanıcı girişi - Bearer Token döner (1 haftalık)
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { 
            message = result.Message,
            data = new {
                accessToken = result.Data!.AccessToken,
                refreshToken = result.Data.RefreshToken,
                expiryDate = result.Data.ExpiryDate,
                user = result.Data.User,
                tokenType = "Bearer", // Bearer token olduğunu belirt
                usage = "Authorization: Bearer " + result.Data.AccessToken.Substring(0, 20) + "..." // Nasıl kullanılacağını göster
            }
        });
    }

    /// <summary>
    /// Bearer Token yenileme
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { 
            message = result.Message,
            data = new {
                accessToken = result.Data!.AccessToken,
                refreshToken = result.Data.RefreshToken,
                expiryDate = result.Data.ExpiryDate,
                user = result.Data.User,
                tokenType = "Bearer"
            }
        });
    }

    /// <summary>
    /// Çıkış - Bearer Token iptal et
    /// </summary>
    [HttpPost("logout")]
    [Authorize] // Bearer token gerekli
    public async Task<ActionResult> Logout()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Geçersiz Bearer token" });
        }

        var result = await _authService.LogoutAsync(userId.Value);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { message = result.Message });
    }

    /// <summary>
    /// Şifre değiştirme - Bearer Token gerekli
    /// </summary>
    [HttpPost("change-password")]
    [Authorize] // Bearer token gerekli
    public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Geçersiz Bearer token" });
        }

        var result = await _authService.ChangePasswordAsync(userId.Value, changePasswordDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { message = result.Message });
    }

    /// <summary>
    /// Kullanıcı profili - Bearer Token gerekli
    /// </summary>
    [HttpGet("profile")]
    [Authorize] // Bearer token gerekli
    public async Task<ActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Geçersiz Bearer token" });
        }

        var result = await _authService.GetUserProfileAsync(userId.Value);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { 
            message = result.Message,
            data = result.Data
        });
    }

    /// <summary>
    /// Bearer Token test endpoint
    /// </summary>
    [HttpGet("test-bearer-token")]
    [Authorize] // Bearer token gerekli
    public ActionResult TestBearerToken()
    {
        var userId = GetCurrentUserId();
        var username = User.Identity?.Name;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new { 
            message = "Bearer token geçerli!",
            data = new {
                userId = userId,
                username = username,
                role = role,
                email = email,
                tokenInfo = "Bearer token başarıyla doğrulandı"
            }
        });
    }

    /// <summary>
    /// Admin only endpoint - Bearer Token + Admin role gerekli
    /// </summary>
    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")] // Bearer token + Admin role gerekli
    public ActionResult AdminOnly()
    {
        return Ok(new { 
            message = "Bu endpoint sadece Admin rolündeki kullanıcılar için!",
            data = new {
                username = User.Identity?.Name,
                role = User.FindFirst(ClaimTypes.Role)?.Value
            }
        });
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}