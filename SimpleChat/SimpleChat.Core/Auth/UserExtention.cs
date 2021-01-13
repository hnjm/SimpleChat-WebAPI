using System;
using System.Linq;
using System.Security.Claims;

namespace SimpleChat.Core.Auth
{
    public static class UserExtention
    {
       public static Guid GetId(this ClaimsPrincipal user)
       {
            var idClaim = user.Claims.FirstOrDefault(c => c.Type == "UserId");

            if(idClaim == null)
                return Guid.Empty;

            Guid userId = Guid.Empty;

            Guid.TryParse(idClaim.Value, out userId);

            return userId;
       }
    }
}
