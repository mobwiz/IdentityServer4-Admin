using IdentityServer4.Storage.FreeSql.Entities;
using Mobwiz.Common.BaseTypes;

namespace IdentityServer4.Storage.FreeSql.Repositories
{
    internal interface IApiScopeRepository
    {
        /// <summary>
        /// create scope
        /// </summary>
        /// <param name="apiScope"></param>
        /// <returns></returns>
        Task<long> CreateApiScopeAsync(MApiScope apiScope);

        /// <summary>
        /// find scopes by name
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        Task<IList<MApiScope>> FindApiScopesByNameAsync(IList<string> names);

        /// <summary>
        /// get all scopes
        /// </summary>
        /// <returns></returns>
        Task<IList<MApiScope>> GetAllApiScopesAsync();

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task RemoveApiScopeAsync(long id);

        /// <summary>
        /// update
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task UpdateApiScopeAsync(MApiScope scope);

        /// <summary>
        /// Query api scopes
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="enabled"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PagedResult<MApiScope>> QueryApiScopes(string keyword, BoolCondition enabled, int pageIndex, int pageSize);

        /// <summary>
        /// Get single scope by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MApiScope> GetApiScopeByIdAsync(long id);

        /// <summary>
        /// 检查是否存在同名 scope
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> SameNameExistedAsync(string name, long id);
    }
}