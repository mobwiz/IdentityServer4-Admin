// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FreeSql;
using IdentityServer4.Storage.FreeSql.Entities;

namespace IdentityServer4.Storage.FreeSql.Repositories.Impl
{
    public abstract class InnerBaseRepository
    {
        private DatabaseConnectionManager _connectionManager;
        //protected IFreeSql _conn;

        protected InnerBaseRepository(DatabaseConnectionManager connectionManager)
        {
            _connectionManager = connectionManager!;
        }

        protected IFreeSql GetConnection()
        {
            //_conn ??= _connectionManager!.GetConnection();

            //return _conn!;

            return _connectionManager.GetConnection();
        }
    }

    //public abstract class InnerBaseRepository<T> : InnerBaseRepository where T : class, IModel
    //{
    //    protected InnerBaseRepository(DatabaseConnectionManager connectionManager) : base(connectionManager)
    //    {
    //    }

    //    protected IFreeSql GetConnection()
    //    {
    //        return base.GetConnection();
    //    }

    //    protected IBaseRepository<T> GetRepository()
    //    {
    //        return GetConnection().GetRepository<T>();
    //    }
    //}
}
