using System;
using System.ServiceModel.Web;
using System.Net;
using System.Text;

namespace SitemapXMLFilterService
{
    class SitemapUrlLoaderInstanceData
    {
        public int MaxDepth { get { return 64; } }
        public long MaxDownload { get { return 1 << 21; } }
        public int MaxSitemapDepth { get { return 3; } }
        public int MaxRequests { get { return 64; } }

        public int CurrDepth { get; set; }
        public long AmountDownloaded { get; set; }
        public int NumRequests { get; set; }

        public System.Xml.XmlDocument Doc { get; set; }
    }

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SitemapXMLService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SitemapXMLService.svc or SitemapXMLService.svc.cs at the Solution Explorer and start debugging.
    public class SitemapXMLService : ISitemapXMLService
    {
        public const string UnixRootDir = "";
        public const string UnixPathSeparator = "/";

        public const string DosRootDir = "C:";
        public const string DosPathSeparator = "\\";

        public const string CurrRootDir = DosRootDir;
        public const string CurrPathSeparator = DosPathSeparator;
        public static string TempFileLocation = System.Configuration.ConfigurationManager.AppSettings["TempFileLocation"];


        public string ConvertSitemap(SitemapConverterOptions options)
        {
            string decodedSitemap;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                SitemapXML.SitemapFilter filter = new SitemapXML.SitemapFilter();

                filter.GenerateHyperlinks = (bool)options.Hyperlinks;
                filter.GenerateTable = (bool)options.Tables;
                filter.SortLexicographically = (bool)options.LexicographicOrder;
                filter.LoadSitemapFromURL(new Uri(options.Url));
                filter.ConvertSitemap();

                SDON.Model.Diagram diagram = filter.GetRoot();
                decodedSitemap = SDON.SDONBuilder.ToJSON(diagram);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = (System.Net.HttpStatusCode)418;

                return "Error parsing sitemap:\n" + e.Message;

            }

            return decodedSitemap;
        }

        public string ConvertSitemapBlock(Guid uploadID, System.IO.Stream stream)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            stream.CopyTo(ms);

            try
            {
                byte[] buff = ms.GetBuffer();
                string str = UnicodeEncoding.Unicode.GetString(buff, 0, (int)ms.Length);

                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-sitemap-" + uploadID.ToString());
                if (!dir.Exists)
                {
                    dir.Create();
                }

                System.IO.FileInfo file = new System.IO.FileInfo(dir.FullName + CurrPathSeparator + "sitemap.xml");

                if (file.Exists && (file.Length > (1 << 22)))
                {
                    throw new Exception("Sitemap length exceeded upper boundary");
                }

                using (System.IO.StreamWriter fs = new System.IO.StreamWriter(file.FullName, true))
                {
                    fs.Write(str);
                }
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                dumpTempFolder(uploadID);
                return "Error while receiving data: " + e.Message;
            }

            return "";
        }

        public string ConvertSitemapFinal(Guid uploadID, SitemapConverterOptions options)
        {
            string decodedSitemap;

            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-sitemap-" + uploadID.ToString());
                SitemapXML.SitemapFilter filter = new SitemapXML.SitemapFilter();

                filter.GenerateHyperlinks = (bool)options.Hyperlinks;
                filter.GenerateTable = (bool)options.Tables;
                filter.SortLexicographically = (bool)options.LexicographicOrder;
                filter.LoadSitemapFromFile(dir.FullName + CurrPathSeparator + "sitemap.xml");
                filter.ConvertSitemap();

                SDON.Model.Diagram diagram = filter.GetRoot();
                decodedSitemap = SDON.SDONBuilder.ToJSON(diagram);

                dir.Delete(true);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                dumpTempFolder(uploadID);
                return "Error when converting sitemap: " + e.Message;
            }

            return decodedSitemap;
        }

        private void dumpTempFolder(Guid uploadID)
        {
            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(TempFileLocation + CurrPathSeparator + "tmp-sitemap-" + uploadID.ToString());

                if (dir.Exists)
                {
                    dir.Delete(true);
                }
            }
            catch (Exception)
            {

            }
        }

        public string GetSitemapFromUrl(System.IO.Stream url)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            url.CopyTo(ms);

            string retStr = "";

            try
            {
                byte[] buff = ms.GetBuffer();
                string urlStr = UTF8Encoding.UTF8.GetString(buff, 0, (int)ms.Length);
                SitemapUrlLoaderInstanceData instance = new SitemapUrlLoaderInstanceData();
                System.Xml.XmlDocument doc;

                instance.AmountDownloaded = 0;
                instance.CurrDepth = 0;
                instance.NumRequests = 0;
                instance.Doc = null;

                doc = getXMLDocumentFromUrl(new Uri(urlStr), instance);
                mergeFromSitemapindex(doc, instance);

                if (instance.Doc == null)
                {
                    throw new Exception("No sitemap in url.");
                }

                retStr = instance.Doc.InnerXml;
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                return "Error when retrieving sitemap: " + e.Message;
            }


            return retStr;
        }

        //Slightly modified from Sitemap filter

        private System.Xml.XmlDocument getXMLDocumentFromUrl(Uri url, SitemapUrlLoaderInstanceData inst)
        {
            if (url.IsFile || url.IsLoopback)
            {
                throw new Exception("Can't reference a file or a local path in URL.");
            }

            inst.NumRequests++;
            if (inst.NumRequests > inst.MaxRequests)
            {
                throw new Exception("Exceeded maximum number of web requests");
            }

            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse res;
            System.IO.MemoryStream memstr = new System.IO.MemoryStream();
            byte[] buff;

            req.Timeout = 10000;

            res = req.GetResponse();
            System.IO.Stream rs = res.GetResponseStream();
            long length = res.ContentLength;

            if ((length + inst.AmountDownloaded) > inst.MaxDownload)
            {
                throw new Exception("Exceeded maximum sitemap length.");
            }

            inst.AmountDownloaded += length;

            rs.CopyTo(memstr);
            buff = memstr.GetBuffer();
            res.Close();
            memstr.Close();

            System.Xml.XmlDocument sitemap;

            if ((buff.Length > 2) && (buff[0] == 0x1f) && (buff[1] == 0x8b))    //Gzipped
            {
                memstr = new System.IO.MemoryStream(buff);
                sitemap = getDocFromGzipStream(memstr);

                if (sitemap == null)
                {
                    new Exception("Internal server error.");
                }
            }
            else
            {
                sitemap = new System.Xml.XmlDocument();
                string docStr = Encoding.UTF8.GetString(buff);
                int hasNull = docStr.IndexOf('\0');

                if (hasNull < 0)
                {
                    sitemap.LoadXml(docStr);
                }
                else
                {
                    sitemap.LoadXml(docStr.Substring(0, hasNull));
                }
            }

            return sitemap;
        }

        private System.Xml.XmlDocument getDocFromGzipStream(System.IO.Stream stream)
        {
            System.Xml.XmlDocument sitemap = new System.Xml.XmlDocument();
            int read, count = 0;
            byte[] buff = new byte[16384];

            using (System.IO.Compression.GZipStream gz = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress))
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    while ((read = gz.Read(buff, 0, 16384)) > 0)
                    {
                        ms.Write(buff, 0, read);
                        count += read;

                        if (count > (1 << 24))
                        {
                            throw new Exception("Gzip file too large (>16 MB).");
                        }
                    }

                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    sitemap.Load(ms);
                }
            }

            return sitemap;
        }

        private void mergeFromSitemapindex(System.Xml.XmlDocument doc, SitemapUrlLoaderInstanceData inst)
        {
            System.Xml.XmlElement listing = doc.DocumentElement;
            System.Xml.XmlNode inode;

            if (listing == null)
            {
                return;
            }

            if (listing.Name == "urlset")
            {
                if (inst.Doc == null)
                {
                    inst.Doc = doc;
                }
                else
                {
                    for (int i = 0; i < listing.ChildNodes.Count; i++)
                    {
                        inode = inst.Doc.ImportNode(listing.ChildNodes[i], true);
                        inst.Doc.DocumentElement.AppendChild(inode);
                    }
                }
            }
            else if (listing.Name == "sitemapindex")
            {
                readFromSitemapindex(listing, inst);
            }
            else
            {
                throw new Exception("Invalid sitemap.");
            }
        }

        private void readFromSitemapindex(System.Xml.XmlNode node, SitemapUrlLoaderInstanceData inst)
        {
            if (inst.CurrDepth >= inst.MaxSitemapDepth)
            {
                throw new Exception("Exceeded maximum sitemap depth.");
            }

            inst.CurrDepth++;

            foreach (System.Xml.XmlNode child in node.ChildNodes)
            {
                if (child.Name == "sitemap")
                {
                    foreach (System.Xml.XmlNode loc in child.ChildNodes)
                    {
                        if (loc.Name == "loc")
                        {
                            mergeFromSitemapindex(getXMLDocumentFromUrl(new Uri(loc.InnerText), inst), inst);
                        }
                    }
                }
            }

            inst.CurrDepth--;
        }
    }
}
