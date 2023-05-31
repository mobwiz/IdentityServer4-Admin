using IdentityServer4.Storage.FreeSql.Entities;

namespace Laiye.SaasUC.Service.UserModule.Repositories
{
    internal interface IPersistedGrantRepository
    {
        /// <summary>
        /// Clean expired data
        /// </summary>
        /// <returns></returns>
        Task CleanExpiredDataAsync();

        /// <summary>
        /// Delete by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task DeleteByKeyAsync(string key);

        /// <summary>
        /// Delete list by subjectId, clientId, sessionId, type
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="clientId"></param>
        /// <param name="sessionId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task DeleteListAsync(string subjectId, string clientId, string sessionId, string type);

        /// <summary>
        /// Get by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<MPersistedGrant> GetByKeyAsync(string key);

        /// <summary>
        /// Get list by subjectId, clientId, sessionId, type
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="clientId"></param>
        /// <param name="sessionId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<MPersistedGrant>> GetPersistedGrantsAsync(string subjectId, string clientId, string sessionId, string type);

        /// <summary>
        /// store persisted grant
        /// </summary>
        /// <param name="persistedGrant"></param>
        /// <returns></returns>
        Task StorePersistedGrantAsync(MPersistedGrant persistedGrant);
    }
}