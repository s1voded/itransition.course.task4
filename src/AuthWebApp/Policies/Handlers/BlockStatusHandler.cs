using AuthWebApp.Model;
using AuthWebApp.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AuthWebApp.Policies.Handlers
{
    public class BlockStatusHandler : AuthorizationHandler<BlockStatusRequirement>
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly SignInManager<MyIdentityUser> _signInManager;

        public BlockStatusHandler(UserManager<MyIdentityUser> userManager, SignInManager<MyIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BlockStatusRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(context.User);
                if (currentUser != null && !currentUser.BlockStatus)
                {
                    context.Succeed(requirement);
                }
                else await _signInManager.SignOutAsync();
            }
        }
    }
}
