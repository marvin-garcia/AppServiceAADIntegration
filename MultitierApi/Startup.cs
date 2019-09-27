using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FrontendApi.Services;
using FrontendApi.Interfaces;

namespace FrontendApi
{
    public class Startup
    {   
        private IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddSingleton<IConfiguration>(Configuration);
                services.AddSingleton<IHttpClient, StandardHttpClient>();
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                //services.AddSingleton<IAuthToken, AuthToken>();
                services.AddMvc();

                // Register the Swagger generator, defining one or more Swagger documents
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Todo Frontend API", Version = "v1" });
                });

                var container = new ContainerBuilder();
                container.Populate(services);

                return new AutofacServiceProvider(container.Build());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            try
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo Frontend API V1");
                });

                //app.UseHttpsRedirection();
                app.UseDefaultFiles();
                app.UseStaticFiles();
                app.UseMvc();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
