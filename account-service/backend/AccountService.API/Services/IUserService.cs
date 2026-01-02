using AccountService.API.Models;
using AccountService.API.Models.DTOs;

namespace AccountService.API.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserResponseDto>> GetUsersAsync(UserSearchDto searchDto);
        Task<UserResponseDto?> GetUserByIdAsync(Guid id);
        Task<UserResponseDto?> GetUserByEmailAsync(string email);
        Task<UserResponseDto?> AuthenticateAsync(string email, string password);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<List<UserResponseDto>> GetUsersByTypeAsync(UserType userType);
        Task<bool> UserExistsAsync(Guid id);
        Task<bool> EmailExistsAsync(string email);
    }
}