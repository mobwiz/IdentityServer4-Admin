using AutoMapper;
using IdentityServer4.Admin.WebApi.Utils;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.BaseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace IdentityServer4.Admin.WebApi.Controllers.Resource
{
    /// <summary>
    /// Identity 资源接口
    /// </summary>
    [Route("api/admin/identityResource")]
    [ApiController]
    [Authorize]
    public class IdentityResourceController : AdminControllerBase, IApiResult
    {
        private IDbIdentityResourceService _identityResourceService;
        private IMapper _mapper;

        public IdentityResourceController(IDbIdentityResourceService identityResourceService, IMapper mapper)
        {
            _identityResourceService = identityResourceService;
            _mapper = mapper;
        }

        /// <summary>
        /// 读取所有资源，包含 API Scope
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<BaseResult<PagedResult<IdentityResourceVo>>> GetAllIdentityResources(string? keyword, int pageIndex, int pageSize, bool onlyEnabled = false)
        {
            var response = await _identityResourceService.QueryIdentityResourcesAsync(new QueryIdentityResourcesRequest
            {
                Enabled = onlyEnabled ? BoolCondition.True : BoolCondition.All,
                Keyword = keyword ?? string.Empty,
                PageSize = pageSize > 0 ? pageSize : 10,
                PageIndex = pageIndex > 0 ? pageIndex : 1
            });

            return new BaseResult<PagedResult<IdentityResourceVo>>(
                new PagedResult<IdentityResourceVo>(response.TotalCount, response.Items.Select(_mapper.Map<IdentityResourceVo>))
                );
        }

        [HttpPost("save")]
        public async Task<BaseResult> SaveIdentityResource([FromBody] IdentityResourceSaveModel model)
        {
            var resourceInfo = new IdentityResourceDto
            {
                Id = model.Id,
                Name = model.Name,
                DisplayName = model.DisplayName,
                Enabled = model.Enabled ? (byte)1 : (byte)0,
            };

            resourceInfo.Claims.AddRange(model.claims);

            if (resourceInfo.Id > 0)
            {
                await _identityResourceService.UpdateIdentityResourceAsync(new UpdateIdentityResourceRequest
                {
                    Resource = resourceInfo,
                    Operator = this.UserName
                });
            }
            else
            {
                await _identityResourceService.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
                {
                    Resource = resourceInfo,
                    Operator = this.UserName
                });
            }

            return this.OkResult();
        }

        [HttpPost("remove")]
        public async Task<BaseResult> RemoveIdentityResource([FromBody] RemoveModel model)
        {
            await _identityResourceService.RemoveIdentityResourceAsync(new RemoveIdentityResourceRequest { Id = model.Id, Operator = this.UserName });

            return this.OkResult();
        }

        [HttpGet("getAvaibleScopes")]
        public async Task<BaseResult<AllResourceResult>> GetAllScopes([FromServices] IDbApiScopeService scopeService, [FromQuery] string? keyword = "")
        {
            var identitResources = await _identityResourceService.QueryIdentityResourcesAsync(new QueryIdentityResourcesRequest
            {
                Keyword = keyword ?? "",
                Enabled = BoolCondition.True,
                PageIndex = 1,
                PageSize = 9999
            });

            var scopes = await scopeService.QueryApiScopesAsync(new QueryApiScopesRequest
            {
                Keyword = keyword ?? "",
                Enabled = BoolCondition.True,
                PageIndex = 1,
                PageSize = 9999
            });

            var list = identitResources.Items.Select(p => new ScopeItem { Name = p.Name, Description = p.DisplayName }).ToList();
            list.AddRange(scopes.Items.Select(p => new ScopeItem { Name = p.Name, Description = p.DisplayName }));

            return this.OkResult(new AllResourceResult() { AvaiableScopes = list });
        }
    }

    public class AllResourceResult
    {
        public IList<ScopeItem> AvaiableScopes { get; set; }
    }

    public class ScopeItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class IdentityResourceSaveModel
    {
        public long Id { get; set; }
        public bool Enabled { get; set; }
        public bool Emphasize { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string DisplayName { get; set; }
        public string[] claims { get; set; }
    }

    public class IdentityResourceVo
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Reqquired
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Emphasize { get; set; }

        /// <summary>
        /// enabled
        /// </summary>
        public bool Enabled { get; set; }

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

        public IList<string> Claims { get; set; } = new List<string>();

        public int IsPreset { get; set; }
    }
}
