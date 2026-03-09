using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
