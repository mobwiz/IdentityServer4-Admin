// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FreeSql;
using IdentityServer4.Admin.WebApi.Config;
using IdentityServer4.Admin.WebApi.Filters;
using IdentityServer4.Admin.WebApi.Intergration.CsRedisCacheHelper;
using IdentityServer4.Admin.WebApi.Intergration.TicketStore;
using IdentityServer4.Admin.WebApi.Utils;
using IdentityServer4.Storage.FreeSql;
using Laiye.SaasMp.WebApi.Integration;
using Mobwiz.Common.BaseTypes;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using System.Text;

Logger logger = Helper.GetLogger(typeof(Program));

logger.Info("Starting application...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddUserSecrets<Program>();

    // 配置服务
    ConfigureServices(builder.Services, builder.Configuration);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    app.UseHealthChecks("/health");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Lifetime.SyncFreesqlStorageDatabase(app.Services, app.Logger);

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("Application start failed");
    Console.WriteLine(ex);
}
finally
{
    NLog.LogManager.Shutdown();
}

// Add services to the container.
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddControllers(config =>
    {
        config.Filters.Add<BllExceptionFilter>();
    });
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddHealthChecks();

    services.AddIdsStorageService(options =>
    {
        var dbType = configuration["Database:Type"];
        var connStr = configuration["Database:ConnectionString"];

        if (!Enum.TryParse<DataType>(dbType, ignoreCase: true, out var result)) throw new Exception("Database:Type is not correct");
        if (string.IsNullOrEmpty(connStr)) throw new Exception("Database connection string is not configured");

        options.DbType = result;
        options.ConnectionString = connStr;
    });

    services.Configure<RedisConfig>(configuration.GetSection("Redis"));
    services.Configure<SecurityOptions>(configuration.GetSection("Security"));

    services.AddSingleton<CSRedisClientManager>();
    services.AddSingleton<ICacheHelper, CsRedisCacheHelper>();

    services.AddTransient<SecurityChecker>();

    services.AddSingleton<ITicketStore, DbBasedTicketStore>();
    services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostConfgureLocalTicketStoreOptions>();
    services.AddAuthentication("Cookie")
        .AddCookie("Cookie", options =>
        {
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.HttpOnly = true;
            // options.Cookie.expi = TimeSpan.FromMinutes(30);
            options.Cookie.Name = ".idsadmin.session";

            options.Events.OnRedirectToLogin = async (options) =>
            {
                options.Response.StatusCode = 401;
                var result = new BaseResult(401, "Unauthorizaed");
                var json = JsonConvert.SerializeObject(result);
                await options.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(json));
            };

            options.Events.OnRedirectToLogout = async (options) =>
            {
                // do nothing
            };
        });

    services.AddDataProtection().PersistKeysToCacheHelper("ids.DataProtectionKeys");

    services.AddAutoMapper(typeof(AutoMapperProfile));
}


// add for test
public partial class Program { }