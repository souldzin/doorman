﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using DoormanAPI.Entities;
using DoormanAPI.Hub;
using DoormanAPI.Seed;
using DoormanAPI.Services;
using DoormanAPI.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;

namespace DoormanAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
	        services
		        .AddAuthentication(options =>
		        {
			        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		        })
		        .AddJwtBearer(options => { options.TokenValidationParameters = TokenBuilder.TokenValidationParams; });

	        services.AddAuthorization(options =>
	        {
		        options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
			        .RequireAuthenticatedUser().Build();
	        });

			services.AddSignalR();
	        services.AddCors(options =>
	        {
		        options.AddPolicy("CorsPolicy",
			        builder => builder.AllowAnyOrigin()
				        .AllowAnyMethod()
				        .AllowAnyHeader()
				        .AllowCredentials());
	        });

			services.AddMvc().AddJsonOptions(o =>
            {
				if(o.SerializerSettings.ContractResolver == null)
					return;

	            var castResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
	            castResolver.NamingStrategy = null;
            });

	        var connectionString = Startup.Configuration["connectionStrings:doormanDbConnectionString"];

	        services.AddDbContext<DoormanContext>(o => o.UseSqlServer(connectionString));
	        services.AddTransient<IDoormanService, DoormanService>();
	        services.AddTransient<IDoormanContext, DoormanContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DoormanContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			context.EnsureSeedDataForContext();

			Mapper.Initialize(cfg => cfg.AddProfiles(Assembly.GetExecutingAssembly()));

			app.UseCors("CorsPolicy");

	        app.UseWebSockets();

			app.UseSignalR(routes =>
	        {
		        routes.MapHub<DoormanHub>("doorman");
	        });

	        app.UseMvc();
		}
    }
}
