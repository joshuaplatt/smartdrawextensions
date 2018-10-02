using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ERDFilterService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ERDFilterService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ERDFilterService.svc or ERDFilterService.svc.cs at the Solution Explorer and start debugging.
    public class ERDFilterService : IERDFilterService
    {
        public string ConvertERD(string showColumns, string showTypes, System.IO.Stream csv)
        {
            string erd = "";

            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                csv.CopyTo(ms);
                byte[] buff = ms.ToArray();

                ERD.ERDFilter conv = new ERD.ERDFilter();

                if(showColumns != null)
                {
                    conv.ShowColumns = (showColumns != "0");
                }
                if(showTypes != null)
                {
                    conv.ShowDataTypes = (showTypes != "0");
                }

                conv.LoadCSVFromString(UTF8Encoding.ASCII.GetString(buff));
                SDON.Model.Diagram diagram = conv.ConvertDatabase();
                erd = SDON.SDONBuilder.ToJSON(diagram);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                return "Error occurred when trying to parse CSV:\n" + e.Message;
            }

            return erd;
        }
    }
}
