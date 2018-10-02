using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ERDFilterService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IERDFilterService" in both code and config file together.
    [ServiceContract]
    public interface IERDFilterService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/erd?showColumns={showColumns}&showTypes={showTypes}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string ConvertERD(string showColumns, string showTypes, System.IO.Stream csv);
    }
}
