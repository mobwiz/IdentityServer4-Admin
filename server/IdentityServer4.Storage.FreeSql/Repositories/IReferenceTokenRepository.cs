using IdentityServer4.Storage.FreeSql.Entities;
using Mobwiz.Common.BaseTypes;

namespace Laiye.SaasUC.Service.UserModule.Repositories
{
    internal interface IReferenceTokenRepository
    {
        /// <summary>
        /// clean expired data
        /// </summary>
        /// <returns></returns>
        Task CleanExpiredDataAsync();

        /// <summary>
        /// Delete by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        Task DeleteByHandleAsync(string handle);

        /// <summary>
        /// delete by delay
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        //Task DeleteByHandleWithDelayAsync(string handle);

        /// <summary>
        /// delete list by subjectId, clientId, companyId, sid
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="clientId"></param>
        /// <param name="sid"></param>
        /// 
        /// <returns></returns>
        Task<IList<string>> DeleteListAsync(string subjectId, string clientId, string sid);

        /// <summary>
        /// get by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        Task<MReferenceToken> GetByHandleAsync(string handle);

        /// <summary>
        /// get list by subjectId
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        Task<IList<string>> GetReferenceTokensAsync(string subjectId);

        /// <summary>
        /// remove by sub
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        Task RemoveReferenceTokenBySubAsync(string subjectId);

        /// <summary>
        /// store refresh token
        /// </summary>
        /// <param name="referenceToken"></param>
        /// <returns></returns>
        Task<long> StoreReferenceTokenAsync(MReferenceToken referenceToken);

        /// <summary>
        /// Query tokens
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="subjectId"></param>
        /// <param name="sid"></param>
        /// <param name="handle"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PagedResult<MReferenceToken>> QueryTokensAsync(string clientId, string subjectId, string sid, string handle, int pageIndex, int pageSize);
    }
}