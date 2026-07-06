using API.DTOs;
using API.Entities;
using API.Interface;

namespace API.Extentions;

public static class AppUserExtensions
{
    public static UserDto ToDto(this AppUser user, ITokenService tokenService)
    {
        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.email,
            Token = tokenService.CreateToken(user)
        };
    }
}