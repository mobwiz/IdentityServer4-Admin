using IdentityServer4.Storage.FreeSql.Entities;
using Mobwiz.Common.BaseTypes;

namespace Laiye.SaasUC.Service.UserModule.Repositories
{
    internal interface IRefreshTokenRepository
    {
        /// <summary>
        /// clean expired data
        /// </summary>
        /// <returns></returns>
        Task CleanExpiredDataAsync();

        /// <summary>
        /// delete by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        Task DeleteByHandleAsync(string handle);

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
        Task<MRefreshToken> GetByHandleAsync(string handle);

        /// <summary>
        /// remove by sub
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        Task RemoveRefreshTokenBySubAsync(string subjectId);

        /// <summary>
        /// store refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<long> StoreRefreshTokenAsync(MRefreshToken refreshToken);

        /// <summary>
        /// update
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task UpdateRefreshTokenAsync(MRefreshToken refreshToken);

        /// <summary>
        /// Query Tokens
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="subjectId"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        Task<PagedResult<MRefreshToken>> QueryRefreshTokens(string clientId, string subjectId, string sessionId, string handle, int pageIndex, int pageSize);
    }
}