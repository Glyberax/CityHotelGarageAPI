// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;
using FluentValidation; // ← BU USING'İ EKLE

namespace CityHotelGarageAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterDto> _registerValidator;  // ← BU PROPERTY'LERİ EKLE
    private readonly IValidator<LoginDto> _loginValidator;        // ← BU PROPERTY'LERİ EKLE

    public AuthController(
        IAuthService authService,
        IValidator<RegisterDto> registerValidator,  // ← CONSTRUCTOR'A EKLE
        IValidator<LoginDto> loginValidator)        // ← CONSTRUCTOR'A EKLE
    {
        _authService = authService;
        _registerValidator = registerValidator;     // ← ASSIGN ET
        _loginValidator = loginValidator;           // ← ASSIGN ET
    }

    /// <summary>
    /// Yeni kullanıcı kaydı - Bearer Token döner
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto registerDto)
    {
        // ← MANUEL ASYNC VALIDATION EKLE
        var validationResult = await _registerValidator.ValidateAsync(registerDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            return BadRequest(new
            {
                success = false,
                message = "Validation failed",
                errors = errors
            });
        }

        var result = await _authService.RegisterAsync(registerDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                success = false,
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { 
            success = true,
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
    /// Kullanıcı girişi - Bearer Token döner (1 haftalık)
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto loginDto)
    {
        // ← MANUEL ASYNC VALIDATION EKLE
        var validationResult = await _loginValidator.ValidateAsync(loginDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            return BadRequest(new
            {
                success = false,
                message = "Validation failed",
                errors = errors
            });
        }

        var result = await _authService.LoginAsync(loginDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                success = false,
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { 
            success = true,
            message = result.Message,
            data = new {
                accessToken = result.Data!.AccessToken,
                refreshToken = result.Data.RefreshToken,
                expiryDate = result.Data.ExpiryDate,
                user = result.Data.User,
                tokenType = "Bearer",
                usage = "Authorization: Bearer " + result.Data.AccessToken.Substring(0, 20) + "..."
            }
        });
    }

    // ← DİĞER METHODLAR AYNI KALSIN
    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                success = false,
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { 
            success = true,
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

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { success = false, message = "Geçersiz Bearer token" });
        }

        var result = await _authService.LogoutAsync(userId.Value);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                success = false,
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { success = true, message = result.Message });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { success = false, message = "Geçersiz Bearer token" });
        }

        var result = await _authService.ChangePasswordAsync(userId.Value, changePasswordDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                success = false,
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { success = true, message = result.Message });
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { success = false, message = "Geçersiz Bearer token" });
        }

        var result = await _authService.GetUserProfileAsync(userId.Value);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                success = false,
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(new { 
            success = true,
            message = result.Message,
            data = result.Data
        });
    }

    [HttpGet("test-bearer-token")]
    [Authorize]
    public ActionResult TestBearerToken()
    {
        var userId = GetCurrentUserId();
        var username = User.Identity?.Name;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new { 
            success = true,
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

    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")]
    public ActionResult AdminOnly()
    {
        return Ok(new { 
            success = true,
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