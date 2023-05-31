using IdentityServer4.Storage.FreeSql.Entities;
using Mobwiz.Common.BaseTypes;

namespace IdentityServer4.Storage.FreeSql.Repositories
{
    internal interface IIdentityResourceRepository
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="identityResource"></param>
        /// <returns></returns>
        Task<long> CreateIdentityResourceAsync(MIdentityResource identityResource);

        /// <summary>
        /// Find by names
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        Task<IList<MIdentityResource>> FindIdentityResourcesByScopeNameAsync(IList<string> names);

        /// <summary>
        /// get all enabled
        /// </summary>
        /// <returns></returns>
        Task<IList<MIdentityResource>> GetAllEnabledIdentityResourcesAsync();

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task RemoveIdentityResourceAsync(long id);

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MIdentityResource> GetIdentityResourceByIdAsync(long id);

        /// <summary>
        /// update
        /// </summary>
        /// <param name="identityResource"></param>
        /// <returns></returns>
        Task UpdateIdentityResourceAsync(MIdentityResource identityResource);

        /// <summary>
        /// Query
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="enabled"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PagedResult<MIdentityResource>> QueryIdentityResourcesAsync(string keyword, BoolCondition enabled, int pageIndex, int pageSize);

        /// <summary>
        /// 检查是否存在同名 scope
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> SameNameExistedAsync(string name, long id);

        /// <summary>
        /// 初始话数据
        /// </summary>
        /// <returns></returns>
        Task<bool> InitData();
    }

}