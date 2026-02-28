using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PLDMS.Core.Entities;
using System.Security.Claims;

namespace PLDMS.PL.Common;

public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, IdentityRole<Guid>>
{
    public AppUserClaimsPrincipalFactory(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options) {}

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        identity.AddClaim(
            new Claim(ClaimTypes.Role, user.Role.ToString()));

        return identity;
    }
}