using BudgetManagement.Interfaces;
using System.Security.Claims;


namespace BudgetManagement.Services {
    public class UsersServices: IUsersService {
        private readonly HttpContext httpContext;

        public UsersServices(IHttpContextAccessor httpContextAccessor) {
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public int GetUserId() {
            if (httpContext.User.Identity.IsAuthenticated) {
                var idClaim = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (idClaim != null) {
                    var userId = int.Parse(idClaim.Value);
                    return userId;
                }
            }
            throw new UnauthorizedAccessException("The user is not authenticated");
        }
    }
}
