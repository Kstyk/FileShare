using AutoMapper;
using backend.Entities;
using backend.Models;
using backend.Models.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.SymbolStore;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace backend.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        LoginResponseDto LoginUser(LoginUserDto dto);
        RefreshTokenResponseDto RefreshAccessToken(string refreshToken);
    }

    public class AccountService : IAccountService
    {
        private FileShareDbContext _dbContext;
        private IPasswordHasher<User> _passwordHasher;
        private AuthenticationSettings _authenticationSettings;
        private readonly IMapper _mapper;
        private IUserContextService _userContextService;


        public AccountService(FileShareDbContext context, IPasswordHasher<User> passwordHasher, AuthenticationSettings authSettings, IMapper mapper, IUserContextService userContextService)
        {
            _dbContext = context;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authSettings;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = _mapper.Map<User>(dto);
            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
            newUser.Password = hashedPassword;
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
        }

        public LoginResponseDto LoginUser(LoginUserDto dto)
        {
            var user = _dbContext.Users
                .FirstOrDefault(u => u.Email == dto.Email);

            if (user is null)
            {
                throw new UnauthorizedAccessException("Niepoprawne dane logowania.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Niepoprawne dane logowania.");
            }

            var accessToken = GenerateAccessToken(user);

            var existingRefreshToken = _dbContext.RefreshTokens.FirstOrDefault(rt => rt.UserId == user.Id);
            var refreshToken = "";

            if (existingRefreshToken == null)
            {
                refreshToken = GenerateRefreshToken();
                SaveRefreshToken(user.Id, refreshToken);
            } else
            {
                if (existingRefreshToken.Expires < DateTime.UtcNow)
                {
                    _dbContext.RefreshTokens.Remove(existingRefreshToken);
                    _dbContext.SaveChanges();

                    refreshToken = GenerateRefreshToken();
                    SaveRefreshToken(user.Id, refreshToken);
                } else
                {
                    refreshToken = existingRefreshToken.Token;
                }

                
            }
            return new LoginResponseDto(accessToken, refreshToken);
        }

        private string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("Id", $"{user.Id}"),
                new Claim("Email", $"{user.Email}"),
                new Claim("FirstName", $"{user.FirstName}"),
                new Claim("LastName", $"{user.LastName}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtAccessExpiresMinutes);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer, _authenticationSettings.JwtIssuer, claims, expires: expires, signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[128];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        private void SaveRefreshToken(int userId, string refreshToken)
        {
            var token = new RefreshToken();
            token.UserId = userId;
            token.Token = refreshToken;
            token.Expires = DateTime.UtcNow.AddDays(_authenticationSettings.JwtRefreshExpiresDays);

            _dbContext.RefreshTokens.Add(token);
            _dbContext.SaveChanges();
        }

        public RefreshTokenResponseDto RefreshAccessToken(string refreshToken) {
            var refreshTokenDb = _dbContext.RefreshTokens.FirstOrDefault(rt => rt.Token.Equals(refreshToken) && rt.Expires > DateTime.UtcNow);

            if (refreshTokenDb == null)
            {
                // Wyloguj użytkownika po stronie frontendu
                throw new Exception("Refresh token expired.");
            }

            var user = _dbContext.Users.FirstOrDefault(x => x.Id == refreshTokenDb.UserId);

            if (user == null)
            {
                throw new Exception("An error during the process of refreshing token.");
            }
            
            var newAccessToken = GenerateAccessToken(user);

            if (newAccessToken != null)
            {
                return new RefreshTokenResponseDto() { AccessToken = newAccessToken };
            } else
            {
                throw new Exception("An error during the process of refreshing token.");
            }
        }

    }
}
