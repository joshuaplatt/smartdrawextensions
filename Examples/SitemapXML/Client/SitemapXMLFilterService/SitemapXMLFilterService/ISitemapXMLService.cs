using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SitemapXMLFilterService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISitemapXMLService" in both code and config file together.
    [DataContract]
    public class SitemapConverterOptions
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "tables")]
        public bool? Tables { get; set; }

        [DataMember(Name = "hyperlinks")]
        public bool? Hyperlinks { get; set; }

        [DataMember(Name = "lexicographicOrder")]
        public bool? LexicographicOrder { get; set; }
    }

    [ServiceContract]
    public interface ISitemapXMLService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/sitemapconvert/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string ConvertSitemap(SitemapConverterOptions options);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/sitemap-upload-block?uploadID={uploadID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string ConvertSitemapBlock(Guid uploadID, System.IO.Stream stream);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/sitemap-upload-final?uploadID={uploadID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string ConvertSitemapFinal(Guid uploadID, SitemapConverterOptions options);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/sitemap-retrieval/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetSitemapFromUrl(System.IO.Stream url);
    }
}
