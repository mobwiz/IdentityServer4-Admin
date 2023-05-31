using AutoMapper;
using FreeSql;
using FreeSql.DataAnnotations;
using IdentityServer4.Storage.FreeSql.Config;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Repositories;
using IdentityServer4.Storage.FreeSql.Repositories.Impl;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Impl;
using IdentityServer4.Storage.FreeSql.Storage;
using IdentityServer4.Storage.FreeSql.Storage.KeyManager;
using IdentityServer4.Stores;
using Laiye.SaasUC.Service.UserModule.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mobwiz.Common.Exceptions;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace IdentityServer4.Storage.FreeSql
{


    public class DatabaseOption
    {
        public const string TablePrefix = "tbl";

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataType DbType { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 是否启用脚本监视
        /// </summary>
        public bool Monitor { get; set; }
    }

    public static class RegisterExtensions
    {

        /// <summary>
        /// 注册FreeSql
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddIdsStorageService(this IServiceCollection services, Action<DatabaseOption> configure)
        {
            if (configure == null) throw new Exception("Configure action can't be null");

            var options = new DatabaseOption();
            configure(options);

            if (string.IsNullOrWhiteSpace(options.ConnectionString)) throw new ArgumentNullException(options.ConnectionString);

            services.Configure(configure);
            services.AddSingleton<DatabaseConnectionManager>();

            var connectionString = options.ConnectionString;
            var maskedConnStr = Regex.Replace(connectionString, @"password=[^;]*", "password=******", RegexOptions.IgnoreCase);
            Console.WriteLine($"Try to connect [{options.DbType}] {maskedConnStr}");

            services.AddAutoMapper(cfg =>
            {
                cfg.ValueTransformers.Add<string>(val => val ?? string.Empty);
            }, typeof(AutoMapperProfile));

            //services.AddTransient<StringCollectionRepositoryImpl>();
            services.AddTransient<IApiScopeRepository, ApiScopeRepositoryImpl>();
            services.AddTransient<IIdentityResourceRepository, IdentityResourceRepositoryImpl>();
            services.AddTransient<IApiResourceRepository, ApiResourceRepository>();
            services.AddTransient<IClientRepository, ClientRepositoryImpl>();
            services.AddTransient<IAdminUserRepository, AdminUserRepositoryImpl>();
            services.AddTransient<ITicketDataRepository, TicketDataRepositoryImpl>();
            services.AddTransient<IKeyValueRepository, KeyValueRepositoryImpl>();
            services.AddTransient<IPersistedGrantRepository, PersistedGrantRepositoryImpl>();
            services.AddTransient<IReferenceTokenRepository, ReferenceTokenRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepositoryImpl>();


            services.AddTransient<IDbApiScopeService, ApiScopeServiceImpl>();
            services.AddTransient<IDbIdentityResourceService, IdentityResourceServiceImpl>();
            services.AddTransient<IDbApiResourceService, ApiResourceServiceImpl>();
            services.AddTransient<IDbClientService, ClientServiceImpl>();
            services.AddTransient<IDbAdminUserService, AdminUserServiceImpl>();
            services.AddTransient<IDbTicketService, TicketServiceImpl>();
            services.AddTransient<IDbKeyValueService, KeyValueServiceImpl>();
            services.AddTransient<IDbPersistedGrantService, PersistedGrantServiceImpl>();
            services.AddTransient<IDbRefreshTokenService, RefreshTokenServiceImpl>();
            services.AddTransient<IDbReferenceTokenService, ReferenceTokenServiceImpl>();

            services.AddSingleton<RotateKeyManager>();
            services.TryAddTransient<ISigningCredentialStore, DbSigningCredentialStoreImpl>();
            services.TryAddTransient<IValidationKeysStore, DbValidationKeysStoreImpl>();


            return services;
        }

        /// <summary>
        /// 注册IdentityServer4
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddFreesqlStorage(this IIdentityServerBuilder builder)
        {



            builder
                .AddClientStore<ClientStoreImpl>()
                .AddResourceStore<ResourceStoreImpl>();

            builder.AddCorsPolicyService<CorsPolicyServiceImpl>();

            builder.Services.RemoveAll<IPersistedGrantStore>();
            builder.Services.RemoveAll<IReferenceTokenStore>();
            builder.Services.RemoveAll<IRefreshTokenStore>();

            builder.AddPersistedGrantStore<PersistedGrantStoreImpl>();
            builder.Services.TryAddTransient<IReferenceTokenStore, ReferenceTokenStoreImpl>();
            builder.Services.TryAddTransient<IRefreshTokenStore, RefreshTokenStoreImpl>();


            return builder;
        }


        /// <summary>
        /// 同步数据库结构
        /// </summary>
        /// <param name="lifetime"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static IHostApplicationLifetime SyncFreesqlStorageDatabase(this IHostApplicationLifetime lifetime,
            IServiceProvider serviceProvider,
            ILogger logger
            )
        {
            lifetime.ApplicationStarted.Register(() =>
            {
                SyncDatabaseStructure(serviceProvider, logger);
            });

            return lifetime;
        }

        public static IHostApplicationLifetime InitApplication(this IHostApplicationLifetime lifetime,
            IServiceProvider serviceProvider,
            ILogger logger)
        {
            var initKey = "_ApplicationInitialized";
            var keyvalueService = serviceProvider.GetRequiredService<IDbKeyValueService>();

            var obj = keyvalueService.GetItemAsync(initKey).Result;

            if (obj == null)
            {
                logger.LogInformation("System is not initialized, try to init...");
                // do init
                var userService = serviceProvider.GetRequiredService<IDbAdminUserService>();

                try
                {
                    userService.CreateAdminUserAsync(new Services.Requests.CreateAdminUserRequest
                    {
                        UserName = "admin",
                        Password = "admin123456",
                        Operator = "Initializer"
                    }).GetAwaiter().GetResult();

                }
                catch (BllException blex)
                {
                    logger.LogWarning(blex, "Can't init");

                    return lifetime;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Can't init");
                    return lifetime;
                }
                finally
                {
                    keyvalueService.SetItemAsync(new Services.Requests.SetKeyValueItemRequest
                    {
                        Key = initKey,
                        Value = DateTime.Now.ToString(),
                        Operator = "Initializer"
                    }).GetAwaiter().GetResult();
                }

                logger.LogInformation("Init succeed");
            }

            return lifetime;
        }



        public static void SyncDatabaseStructure(IServiceProvider serviceProvider, ILogger logger)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var tableAttribteType = typeof(TableAttribute);

                var mgr = scope.ServiceProvider.GetRequiredService<DatabaseConnectionManager>();
                using var conn = mgr.GetConnection();
                var modelType = typeof(IModel);
                var types = Assembly.GetAssembly(typeof(RegisterExtensions))!
                    .GetTypes()
                        .Where(type => modelType.IsAssignableFrom(type) && type != modelType && type.CustomAttributes.Any(attr => attr.AttributeType == tableAttribteType)).ToArray()
                        .ToArray();

                logger.LogInformation("Sync database structures...");

                {
                    Parallel.ForEach(types, item =>
                    {
                        logger.LogInformation($"Sync Type: {item.FullName}");
                        conn.CodeFirst.SyncStructure(item);
                    });
                }

                logger.LogInformation($"Sync database structures successfull");

                logger.LogInformation($"check identity resource ...");

                var identityRepos = scope.ServiceProvider.GetRequiredService<IIdentityResourceRepository>();
                identityRepos.InitData().GetAwaiter().GetResult();
                logger.LogInformation($"check identity resource finished");

            }
        }
    }
}
