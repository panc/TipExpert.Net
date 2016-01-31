using System.Security.Claims;
using MongoDB.Bson;
using TipExpert.Core;

namespace TipExpert.Net
{
    public static class Extenions
    {
        public static ObjectId GetUserIdAsObjectId(this ClaimsPrincipal principal)
        {
            var userId = principal.GetUserId();
            return userId.ToObjectId();
        }
    }
}