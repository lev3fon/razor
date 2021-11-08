using System;
using BadNews.ModelBuilders.News;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Controllers
{
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client, VaryByHeader = "Cookie")]
    public class NewsController : Controller
    {
        private readonly INewsModelBuilder newsModelBuilder;

        public NewsController(INewsModelBuilder newsModelBuilder)
        {
            this.newsModelBuilder = newsModelBuilder;
        }

        public IActionResult Index(int? year, int pageIndex = 0)
        {
            var model = newsModelBuilder.BuildIndexModel(pageIndex, true, year);
            return View(model);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult FullArticle(Guid id)
        {
            var model = newsModelBuilder.BuildFullArticleModel(id);
            
            if (model == null)
                return NotFound();
            
            return View(model);
        }
    }
}