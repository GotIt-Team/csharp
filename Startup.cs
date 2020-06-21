using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GotIt.BLL.Managers;
using GotIt.Common.Helper;
using GotIt.Configuration;
using GotIt.MSSQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GotIt
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddControllers();

            #region Dependancy Injection
            services.AddScoped(typeof(RequestAttributes));

            services.AddScoped(typeof(ChatManager));
            services.AddScoped(typeof(CommentManager));
            services.AddScoped(typeof(SystemManager));
            services.AddScoped(typeof(ItemManager));
            services.AddScoped(typeof(MessageManager));
            services.AddScoped(typeof(NotificationManager));
            services.AddScoped(typeof(ObjectManager));
            services.AddScoped(typeof(PersonManager));
            services.AddScoped(typeof(RequestManager));
            services.AddScoped(typeof(UserManager));
            services.AddScoped(typeof(TokenManager));

            services.AddDbContext<GotItDbContext>(options =>
            {
                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()));
                options.UseSqlServer(Configuration["ConnectionStrings:Local"]);
            });
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseCors(options => options
                .SetIsOriginAllowed(x => _ = true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );

            app.UseHttpsRedirection();

            app.UseMiddleware<RequestMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
