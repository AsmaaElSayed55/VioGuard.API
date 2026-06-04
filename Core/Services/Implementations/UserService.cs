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

            // AutoMapper reads UserProfile rules above and perfectly outputs UserDto
            return _mapper.Map<UserDto>(user);
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

            // Check if a user with this email (Id) already exists!
            var existingUser = await repo.GetByIdAsync(registerUserDto.Email);
            if (existingUser != null)
            {
                // 💡 CHANGE THIS LINE: Remove the throw statement and return null instead!
                return null;
            }

            var userEntity = new User
            {
                Id = registerUserDto.Email,
                FullName = registerUserDto.FullName,
                Password = registerUserDto.Password,
                UserInternalId = Guid.NewGuid().ToString()[..8],
                IsMonthlyReportEnabled = true
            };

            await repo.AddAsync(userEntity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDto>(userEntity);
        }
        public async Task<UserDto> UpdateProfileAsync(string email, UpdateProfileDto updateProfileDto)
        {
            var repo = _unitOfWork.GetRepository<User, string>();
            var user = await repo.GetByIdAsync(email);

            if (user == null) throw new KeyNotFoundException("The requested user profile does not exist.");

            user.FullName = updateProfileDto.FullName;

            repo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdatePreferencesAsync(string email, UpdatePreferencesDto updatePreferencesDto)
        {
            var repo = _unitOfWork.GetRepository<User, string>();
            var user = await repo.GetByIdAsync(email);

            if (user == null) throw new KeyNotFoundException("The requested user profile does not exist.");

            user.IsMonthlyReportEnabled = updatePreferencesDto.IsMonthlyReportEnabled;
            // Map additional fields (like IsDarkMode) here if they get added to your DB schema later

            repo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> ChangePasswordAsync(string email, ChangePasswordDto changePasswordDto)
        {
            var repo = _unitOfWork.GetRepository<User, string>();
            var user = await repo.GetByIdAsync(email);

            if (user == null) return false;

            // Optional: verify that user.Password == changePasswordDto.CurrentPassword first

            user.Password = changePasswordDto.NewPassword;

            repo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}