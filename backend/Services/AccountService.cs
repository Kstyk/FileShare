using AutoMapper;
using backend.Entities;
using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
    }

    public class AccountService : IAccountService
    {
        private FileShareDbContext _dbContext;
        private IPasswordHasher<User> _passwordHasher;
        private AuthenticationSettings _authenticationSettings;
        private readonly IMapper _mapper;


        public AccountService(FileShareDbContext context, IPasswordHasher<User> passwordHasher, AuthenticationSettings authSettings, IMapper mapper)
        {
            _dbContext = context;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authSettings;
            _mapper = mapper;
        }

        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = _mapper.Map<User>(dto);
            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
            newUser.Password = hashedPassword;
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
        }

    }
}
