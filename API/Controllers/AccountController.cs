using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext context, ITokenService tokenService):BaseApiController
{
    [HttpPost("register")] //api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {

        if (await EmailExists(registerDto.Email)) return BadRequest("Email taken");

        //using statement will dispose of the object when done
        using var hmac = new HMACSHA512(); //cryptography class use to hash password, the randomly generated key will be used to salt the password

        var user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            email = registerDto.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        context.User.Add(user);
        await context.SaveChangesAsync();

        return user.ToDto(tokenService);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.User.SingleOrDefaultAsync(x => x.email == loginDto.Email);

        if (user == null) return Unauthorized("Invalid email address");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (var i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return user.ToDto(tokenService);
    }

    // [HttpDelete("remove")] //api/account/remove
    // public async Task<IActionResult> Remove()
    // {
    //     await context.User.Where(u => u.email == "").ExecuteDeleteAsync();
        
    //     return Ok(new {message="Removed"});
        
    // }
    private async Task<bool> EmailExists(string email)
    {
        return await context.User.AnyAsync(x => x.email.ToLower() == email.ToLower());
    }

}