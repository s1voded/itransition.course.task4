using AuthWebApp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthWebApp.Pages
{
    [Authorize(Policy = "UserIsNotBlocked")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<MyIdentityUser> _userManager;

        public IList<MyIdentityUser> Users { get; set; } = default!;

        public IndexModel(ILogger<IndexModel> logger, UserManager<MyIdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            Users = _userManager.Users.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostBlockAsync(List<string> SelectedUserId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await UpdateBlockStatusUser(SelectedUserId, true);

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUnblockAsync(List<string> SelectedUserId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await UpdateBlockStatusUser(SelectedUserId, false);

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(List<string> SelectedUserId)
        {
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
    }
}
