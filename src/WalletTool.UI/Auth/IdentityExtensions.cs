namespace WalletTool.UI.Auth;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        var userId = context.User.Claims.SingleOrDefault(x => x.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier");

        return Guid.Parse(userId!.Value);
    }
}