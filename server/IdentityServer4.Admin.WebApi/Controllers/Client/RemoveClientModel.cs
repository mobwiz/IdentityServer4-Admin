namespace IdentityServer4.Admin.WebApi.Controllers.Client
{
    /// <summary>
    /// 删除客户端请求
    /// </summary>
    public class RemoveClientModel
    {
        /// <summary>
        /// 客户端 Id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 数据库 Id
        /// </summary>
        public long Id { get; set; }
    }

}
