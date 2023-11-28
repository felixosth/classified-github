using InSupport.O3C.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InSupport.O3C.API
{
    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The startup.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// The configuration.
        /// </summary>
        public IConfiguration Configuration { get; }


        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            var connStrKey = Configuration.GetSection("ConnectionStringToUse").Value;
            services.AddDbContext<O3CDeviceDbContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString(
                    connStrKey)
                ));
            //services.AddDbContext<O3CDeviceDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("O3CDeviceDbContext")));

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InSupport.O3C.API", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "InSupport.O3C.API.xml"));
            });
            services.AddRazorPages();


        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation("Using connection string {0}: {1}", Configuration.GetSection("ConnectionStringToUse").Value, Configuration.GetConnectionString(Configuration.GetSection("ConnectionStringToUse").Value));

            app.Use(async (ctx, next) =>
            {
                await next.Invoke();
                logger.LogInformation(ctx.Connection.RemoteIpAddress + " -> " + ctx.Request.Host + ctx.Request.Path + ctx.Request.QueryString);
            });

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InSupport.O3C.API v1"));
            //}

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InSupport.O3C.API v1"));

            app.UseRouting();

            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
