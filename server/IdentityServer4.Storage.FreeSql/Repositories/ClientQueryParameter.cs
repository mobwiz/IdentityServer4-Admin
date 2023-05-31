using Mobwiz.Common.BaseTypes;

namespace Laiye.SaasUC.Service.UserModule.Repositories
{
    public class ClientQueryParameter
    {
        /// <summary>
        /// Keyword
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Page index
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Pagesizes
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// enabled only
        /// </summary>
        public BoolCondition Enabled { get; set; } = BoolCondition.All;
    }
}