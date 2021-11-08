using System;
using BadNews.Repositories.News;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BadNews.Components
{
    public class ArchiveLinksViewComponent : ViewComponent
    {
        private readonly INewsRepository newsRepository;
        private readonly IMemoryCache cache;

        public ArchiveLinksViewComponent(INewsRepository newsRepository, IMemoryCache cache)
        {
            this.newsRepository = newsRepository;
            this.cache = cache;
        }

        public IViewComponentResult Invoke()
        {
            string cacheKey = nameof(ArchiveLinksViewComponent);
            if (!cache.TryGetValue(cacheKey, out var years))
            {
                years = newsRepository.GetYearsWithArticles();
                if (years != null)
                {
                    cache.Set(cacheKey, years, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                    });
                }
            }
            return View(years);
        }
    }
}