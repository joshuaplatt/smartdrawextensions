using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ClassDiagramFilterService
{
    class ClassDiagramFilterServiceGithubUserdata
    {
        public string User { get; set; }
        public string Repo { get; set; }
        public string Branch { get; set; }
    }

    [DataContract]
    class GithubRateLimitQuery
    {
        [DataMember]
        public CRate rate;

        [DataContract]
        public class CRate
        {
            [DataMember]
            public int limit = 0;

            [DataMember]
            public int remaining = 0;

            [DataMember]
            public int reset = int.MaxValue;
        }
    }

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ClassDiagramFilterService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ClassDiagramFilterService.svc or ClassDiagramFilterService.svc.cs at the Solution Explorer and start debugging.
    public class ClassDiagramFilterService : IClassDiagramFilterService
    {
        public const string UnixRootDir = "";
        public const string UnixPathSeparator = "/";
        public const string UnixCtagsDir = UnixRootDir + UnixPathSeparator + "home" + UnixPathSeparator;

        public const string DosRootDir = "C:";
        public const string DosPathSeparator = "\\";
        public const string DosCtagsDir = DosRootDir + DosPathSeparator;

        public const string CurrRootDir = DosRootDir;
        public const string CurrPathSeparator = DosPathSeparator;
        public const string CurrCtagsDir = DosCtagsDir;

        public static string CtagsFileLocation = System.Configuration.ConfigurationManager.AppSettings["CtagsLocation"];
        public static string TempFileLocation = System.Configuration.ConfigurationManager.AppSettings["TempFileLocation"];

        public const string EnvironmentName = "SDENVIRONMENT";

        //Change the following before deploying live (or revoke access from account) --temp--
        public static string ClientID = "";
        public static string ClientSecret = "";

        private static string debugOutput = "";
        private static bool isLocal = false;

        static ClassDiagramFilterService()
        {
            Func < string, string > getPathFromResource = (Func < string, string > )(delegate (string resourcePath)
               {
                   if ((resourcePath == null) || (resourcePath.Length < 2))
                   {
                       return resourcePath;
                   }
                   else if (resourcePath[1] == ':')
                   {
                       if (resourcePath[0] == 'a') //absolute
                       {
                           return resourcePath.Substring(2);
                       }
                       else if (resourcePath[0] == 'r') //relative
                       {
                           return System.Web.Hosting.HostingEnvironment.MapPath(resourcePath.Substring(2));
                       }
                   }

                   return resourcePath;
               });

            CtagsFileLocation = getPathFromResource(CtagsFileLocation);
            TempFileLocation = getPathFromResource(TempFileLocation);

            string hostMod = "";

            if ((EnvironmentName == "DEV") || (EnvironmentName == "QA"))
            {
                hostMod = System.Web.HttpContext.Current.Request.Url.Scheme.ToLower();
            }

            ClientID = System.Configuration.ConfigurationManager.AppSettings["ClientID" + EnvironmentName + hostMod];
            ClientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret" + EnvironmentName + hostMod];

            if (ClientID == null)
            {
                ClientID = System.Configuration.ConfigurationManager.AppSettings["ClientIDLOCAL"];
                isLocal = true;
            }
            if (ClientSecret == null)
            {
                ClientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecretLOCAL"];
            }
        }
        
        public string ConvertCtagsFileBlobInit(Guid uploadID, System.IO.Stream fileSection)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            fileSection.CopyTo(ms);

            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-" + uploadID.ToString());
                if (!dir.Exists)
                {
                    dir.Create();
                }

                using (System.IO.FileStream fs = new System.IO.FileStream(dir.FullName + CurrPathSeparator + "temp.raw", System.IO.FileMode.Append))
                {
                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    ms.CopyTo(fs);
                }
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                return "Error while receiving data: " + e.Message;
            }

            return "";
        }

        public string ConvertCtagsFileBlobAdd(Guid uploadID, System.IO.Stream fileSection)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            fileSection.CopyTo(ms);

            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-" + uploadID.ToString());
                if (!dir.Exists)
                {
                    dir.Create();
                }

                using (System.IO.FileStream fs = new System.IO.FileStream(dir.FullName + CurrPathSeparator + "temp.raw", System.IO.FileMode.Append))
                {
                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    ms.CopyTo(fs);
                }
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                return "Error while receiving data: " + e.Message;
            }

            return "";
        }

        public string ConvertCtagsFileBlobFinal(Guid uploadID, string hideMethods, string hideProperties)
        {
            try
            {
                SplitCtagsFiles(uploadID);
                ProcessCtagsFiles(uploadID);
                return ReturnConvertedCtags(uploadID, hideMethods, hideProperties);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                return "Error on conversion: " + e.Message;
            }
        }

        private void SplitCtagsFiles(Guid uploadID)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-" + uploadID.ToString());
            System.IO.FileInfo rawfile = new System.IO.FileInfo(dir.FullName + CurrPathSeparator + "temp.raw");
            byte[] buff;
            string file;

            using (System.IO.FileStream fs = new System.IO.FileStream(rawfile.FullName, System.IO.FileMode.Open))
            {
                buff = new byte[fs.Length];
                fs.Read(buff, 0, (int)fs.Length);
            }

            rawfile.Delete();
            file = UnicodeEncoding.Unicode.GetString(buff);
            //file = UTF8Encoding.ASCII.GetString(buff);
            char[] splitChar = new char[1] { '\t' };
            int fileCount;
            int[] endOfLine = new int[2];
            int fileStart, fileEnd;

            endOfLine[0] = file.IndexOf('\n');

            if (endOfLine[0] <= 0)
            {
                throw new Exception("Error: Invalid message");
            }

            endOfLine[1] = file.IndexOf('\n', endOfLine[0] + 1);

            if (endOfLine[1] <= 0)
            {
                throw new Exception("Error: Invalid message");
            }

            string[] header1 = file.Substring(0, endOfLine[0]).Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
            string[] header2 = file.Substring(endOfLine[0] + 1, endOfLine[1] - endOfLine[0] - 1).Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
            fileCount = header2.Length;

            if (header1.Length != (header2.Length + 1))
            {
                throw new Exception("Error: Invalid message");
            }

            fileStart = (endOfLine[1] + 1) * 2;

            for (int i = 0; i < fileCount; i++)
            {
                fileEnd = int.Parse(header2[i]);

                if ((fileStart + fileEnd) > buff.Length)
                {
                    throw new Exception("Error: Invalid message");
                }

                using (System.IO.FileStream fs = new System.IO.FileStream(dir.FullName + CurrPathSeparator + header1[i + 1], System.IO.FileMode.Create))
                {
                    fs.Write(buff, fileStart, fileEnd);
                }

                fileStart += fileEnd;
            }
        }

        private void ProcessCtagsFiles(Guid uploadID)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-" + uploadID.ToString());
            System.IO.FileInfo[] files = dir.GetFiles();
            System.Diagnostics.Process proc = new System.Diagnostics.Process();

            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = TempFileLocation;
            proc.StartInfo.FileName = CtagsFileLocation;
            proc.StartInfo.Arguments = "-V -f \"" + dir.FullName + CurrPathSeparator +  "out.txt\" --fields=+zSKalin";

            for (int i = 0; i < files.Length; i++)
            {
                proc.StartInfo.Arguments += " \"" + files[i].FullName + "\"";
            }

            proc.StartInfo.UseShellExecute = false;
            proc.Start();

            string test = proc.StandardError.ReadToEnd();
            string test2 = proc.StandardOutput.ReadToEnd();

            proc.WaitForExit();



            if (proc.ExitCode != 0)
            {
                //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                //proc.StandardError.BaseStream.CopyTo(ms);
                throw new Exception("Ctags error:\n" + test);
            }
        }

        private string ReturnConvertedCtags(Guid uploadID, string hideMethods, string hideProperties)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-" + uploadID.ToString());
            ClassDiagram.ClassDiagramFilter filter = new ClassDiagram.ClassDiagramFilter();
            filter.SignatureInNote = true;

            if (!((hideMethods == null) || (hideMethods == "0")))
            {
                filter.MethodsInNote = true;
            }

            if (!((hideProperties == null) || (hideProperties == "0")))
            {
                filter.PropertiesInNote = true;
            }

            filter.LoadFromCtagsFile(dir.FullName + CurrPathSeparator + "out.txt");
            SDON.Model.Diagram diagram = filter.ConvertCtags();
            dir.Delete(true);

            return SDON.SDONBuilder.ToJSON(diagram);
        }

        public string ConvertCtagsGithubRepositoryStartRequest(Guid uploadID, System.IO.Stream fileSection)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            fileSection.CopyTo(ms);

            try
            {
                byte[] buff = ms.GetBuffer();
                string str = UnicodeEncoding.Unicode.GetString(buff, 0, (int)ms.Length);

                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-github-request-" + uploadID.ToString());
                if (!dir.Exists)
                {
                    dir.Create();
                }

                using (System.IO.StreamWriter fs = new System.IO.StreamWriter(dir.FullName + CurrPathSeparator + "exclusion.txt", true))
                {
                    fs.Write(str);
                }
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                return "Error while receiving data: " + e.Message;
            }

            return "";
        }

        public string ConvertCtagsGithubRepositoryFinalizeRequest(Guid uploadID, string user, string repo, string branch, string accessToken, string hideMethods, string hideProperties)
        {
            try
            {
                ClassDiagramFilterServiceGithubUserdata ud = new ClassDiagramFilterServiceGithubUserdata();
                ud.User = user;
                ud.Repo = repo;
                ud.Branch = branch;

                if (isLocal)
                {
                    debugOutput = "";
                }

                GetGithubRepositoryZip(uploadID, user, repo, branch, accessToken);
                RunCtagsOnGithubRepository(uploadID);
                return ConvertCtagsGithubFile(uploadID, ud, hideMethods, hideProperties);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/json";

                if (isLocal)
                {
                    debugOutput += "Error: " + e.Message + "\r\n";
                    debugOutput += "Stack trace: " + e.StackTrace;

                    try
                    {
                        using(System.IO.FileStream fs = new System.IO.FileStream(TempFileLocation + CurrPathSeparator + "tmp-github-request-" + uploadID.ToString() + CurrPathSeparator + "crashlog.txt", System.IO.FileMode.Create))
                        {
                            byte[] buff = Encoding.UTF8.GetBytes(debugOutput);
                            fs.Write(buff, 0, buff.Length);
                        }
                        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-github-request-" + uploadID.ToString() + CurrPathSeparator);
                    }
                    catch(Exception x)
                    {
                        return "Error when finalizing transfer: " + e.Message + "\n\nAdditionally, could not create output log.";
                    }
                }

                return "Error when finalizing transfer: " + e.Message;
            };
        }

        private void GetGithubRepositoryZip(Guid uploadID, string user, string repo, string branch, string accessToken)
        {
            if (!VerifyAccessToken(accessToken))
            {
                throw new Exception("User authentication not valid.");
            }

            if (isLocal)
            {
                debugOutput += "Passed verification\r\n";
            }

            string url = "https://api.github.com/repos/" + user + "/" + repo + "/zipball/" + branch + "?access_token=" + accessToken;
            System.IO.MemoryStream memstr = PerformHttpsRequest(url, 5, 100);

            if (isLocal)
            {
                debugOutput += "Recieved response from Github.  Saving...\r\n";
            }

            using (System.IO.FileStream fs = new System.IO.FileStream(TempFileLocation + CurrPathSeparator + "tmp-github-request-" + uploadID.ToString() + CurrPathSeparator + "archive.zip", System.IO.FileMode.Create))
            {
                memstr.CopyTo(fs);
            }

            ExtractFilesToRepositoryDir(uploadID);
        }

        private bool VerifyAccessToken(string accessToken)
        {
            string url = "https://api.github.com/" + "rate_limit?access_token=" + accessToken;
            System.IO.MemoryStream memstr = PerformHttpsRequest(url, 5, 100);

            System.Runtime.Serialization.Json.DataContractJsonSerializer serial = 
                new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GithubRateLimitQuery));

            GithubRateLimitQuery limit = (GithubRateLimitQuery)serial.ReadObject(memstr);

            if ((limit != null) && (limit.rate != null) && (limit.rate.limit > 60))
            {
                return true;
            }

            return false;
        }

        private void ExtractFilesToRepositoryDir(Guid uploadID)
        {
            if (isLocal)
            {
                debugOutput += "Saved successfully.  Extracting...\r\n";
            }

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-github-request-" + uploadID.ToString() + CurrPathSeparator + "repositoryTemp");
            System.IO.Compression.ZipFile.ExtractToDirectory(TempFileLocation + CurrPathSeparator + "tmp-github-request-" + uploadID.ToString() + CurrPathSeparator + "archive.zip", dir.FullName);
            System.IO.DirectoryInfo[] children = dir.GetDirectories();

            if (children.Length != 1)
            {
                throw new Exception("Internal repository error");
            }

            if (isLocal)
            {
                debugOutput += "Successfully extracted repository\r\n";
            }

            System.IO.DirectoryInfo dstDir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-github-request-" + uploadID.ToString() + CurrPathSeparator + "repository");

            if (dstDir.Exists)
            {
                dstDir.Delete(true);
            }

            dstDir.Create();

            System.IO.DirectoryInfo[] allDirs = children[0].GetDirectories();
            System.IO.FileInfo[] allFiles = children[0].GetFiles();

            for (int i = 0; i < allDirs.Length; i++)
            {
                allDirs[i].MoveTo(dstDir.FullName + CurrPathSeparator + allDirs[i].Name);
            }
            for (int i = 0; i < allFiles.Length; i++)
            {
                allFiles[i].MoveTo(dstDir.FullName + CurrPathSeparator + allFiles[i].Name);
            }

            if (isLocal)
            {
                debugOutput += "Moved to repository folder\r\n";
            }
        }

        private void RunCtagsOnGithubRepository(Guid uploadID)
        {
            if (isLocal)
            {
                debugOutput += "Beginning ctags running...\r\n";
            }

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-github-request-" + uploadID.ToString());
            System.IO.FileInfo[] files = dir.GetFiles();
            System.Diagnostics.Process proc = new System.Diagnostics.Process();

            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = dir.FullName;
            proc.StartInfo.FileName = CtagsFileLocation;
            proc.StartInfo.Arguments = "-V -f \"" + dir.FullName + CurrPathSeparator + "out.txt\" --fields=+zSKalin --exclude=@\"exclusion.txt\" --recurse=yes repository";
            //    TempFileLocation + CurrPathSeparator + "tmp-github-request-" + uploadID.ToString() + CurrPathSeparator + "exclude.txt\" \"" +
            //    TempFileLocation + CurrPathSeparator + "tmp-github-request-" + uploadID.ToString() + CurrPathSeparator + "repository\"";

            proc.StartInfo.UseShellExecute = false;
            proc.Start();

            if (isLocal)
            {
                debugOutput += "Process started, outputting to: \"" + dir.FullName + "\"\r\n";
            }

            string test = proc.StandardError.ReadToEnd();
            string test2 = proc.StandardOutput.ReadToEnd();

            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                //proc.StandardError.BaseStream.CopyTo(ms);
                throw new Exception("Ctags error:\n" + test);
            }
        }

        private string ConvertCtagsGithubFile(Guid uploadID, ClassDiagramFilterServiceGithubUserdata ud, string hideMethods, string hideProperties)
        {
            if (isLocal)
            {
                debugOutput += "Ctags successful, running filter...\r\n";
            }

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-github-request-" + uploadID.ToString());
            ClassDiagram.ClassDiagramFilter filter = new ClassDiagram.ClassDiagramFilter();
            filter.SignatureInNote = true;

            if (!((hideMethods == null) || (hideMethods == "0")))
            {
                filter.MethodsInNote = true;
            }

            if (!((hideProperties == null) || (hideProperties == "0")))
            {
                filter.PropertiesInNote = true;
            }

            filter.LinkGenerator = GetLinkFromInfo;
            filter.Userdata = ud;
            filter.LoadFromCtagsFile(dir.FullName + CurrPathSeparator + "out.txt");
            SDON.Model.Diagram diagram = filter.ConvertCtags();

            if (isLocal)
            {
                debugOutput += "Filter successful\r\n";
            }

            if (!isLocal)
            {
                dir.Delete(true);
            }

            return SDON.SDONBuilder.ToJSON(diagram);
        }

        private string GetLinkFromInfo(string file, int line, Object userdata)
        {
            int endPrefix = file.IndexOf("repository" + CurrPathSeparator);

            if ((endPrefix == -1) || (userdata.GetType() != typeof(ClassDiagramFilterServiceGithubUserdata)))
            {
                return null;
            }

            ClassDiagramFilterServiceGithubUserdata ud = (ClassDiagramFilterServiceGithubUserdata)userdata;
            string[] path = file.Substring(endPrefix + ("repository" + CurrPathSeparator).Length).Split(new string[1] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            string pathName = "";

            for (int i = 0; i < path.Length; i++)
            {
                pathName += "/" + path[i];
            }

            return "https://github.com/" + ud.User + "/" + ud.Repo + "/blob/" + ud.Branch + pathName + "#L" + line.ToString();
        }

        public string CtagsGetGHUserID(string code, string state)
        {
            string tempClientID = "00";
            string tempClientSecret = "00";
            string env = EnvironmentName;

            try
            {
                string hostMod = "";

                if ((EnvironmentName == "DEV") || (EnvironmentName == "QA") || (DateTime.Now.Equals(DateTime.MaxValue)))
                {
                    hostMod = System.Web.HttpContext.Current.Request.Url.Scheme.ToLower();
                    env += hostMod;
                }

                tempClientID = System.Configuration.ConfigurationManager.AppSettings["ClientID" + EnvironmentName + hostMod];
                tempClientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret" + EnvironmentName + hostMod];

                if (tempClientID == null)
                {
                    tempClientID = System.Configuration.ConfigurationManager.AppSettings["ClientIDLOCAL"];
                    isLocal = true;
                }
                if (tempClientSecret == null)
                {
                    tempClientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecretLOCAL"];
                }

                string url = "https://www.github.com/login/oauth/access_token?client_id=" + tempClientID + "&client_secret=" + tempClientSecret + "&code=" + code + "&state=" + state;
                System.IO.MemoryStream memstr = PerformHttpsRequest(url, 5, 100);

                string ret = UTF8Encoding.UTF8.GetString(memstr.GetBuffer());
                string[] split = ret.Split('&');
                ret = "";
                string[] setting;

                for (int i = 0; i < split.Length; i++)
                {
                    setting = split[i].Split('=');

                    if ((setting.Length == 2) && (setting[0] == "access_token"))
                    {
                        ret = setting[1];
                        break;
                    }
                }

                if (ret.Length == 0)
                {
                    throw new Exception();
                }

                return ret;
            }
            catch(Exception e) 
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/json";
                WebOperationContext.Current.OutgoingResponse.Headers.Add("CLASS-DIAGRAM-DEBUG-ENVIRONMENT", EnvironmentName);
                WebOperationContext.Current.OutgoingResponse.Headers.Add("CLASS-DIAGRAM-CLIENT-ID", tempClientID.Substring(0, 2));
                WebOperationContext.Current.OutgoingResponse.Headers.Add("CLASS-DIAGRAM-CLIENT-ID", tempClientSecret.Substring(0, 2));

                return "";
            }
        }

        private System.IO.MemoryStream PerformHttpsRequest(string url, int maxRetries, int msDelay)
        {
            System.Net.HttpWebRequest req;
            System.IO.MemoryStream memstr = null;

            System.Net.SecurityProtocolType proto = System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls12 |
                System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
            System.Net.ServicePointManager.SecurityProtocol = proto;

            int tryCount = 0;

            while (true)
            {
                tryCount++;

                try
                {
                    req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                    memstr = new System.IO.MemoryStream();

                    req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36";

                    req.Timeout = 10000;
                    req.AllowAutoRedirect = true;
                    req.MaximumAutomaticRedirections = 1;

                    using (System.Net.WebResponse res = req.GetResponse())
                    {
                        System.IO.Stream rs = res.GetResponseStream();
                        long length = res.ContentLength;

                        rs.CopyTo(memstr);
                        memstr.Seek(0, System.IO.SeekOrigin.Begin);
                    }

                    break;
                }
                catch (Exception e)
                {
                    if (tryCount >= maxRetries)
                    {
                        throw e;
                    }
                }

                System.Threading.Thread.Sleep(msDelay);
            }

            return memstr;
        }
    }
}
