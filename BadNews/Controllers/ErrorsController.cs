using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BadNews.Controllers
{
    public class ErrorsController : Controller
    {
        private readonly ILogger<ErrorsController> logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            this.logger = logger;
        }

        public IActionResult StatusCode(int? code)
        {
            logger.LogWarning("status-code {code} at {time}", code, DateTime.Now);
            return View(code);
        }

        public IActionResult Exception()
        {
            return View(null, HttpContext.TraceIdentifier);
        }
    }
}