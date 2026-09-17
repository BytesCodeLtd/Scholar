using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Scholar.Constants;

namespace Scholar.Common.Identity
{
    public interface ITenantProvider
    {
        int? InstituteId { get; }

        bool IsSuperAdmin { get; }

        bool BypassFilter { get; }
    }

    public class HttpTenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _http;

        public HttpTenantProvider(IHttpContextAccessor http) => _http = http;

        private ClaimsPrincipal? User => _http.HttpContext?.User;

        public int? InstituteId => User?.GetInstituteId();

        public bool IsSuperAdmin => User?.IsInRole(Roles.SuperAdmin) ?? false;

        public bool BypassFilter => User is null || !(User.Identity?.IsAuthenticated ?? false)
                                                 || IsSuperAdmin;
    }
}
