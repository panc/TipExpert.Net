using System;
using System.Security.Claims;

namespace TipExpert.Net.Authentication
{
    public static class Extenions
    {
        public static Guid GetUserIdAsGuid(this ClaimsPrincipal principal)
        {
            var userId = principal.GetUserId();
            Guid id;

            if (Guid.TryParse(userId, out id))
                return id;

            return Guid.Empty;
        }
    }
}