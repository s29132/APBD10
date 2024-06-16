using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication4.Context;
using WebApplication4.DTO.User;
using WebApplication4.Models;
using WebApplication4.Services;

namespace WebApplication4.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private ITokenService _tokenService;

    public UserController(ApplicationDbContext context, ITokenService service)
    {
        _context = context;
        _tokenService = service;
    }
    
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto dto)
    {
        var isEmailTaken = _context.Users.Any(u => u.Email.Equals(dto.Email, StringComparison.CurrentCultureIgnoreCase));
        if (isEmailTaken) return Conflict("Email is already taken");

        var user = new User
        {
            Email = dto.Email
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, dto.Password);
        _context.Users.Add(user);
        
        return NoContent();
    }
    
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto dto)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email.Equals(dto.Email, StringComparison.CurrentCultureIgnoreCase));
        if (user == null) return Unauthorized();

        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed) return Unauthorized();

        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken.token;
        user.RefreshTokenExpiration = refreshToken.expiration;

        var response = new TokenResponseDto
        {
            AccessToken = _tokenService.GenerateAccessToken(user),
            RefreshToken = refreshToken.token,
        };
        
        return Ok(response);
    }
    
    
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDto dto)
    {
        var principal = _tokenService.ValidateAndGetPrincipalFromJwt(dto.AccessToken, false);
        if (principal is null) return Unauthorized();
        
        var claimIdUser = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (claimIdUser is null || !int.TryParse(claimIdUser, out _))
            return Unauthorized();
        
        var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(claimIdUser));
        if (user is null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiration < DateTime.UtcNow)
            return Unauthorized();

        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken.token;
        user.RefreshTokenExpiration = refreshToken.expiration;
        
        var response = new TokenResponseDto
        {
            AccessToken = _tokenService.GenerateAccessToken(user),
            RefreshToken = refreshToken.token,
        };
        
        return Ok(response);
    }
    
}