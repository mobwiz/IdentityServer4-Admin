using IdentityServer4.Storage.FreeSql.Entities;
using Mobwiz.Common.BaseTypes;

namespace Laiye.SaasUC.Service.UserModule.Repositories
{
    internal interface IClientRepository
    {
        /// <summary>
        /// create client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        Task<long> CreateClientAsync(MClient client);

        /// <summary>
        /// get valid cors origins
        /// </summary>
        /// <returns></returns>
        Task<IList<string>> GetAllowedCorsOriginsAsync();

        /// <summary>
        /// Get client by client id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<MClient?> GetClientByClientIdAsync(string clientId);

        /// <summary>
        /// Get client by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MClient?> GetClientByIdAsync(long id);

        /// <summary>
        /// Query clients
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<PagedResult<MClient>> QueryClientsAsync(ClientQueryParameter parameter);

        /// <summary>
        /// Remove client by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task RemoveClientAsync(long id);

        /// <summary>
        /// Update client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        Task UpdateClientAsync(MClient client);


        /// <summary>
        /// get valid redirect uris
        /// </summary>
        /// <returns></returns>
        Task<IList<string>> GetAllowedRedirectUrisAsync();

        /// <summary>
        /// get valid post logout uris
        /// </summary>
        /// <returns></returns>
        Task<IList<string>> GetAllowedPostLogoutUrisAsync();
    }
}