using IdentityServer4.Storage.FreeSql.Entities;
using Mobwiz.Common.BaseTypes;

namespace Laiye.SaasUC.Service.UserModule.Repositories
{
    internal interface IAdminUserRepository
    {
        Task<long> CreateAdminUserAsync(MAdminUser adminUser);

        Task UpdateAdminUserAsync(MAdminUser adminUser);

        Task RemoveAdminUserAsync(long id);

        Task<PagedResult<MAdminUser>> QueryAdminUsersAsync(string keyword, int page, int pageSize);

        Task<MAdminUser> GetAdminUserByIdAsync(long id);

        Task<MAdminUser> GetAdminUserByUsernameAsync(string username);
    }

}
