using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace BAModel
{
    public abstract class HTMLSite : INotifyPropertyChanged
    {
        public string Name { get; set; }

        private BSParameterValue _queryString = new BSParameterValue();
        public BSParameterValue QueryString
        {
            get { return _queryString; }
            set
            {
                _queryString = value;
                _queryStringTextEntry = _queryString.GetValue();
            }
        }

        private string _queryStringTextEntry = String.Empty;
        public string QueryStringTextEntry
        {
            get { return _queryStringTextEntry; }
            set { _queryStringTextEntry = value; }
        }

        // this member is only used for tracking changes made during HTML site editing
        public string _originalName = String.Empty;
        public string OriginalName
        {
            get { return _originalName; }
            set { _originalName = value; }
        }

        protected Uri _uriSpec = null;
        public Uri UriSpec
        {
            get { return _uriSpec; }
        }

        public abstract Object Copy();
        public abstract void WriteToXml(System.Xml.XmlTextWriter writer, Sign sign, bool publish);

        protected void CopyMembers(HTMLSite htmlSite)
        {
            htmlSite.Name = this.Name;
            htmlSite.QueryString = this.QueryString.Clone();
        }

        protected void WriteMembersToXml(System.Xml.XmlTextWriter writer)
        {
            writer.WriteElementString("name", Name);
            writer.WriteStartElement("queryString");
            QueryString.WriteToXml(writer);
            writer.WriteEndElement();
        }

        public virtual bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            HTMLSite htmlSite = (HTMLSite)obj;

            return this.Name == htmlSite.Name &&
                   this.QueryString.IsEqual(htmlSite.QueryString);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(
                    this, new PropertyChangedEventArgs(propName));
        }

        #endregion
    }

    public class LocalHTMLSite : HTMLSite
    {
        private string _filePath = String.Empty;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                _uriSpec = new Uri(_filePath);
                this.OnPropertyChanged("FilePath");
            }
        }

        public override object Copy() // ICloneable implementation
        {
            LocalHTMLSite htmlSite = new LocalHTMLSite();
            htmlSite.FilePath = this.FilePath;
            htmlSite._uriSpec = new Uri(htmlSite.FilePath);

            base.CopyMembers(htmlSite);
            return htmlSite;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            LocalHTMLSite htmlSite = (LocalHTMLSite)obj;

            if (htmlSite.FilePath != this.FilePath) return false;
            return base.IsEqual(htmlSite);
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, Sign sign, bool publish)
        {
            writer.WriteStartElement("localHTMLSite");
            base.WriteMembersToXml(writer);

            if (publish)
            {
                // file prefix is "<site name>-". If it changes here, it must also change in GetHTMLContent
                string prefix = Name + "-";
                writer.WriteElementString("prefix", prefix);
                writer.WriteElementString("filePath", System.IO.Path.GetFileName(FilePath));
            }
            else
            {
                writer.WriteElementString("filePath", FilePath);
            }

            writer.WriteEndElement(); // localHTMLSite
        }

        public static LocalHTMLSite ReadXml(XmlReader reader)
        {
            string name = String.Empty;
            string filePath = String.Empty;
            BSParameterValue queryString = new BSParameterValue();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        name = reader.ReadString();
                        break;
                    case "filePath":
                        filePath = reader.ReadString();
                        break;
                    case "queryString":
                        queryString = BrightAuthorUtils.ReadBSParameterValue(reader);
                        break;
                }
            }

            LocalHTMLSite localHTMLSite = new LocalHTMLSite
            {
                Name = name,
                FilePath = filePath,
                QueryString = queryString
            };

            return localHTMLSite;
        }
    }

    public class RemoteHTMLSite : HTMLSite
    {
        private BSParameterValue _url = new BSParameterValue();

        public BSParameterValue Url
        {
            get { return _url; }
            set 
            {
                _url = value;
                _uriSpec = new Uri(_url.GetValue());
            }
        }

        public string UrlSpec
        {
            get
            {
                if (_url == null)
                {
                    return String.Empty;
                }
                else
                {
                    return _url.GetValue();
                }
            }
            set
            {
                if (_url != null)
                {
                    _url.SetValue(value);
                    try
                    {
                        _uriSpec = new Uri(value);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Exception in UrlSpec: " + ex.ToString());
                    }
                }
            }
        }

        public override object Copy() // ICloneable implementation
        {
            RemoteHTMLSite htmlSite = new RemoteHTMLSite();
            htmlSite.Url = this.Url.Clone();
            htmlSite._uriSpec = new Uri(this.Url.GetValue());

            base.CopyMembers(htmlSite);
            return htmlSite;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            RemoteHTMLSite htmlSite = (RemoteHTMLSite)obj;

            if (!htmlSite.Url.IsEqual(this.Url)) return false;
            return base.IsEqual(htmlSite);
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, Sign sign, bool publish)
        {
            writer.WriteStartElement("remoteHTMLSite");
            base.WriteMembersToXml(writer);

            writer.WriteStartElement("url");
            _url.WriteToXml(writer);
            writer.WriteEndElement(); // url

            writer.WriteEndElement(); // remoteHTMLSite
        }

        public static RemoteHTMLSite ReadXml(XmlReader reader)
        {
            string name = String.Empty;
            BSParameterValue url = null;
            BSParameterValue queryString = new BSParameterValue();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        name = reader.ReadString();
                        break;
                    case "url":
                        url = BrightAuthorUtils.ReadBSParameterValue(reader);
                        break;
                    case "queryString":
                        queryString = BrightAuthorUtils.ReadBSParameterValue(reader);
                        break;
                }
            }

            RemoteHTMLSite remoteHTMLSite = new RemoteHTMLSite
            {
                Name = name,
                Url = url,
                QueryString = queryString
            };

            return remoteHTMLSite;
        }
    }

    public class HTMLSiteSet
    {
        private Dictionary<string, HTMLSite> _htmlSites = new Dictionary<string, HTMLSite>();
        public Dictionary<string, HTMLSite> HTMLSites
        {
            get { return _htmlSites; }
        }

        public HTMLSiteSet()
        {
        }

        public HTMLSiteSet Clone()
        {
            HTMLSiteSet htmlSiteSet = new HTMLSiteSet();
            Dictionary<string, HTMLSite> newHTMLSites = htmlSiteSet.HTMLSites;

            Dictionary<string, HTMLSite> htmlSites = this.HTMLSites;
            foreach (KeyValuePair<string, HTMLSite> kvp in htmlSites)
            {
                HTMLSite htmlSiteCopy = (HTMLSite)kvp.Value.Copy();
                newHTMLSites.Add(kvp.Key, htmlSiteCopy);
            }

            return htmlSiteSet;
        }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            HTMLSiteSet htmlSiteSet = (HTMLSiteSet)obj;

            if (htmlSiteSet.HTMLSites.Count != this.HTMLSites.Count) return false;

            foreach (KeyValuePair<string, HTMLSite> kvp in htmlSiteSet.HTMLSites)
            {
                if (!this.HTMLSites.ContainsKey(kvp.Key)) return false;
                if (!htmlSiteSet.HTMLSites[kvp.Key].IsEqual(this.HTMLSites[kvp.Key])) return false;
            }

            return true;
        }

        public void AddHTMLSite(HTMLSite htmlSite)
        {
            if (!_htmlSites.ContainsKey(htmlSite.Name))
            {
                _htmlSites.Add(htmlSite.Name, htmlSite);
            }
        }

        public HTMLSite GetHTMLSite(string htmlSiteName)
        {
            if (_htmlSites.ContainsKey(htmlSiteName))
            {
                return _htmlSites[htmlSiteName];
            }

            return null;
        }

        // after editing html sites, check to see if the HTML site name has changed; if yes, return new value
        public string UpdateHTMLSiteName(string originalHTMLSiteName)
        {
            foreach (KeyValuePair<string, HTMLSite> kvp in HTMLSites)
            {
                HTMLSite htmlSite = kvp.Value;
                if (htmlSite.OriginalName == originalHTMLSiteName)
                {
                    return htmlSite.Name;
                }
            }

            // error??
            return originalHTMLSiteName;
        }

        public void WriteToXml(XmlTextWriter writer, Sign sign, bool publish)
        {
            writer.WriteStartElement("htmlSites");

            foreach (KeyValuePair<string, HTMLSite> kvp in HTMLSites)
            {
                kvp.Value.WriteToXml(writer, sign, publish);
            }

            writer.WriteFullEndElement(); // htmlSites
        }

        public static HTMLSiteSet ReadXml(XmlReader reader)
        {
            HTMLSiteSet htmlSiteSet = new HTMLSiteSet();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "localHTMLSite":
                        LocalHTMLSite localHTMLSite = LocalHTMLSite.ReadXml(reader);
                        htmlSiteSet.AddHTMLSite(localHTMLSite);
                        break;
                    case "remoteHTMLSite":
                        RemoteHTMLSite remoteHTMLSite = RemoteHTMLSite.ReadXml(reader);
                        htmlSiteSet.AddHTMLSite(remoteHTMLSite);
                        break;
                }
            }

            return htmlSiteSet;
        }
    }

}
