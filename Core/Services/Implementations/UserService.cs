using AutoMapper;
using Domain.Contracts;
using Domain.Entities.UserModule;
using Services.Abstraction.Contracts;
using Shared.Dtos.User;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var repo = _unitOfWork.GetRepository<User, string>();
            var user = await repo.GetByIdAsync(email);
            return user is null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var repo = _unitOfWork.GetRepository<User, string>();
            var users = await repo.GetAllAsync(asNoTracking: true);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> CreateUserAsync(RegisterUserDto registerUserDto)
        {
            var repo = _unitOfWork.GetRepository<User, string>();
            var existingUser = await repo.GetByIdAsync(registerUserDto.Email);
            if (existingUser != null)
                return null;

            var userEntity = new User
            {
                Id = registerUserDto.Email,
                FullName = registerUserDto.FullName,
                Password = registerUserDto.Password,
                UserInternalId = Guid.NewGuid().ToString()[..8],
                IsMonthlyReportEnabled = true,
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(userEntity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDto>(userEntity);
        }

        public async Task<UserDto?> AuthenticateAsync(LoginDto loginDto)
        {
            var repo = _unitOfWork.GetRepository<User, string>();
            var user = await repo.GetByIdAsync(loginDto.Email);
            if (user is null || user.Password != loginDto.Password)
                return null;

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateProfileAsync(string email, UpdateProfileDto updateProfileDto)
        {
            var repo = _unitOfWork.GetRepository<User, string>();
            var user = await repo.GetByIdAsync(email)
                ?? throw new KeyNotFoundException("The requested user profile does not exist.");

            user.FullName = updateProfileDto.FullName;
            user.LastModified = DateTime.UtcNow;

            repo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdatePreferencesAsync(string email, UpdatePreferencesDto updatePreferencesDto)
        {
            var repo = _unitOfWork.GetRepository<User, string>();
            var user = await repo.GetByIdAsync(email)
                ?? throw new KeyNotFoundException("The requested user profile does not exist.");

            user.IsMonthlyReportEnabled = updatePreferencesDto.IsMonthlyReportEnabled;
            user.LastModified = DateTime.UtcNow;

            repo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> ChangePasswordAsync(string email, ChangePasswordDto changePasswordDto)
        {
            var repo = _unitOfWork.GetRepository<User, string>();
            var user = await repo.GetByIdAsync(email);
            if (user is null)
                return false;

            if (!string.IsNullOrEmpty(changePasswordDto.CurrentPassword)
                && user.Password != changePasswordDto.CurrentPassword)
                return false;

            user.Password = changePasswordDto.NewPassword;
            user.LastModified = DateTime.UtcNow;

            repo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
