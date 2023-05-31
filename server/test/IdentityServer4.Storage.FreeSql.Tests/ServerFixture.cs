// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FreeSql;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Tests
{
    public class ServerFixture
    {
        private static string TestMysqlConnection = "";// "server=192.168.0.221;port=3306;user id=root;password=UiBot2020;database=ids_admin_test;CharSet=utf8mb4;";

        private IServiceCollection _services = new ServiceCollection();

        public IServiceProvider ServiceProvider { get; private set; }

        public ServerFixture()
        {
            var cb = new ConfigurationBuilder();
            cb.AddUserSecrets<ServerFixture>();
            var configuration = cb.Build();

            TestMysqlConnection = configuration["TestMysqlConnection"];

            if (string.IsNullOrWhiteSpace(TestMysqlConnection))
            {
                throw new Exception("Not database connection string found, please add TestMysqlConnection to user secrets.");
            }

            _services.AddLogging(config =>
            {
                config.AddConsole();
            });

            //_services.AddScoped<IFreeSql>((provider) => GetConnection());

            _services.AddIdsStorageService(config =>
            {
                config.ConnectionString = TestMysqlConnection;
                config.DbType = DataType.MySql;
            });

            // _services.AddIdsInternalServices();

            _services.AddDataProtection();

            this.ServiceProvider = _services.BuildServiceProvider();



            this.InitDb();
        }

        public void InitDb()
        {
            RegisterExtensions.SyncDatabaseStructure(ServiceProvider, this.ServiceProvider.GetRequiredService<ILogger<ServerFixture>>());
        }

        public void CleanDb()
        {
            var mgr = ServiceProvider.GetRequiredService<DatabaseConnectionManager>();
            var freesql = mgr.GetConnection();
            freesql.Ado.ExecuteNonQuery("TRUNCATE table tbl_admin_api_resource;");
            freesql.Ado.ExecuteNonQuery("TRUNCATE table tbl_admin_api_scope;");
            freesql.Ado.ExecuteNonQuery("TRUNCATE table tbl_admin_client;");
            // freesql.Ado.ExecuteNonQuery("TRUNCATE table tbl_admin_identity_resource;");
            freesql.Ado.ExecuteNonQuery("delete from tbl_admin_identity_resource where is_preset =0;");
            freesql.Ado.ExecuteNonQuery("TRUNCATE table tbl_admin_reference_token;");
            freesql.Ado.ExecuteNonQuery("TRUNCATE table tbl_admin_refresh_token;");
            freesql.Ado.ExecuteNonQuery("TRUNCATE table tbl_admin_persisted_grant;");
            freesql.Ado.ExecuteNonQuery("TRUNCATE table tbl_admin_string_collection;");
            freesql.Ado.ExecuteNonQuery("TRUNCATE table tbl_admin_user;");
            freesql.Ado.ExecuteNonQuery("TRUNCATE table tbl_admin_ticket;");
            freesql.Ado.ExecuteNonQuery("TRUNCATE table tbl_system_keyvalue;");
        }

        //public IFreeSql GetConnection()
        //{
        //    var fb = new FreeSqlBuilder();
        //    fb.UseConnectionString(DataType.MySql, TestMysqlConnection)
        //        .UseGenerateCommandParameterWithLambda(true)
        //        .UseAutoSyncStructure(false);

        //    return fb.Build();
        //}
    }
}
