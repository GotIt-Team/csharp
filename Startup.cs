using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GotIt.BLL.Managers;
using GotIt.Common.Helper;
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
            services.AddControllers();

            #region Dependancy Injection
            services.AddScoped(typeof(RequestAttributes));

            services.AddScoped(typeof(ChatManager));
            services.AddScoped(typeof(CommentManager));
            services.AddScoped(typeof(FeedbackManager));
            services.AddScoped(typeof(ItemManager));
            services.AddScoped(typeof(MessageManager));
            services.AddScoped(typeof(NotificationManager));
            services.AddScoped(typeof(ObjectManager));
            services.AddScoped(typeof(PersonManager));
            services.AddScoped(typeof(RequestManager));
            services.AddScoped(typeof(UserManager));

            services.AddDbContext<GotItDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Local-Abdalrahman"]));
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
