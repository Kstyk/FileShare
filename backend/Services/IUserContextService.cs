using System.Security.Claims;

namespace backend.Services
{
    public interface IUserContextService
    {
        int? GetUserId { get; }
        ClaimsPrincipal? User { get; }
    }
}