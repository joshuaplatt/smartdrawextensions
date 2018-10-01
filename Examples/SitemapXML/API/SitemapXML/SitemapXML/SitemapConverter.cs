using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SitemapXML
{
    /**
     * The main Filter class for external use.
     */
    public class SitemapFilter
    {
        /// <summary>
        /// Set to true to have the diagram generated merge leaves into a table.
        /// </summary>
        public bool GenerateTable { get; set; }

        /// <summary>
        /// Set to true to have the default System.Console used for verbose output.
        /// </summary>
        public bool UseConsole { get; set; }

        /// <summary>
        /// Set to true to have hyperlinks created within the diagram that
        /// point to the page that the node represents.
        /// </summary>
        public bool GenerateHyperlinks { get; set; }

        /// <summary>
        /// Set to true to have the output table be sorted in alphabetical order.
        /// </summary>
        public bool SortLexicographically { get; set; }

        /// <summary>
        /// Set to a DateTime to exclude every sitemap entry before specified date.
        /// </summary>
        public DateTime? ExcludeBefore { get; set; }

        /// <summary>
        /// Set to a string to have the verbose output send output to a file
        /// with the path specified by this value.
        /// </summary>
        public string ErrorOutputFile { get; set; }

        private SDON.Model.Diagram _root;
        private System.IO.StreamWriter _errorOutputStream;
        private ConvertLeavesToTables _convTables;
        private InsertURIToRoot _convUri;
        private string _previousOutputFilePath;
        private int _numShapes;
        private int _currDepth;
        private long _amountDownloaded;
        private int _numRequests;

        private const int MaxDepth = 64;
        private const int MaxShapes = 600;
        private const long MaxDownload = 1 << 21;
        private const int MaxSitemapDepth = 3;
        private const int MaxRequests = 64;

        /// <summary>
        /// Loads a sitemap from a specific .xml file.  If it is a
        /// sitemapindex, it will load multiple sitemaps from online.  Multiple
        /// sitemaps can be loaded before the diagram is generated.
        /// </summary>
        /// <param name="name">The file path of the .xml file.</param>
        public void LoadSitemapFromFile(string name)
        {
            XmlDocument sitemap = null;
            System.IO.FileStream fs;
            fs = new System.IO.FileStream(name, System.IO.FileMode.Open);

            if ((fs.Length >= 2) && (fs.ReadByte() == 0x1f) && (fs.ReadByte() == 0x8b)) //Gzipped
            {
                fs.Seek(0, System.IO.SeekOrigin.Begin);
                sitemap = getDocFromGzipStream(fs);
                fs.Close();

                if (sitemap == null)
                {
                    return;
                }
            }
            else
            {
                fs.Seek(0, System.IO.SeekOrigin.Begin);
                sitemap = new XmlDocument();
                sitemap.Load(fs);
                fs.Close();
            }

            outputToConsole("Loaded from File: " + name);
            convertSitemapSegment(sitemap);
        }

        /// <summary>
        /// Loads a sitemap from a specific URL.  Must return a properly
        /// formatted sitemap.Can be a sitemapindex to have multiple sitemaps
        /// loaded.  Multiple sitemaps can be loaded before the diagram is
        /// generated.
        /// </summary>
        /// <param name="url">URL of the online sitemap.</param>
        public void LoadSitemapFromURL(Uri url)
        {
            outputToConsole("Loading from URL: " + url);
            if (url.IsFile || url.IsLoopback)
            {
                throw new Exception("Error: Can't reference a file or a local path in URL.");
            }

            _numRequests++;
            if (_numRequests > MaxRequests)
            {
                throw new Exception("Exceeded maximum number of web requests.");
            }

            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse res;
            System.IO.MemoryStream memstr = new System.IO.MemoryStream();
            byte[] buff;

            req.Timeout = 10000;

            res = req.GetResponse();
            System.IO.Stream rs = res.GetResponseStream();
            long length = res.ContentLength;

            if ((length + _amountDownloaded) > MaxDownload)
            {
                throw new Exception("Exceeded maximum sitemap length.");
            }

            _amountDownloaded += length;

            rs.CopyTo(memstr);
            buff = memstr.GetBuffer();
            res.Close();
            memstr.Close();

            XmlDocument sitemap;

            if ((buff.Length > 2) && (buff[0] == 0x1f) && (buff[1] == 0x8b))    //Gzipped
            {
                memstr = new System.IO.MemoryStream(buff);
                sitemap = getDocFromGzipStream(memstr);

                if (sitemap == null)
                {
                    return;
                }
            }
            else
            {
                sitemap = new XmlDocument();
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

            outputToConsole("Loaded from URL: " + url);
            convertSitemapSegment(sitemap);
        }

        /// <summary>
        /// Loads a sitemap from a local string.  Use this if the sitemap is
        /// not accessible from a file or a URL.  Multiple sitemaps can be
        /// loaded before the diagram is generated.
        /// </summary>
        /// <param name="sitemap">The string representation of the sitemap.</param>
        public void LoadSitemapFromString(string sitemap)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sitemap);

            convertSitemapSegment(doc);
        }

        /// <summary>
        /// Converts the sitemap and generates the diagram.  Make sure that the
        /// sitemap is already loaded using one of the Load Sitemap methods.
        /// </summary>
        /// <returns>The generated diagram from loaded sitemaps.</returns>
        /// <see cref="LoadSitemapFromFile(string)"/>
        /// <seealso cref="LoadSitemapFromFile(string)"/>
        /// <seealso cref="LoadSitemapFromURL(Uri)"/>
        /// <seealso cref="LoadSitemapFromString(string)"/>
        public SDON.Model.Diagram ConvertSitemap()
        {
            if(SortLexicographically)
            {
                sortDiagramLexicographically();
            }

            if (GenerateTable)
            {
                _convTables.Convert();
            }

            return _root;
        }

        /// <summary>
        /// Gets the root after the diagram is generated.  For use for internal
        /// diagram manipulation after generation.
        /// </summary>
        /// <returns>The generated diagram, or <c>null</c> if the sitemaps have
        /// not been converted.</returns>
        /// <seealso cref="ConvertSitemap()"/>
        public SDON.Model.Diagram GetRoot()
        {
            return _root;
        }

        /// <summary>
        /// Saves the diagram to an SDON file after the diagram has been generated.
        /// </summary>
        /// <param name="outputFile">Path for the file that will be generated.</param>
        /// <seealso cref="ConvertSitemap()"/>
        public void SaveConvertedDocument(string outputFile)
        {
            if (_root.Shape == null)
            {
                return;
            }

            outputToConsole("Total nodes: " + _numShapes);

            string jsonData = SDON.SDONBuilder.ToJSON(_root);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(outputFile, false, Encoding.UTF8))
            {
                file.Write(jsonData);
            }
        }

        /// <summary>
        /// Converts a sitemap from a System.Xml.XmlDocument object.
        /// </summary>
        /// <param name="doc">XmlDocument object containing the sitemap.</param>
        private void convertSitemapSegment(XmlDocument doc)
        {
            XmlElement listing = doc.DocumentElement;

            if (listing == null)
            {
                return;
            }

            if (listing.Name == "urlset")
            {
                outputToConsole("Converting...");
                readFromUrlset(listing);
            }
            else if (listing.Name == "sitemapindex")
            {
                outputToConsole("Reading sitemap directories...");
                readFromSitemapindex(listing);
            }
            else
            {
                outputToConsole("Error: Unknown element! (" + doc.Name + ")");
            }
        }

        private XmlDocument getDocFromGzipStream(System.IO.Stream stream)
        {
            XmlDocument sitemap = new XmlDocument();
            int read, count = 0;
            byte[] buff = new byte[16384];

            using(System.IO.Compression.GZipStream gz = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress)) {
                using(System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
                    while ((read = gz.Read(buff, 0, 16384)) > 0)
                    {
                        ms.Write(buff, 0, read);
                        count += read;

                        if (count > (2 << 24))
                        {
                            throw new Exception("Gzip file invalid.");
                        }
                    }

                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    sitemap.Load(ms);
                }
            }

            return sitemap;
        }

        private void readFromSitemapindex(XmlNode node)
        {
            if (_currDepth >= MaxSitemapDepth)
            {
                throw new Exception("Exceeded maximum sitemap depth.");
            }

            _currDepth++;

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "sitemap")
                {
                    foreach (XmlNode loc in child.ChildNodes)
                    {
                        if ((loc.Name == "loc") && (_numShapes < MaxShapes))
                        {
                            LoadSitemapFromURL(new Uri(loc.InnerText));
                        }
                    }
                }
            }

            _currDepth--;
        }

        private void readFromUrlset(XmlNode node)
        {
            string tempLoc;
            string tempLastModified;

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "url")
                {
                    tempLoc = "";
                    tempLastModified = "";

                    foreach (XmlNode loc in child.ChildNodes)
                    {
                        if (loc.Name == "loc")
                        {
                            tempLoc = loc.InnerText;
                        }
                        else if (loc.Name == "lastmod")
                        {
                            tempLastModified = loc.InnerText;
                        }
                    }

                    if (tempLoc != "")
                    {
                        _convUri.InsertURI(new Uri(tempLoc), tempLastModified, ref _numShapes);
                    }
                }
            }
        }

        private void sortDiagramLexicographically()
        {
            SDON.Model.ShapeConnector con;

            outputToConsole("Sorting diagram...");

            if ((_root.Shape != null) && (_root.Shape.ShapeConnector != null))
            {
                for(int i = 0; i < _root.Shape.ShapeConnector.Count; i++)
                {
                    con = _root.Shape.ShapeConnector[i];

                    if(con.Shapes != null)
                    {
                        recursiveSort(con.Shapes);
                    }
                }
            }
        }

        private void recursiveSort(List<SDON.Model.Shape> shapes)
        {
            shapes.Sort(compareShapes);

            SDON.Model.Shape shape;
            SDON.Model.ShapeConnector con;
            int i, j;

            for(i = 0; i < shapes.Count; i++)
            {
                shape = shapes[i];

                if(shape.ShapeConnector != null)
                {
                    for(j = 0; j < shape.ShapeConnector.Count; j++)
                    {
                        con = shape.ShapeConnector[j];

                        if(con.Shapes != null)
                        {
                            recursiveSort(con.Shapes);
                        }
                    }
                }
            }
        }

        private static int compareShapes(SDON.Model.Shape x, SDON.Model.Shape y)
        {
            return x.Label.CompareTo(y.Label);
        }

        public void outputToConsole(string output)  //Re-wire and copy to other filters or remove
        {                                           //Preferrably remove and replace with exceptions? (custom ones)
            if (UseConsole)                                                                         //(and cool ones too)
            {                                       //Though, non-destructive feedback needs output (?)
                Console.WriteLine(output);
            }

            if (ErrorOutputFile != "")
            {
                if (ErrorOutputFile != _previousOutputFilePath)
                {
                    if (_errorOutputStream != null)
                    {
                        _errorOutputStream.Close();
                    }

                    _errorOutputStream = new System.IO.StreamWriter(ErrorOutputFile, false);
                    _previousOutputFilePath = ErrorOutputFile;
                }

                _errorOutputStream.WriteLine(output);
                _errorOutputStream.Flush();
            }
            else if (_errorOutputStream != null)
            {
                _errorOutputStream.Close();
                _errorOutputStream = null;
            }
        }

        public SitemapFilter()
        {
            GenerateTable = false;  //Have globals set at top instead of constructor or...?
            UseConsole = false;
            GenerateHyperlinks = false;
            ExcludeBefore = null;
            ErrorOutputFile = "";
            _previousOutputFilePath = "";
            _numShapes = 0;
            _currDepth = 0;
            _amountDownloaded = 0;
            _numRequests = 0;

            _root = new SDON.Model.Diagram();
            _root.Template = "Sitemap";
            _root.Version = "20";

            SDON.Model.SymbolEntry parentSymbol = new SDON.Model.SymbolEntry();
            parentSymbol.Name = "Parent";
            parentSymbol.ID = new Guid("447105dc-fe46-4454-a35b-2317e422efd6");

            SDON.Model.SymbolEntry leafSymbol = new SDON.Model.SymbolEntry();
            leafSymbol.Name = "Leaf";
            leafSymbol.ID = new Guid("831246c2-437a-429c-81a7-bb4064b2a73a");

            _root.Symbols = new List<SDON.Model.SymbolEntry>();
            _root.Symbols.Add(parentSymbol);
            _root.Symbols.Add(leafSymbol);

            _convTables = new ConvertLeavesToTables(_root, this);
            _convUri = new InsertURIToRoot(_root, MaxDepth, MaxShapes, this);
        }
    }
}
