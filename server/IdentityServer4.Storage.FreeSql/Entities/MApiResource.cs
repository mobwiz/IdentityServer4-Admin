using FreeSql.DataAnnotations;

namespace IdentityServer4.Storage.FreeSql.Entities
{
    [Table(Name = "tbl_admin_api_resource")]
    [Index("idx_admin_api_resource_name", "name", true)]
    public class MApiResource : IModel
    {
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        [Column(Name = "name", StringLength = 127, IsNullable = false)]
        public string Name { get; set; }

        [Column(Name = "display_name", StringLength = 255, IsNullable = false)]
        public string DisplayName { get; set; }

        [Column(Name = "enabled", IsNullable = false)]
        public byte Enabled { get; set; }

        /// <summary>
        /// 创建时间，timestamp，秒
        /// </summary>
        [Column(Name = "create_time", IsNullable = false, CanUpdate = false)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column(Name = "create_by", IsNullable = false, CanUpdate = false, StringLength = 63)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column(Name = "update_by", IsNullable = false, CanUpdate = true, StringLength = 63)]
        public string UpdateBy { get; set; }

        /// <summary>
        /// 创建时间，timestamp，秒
        /// </summary>
        [Column(Name = "update_time", IsNullable = false, CanUpdate = true)]
        public DateTime UpdateTime { get; set; }

        [Column(IsIgnore = true)]
        public ICollection<string> Secrets { get; set; }

        [Column(IsIgnore = true)]
        public ICollection<string> Scopes { get; set; }

        [Column(IsIgnore = true)]
        public ICollection<string> Claims { get; set; }

    }



}
