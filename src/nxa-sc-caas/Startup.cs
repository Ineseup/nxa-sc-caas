using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NXA.SC.Caas.Services.Persist;
using NXA.SC.Caas.Services.Persist.Impl;
using NXA.SC.Caas.Services.Compiler;
using NXA.SC.Caas.Services.Compiler.Impl;
using NXA.SC.Caas.Services.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NXA.SC.Caas.Models;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using System.IO;
using NXA.SC.Caas.Services.Db;
using Microsoft.AspNetCore.Diagnostics;

namespace NXA.SC.Caas
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
            services.AddScoped<ITaskPersistService, TaskPersistService>();
            services.AddScoped<ICompilerService, CompilerService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDbSettings, DbSettings>();
            services.AddDbContext<ApiTokenContext>();
            services.AddAuthentication(TokenAuthOptions.DefaultScemeName)
                    .AddScheme<TokenAuthOptions, ApiTokenHandler>(TokenAuthOptions.DefaultScemeName, null);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NXA SC Caas", Version = "v1" });
                c.OperationFilter<ApiTokenOperationFilter>();
            });
            services.AddHealthChecks();
            var clientAppRoot = "./CodeEditor/dist";
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = clientAppRoot;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.Use((context, next) =>
            {
                context.Request.PathBase = new PathString("/api");
                return next();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NXA SC Caas v1"));
            app.UsePathBase("/api");
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
                var lf = app.ApplicationServices.GetService<ILoggerFactory>();
                var logger = lf!.CreateLogger("exceptionHandlerLogger");
                logger.LogDebug(exception.StackTrace);
                await context.Response.WriteAsJsonAsync(exception.Message);
            }));
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/Status");
            });
            app.UseFileServer();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "./CodeEditor";
                spa.Options.DefaultPage = new PathString("/code-editor.html");
                spa.UseAngularCliServer(npmScript: "start");
            });
        }
    }
}
