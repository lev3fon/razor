using BadNews.ModelBuilders.News;
using BadNews.Repositories.News;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
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
            var mvcBuilder = services.AddControllersWithViews();
            if (env.IsDevelopment())
                mvcBuilder.AddRazorRuntimeCompilation();
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
                endpoints.MapControllerRoute("default", "{controller=News}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("status-code", "StatusCode/{code?}", new
                {
                    controller = "Errors",
                    action = "StatusCode"
                });
            });

            // app.Map("/news/fullarticle", fullArticleApp =>
            // {
            //     fullArticleApp.Run(RenderFullArticlePage);
            // });

            // app.MapWhen(context => context.Request.Path == "/", rootPathApp =>
            // {
            //     rootPathApp.Run(RenderIndexPage);
            // });

            // Остальные запросы — 404 Not Found
        }

        // Региональные настройки, которые используются при обработке запросов новостей.
        private static CultureInfo culture = CultureInfo.CreateSpecificCulture("ru-ru");
        
    }
}
