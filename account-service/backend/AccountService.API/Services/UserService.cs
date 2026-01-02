using AccountService.API.Data;
using AccountService.API.Models;
using AccountService.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AccountService.API.Services
{
    public class UserService : IUserService
    {
        private readonly AccountDbContext _context;

        public UserService(AccountDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<UserResponseDto>> GetUsersAsync(UserSearchDto searchDto)
        {
            var query = _context.Users.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.PhoneNumber.Contains(searchTerm));
            }

            if (searchDto.UserType.HasValue)
            {
                query = query.Where(u => u.UserType == searchDto.UserType.Value);
            }

            if (searchDto.Status.HasValue)
            {
                query = query.Where(u => u.Status == searchDto.Status.Value);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var users = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    UserType = u.UserType,
                    Status = u.Status,
                    Address = u.Address,
                    City = u.City,
                    State = u.State,
                    ZipCode = u.ZipCode,
                    Country = u.Country,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

            return new PagedResult<UserResponseDto>
            {
                Items = users,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages,
                HasNext = searchDto.Page < totalPages,
                HasPrevious = searchDto.Page > 1
            };
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                Status = user.Status,
                Address = user.Address,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                Country = user.Country,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            
            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                Status = user.Status,
                Address = user.Address,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                Country = user.Country,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<UserResponseDto?> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            
            if (user == null || user.Password != password)
            {
                return null; // Invalid credentials
            }

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                Status = user.Status,
                Address = user.Address,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                Country = user.Country,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Check if email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == createUserDto.Email.ToLower());
            
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            var user = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                Password = createUserDto.Password,
                PhoneNumber = createUserDto.PhoneNumber,
                UserType = createUserDto.UserType,
                Address = createUserDto.Address,
                City = createUserDto.City,
                State = createUserDto.State,
                ZipCode = createUserDto.ZipCode,
                Country = createUserDto.Country
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                Status = user.Status,
                Address = user.Address,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                Country = user.Country,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            // Check if email is being changed and if it already exists
            if (!string.IsNullOrEmpty(updateUserDto.Email) && 
                updateUserDto.Email.ToLower() != user.Email.ToLower())
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == updateUserDto.Email.ToLower());
                
                if (existingUser != null)
                {
                    throw new InvalidOperationException("A user with this email already exists.");
                }
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
                user.FirstName = updateUserDto.FirstName;
            
            if (!string.IsNullOrEmpty(updateUserDto.LastName))
                user.LastName = updateUserDto.LastName;
            
            if (!string.IsNullOrEmpty(updateUserDto.Email))
                user.Email = updateUserDto.Email;
            
            if (!string.IsNullOrEmpty(updateUserDto.PhoneNumber))
                user.PhoneNumber = updateUserDto.PhoneNumber;
            
            if (updateUserDto.Status.HasValue)
                user.Status = updateUserDto.Status.Value;
            
            if (updateUserDto.Address != null)
                user.Address = updateUserDto.Address;
            
            if (updateUserDto.City != null)
                user.City = updateUserDto.City;
            
            if (updateUserDto.State != null)
                user.State = updateUserDto.State;
            
            if (updateUserDto.ZipCode != null)
                user.ZipCode = updateUserDto.ZipCode;
            
            if (updateUserDto.Country != null)
                user.Country = updateUserDto.Country;

            await _context.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                Status = user.Status,
                Address = user.Address,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                Country = user.Country,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            // Soft delete by setting status to Deleted
            user.Status = UserStatus.Deleted;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserResponseDto>> GetUsersByTypeAsync(UserType userType)
        {
            return await _context.Users
                .Where(u => u.UserType == userType && u.Status == UserStatus.Active)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    UserType = u.UserType,
                    Status = u.Status,
                    Address = u.Address,
                    City = u.City,
                    State = u.State,
                    ZipCode = u.ZipCode,
                    Country = u.Country,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<bool> UserExistsAsync(Guid id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id && u.Status != UserStatus.Deleted);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.Status != UserStatus.Deleted);
        }
    }
}