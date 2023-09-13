using Microsoft.AspNetCore.Mvc;

namespace MyOwnSummary_API.Controllers
{
    public class Category : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
