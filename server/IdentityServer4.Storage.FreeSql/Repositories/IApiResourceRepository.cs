using IdentityServer4.Storage.FreeSql.Entities;
using Mobwiz.Common.BaseTypes;

namespace Laiye.SaasUC.Service.UserModule.Repositories
{
    /// <summary>
    /// Api Resource 接口
    /// </summary>
    internal interface IApiResourceRepository
    {
        /// <summary>
        /// 创建 Api  Resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        Task<long> CreateApiResourceAsync(MApiResource resource);

        /// <summary>
        /// 根据名称查找 Api Resource
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        Task<IList<MApiResource>> FindApiResourcesByNamesAsync(IList<string> names);

        /// <summary>
        /// 根据 ScopeName 查找 Api Resource
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns></returns>
        Task<IList<MApiResource>> FindApiResourcesByScopeNameAsync(IList<string> scopes);

        /// <summary>
        /// 获取所有 APiResource
        /// </summary>
        /// <returns></returns>
        Task<IList<MApiResource>> GetAllEnabledApiResourcesAsync();

        /// <summary>
        /// 删除  Api Resource
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task RemoveApiResourceAsync(long id);


        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MApiResource> GetApiResourceByIdAsync(long id);

        /// <summary>
        /// 更新 Api Resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        Task UpdateApiResourceAsync(MApiResource resource);

        /// <summary>
        /// Check if there is already a resource with the same name existed
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> SameNameExistedAsync(string name, long id);


        /// <summary>
        /// Query Api Resources
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="enabled"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PagedResult<MApiResource>> QueryApiResourcesAsync(string keyword, BoolCondition enabled, int pageIndex, int pageSize);
    }
}