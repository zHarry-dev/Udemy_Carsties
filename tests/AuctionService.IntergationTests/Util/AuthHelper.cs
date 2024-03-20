using System.Security.Claims;

namespace AuctionService.IntegrationTests;

public static class AuthHelper
{
    public static Dictionary<string, Object> getBearerForUser(string username)
        => new() { { ClaimTypes.Name, username } };
}
