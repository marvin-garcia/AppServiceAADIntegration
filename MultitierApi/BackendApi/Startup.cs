﻿using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using BackendApi.Models;
using BackendApi.Interfaces;
using BackendApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackendApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            // Choose either MongoDB or in-memory DB based on whether the connection string exists in the app settings
            string mongoConnectionString = Configuration["mongodb"];

            // Use in-memory DB
            if (string.IsNullOrEmpty(mongoConnectionString))
            {
                var optionsBuilder = new DbContextOptionsBuilder<InMemoryDbRepository>();
                optionsBuilder.UseInMemoryDatabase("TodoList");
                var dbContext = new InMemoryDbRepository(optionsBuilder.Options);

                services.AddSingleton<IDbContext>(dbContext);
            }
            // Use MongoDB
            else
            {
                services.AddSingleton<IDbContext>(new MongoDbRepository(Configuration));
            }

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Todo Backend API", Version = "v1" });
            });

            var container = new ContainerBuilder();
            container.Populate(services);
            
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo Backend API V1");
            });

            //app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
