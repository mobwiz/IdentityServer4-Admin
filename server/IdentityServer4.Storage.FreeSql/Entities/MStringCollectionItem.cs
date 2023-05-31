using FreeSql.DataAnnotations;
using IdentityServer4.Storage.FreeSql.Types;

namespace IdentityServer4.Storage.FreeSql.Entities
{
    [Table(Name = "tbl_admin_string_collection")]
    public class MStringCollectionItem : IModel
    {
        [Column(Name = "id", IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }

        [Column(Name = "string_type", DbType = "int")]
        public EStringType StringType { get; set; }

        [Column(Name = "parent_id")]
        public long ParentId { get; set; }

        [Column(Name = "string_value", StringLength = 255)]
        public string StringValue { get; set; }
    }

}
