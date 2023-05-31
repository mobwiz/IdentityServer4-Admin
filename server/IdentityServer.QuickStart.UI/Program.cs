// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer.QuickStart.UI.Integration.CsRedisCacheHelper;
using IdentityServer4.Storage.FreeSql;
using Laiye.SaasMp.WebApi.Integration;
using Mobwiz.Common.General;

namespace IdentityServer.QuickStart.UI
{
    public class Program
    {
        private const string CookieAuthenicaionScheme = "DefaultCookies";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // *** Step 1:
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            ConfigureServices(builder.Services, builder.Configuration);

            // *** Step 2

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 启用 Identity Server
            app.UseIdentityServer();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisConfig>(configuration.GetSection("Redis"));
            services.AddSingleton<CSRedisClientManager>();
            services.AddSingleton<ICacheHelper, CsRedisCacheHelper>();

            services.AddSingleton<IIdGenerator, IdGenerator>();

            services.AddIdsStorageService((options) =>
            {
                var dbType = configuration["Database:Type"];
                var connStr = configuration["Database:ConnectionString"];

                if (!Enum.TryParse<FreeSql.DataType>(dbType, ignoreCase: true, out var result)) throw new Exception("Database:Type is not correct");
                if (string.IsNullOrEmpty(connStr)) throw new Exception("Database connection string is not configured");

                options.DbType = result;
                options.ConnectionString = connStr;
            });

            services.AddAuthentication();

            services.AddIdentityServer(options =>
            {
                //options.Authentication.CookieAuthenticationScheme = CookieAuthenicaionScheme;

                options.UserInteraction.LoginUrl = "/Account/Login";
                options.UserInteraction.LogoutUrl = "/Account/Logout";
                options.UserInteraction.ConsentUrl = "/Consent";

                options.Authentication.CookieSameSiteMode = SameSiteMode.Strict;
                options.Authentication.CheckSessionCookieSameSiteMode = SameSiteMode.Lax;                
            })
                .AddFreesqlStorage()
                ;

            //.AddCookie(CookieAuthenicaionScheme, options =>
            //{
            //    options.Cookie.Path = "/";

            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.SameSite = SameSiteMode.Strict;
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

            //    options.LoginPath = "/Account/Login";
            //    options.LogoutPath = "/Account/Logout";
            //});

            services.AddAntiforgery();
            services.AddAuthorization();
        }
    }
}