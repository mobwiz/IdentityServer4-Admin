using IdentityServer4.Storage.FreeSql.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laiye.SaasUC.Service.UserModule.Repositories
{
    interface IAutoClean
    {
        Task CleanExpiredDataAsync();
    }

}
