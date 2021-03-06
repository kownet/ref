﻿using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Ref.Api.Helpers;
using Ref.Api.Settings;
using Ref.Data.Db;
using Ref.Data.Repositories;
using Ref.Services.Contracts;
using Ref.Services.Features.Commands.Users;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using System.Reflection;
using System.Text;

namespace Ref.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            services.AddMediatR(typeof(Register).GetTypeInfo().Assembly);

            services.Configure<IISServerOptions>(opt =>
            {
                opt.AutomaticAuthentication = false;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                        var userId = int.Parse(context.Principal.Identity.Name);
                        var user = await userService.GetAsync(userId);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            var emailSettingsSection = Configuration.GetSection("EmailSettings");
            services.Configure<EmailSettings>(emailSettingsSection);

            var emailSettings = emailSettingsSection.Get<EmailSettings>();

            services.AddTransient<IEmailProvider>(
                s => new EmailProvider(
                    emailSettings.Host,
                    emailSettings.ApiKey)
                );

            services.AddTransient<ISenderProvider>(
                s => new SenderProvider(
                    emailSettings.Sender,
                    emailSettings.ReplyTo)
                );

            services.AddTransient<IEmailNotification, EmailNotification>();

            services.AddScoped<IDbAccess>(
                db => new DbAccess(Configuration.GetConnectionString("RefDb")));

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IFilterRepository, FilterRepository>();
            services.AddTransient<ICitiesRepository, CitiesRepository>();
            services.AddTransient<IAdminInfosRepository, AdminInfosRepository>();
            services.AddTransient<ISiteRepository, SiteRepository>();
            services.AddTransient<IDistrictRepository, DistrictRepository>();
            services.AddTransient<IEventRepository, EventRepository>();

            services.AddScoped<IPasswordProvider, PasswordProvider>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}