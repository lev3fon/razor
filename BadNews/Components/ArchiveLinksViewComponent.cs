using BadNews.Repositories.News;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Components
{
    public class ArchiveLinksViewComponent : ViewComponent
    {
        private readonly INewsRepository newsRepository;

        public ArchiveLinksViewComponent(INewsRepository newsRepository)
        {
            this.newsRepository = newsRepository;
        }

        public IViewComponentResult Invoke()
        {
            var years = newsRepository.GetYearsWithArticles();
            return View(years);
        }
    }
}