using DShop.PluginShared;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DShop.Infrastructure
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long CurrentUserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user?.Identity?.IsAuthenticated != true)
                    return 0;

                // 从 ClaimTypes.NameIdentifier 中读取
                var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(idClaim))
                    return 0;

                if (long.TryParse(idClaim, out var id))
                    return id;

                return 0;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }
}
