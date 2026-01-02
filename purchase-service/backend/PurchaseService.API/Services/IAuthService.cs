using PurchaseService.API.Models.DTOs;

namespace PurchaseService.API.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> AuthenticateBuyerAsync(string email);
        Task<bool> ValidateBuyerAsync(Guid buyerId);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<AuthenticatedUserDto?> AuthenticateUserAsync(string email, string password);
    }
}