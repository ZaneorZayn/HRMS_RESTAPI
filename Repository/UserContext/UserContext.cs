using System.Security.Claims;

namespace hrms_api.Repository.UserContext
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserRole()
        {
            var user = _httpContextAccessor.HttpContext.User.Claims;
            var role = user.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            return role ?? string.Empty;
        }

        public int GetUserId()
        {
            var user = _httpContextAccessor.HttpContext.User.Claims;
            var id = user.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return id != null ? int.Parse(id) : 0;
        }
    }
}
