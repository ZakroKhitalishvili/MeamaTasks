
using Meama_Tasks.Models.User;
using System.Data;
using System.Security.Claims;

namespace Meama_Tasks.Extensions;

public static class HttpContextExtensions
{
    public static UserVM GetUserData(this HttpContext context)
    {
        var mailClaim = context.User.FindFirst(ClaimTypes.Email);
        var nameClaim = context.User.FindFirst(ClaimTypes.Name);
        var idClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

        return new UserVM { Email = mailClaim.Value,Name = nameClaim.Value,Id = idClaim.Value };
    }
}