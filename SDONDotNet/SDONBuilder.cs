using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using SDON.Model;

namespace SDON
{
    /// <summary>
    /// Utility class for bulding SDON.
    /// </summary>
    public class SDONBuilder
    {
        /// <summary>
        /// Turns the SDON Diagram into a JSON string.
        /// </summary>
        /// <param name="diagram">The diagram to turn into JSON.</param>
        /// <returns></returns>
        public static string ToJSON(Diagram diagram)
        {
            if (diagram == null) return null;

            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
            settings.EmitTypeInformation = EmitTypeInformation.Never;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Diagram), settings);
            MemoryStream stream = new MemoryStream();

            serializer.WriteObject(stream, diagram);
            stream.Seek(0, SeekOrigin.Begin);

            string json = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            return json;
        }

        /// <summary>
        /// Turns a JSON string into a Diagram object.
        /// </summary>
        /// <param name="json">The SDON JSON.</param>
        /// <returns></returns>
        public static Diagram FromJSON(string json)
        {
            if (json == null) return null;

            if (json.Contains("\"__type\":\"Shape:#SDON.Model\"") == true) //if there is type information in the JSON we must use the DataContract serializer. If not, we have to use the JavaScript serializer.
            {
                var serializer = new DataContractJsonSerializer(typeof(Diagram));
                MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

                Diagram diagram = (Diagram)serializer.ReadObject(stream);

                return diagram;
            }
            else
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Diagram diagram = serializer.Deserialize<Diagram>(json);

                return diagram;
            }
        }
    }
}
