using AuthWebApp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthWebApp.Pages
{
    //[Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly SignInManager<MyIdentityUser> _signInManager;
        private string loginUrl = "/Identity/Account/Login";
        public IList<MyIdentityUser> Users { get; set; } = default!;
        [BindProperty]
        public bool SelectAllUsers { get; set; }

        public IndexModel(ILogger<IndexModel> logger, UserManager<MyIdentityUser> userManager, SignInManager<MyIdentityUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIsAllowPost = await UserIsAllowPost();
            if (!userIsAllowPost)
            {
                return LocalRedirect(loginUrl);
            }

            Users = _userManager.Users.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostBlockAsync(List<string> SelectedUserId)
        {
            var userIsAllowPost = await UserIsAllowPost();
            if (!userIsAllowPost) 
            {
                return LocalRedirect(loginUrl);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await UpdateBlockStatusUser(SelectedUserId, true);

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUnblockAsync(List<string> SelectedUserId)
        {
            var userIsAllowPost = await UserIsAllowPost();
            if (!userIsAllowPost)
            {
                return LocalRedirect(loginUrl);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await UpdateBlockStatusUser(SelectedUserId, false);

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(List<string> SelectedUserId)
        {
            var userIsAllowPost = await UserIsAllowPost();
            if (!userIsAllowPost)
            {
                return LocalRedirect(loginUrl);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var selectedId in SelectedUserId)
            {
                var user = await _userManager.FindByIdAsync(selectedId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            return RedirectToPage("./Index");
        }


        private async Task UpdateBlockStatusUser(List<string> SelectedUserId, bool status)
        {
            foreach (var selectedId in SelectedUserId)
            {
                var user = await _userManager.FindByIdAsync(selectedId);
                if (user != null)
                {
                    if (user.BlockStatus != status)
                    {
                        user.BlockStatus = status;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }
        }

        private async Task<bool> UserIsAllowPost()
        {
            var allowPost = false;     

            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null && !currentUser.BlockStatus) allowPost = true;
                else
                {
                    await _signInManager.SignOutAsync();
                }
            }

            return allowPost;
        }
    }
}
