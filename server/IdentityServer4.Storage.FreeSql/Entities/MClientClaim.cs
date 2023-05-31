using FreeSql.DataAnnotations;

/*
 * 2022年5月8日 需要做一些改动，把这个表拆成两个
 * 1. AdminUI 可以修改的字段
 * 2. 系统接入可以修改的内容
 *      clientName
 *      增加 create_by, create_time 字段
 *      增加 标识 inner_system 内部系统，仅允许内部编辑
 *      
*/
namespace IdentityServer4.Storage.FreeSql.Entities
{
    [Table(Name = "tbl_admin_client_claim")]
    [Index(name: "idx_ids_client_claim_client_id", "client_id", false)]
    public class MClientClaim : IModel
    {
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }
        [Column(Name = "client_id", IsNullable = false)]
        public long ClientId { get; set; }
        [Column(Name = "type", IsNullable = false, StringLength = 127)]
        public string Type { get; set; }
        [Column(Name = "value", IsNullable = false, StringLength = 127)]
        public string Value { get; set; }
    }

    //public class ClientClaim
    //{
    //    public string Type { get; set; }
    //    public string Value { get; set; }
    //    public string ValueType { get; set; }
    //}
}
