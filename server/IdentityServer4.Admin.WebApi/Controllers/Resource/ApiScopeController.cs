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
using IdentityServer4.Admin.WebApi.Filters;

namespace IdentityServer4.Admin.WebApi.Controllers.Resource
{
    /// <summary>
    /// API Scope 管理接口
    /// </summary>
    [Route("api/admin/apiscope")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(DemoModeFilter))]
    public class ApiScopeController : AdminControllerBase, IApiResult
    {
        private IDbApiScopeService _apiScopeService;
        private IMapper _mapper;

        public ApiScopeController(IDbApiScopeService apiScopeService, IMapper mapper)
        {
            _apiScopeService = apiScopeService;
            _mapper = mapper;
        }

        /// <summary>
        /// 读取所有资源，包含 API Scope
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<BaseResult<PagedResult<ApiScopeVo>>> GetAllResources(string? keyword, int pageIndex, int pageSize, bool onlyEnabled = false)
        {
            var response = await _apiScopeService.QueryApiScopesAsync(new QueryApiScopesRequest
            {
                Enabled = onlyEnabled ? BoolCondition.True : BoolCondition.All,
                Keyword = keyword ?? string.Empty,
                PageSize = pageSize > 0 ? pageSize : 10,
                PageIndex = pageIndex > 0 ? pageIndex : 1
            });

            return new BaseResult<PagedResult<ApiScopeVo>>(new PagedResult<ApiScopeVo>(response.TotalCount,
                response.Items.Select(_mapper.Map<ApiScopeVo>)
                ));
        }

        /// <summary>
        /// 保存 API Scope
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<BaseResult> CreateApiScope([FromBody] ApiScopeCreateModel model)
        {
            var scope = new ApiScopeDto()
            {
                Name = model.Name,
                Enabled = model.Enabled ? (byte)1 : (byte)0,
                Emphasize = model.Emphasize ? (byte)1 : (byte)0,
                DisplayName = model.DisplayName,
                Required = model.Required ? (byte)1 : (byte)0,
                Id = model.Id
            };

            if (model.Claims.Where(p => !string.IsNullOrWhiteSpace(p)).Any())
            {
                scope.Claims.AddRange(model.Claims);
            }
            else
            {
                scope.Claims.Add(scope.Name);
            }

            if (model.Id == 0)
            {
                await _apiScopeService.CreateApiScopeAsync(new CreateApiScopeRequest { ApiScope = scope, Operator = this.UserName });
            }
            else
            {
                await _apiScopeService.UpdateApiScopeAsync(new UpdateApiScopeRequest { ApiScope = scope, Operator = this.UserName });
            }

            return this.OkResult();
        }

        /// <summary>
        /// 删除 API Scope
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("remove")]
        public async Task<BaseResult> RemoveApiScope([FromBody] RemoveModel model)
        {
            await _apiScopeService.RemoveApiScopeAsync(new RemoveApiScopeRequest { Id = model.Id, Operator = this.UserName });

            return this.OkResult();
        }
    }

    public class ApiScopeCreateModel
    {
        public long Id { get; set; }
        public bool Enabled { get; set; }
        public bool Emphasize { get; set; }
        public bool Required { get; set; }

        [Required]
        [SecureText]
        public string Name { get; set; }

        [Required]
        [SecureText]
        public string DisplayName { get; set; }

        public string[] Claims { get; set; }
    }

    public class RemoveModel
    {
        [Required]
        [Range(1, long.MaxValue)]
        public long Id { get; set; }
    }

    public class ApiScopeVo
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool Required { get; set; }

        public bool Emphasize { get; set; }

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
    }

}
