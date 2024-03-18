using AutoMapper;
using backend.Entities;
using Microsoft.AspNetCore.Identity;

namespace backend.Services
{
    public class AccountService
    {
        private FileShareDbContext _dbContext;
        private IPasswordHasher<User> _passwordHasher;
        private AuthenticationSettings _authenticationSettings;
        private readonly IMapper _mapper;
    }
}
