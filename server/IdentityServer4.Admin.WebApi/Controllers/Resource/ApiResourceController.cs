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
    /// API 客户端接口
    /// </summary>
    [Route("api/admin/apiresource")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(DemoModeFilter))]
    public class ApiResourceController : AdminControllerBase, IApiResult
    {
        private IDbApiResourceService _apiResourceService;
        private IMapper _mapper;

        public ApiResourceController(IDbApiResourceService apiResourceService, IMapper mapper)
        {
            _apiResourceService = apiResourceService;
            _mapper = mapper;
        }

        /// <summary>
        /// 读取所有资源，包含 API Scope
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<BaseResult<PagedResult<ApiResourceVo>>> GetAllResources(string? keyword, int pageIndex, int pageSize, bool onlyEnabled = false)
        {
            var response = await _apiResourceService.QueryApiResourcesAsync(new QueryApiResourcesRequest
            {
                Enabled = onlyEnabled ? BoolCondition.True : BoolCondition.All,
                Keyword = keyword ?? string.Empty,
                PageSize = pageSize > 0 ? pageSize : 10,
                PageIndex = pageIndex > 0 ? pageIndex : 1
            });

            return new BaseResult<PagedResult<ApiResourceVo>>(new PagedResult<ApiResourceVo>(response.TotalCount, response.Items.Select(_mapper.Map<ApiResourceVo>)));
        }

        /// <summary>
        /// 保存 API 客户端
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<BaseResult> SaveApiResource([FromBody] ApiResourceSaveModel model)
        {
            var resourceInfo = new ApiResourceDto
            {
                Id = model.Id,
                Name = model.Name,
                DisplayName = model.DisplayName,
                Enabled = model.Enabled ? (byte)1 : (byte)0,
            };

            resourceInfo.Claims.AddRange(model.Claims);
            resourceInfo.Secrets.AddRange(model.Secrets);
            resourceInfo.Scopes.AddRange(model.Scopes);

            if (resourceInfo.Id > 0)
            {
                await _apiResourceService.UpdateApiResourceAsync(new UpdateApiResourceRequest
                {
                    ApiResource = resourceInfo,
                    Operator = this.UserName
                });
            }
            else
            {
                await _apiResourceService.CreateApiResourceAsync(new CreateApiResourceRequest
                {
                    ApiResource = resourceInfo,
                    Operator = this.UserName
                });
            }

            return this.OkResult();
        }

        /// <summary>
        /// 删除 API 客户端
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("remove")]
        public async Task<BaseResult> RemoveApiResource([FromBody] RemoveModel model)
        {
            await _apiResourceService.RemoveApiResourceAsync(new RemoveApiResourceRequest { Id = model.Id, Operator = this.UserName });

            return this.OkResult();
        }
    }

    public class ApiResourceSaveModel
    {
        public long Id { get; set; }
        public bool Enabled { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }
        public string[] Secrets { get; set; }
        public string[] Claims { get; set; }
        public string[] Scopes { get; set; }
    }

    public class ApiResourceVo
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

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

        public IList<string> Secrets { get; set; } = new List<string>();

        public IList<string> Scopes { get; set; } = new List<string>();

        public IList<string> Claims { get; set; } = new List<string>();
    }
}
