using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Doorman.Master.Entities;
using Doorman.Master.Hub;
using Doorman.Master.Seed;
using Doorman.Master.Services;
using Doorman.Master.Utility;
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

namespace Doorman.Master
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

		public static String ConfigDbType => Configuration["development:databaseType"];

		public static String ConfigDbName => Configuration["development:databaseName"] ?? "doormandb";

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

	        services.AddDbContext<DoormanContext>(ConfigDbType == "memory"
				? (Action<DbContextOptionsBuilder>)UseInMemoryDatabase
				: UseSqlServerDatabase
			);
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

		private void UseSqlServerDatabase(DbContextOptionsBuilder o) {
	        var connectionString = Startup.Configuration["connectionStrings:doormanDbConnectionString"];

			o.UseSqlServer(connectionString);
		}

		private void UseInMemoryDatabase(DbContextOptionsBuilder o) {
			o.UseInMemoryDatabase(ConfigDbName);
		}
    }
}
