// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FreeSql;
using FreeSql.Internal;
using Microsoft.Extensions.Options;

namespace IdentityServer4.Storage.FreeSql
{
    public class DatabaseConnectionManager
    {
        private DatabaseOption _option;

        public DatabaseConnectionManager(IOptions<DatabaseOption> options)
        {
            _option = options.Value;
        }

        public IFreeSql GetConnection()
        {
            var fb = new FreeSqlBuilder();
            fb.UseConnectionString(_option.DbType, _option.ConnectionString)
                .UseGenerateCommandParameterWithLambda(true)
                .UseAutoSyncStructure(false);

            if (_option.DbType == DataType.Dameng
            || _option.DbType == DataType.Oracle)
            {
                fb.UseNameConvert(NameConvertType.ToUpper);
            }

            return fb.Build();
        }
    }
}
