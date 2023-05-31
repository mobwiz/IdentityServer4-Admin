namespace IdentityServer4.Admin.WebApi.Controllers.Client
{
    public class ClientListInfo
    {
        public long Id { get; set; }
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public string ClientDescription { get; set; }
        public string LogoUri { get; set; }
        public string ClientId { get; set; }
        public bool Enabled { get; set; }
        public bool ShowInApplist { get; set; }
        public bool ShowInWellknown { get; set; }

        public string WellknowServiceKey { get; set; }

        public int DisplayOrder { get; set; }

        public string ApiBaseUri { get; set; }
    }

}
