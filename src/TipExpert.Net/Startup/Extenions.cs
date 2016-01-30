using System.Security.Claims;
using MongoDB.Bson;

namespace TipExpert.Net
{
    public static class Extenions
    {
        public static ObjectId GetUserIdAsObjectId(this ClaimsPrincipal principal)
        {
            var userId = principal.GetUserId();
            return userId.ToObjectId();
        }

        public static ObjectId ToObjectId(this string value)
        {
            ObjectId id;
            if (ObjectId.TryParse(value, out id))
                return id;

            return ObjectId.Empty;
        }
    }
}