using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using IdentityServer4.Storage.FreeSql.Types;
using Mobwiz.Common.BaseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Collections.Generic;
using IdentityServer4.Admin.WebApi.Utils;
using Org.BouncyCastle.Crypto;
using AutoMapper;

namespace IdentityServer4.Admin.WebApi.Controllers.Client
{
    /// <summary>
    /// Ids 客户端管理接口
    /// </summary>
    [ApiController]
    [Route("api/admin/client")]
    [Authorize]
    public class ClientController : AdminControllerBase, IApiResult
    {
        private IDbClientService _clientService;
        private IMapper _mapper;

        public ClientController(IDbClientService clientService, IMapper mapper)
        {
            _clientService = clientService;
            _mapper = mapper;
        }


        /// <summary>
        /// 查询所有 client
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="onlyEnabled"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<BaseResult<PagedResult<ClientListInfo>>> GetClients(string? keyword, int pageIndex, int pageSize, bool onlyEnabled = false)
        {
            var response = await _clientService.QueryClientsAsync(new Storage.FreeSql.Services.Requests.QueryClientsRequest
            {
                Enabled = onlyEnabled ? BoolCondition.True : BoolCondition.All,
                Keyword = keyword ?? string.Empty,
                PageSize = pageSize > 0 ? pageSize : 10,
                PageIndex = pageIndex > 0 ? pageIndex : 1
            });

            var items = response.Items.Select(p => new ClientListInfo
            {
                Id = p.Id,
                ClientId = p.ClientId,
                ClientName = p.ClientName,
                ClientDescription = p.ClientDescription,
                LogoUri = p.LogoUri,
                ClientUri = p.ClientUri,
                Enabled = p.Enabled == 1,
                DisplayOrder = p.DisplayOrder,
            }).ToList();

            var pagedResult = new PagedResult<ClientListInfo>(response.TotalCount, items);

            return new BaseResult<PagedResult<ClientListInfo>>(pagedResult);
        }

        /// <summary>
        /// 获取客户端详情
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet("get")]
        public async Task<BaseResult<ClientVo>> GetClientDetail(string clientId)
        {
            var clientInfo = await _clientService.GetClientByClientIdAsync(new GetClientByClientIdRequest { ClientId = clientId });

            return this.OkResult(_mapper.Map<ClientVo>(clientInfo));
        }

        /// <summary>
        /// 保存客户端
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<BaseResult> SaveClient([FromBody] ClientInputModel model)
        {
            var info = new ClientDto()
            {
                Id = model.id,
                ClientName = model.clientName,
                Enabled = model.enabled ? (byte)1 : (byte)0,
                AccessTokenType = (EAccessTokenType)model.accessTokenType,
                ClientUri = model.clientUri,
                ClientDescription = model.clientDescription,
                ClientId = model.clientId,
                RequireConsent = model.requireConsent ? (byte)1 : (byte)0,
                RequirePkce = model.requirePkce ? (byte)1 : (byte)0,
                AllowOfflineAccess = model.allowOfflineAccess ? (byte)1 : (byte)0,
                FrontChannelLogoutUri = model.frontChannelLogoutUri,
                BackChannelLogoutUri = model.backChannelLogoutUri,
                LogoUri = model.logoUri,
                RequireClientSecret = model.requireClientSecret ? (byte)1 : (byte)0,
                AllowRememberConsent = model.allowRememberConsent ? (byte)1 : (byte)0,
                TokenLifetime = model.tokenLifetime,                
            };

            info.AllowedGrantTypes.AddRange(model.allowedGrantTypes.Where(str => !string.IsNullOrWhiteSpace(str)));
            info.RedirectUris.AddRange(model.redirectUris.Where(str => !string.IsNullOrWhiteSpace(str)));
            info.PostLogoutRedirectUris.AddRange(model.postLogoutRedirectUris.Where(str => !string.IsNullOrWhiteSpace(str)));
            info.AllowedCorsOrigins.AddRange(model.allowedCorsOrigins.Where(str => !string.IsNullOrWhiteSpace(str)));
            info.ClientSecrets.AddRange(model.clientSecrets.Where(str => !string.IsNullOrWhiteSpace(str)));
            info.AllowedScopes.AddRange(model.allowedScopes.Where(str => !string.IsNullOrWhiteSpace(str)));
            // info.ClientClaims.AddRange(model.clientClaims.Where(str => !string.IsNullOrWhiteSpace(str)));
            info.ClientClaims.AddRange(model.clientClaims.Select(p => new ClientClaim { Type = p.Type, Value = p.Value, ValueType = "string" }));

            if (model.id > 0)
            {
                await _clientService.UpdateClientAsync(new UpdateClientRequest
                {
                    Client = info,
                    Operator = this.UserName,
                });
            }
            else if (model.id == 0)
            {
                await _clientService.CreateClientAsync(new CreateClientRequest { Operator = this.UserName, Client = info });
            }
            else
            {
                return this.FailResult(400, "Parameters error");
            }

            return this.OkResult();
        }

        /// <summary>
        /// 删除客户端
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("remove")]
        public async Task<BaseResult> RemoveClient([FromBody] RemoveClientModel model)
        {
            var client = await _clientService.GetClientByClientIdAsync(new GetClientByClientIdRequest
            {
                ClientId = model.ClientId,
            });

            if (client?.Id == model.Id)
            {
                await _clientService.RemoveClientAsync(new RemoveClientRequest { Id = model.Id, Operator = this.UserName });
                return this.OkResult();
            }

            return this.FailResult(500, "Delete failed");
        }
    }

    public class ClientVo
    {
        public long Id { get; set; }

        public bool Enabled { get; set; }

        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string ClientUri { get; set; }

        public string LogoUri { get; set; }

        public string ClientDescription { get; set; }

        public bool RequireConsent { get; set; }

        public bool AllowRememberConsent { get; set; }

        public bool RequirePkce { get; set; }

        public string FrontChannelLogoutUri { get; set; }

        public string BackChannelLogoutUri { get; set; }

        public bool AllowOfflineAccess { get; set; }

        public bool RequireClientSecret { get; set; }

        public EAccessTokenType AccessTokenType { get; set; }

        public int TokenLifetime { get; set; }


        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }


        /// <summary>
        /// 创建时间，timestamp，秒
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 创建时间，timestamp，秒
        /// </summary>
        public DateTime UpdateTime { get; set; }


        public IList<string> AllowedGrantTypes { get; set; } = new List<string>();
        public IList<string> RedirectUris { get; set; } = new List<string>();
        public IList<string> PostLogoutRedirectUris { get; set; } = new List<string>();
        public IList<string> ClientSecrets { get; set; } = new List<string>();
        public IList<string> AllowedScopes { get; set; } = new List<string>();
        public IList<string> AllowedCorsOrigins { get; set; } = new List<string>();
        public IList<ClientClaim> ClientClaims { get; set; } = new List<ClientClaim>();
    }
}
