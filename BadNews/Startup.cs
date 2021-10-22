using BadNews.ModelBuilders.News;
using BadNews.Models.News;
using BadNews.Repositories.News;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Serilog;

namespace BadNews
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;

        // В конструкторе уже доступна информация об окружении и конфигурация
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            this.env = env;
            this.configuration = configuration;
        }

        // В этом методе добавляются сервисы в DI-контейнер
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<INewsRepository, NewsRepository>();
            services.AddSingleton<INewsModelBuilder, NewsModelBuilder>();
            services.AddControllersWithViews();
        }

        // В этом методе конфигурируется последовательность обработки HTTP-запроса
        public void Configure(IApplicationBuilder app)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Errors/Exception");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller}/{action}");
                endpoints.MapControllerRoute("status-code", "StatusCode/{code?}", new
                {
                    controller = "Errors",
                    action = "StatusCode"
                });
            });

            app.Map("/news", newsApp =>
            {
                newsApp.Map("/fullarticle", fullArticleApp =>
                {
                    fullArticleApp.Run(RenderFullArticlePage);
                });

                newsApp.Run(RenderIndexPage);
            });

            app.MapWhen(context => context.Request.Path == "/", rootPathApp =>
            {
                rootPathApp.Run(RenderIndexPage);
            });

            // Остальные запросы — 404 Not Found
        }

        // Региональные настройки, которые используются при обработке запросов новостей.
        private static CultureInfo culture = CultureInfo.CreateSpecificCulture("ru-ru");

        private async Task RenderIndexPage(HttpContext context)
        {
            // Model Builder достается из DI-контейнера
            var newsModelBuilder = context.RequestServices.GetRequiredService<INewsModelBuilder>();

            // Извлекаются входные параметры запроса
            int.TryParse(context.Request.Query["pageIndex"], out var pageIndex);

            // Строится модель страницы
            var model = newsModelBuilder.BuildIndexModel(pageIndex, false, null);

            // Строится HTML для модели
            string pageHtml = BuildIndexPageHtml(model);

            // Результат записывается в ответ
            await context.Response.WriteAsync(pageHtml);
        }

        private static string BuildIndexPageHtml(IndexModel model)
        {
            var articlesBuilder = new StringBuilder();
            var articleTemplate = File.ReadAllText("./$Content/Templates/NewsArticle.hbs");
            foreach (var articleModel in model.PageArticles)
            {
                var articleHtml = articleTemplate
                    .Replace("{{header}}", articleModel.Header)
                    .Replace("{{date}}", articleModel.Date.ToString("d MMM yyyy", culture))
                    .Replace("{{teaser}}", articleModel.Teaser)
                    .Replace("{{url}}", $"/news/fullarticle/{HttpUtility.UrlEncode(articleModel.Id.ToString())}");
                articlesBuilder.AppendLine(articleHtml);
            }

            var pageTemplate = File.ReadAllText("./$Content/Templates/Index.hbs");
            var pageHtml = pageTemplate
                .Replace("{{articles}}", articlesBuilder.ToString())
                .Replace("{{newerUrl}}", !model.IsFirst
                    ? $"/news?pageIndex={HttpUtility.UrlEncode((model.PageIndex - 1).ToString())}"
                    : "")
                .Replace("{{olderUrl}}", !model.IsLast
                    ? $"/news?pageIndex={HttpUtility.UrlEncode((model.PageIndex + 1).ToString())}"
                    : "");
            return pageHtml;
        }

        private async Task RenderFullArticlePage(HttpContext context)
        {
            // Model Builder достается из DI-контейнера
            var newsModelBuilder = context.RequestServices.GetRequiredService<INewsModelBuilder>();

            // Извлекаются входные параметры запроса
            var idString = context.Request.Path.Value.Split('/').ElementAtOrDefault(1);
            Guid.TryParse(idString, out var id);

            // Строится модель страницы
            var model = newsModelBuilder.BuildFullArticleModel(id);

            // Строится HTML для модели
            string pageHtml = BuildFullArticlePageHtml(model);

            // Результат записывается в ответ
            await context.Response.WriteAsync(pageHtml);
        }

        private static string BuildFullArticlePageHtml(FullArticleModel model)
        {
            var pageTemplate = File.ReadAllText("./$Content/Templates/FullArticle.hbs");
            var pageHtml = pageTemplate
                .Replace("{{header}}", model.Article.Header)
                .Replace("{{date}}", model.Article.Date.ToString("d MMM yyyy", culture))
                .Replace("{{content}}", model.Article.ContentHtml);
            return pageHtml;
        }
    }
}
