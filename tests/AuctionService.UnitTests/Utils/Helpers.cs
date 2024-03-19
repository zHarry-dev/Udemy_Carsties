using System.Security.Claims;

namespace AuctionService.UnitTests.Utils;

public class Helpers
{
    public readonly static string Name = "testName";
    public static ClaimsPrincipal GetClaimsPrincipal()
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, Name) };
        var identity = new ClaimsIdentity(claims, "testing");
        return new ClaimsPrincipal(identity);
    }
}