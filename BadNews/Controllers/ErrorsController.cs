using Microsoft.AspNetCore.Mvc;

namespace BadNews.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult StatusCode(int? code)
        {
            return View(code);
        }
    }
}