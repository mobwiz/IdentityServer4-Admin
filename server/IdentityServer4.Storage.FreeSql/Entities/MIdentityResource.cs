using FreeSql.DataAnnotations;
using NetTopologySuite.Operation.Distance;

namespace IdentityServer4.Storage.FreeSql.Entities
{
    [Table(Name = "tbl_admin_identity_resource")]
    [Index("idx_name", "name", true)]
    public class MIdentityResource : IModel
    {
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        [Column(Name = "name", StringLength = 127, IsNullable = false)]
        public string Name { get; set; }

        [Column(Name = "display_name", StringLength = 255, IsNullable = false)]
        public string DisplayName { get; set; }

        [Column(Name = "required", IsNullable = false)]
        public byte Required { get; set; }

        [Column(Name = "emphasize", IsNullable = false)]
        public byte Emphasize { get; set; }

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
        public ICollection<string> Claims { get; set; }

        /// <summary>
        /// 是否预置，预置的不能删除，不能修改
        /// </summary>
        [Column(Name = "is_preset", IsNullable = false, CanUpdate = false)]
        public int IsPreset { get; set; } = 0;

    }
}
