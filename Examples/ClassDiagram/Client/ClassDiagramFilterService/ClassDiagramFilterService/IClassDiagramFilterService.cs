using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ClassDiagramFilterService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IClassDiagramFilterService" in both code and config file together.
    [ServiceContract]
    public interface IClassDiagramFilterService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ctags-block-init?uploadID={uploadID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string ConvertCtagsFileBlobInit(Guid uploadID, System.IO.Stream fileSection);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ctags-block-add?uploadID={uploadID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string ConvertCtagsFileBlobAdd(Guid uploadID, System.IO.Stream fileSection);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ctags-block-final?uploadID={uploadID}&hideMethods={hideMethods}&hideProperties={hideProperties}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string ConvertCtagsFileBlobFinal(Guid uploadID, string hideMethods, string hideProperties);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ctags-github-block?uploadID={uploadID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string ConvertCtagsGithubRepositoryStartRequest(Guid uploadID, System.IO.Stream fileSection);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ctags-github-final?uploadID={uploadID}&user={user}&repo={repo}&branch={branch}&accessToken={accessToken}&hideMethods={hideMethods}&hideProperties={hideProperties}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string ConvertCtagsGithubRepositoryFinalizeRequest(Guid uploadID, string user, string repo, string branch, string accessToken, string hideMethods, string hideProperties);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/github-get-userid?code={code}&state={state}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string CtagsGetGHUserID(string code, string state);
    }
}
