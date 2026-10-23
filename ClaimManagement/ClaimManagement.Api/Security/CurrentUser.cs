using ClaimManagement.Application.Interface;
using System.Security.Claims;

namespace ClaimManagement.Api.Security
{
    public class CurrentUser : ICurrentUser
    {
        IHttpContextAccessor accessor;

        public CurrentUser(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        ClaimsPrincipal? Principal => accessor.HttpContext?.User;

        public long UserId =>
            long.TryParse(Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        public string Role => Principal?.FindFirst(ClaimTypes.Role)?.Value ?? "";

        // The Authentication service does NOT write this claim yet
        public long? CustomerId =>
            long.TryParse(Principal?.FindFirst("customer_id")?.Value, out var id) ? id : null;

        public bool IsCustomer => Role == Roles.Customer;
    }
}
