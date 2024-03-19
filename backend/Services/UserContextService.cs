using System.Security.Claims;

namespace backend.Services
{
    public class UserContextService : IUserContextService
    {
        private IHttpContextAccessor _contextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal? User => _contextAccessor.HttpContext?.User;
        public int? GetUserId => User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) is null || User is null ? null
            : int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
    }
}
