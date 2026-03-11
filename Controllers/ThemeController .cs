using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementApp.Controllers
{
    public class ThemeController : Controller
    {
        [HttpPost]
        public IActionResult SetTheme(string theme, string returnUrl)
        {
            Response.Cookies.Append("theme", theme,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
