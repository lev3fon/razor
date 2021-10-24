using System;
using BadNews.ModelBuilders.News;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsModelBuilder newsModelBuilder;

        public NewsController(INewsModelBuilder newsModelBuilder)
        {
            this.newsModelBuilder = newsModelBuilder;
        }

        public IActionResult Index(int pageIndex = 0)
        {
            var model = newsModelBuilder.BuildIndexModel(pageIndex, true, null);
            return View(model);
        }

        public IActionResult FullArticle(Guid id)
        {
            var model = newsModelBuilder.BuildFullArticleModel(id);
            
            if (model == null)
                return NotFound();
            
            return View(model);
        }
    }
}