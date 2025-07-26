using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrderServer;

public class Startup
{
	public IConfiguration Configuration { get; }

	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddMvc(delegate(MvcOptions options)
		{
			options.EnableEndpointRouting = false;
		});
		services.Configure(delegate(KestrelServerOptions options)
		{
			options.AllowSynchronousIO = true;
		});
		services.Configure(delegate(IISServerOptions options)
		{
			options.AllowSynchronousIO = true;
		});
		services.AddCors(delegate(CorsOptions options)
		{
			options.AddDefaultPolicy(delegate(CorsPolicyBuilder builder)
			{
				builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
			});
		});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
        app.UseCors();
        app.UseDeveloperExceptionPage();
		DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
		defaultFilesOptions.DefaultFileNames.Clear();
		defaultFilesOptions.DefaultFileNames.Add("index.html");
		app.UseDefaultFiles(defaultFilesOptions);
		app.Use(delegate(HttpContext context, Func<Task> next)
		{
			context.Request.EnableBuffering();
			return next();
		});
		app.UseMvc(delegate(IRouteBuilder routes)
		{
			routes.MapRoute("Default", "{controller=Home}/{action=Index}/");
		});
		
		app.Run(async delegate(HttpContext context)
		{
			context.Response.ContentType = "text/plain; charset=utf-8";
			await context.Response.WriteAsync("支持Poppin'Party喵，支持Roeslia谢谢喵\n");
            await context.Response.WriteAsync(DataClass.desc+"\n");

            await context.Response.WriteAsync("⠄⠄⠄⠄⠄⡇⠄⠄⠄⠄⠄⢠⡀⠄⠄⢀⡬⠛⠁⠄⠄⠄⠄⠄⠄⠄⠉⠻⣿⣿⣿⣽⣿⣿⣿⣿⣿⣿⣿⣿⣧⠄⠄⠙⢦\n ⠄⠄⠄⠄⠄⡇⠄⠄⠄⠄⢰⠼⠙⢀⡴⠋⠄⠄⠄⠄⠄⠄⠄⠄⠄⡠⠖⠄⠄⠙⠿⢿⣿⣿⣿⣿⣿⣿⣿⣿⣯⣀⡀⠄⠄⠄⡀\n ⠄⠄⠄⠄⠄⡇⠄⠄⠄⠄⠄⠄⡴⠋⠄⠄⠄⠄⠄⠄⠄⠄⠄⢠⠞⠄⠄⠄⠄⠄⠄⠄⠄⠄⠉⠉⠉⠙⠋⠙⠋⠙⠻⠦⠤⣤⣼⣆⣀⣀⣀⣀⡀\n ⠄⠄⠄⠄⠄⢷⠄⠄⠄⠄⢠⠞⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⡰⠃⡄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠉⠉⠉\n ⠄⠄⠄⠄⠄⢸⡀⠄⠄⢠⠏⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣰⠁⣸⠁⠄⠄⠄⠄⠄⠄⠄⠄⢀⠄⠄⡄\n ⠄⠄⠄⠄⠄⢀⣧⠄⢠⠏⠄⠄⠄⠄⠄⠄⠄⠄⠄⢀⢾⠃⡜⡿⠄⠄⠄⠄⠄⠄⠄⠄⣠⠋⢀⣼⠁⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠸⢾⣀⣠\n ⠄⠄⠄⢀⣠⢌⣦⢀⡏⠄⡄⠄⠄⢠⠃⠄⠄⠐⣶⡁⡞⡼⠄⣇⠄⠄⠄⠄⠄⠄⠄⡴⠁⢠⠎⢸⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⢈⠝⠁\n ⠄⢀⠞⠉⠄⠄⠹⡼⠄⣼⠁⠄⠄⡏⠄⠄⢀⡞⠄⠈⣷⠇⠄⢻⠄⠄⠄⠄⠄⢐⣞⣀⣰⣃⣀⣸⠄⢀⠇⠄⠄⠄⠄⠄⠄⠄⠄⠄⠁⠄⠄⠄⠄⠄⢀\n ⢰⠋⠄⠄⠄⠄⢀⡇⢠⡏⠄⠄⢸⠄⠄⢀⠎⠄⠄⠄⡇⠄⠄⢸⡀⠄⠄⠄⢠⢾⢁⡜⠁⠄⠄⢸⠄⣸⠄⠄⠄⠄⠄⠄⡀⠄⠄⠄⠄⠄⠄⠄⢀⡴⠃\n ⡞⠄⠄⠄⠄⠄⢸⠄⡞⡷⠄⠄⡟⠄⢘⡟⠛⠷⠶⣤⣅⠄⠄⠄⣇⠄⠄⢠⠋⡧⠊⠄⠄⠄⠄⢸⢀⠇⠄⠄⠄⠄⠄⢰⠁⠄⠄⠄⠄⠄⢀⡴⠋\n ⢹⠄⠄⠄⠄⠄⡾⢰⠃⡇⠄⠄⡇⠄⡜⢀⣠⣤⠶⠞⠛⠁⠄⠄⠘⡄⡰⠃⠘⠱⣾⣟⡛⠛⠛⠛⡟⠂⠄⠄⠄⠄⠄⡎⠄⠄⠄⠄⣀⠴⡋\n ⠈⢳⠄⠄⠄⠄⡇⡼⠄⢻⠄⢠⡇⢸⠁⠈⠁⠄⠄⠄⠄⠄⠄⠄⠄⠈⠁⠄⠄⠄⠄⠙⠿⣶⣄⡰⠇⠄⠄⠄⠄⠄⡼⠄⠄⠄⡠⢾⣿⣆⢳\n ⣀⣬⠿⠷⠦⠤⣷⣇⡠⠾⡄⢸⣇⢸⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⢹⠛⠂⠄⠄⠄⠄⣰⠁⠄⣀⣬⣷⠞⠛⠙⠛⢧⣤⣀\n ⠉⠄⠄⠄⠄⠄⢻⠄⠄⠄⢧⢸⢸⠘⡇⠄⠄⠄⠄⠄⠄⠄⠄⣠⠄⠄⠄⠄⠄⠄⠄⠄⠄⢠⠃⠄⠄⠄⠄⠄⣰⠃⣶⢉⠜⠋⠄⠄⠄⠄⠄⠄⠄⠈⢳\n ⠄⠄⠄⢀⣤⡀⢸⠄⠄⠄⠈⢿⠄⠄⣿⣆⠄⠄⠄⠄⠄⠄⠄⡟⣧⠄⠄⠄⠄⠄⠄⠄⡴⠃⠄⠄⠄⡠⠊⡰⡗⠋⡰⡼⠃⠄⠄⠄⠄⠄⠄⠄⠄⠄⢨\n ⠄⢀⡔⠉⠄⠙⢦⠄⠄⠄⠄⢸⡀⢰⡏⠈⠳⣄⠄⠄⠄⠄⠄⠉⠁⠄⠄⠄⠄⠄⢀⡞⠁⠄⢀⣤⠎⠄⡔⣡⠃⢰⡇⣹⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⢼\n ⣰⠋⠄⢀⣴⣖⠒⠓⡆⠄⠄⠈⣇⣿⣿⠄⢸⠹⡷⢄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣠⠋⠄⢀⣴⣿⠏⠠⠊⣴⡏⢠⢻⡇⢹⡆⠄⠄⠄⠄⠄⣷⠄⠄⣰⠋\n ⡇⠄⣰⣿⣿⣿⠒⠋⣛⣄⠄⠄⣹⢸⠈⢧⠸⡄⠹⣄⠙⠶⢶⣶⣶⣶⣶⡶⢾⠃⠄⡴⢋⡾⠋⣀⡴⣞⡝⡰⠃⠄⡇⡸⠉⠳⣄⣀⣀⣀⣿⣦⠞⠁\n ⢷⢰⣿⣿⣿⣿⠄⠈⠁⠈⡇⠄⡏⢸⠄⠈⠓⢧⣀⣈⣤⡤⠖⠛⠉⠁⠄⢡⠃⢠⡞⠓⠚⠓⠚⣳⡞⠈⠘⠁⠄⠄⢹⡇⠄⠄⠄⠈⠉⠁⠸⣷⣀⣀⣀\n ⢸⣿⣿⣿⣿⣿⠄⣿⣁⠜⠁⢸⡇⢸⣄⣀⡀⠘⢦⡀⠄⠄⠄⠄⠄⠄⢀⠏⡴⡻⠄⠄⠄⠠⣎⠹⡄⠄⢀⣀⣤⣤⣀⠁⠄⠄⠄⠄⠄⠄⠄⠈⢻⣿⣿\n ⢸⣿⣿⣿⣿⣿⡇⢸⠄⠄⢀⡴⡇⠈⡇⠈⣩⠗⠒⣵⠆⠄⠄⠄⠄⠄⢸⡞⢰⠃⢀⠄⣀⡰⠟⠒⠒⡿⠉⠄⠄⠄⠈⠑⣄⠄⠄⠄⠄⠄⠄⠄⠈⢿⣿\n ⣿⣿⣿⣿⣿⣿⣷⠎⠄⢠⠏⠄⠹⣄⢣⢠⠃⠄⠄⢤⠤⠄⠄⠠⠤⢶⡏⠄⡎⢠⠞⠋⠁⠄⠄⠄⣸⠁⠄⠄⠄⠄⠄⠄⠈⣧⠄⠄⠄⠄⠄⠄⠄⠄⠻\n ⣿⣿⣿⣿⣿⣿⣃⡀⢠⠏⠄⠄⠄⠄⣨⠇⠄⣠⠴⠚⠁⠄⠄⠄⠄⠈⡇⢰⠃⠄⠄⠄⠄⠄⠄⢰⠇⠄⠄⠄⠄⠄⠄⠄⠄⢹⡀\n ⣿⣿⣿⣿⣿⡿⢉⣇⡎⠄⠄⠄⠄⢰⠇⠄⢨⠇⠄⠄⠄⠄⠄⠄⠄⠄⠘⢾⡀⠄⠄⠄⠄⠄⠄⡞⢀⠄⠄⠄⠄⠄⠄⠄⠄⢸⡇\n");
		});
	}
}
