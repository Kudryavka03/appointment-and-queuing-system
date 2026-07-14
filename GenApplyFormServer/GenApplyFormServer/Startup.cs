using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenApplyFormServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => { options.EnableEndpointRouting = false; });
            services.AddRazorPages();
            services.AddCors(delegate (CorsOptions options)
            {
                options.AddDefaultPolicy(delegate (CorsPolicyBuilder builder)
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseCors();

            app.UseDeveloperExceptionPage();
            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.Use(delegate (HttpContext context, Func<Task> next)
            {
                context.Request.EnableBuffering();
                return next();
            });
            app.UseMvc(delegate (IRouteBuilder routes)
            {
                routes.MapRoute("Default", "{controller=Home}/{action=Index}/");
            });

            app.Run(async delegate (HttpContext context)
            {
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("支持Poppin'Party喵，支持Roeslia谢谢喵\n");
            });
        }
    }
}
