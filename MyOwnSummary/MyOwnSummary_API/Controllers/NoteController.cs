using Microsoft.AspNetCore.Mvc;

namespace MyOwnSummary_API.Controllers
{
    public class NoteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
