using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Diagnostics;
using System.IO;

namespace BAModel
{
    public class LiveDataFeed : INotifyPropertyChanged
    {
        public enum DataFeedUsage
        {
            Text,
            MRSS,
            Content,
            MRSSWith4K,
            Undefined
        }

        public string Name { get; set; }

        // this member is only used for tracking changes made during LiveDataFeed editing
        public string _originalName = String.Empty;
        public string OriginalName
        {
            get { return _originalName; }
            set { _originalName = value; }
        }

        public LiveBSNFeed LiveBSNDataFeed { get; set; }

        public LiveBSNFeed LiveBSNMediaFeed { get; set; }

        public LiveDynamicPlaylistFeed LiveDynamicPlaylist { get; set; }

        protected BSParameterValue _url = new BSParameterValue();
        public BSParameterValue Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private bool _autoGenerateUserVariables = false;
        public bool AutoGenerateUserVariables
        {
            get { return _autoGenerateUserVariables; }
            set { _autoGenerateUserVariables = value; }
        }

        private UserVariable.UserVariableAccess _userVariableAccess = UserVariable.UserVariableAccess.Private;
        public UserVariable.UserVariableAccess UserVariableAccess
        {
            get { return _userVariableAccess; }
            set { _userVariableAccess = value; }
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
                }
            }
        }

        public string GetUrlValue()
        {
            if (LiveBSNDataFeed != null)
            {
                return LiveBSNDataFeed.Url;
            }
            else if (LiveDynamicPlaylist != null)
            {
                return LiveDynamicPlaylist.Url;
            }
            else if (LiveBSNMediaFeed != null)
            {
                return LiveBSNMediaFeed.Url;
            }
            else if (_url != null)
            {
                return _url.GetCurrentValue();
            }
            else
            {
                return String.Empty;
            }
        }

        public bool IsSpecifiedByUserVariable()
        {
            if (Url != null)
            {
                List<BSParameterValueItem> bsParameterValueItems = Url.BSParameterValueItems;
                foreach (BSParameterValueItem bsParameterValueItem in bsParameterValueItems)
                {
                    if (bsParameterValueItem is BSParameterValueItemVariable)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private LiveDataFeed.DataFeedUsage _dataFeedUse = LiveDataFeed.DataFeedUsage.Text;
        public LiveDataFeed.DataFeedUsage DataFeedUse
        {
            get { return _dataFeedUse; }
            set { _dataFeedUse = value; }
        }

        //private bool _downloadContent = false;
        //public bool DownloadContent
        //{
        //    get { return _downloadContent; }
        //    set { _downloadContent = value; }
        //}

        public string ParserFunctionName { get; set; }
        public string UpdateInterval { get; set; }

        private string _pluginFilePath = String.Empty;
        public string PluginFilePath
        {
            get { return _pluginFilePath; }
            set
            {
                _pluginFilePath = value;
                this.OnPropertyChanged("PluginFilePath");
            }
        }

        private string _uvParserFunctionName = String.Empty;
        public string UVParserFunctionName
        {
            get { return _uvParserFunctionName; }
            set { _uvParserFunctionName = value; }
        }

        private string _uvPluginFilePath = String.Empty;
        public string UVPluginFilePath
        {
            get { return _uvPluginFilePath; }
            set
            {
                _uvPluginFilePath = value;
                this.OnPropertyChanged("UVPluginFilePath");
            }
        }

        public bool IncludesPlugin
        {
            get
            {
                return (!String.IsNullOrEmpty(ParserFunctionName) && !String.IsNullOrEmpty(PluginFilePath));
            }
        }

        public LiveDataFeed Clone()
        {
            LiveDataFeed liveDataFeed = new LiveDataFeed();
            liveDataFeed.Name = this.Name;

            if (this._url != null)
            {
                liveDataFeed._url = this._url.Clone();
            }
            else
            {
                liveDataFeed._url = null;
            }

            if (this.LiveBSNDataFeed != null)
            {
                liveDataFeed.LiveBSNDataFeed = this.LiveBSNDataFeed.Clone();
            }
            else
            {
                liveDataFeed.LiveBSNDataFeed = null;
            }

            if (this.LiveBSNMediaFeed != null)
            {
                liveDataFeed.LiveBSNMediaFeed = this.LiveBSNMediaFeed.Clone();
            }
            else
            {
                liveDataFeed.LiveBSNMediaFeed = null;
            }

            if (this.LiveDynamicPlaylist != null)
            {
                liveDataFeed.LiveDynamicPlaylist = this.LiveDynamicPlaylist.Clone();
            }
            else
            {
                liveDataFeed.LiveDynamicPlaylist = null;
            }

            liveDataFeed.DataFeedUse = this.DataFeedUse;
            liveDataFeed.PluginFilePath = this.PluginFilePath;
            liveDataFeed.ParserFunctionName = this.ParserFunctionName;
            liveDataFeed.UpdateInterval = this.UpdateInterval;
            liveDataFeed.AutoGenerateUserVariables = this.AutoGenerateUserVariables;
            liveDataFeed.UserVariableAccess = this.UserVariableAccess;
            liveDataFeed.UVPluginFilePath = this.UVPluginFilePath;
            liveDataFeed.UVParserFunctionName = this.UVParserFunctionName;

            return liveDataFeed;
        }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            LiveDataFeed liveDataFeed = (LiveDataFeed)obj;

            if (this.Url == null && liveDataFeed.Url != null) return false;
            if (this.Url != null && liveDataFeed.Url == null) return false;
            if (this.Url != null && liveDataFeed.Url != null)
            {
                if (this.UrlSpec != liveDataFeed.UrlSpec) return false;
            }

            if (this.LiveBSNDataFeed == null && liveDataFeed.LiveBSNDataFeed != null) return false;
            if (this.LiveBSNDataFeed != null && liveDataFeed.LiveBSNDataFeed == null) return false;
            if (this.LiveBSNDataFeed != null && liveDataFeed.LiveBSNDataFeed != null)
            {
                if (!this.LiveBSNDataFeed.IsEqual(liveDataFeed.LiveBSNDataFeed)) return false;
            }

            if (this.LiveDynamicPlaylist == null && liveDataFeed.LiveDynamicPlaylist != null) return false;
            if (this.LiveDynamicPlaylist != null && liveDataFeed.LiveDynamicPlaylist == null) return false;
            if (this.LiveDynamicPlaylist != null && liveDataFeed.LiveDynamicPlaylist != null)
            {
                if (!this.LiveDynamicPlaylist.IsEqual(liveDataFeed.LiveDynamicPlaylist)) return false;
            }

            return (
                (this.Name == liveDataFeed.Name) &&
                (this.DataFeedUse == liveDataFeed.DataFeedUse) &&
                (this.PluginFilePath == liveDataFeed.PluginFilePath) &&
                (this.ParserFunctionName == liveDataFeed.ParserFunctionName) &&
                (this.UpdateInterval == liveDataFeed.UpdateInterval) &&
                (this.AutoGenerateUserVariables == liveDataFeed.AutoGenerateUserVariables) &&
                (this.UserVariableAccess == liveDataFeed.UserVariableAccess) &&
                (this.UVPluginFilePath == liveDataFeed.UVPluginFilePath) &&
                (this.UVParserFunctionName == liveDataFeed.UVParserFunctionName)
                );
        }


        public void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("liveDataFeed");
            writer.WriteElementString("name", Name);

            if (_url != null)
            {
                writer.WriteStartElement("url");
                _url.WriteToXml(writer);
                writer.WriteEndElement(); // url
                //writer.WriteElementString("url", Url);
            }
            else if (LiveBSNDataFeed != null)
            {
                writer.WriteStartElement("liveBSNDataFeed");
                LiveBSNDataFeed.WriteToXml(writer);
                writer.WriteEndElement(); // liveBSNDataFeed
            }
            else if (LiveBSNMediaFeed != null)
            {
                writer.WriteStartElement("liveBSNMediaFeed");
                LiveBSNMediaFeed.WriteToXml(writer);
                writer.WriteEndElement(); // liveBSNMediaFeed
            }
            else
            {
                writer.WriteStartElement("liveDynamicPlaylist");
                LiveDynamicPlaylist.WriteToXml(writer);
                writer.WriteEndElement(); // liveDynamicPlaylist
            }

            if (!publish)
            {
                writer.WriteElementString("pluginFilePath", PluginFilePath);
                writer.WriteElementString("uvPluginFilePath", UVPluginFilePath);
            }

            writer.WriteElementString("dataFeedUse", DataFeedUse.ToString());
            writer.WriteElementString("parserFunctionName", ParserFunctionName);
            writer.WriteElementString("updateInterval", UpdateInterval);
            writer.WriteElementString("autoGenerateUserVariables", AutoGenerateUserVariables.ToString());
            writer.WriteElementString("userVariableAccess", UserVariableAccess.ToString());
            writer.WriteElementString("uvParserFunctionName", UVParserFunctionName);

            writer.WriteFullEndElement(); // liveDataFeed
        }

        public string GetUrl()
        {
            if (_url != null)
            {
                return _url.GetCurrentValue();
            }
            else if (LiveBSNDataFeed != null)
            {
                return LiveBSNDataFeed.Url;
            }
            else if (LiveBSNMediaFeed != null)
            {
                return LiveBSNMediaFeed.Url;
            }
            else
            {
                return LiveDynamicPlaylist.Url;
            }
        }

        public static LiveDataFeed ReadXml(XmlReader reader)
        {
            LiveDataFeed liveDataFeed = new LiveDataFeed();

            string name = String.Empty;
            BSParameterValue bsParameterValue = null;
            LiveBSNFeed liveBSNDataFeed = null;
            LiveBSNFeed liveBSNMediaFeed = null;
            LiveDynamicPlaylistFeed liveDynamicPlaylist = null;
            bool downloadContent = false;
            string pluginFilePath = String.Empty;
            string parserFunctionName = String.Empty;
            string updateInterval = String.Empty;
            bool autoGenerateUserVariables = false;
            UserVariable.UserVariableAccess userVariableAccess = UserVariable.UserVariableAccess.Private;
            string uvPluginFilePath = String.Empty;
            string uvParserFunctionName = String.Empty;
            DataFeedUsage dataFeedUse = DataFeedUsage.Undefined;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        name = reader.ReadString();
                        break;
                    case "url":
                        bsParameterValue = BrightAuthorUtils.ReadBSParameterValue(reader);
                        break;
                    case "liveBSNDataFeed":
                        liveBSNDataFeed = LiveBSNFeed.ReadXml(reader);
                        break;
                    case "liveBSNMediaFeed":
                        liveBSNMediaFeed = LiveBSNFeed.ReadXml(reader);
                        break;
                    case "liveDynamicPlaylist":
                        liveDynamicPlaylist = LiveDynamicPlaylistFeed.ReadXml(reader);
                        break;
                    case "downloadContent":
                        downloadContent = Convert.ToBoolean(reader.ReadString());
                        break;
                    case "pluginFilePath":
                        pluginFilePath = reader.ReadString();
                        break;
                    case "parserFunctionName":
                        parserFunctionName = reader.ReadString();
                        break;
                    case "updateInterval":
                        updateInterval = reader.ReadString();
                        break;
                    case "autoGenerateUserVariables":
                        autoGenerateUserVariables = Convert.ToBoolean(reader.ReadString());
                        break;
                    case "userVariableAccess":
                        userVariableAccess = (UserVariable.UserVariableAccess)Enum.Parse(typeof(UserVariable.UserVariableAccess), reader.ReadString());
                        break;
                    case "uvPluginFilePath":
                        uvPluginFilePath = reader.ReadString();
                        break;
                    case "uvParserFunctionName":
                        uvParserFunctionName = reader.ReadString();
                        break;
                    case "dataFeedUse":
                        dataFeedUse = (DataFeedUsage)Enum.Parse(typeof(DataFeedUsage), reader.ReadString());
                        break;
                }
            }

            // deal with legacy data feeds
            if (dataFeedUse == DataFeedUsage.Undefined)
            {
                if (downloadContent)
                {
                    dataFeedUse = DataFeedUsage.Content;
                }
                else if (liveBSNDataFeed != null)
                {
                    dataFeedUse = DataFeedUsage.Text;
                }
                else if (liveBSNMediaFeed != null || liveDynamicPlaylist != null)
                {
                    dataFeedUse = DataFeedUsage.MRSS;
                }
            }

            liveDataFeed.Name = name;
            liveDataFeed.Url = bsParameterValue;
            liveDataFeed.LiveBSNDataFeed = liveBSNDataFeed;
            liveDataFeed.LiveBSNMediaFeed = liveBSNMediaFeed;
            liveDataFeed.LiveDynamicPlaylist = liveDynamicPlaylist;
            liveDataFeed.DataFeedUse = dataFeedUse;
            liveDataFeed.PluginFilePath = pluginFilePath;
            liveDataFeed.ParserFunctionName = parserFunctionName;
            liveDataFeed.UpdateInterval = updateInterval;
            liveDataFeed.AutoGenerateUserVariables = autoGenerateUserVariables;
            liveDataFeed.UserVariableAccess = userVariableAccess;
            liveDataFeed.UVPluginFilePath = uvPluginFilePath;
            liveDataFeed.UVParserFunctionName = uvParserFunctionName;

            return liveDataFeed;
        }

        public static bool ValidateLiveDataFeedUrl(string url)
        {
            try
            {
                XmlDocument objXml = new XmlDocument();
                NetworkMgr.LoadXml(url, objXml);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ValidateLiveDataFeedUrl failure");
                Trace.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        public static string NameValid(string feedName, Dictionary<string, string> liveDataFeedNames)
        {
            if (feedName == String.Empty)
            {
                return BrightAuthorUtils.GetLocalizedString("LiveDataFeedNameRequired");
            }

            if (liveDataFeedNames.ContainsKey(feedName))
            {
                return BrightAuthorUtils.GetLocalizedString("LiveDataFeedDuplicateNamesNotAllowed");
            }

            return String.Empty;
        }

        public static string PlugInParserFunctionValid(string pluginScript, string parserFunctionName, Dictionary<string, string> pluginFileNamesToPaths, Dictionary<string, string> parserFunctionNamesToPaths)
        {
            if ((String.IsNullOrEmpty(pluginScript) && !String.IsNullOrEmpty(parserFunctionName)) ||
                 (!String.IsNullOrEmpty(pluginScript) && String.IsNullOrEmpty(parserFunctionName)))
            {
                return BrightAuthorUtils.GetLocalizedString("LiveDataFeedPlugInFileParserFunctionNameRequired");
            }

            if (!String.IsNullOrEmpty(parserFunctionName))
            {
                if (parserFunctionNamesToPaths.ContainsKey(parserFunctionName.ToLower()))
                {
                    string filePath = parserFunctionNamesToPaths[parserFunctionName.ToLower()];
                    if (filePath != pluginScript)
                    {
                        return BrightAuthorUtils.GetLocalizedString("LiveDataFeedDuplicateParserFunctionNames");
                    }
                }
            }

            if (!String.IsNullOrEmpty(pluginScript))
            {
                FileInfo fi = new FileInfo(pluginScript);
                string fileName = fi.Name;

                if (pluginFileNamesToPaths.ContainsKey(fileName))
                {
                    string filePath = pluginFileNamesToPaths[fileName];

                    if (filePath != pluginScript)
                    {
                        return BrightAuthorUtils.GetLocalizedString("LiveDataFeedPluginsSameFileName");
                    }
                }
            }

            return String.Empty;
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

    public class MediaFeedCustomFields
    {
        public bool CustomFieldsFetched { get; set; }
        private List<string> _customFields = new List<string>();
        public List<string> CustomFields
        {
            get { return _customFields; }
            set { _customFields = value; }
        }
    }

    public class LiveDataFeedSet
    {
        private Dictionary<string, LiveDataFeed> _liveDataFeeds = new Dictionary<string, LiveDataFeed>();
        public Dictionary<string, LiveDataFeed> LiveDataFeeds
        {
            get { return _liveDataFeeds; }
        }

        public LiveDataFeedSet()
        {
        }

        public LiveDataFeedSet Clone()
        {
            LiveDataFeedSet liveDataFeedSet = new LiveDataFeedSet();
            Dictionary<string, LiveDataFeed> newLiveDataFeeds = liveDataFeedSet.LiveDataFeeds;

            Dictionary<string, LiveDataFeed> liveDataFeeds = this.LiveDataFeeds;
            foreach (KeyValuePair<string, LiveDataFeed> kvp in liveDataFeeds)
            {
                newLiveDataFeeds.Add(kvp.Key, kvp.Value.Clone());
            }

            return liveDataFeedSet;
        }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            LiveDataFeedSet liveDataFeedSet = (LiveDataFeedSet)obj;

            if (liveDataFeedSet.LiveDataFeeds.Count != this.LiveDataFeeds.Count) return false;

            foreach (KeyValuePair<string, LiveDataFeed> kvp in liveDataFeedSet.LiveDataFeeds)
            {
                if (!this.LiveDataFeeds.ContainsKey(kvp.Key)) return false;
                if (!liveDataFeedSet.LiveDataFeeds[kvp.Key].IsEqual(this.LiveDataFeeds[kvp.Key])) return false;
            }

            return true;
        }

        public void AddLiveDataFeed(LiveDataFeed liveDataFeed)
        {
            if (!_liveDataFeeds.ContainsKey(liveDataFeed.Name))
            {
                _liveDataFeeds.Add(liveDataFeed.Name, liveDataFeed);
            }
        }

        public LiveDataFeed GetLiveDataFeed(string liveDataFeedName)
        {
            if (_liveDataFeeds.ContainsKey(liveDataFeedName))
            {
                return _liveDataFeeds[liveDataFeedName];
            }

            return null;
        }

        // after editing user variables, check to see if the user variable name has changed; if yes, return new value
        public string UpdateLiveDataFeedName(string originalLiveDataFeedName)
        {
            foreach (KeyValuePair<string, LiveDataFeed> kvp in LiveDataFeeds)
            {
                LiveDataFeed liveDataFeed = kvp.Value;
                if (liveDataFeed.OriginalName == originalLiveDataFeedName)
                {
                    return liveDataFeed.Name;
                }
            }

            // error??
            return originalLiveDataFeedName;
        }

        public void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("liveDataFeeds");

            foreach (KeyValuePair<string, LiveDataFeed> kvp in LiveDataFeeds)
            {
                kvp.Value.WriteToXml(writer, publish);
            }

            writer.WriteFullEndElement(); // liveDataFeeds
        }

        public static LiveDataFeedSet ReadXml(XmlReader reader)
        {
            LiveDataFeedSet liveDataFeedSet = new LiveDataFeedSet();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "liveDataFeed":
                        LiveDataFeed liveDataFeed = LiveDataFeed.ReadXml(reader);
                        liveDataFeedSet.LiveDataFeeds.Add(liveDataFeed.Name, liveDataFeed);
                        break;
                }
            }

            return liveDataFeedSet;
        }
    }

    public class LiveBSNFeed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public LiveBSNFeed Clone()
        {
            LiveBSNFeed liveBSNFeed = new LiveBSNFeed
            {
                Id = this.Id,
                Name = this.Name,
                Url = this.Url
            };
            return liveBSNFeed;
        }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            LiveBSNFeed liveBSNFeed = (LiveBSNFeed)obj;

            return
                (this.Id == liveBSNFeed.Id) &&
                (this.Name == liveBSNFeed.Name) &&
                (this.Url == liveBSNFeed.Url);
        }

        public void WriteToXml(XmlTextWriter writer)
        {
            writer.WriteElementString("id", Id.ToString());
            writer.WriteElementString("name", Name);
            writer.WriteElementString("url", Url);
        }

        public static LiveBSNFeed ReadXml(XmlReader reader)
        {
            LiveBSNFeed liveBSNFeed = new LiveBSNFeed();

            int id = -1;
            string name = String.Empty;
            string url = String.Empty;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "id":
                        id = Convert.ToInt32(reader.ReadString());
                        break;
                    case "name":
                        name = reader.ReadString();
                        break;
                    case "url":
                        url = reader.ReadString();
                        break;
                }
            }

            liveBSNFeed.Id = id;
            liveBSNFeed.Name = name;
            liveBSNFeed.Url = url;

            return liveBSNFeed;
        }
    }

    public class LiveDynamicPlaylistFeed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool SupportsAudio { get; set; }

        public LiveDynamicPlaylistFeed Clone()
        {
            LiveDynamicPlaylistFeed liveDynamicPlaylistFeed = new LiveDynamicPlaylistFeed
            {
                Id = this.Id,
                Name = this.Name,
                Url = this.Url,
                SupportsAudio = this.SupportsAudio
            };
            return liveDynamicPlaylistFeed;
        }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            LiveDynamicPlaylistFeed liveDynamicPlaylistFeed = (LiveDynamicPlaylistFeed)obj;

            return
                (this.Id == liveDynamicPlaylistFeed.Id) &&
                (this.Name == liveDynamicPlaylistFeed.Name) &&
                (this.Url == liveDynamicPlaylistFeed.Url) &&
                (this.SupportsAudio == liveDynamicPlaylistFeed.SupportsAudio);
        }

        public void WriteToXml(XmlTextWriter writer)
        {
            writer.WriteElementString("id", Id.ToString());
            writer.WriteElementString("name", Name);
            writer.WriteElementString("url", Url);
            writer.WriteElementString("supportsAudio", SupportsAudio.ToString());
        }

        public static LiveDynamicPlaylistFeed ReadXml(XmlReader reader)
        {
            LiveDynamicPlaylistFeed liveDynamicPlaylistFeed = new LiveDynamicPlaylistFeed();

            int id = -1;
            string name = String.Empty;
            string url = String.Empty;
            bool supportsAudio = false;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "id":
                        id = Convert.ToInt32(reader.ReadString());
                        break;
                    case "name":
                        name = reader.ReadString();
                        break;
                    case "url":
                        url = reader.ReadString();
                        break;
                    case "supportsAudio":
                        supportsAudio = Convert.ToBoolean(reader.ReadString());
                        break;
                }
            }

            liveDynamicPlaylistFeed.Id = id;
            liveDynamicPlaylistFeed.Name = name;
            liveDynamicPlaylistFeed.Url = url;
            liveDynamicPlaylistFeed.SupportsAudio = supportsAudio;

            return liveDynamicPlaylistFeed;
        }
    }

}
