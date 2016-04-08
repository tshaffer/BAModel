using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Xml;

namespace BAModel
{
    public class PlaylistItem : INotifyPropertyChanged
    {
        protected string _itemLabel = "";

        protected int _uniqueID;

        public PlaylistItem()
        {
        }

        public virtual void WriteToXml(XmlTextWriter writer, bool publish, Sign sign, MediaState mediaState)
        {
        }

        public virtual object Clone() // ICloneable implementation
        {
            return null;
        }

        public object Copy(Object obj)
        {
            PlaylistItem playlistItem = (PlaylistItem)obj;

            playlistItem.UniqueID = this.UniqueID;

            return playlistItem;
        }

        public virtual object CloneWithNewUniqueID() // ICloneable implementation
        {
            return null;
        }

        public virtual bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            PlaylistItem playlistItem = (PlaylistItem)obj;

            return playlistItem.UniqueID == this.UniqueID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual string ItemLabel
        {
            get { return _itemLabel; }
            set 
            {
                _itemLabel = value;
                this.OnPropertyChanged("ItemLabel");
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public virtual string Description()
        {
            return GetType().Name;
        }   

        public virtual void EditItem(Zone zone)
        {
        }

        public virtual void EditItems(Zone zone, List<PlaylistItem> playlistItems)
        {
        }

        public int UniqueID
        {
            get { return _uniqueID; }
            set { _uniqueID = value; }
        }

        public static PlaylistItem ReadXml(XmlReader reader, Zone zone)
        {
            PlaylistItem newPlaylistItem = null;
            switch (reader.LocalName)
            {
                case "videoItem":
                    newPlaylistItem = VideoPlaylistItem.ReadXml(reader);
                    break;
                case "imageItem":
                    newPlaylistItem = ImagePlaylistItem.ReadXml(reader);
                    break;
                case "audioItem":
                    newPlaylistItem = AudioPlaylistItem.ReadXml(reader);
                    break;
                case "twitterItem":
                    newPlaylistItem = TwitterPlaylistItem.ReadXml(reader);
                    break;
                case "rssDataFeedPlaylistItem":
                    newPlaylistItem = RSSDataFeedPlaylistItem.ReadXml(reader);
                    break;
                case "rssItem":
                    newPlaylistItem = RSSDataFeedPlaylistItem.ReadRSSPlaylistItemXml(reader);
                    break;
                case "textItem":
                    newPlaylistItem = TextPlaylistItem.ReadXml(reader);
                    break;
                case "clockItem":
                    newPlaylistItem = ClockPlaylistItem.ReadXml(reader, (ClockZone)zone);
                    break;
                case "backgroundImageItem":
                    newPlaylistItem = BackgroundImagePlaylistItem.ReadXml(reader);
                    break;
                case "liveVideoItem":
                    newPlaylistItem = LiveVideoPlaylistItem.ReadXml(reader);
                    break;
                case "templatePlaylistItem":
                    newPlaylistItem = TemplatePlaylistItem.ReadXml(reader);
                    break;
                case "liveTextItem":
                    newPlaylistItem = TemplatePlaylistItem.ReadLiveTextXml(reader);
                    break;
                case "playFileItem":
                    newPlaylistItem = PlayFilePlaylistItem.ReadXml(reader);
                    break;
                case "eventHandlerItem":
                    newPlaylistItem = EventHandlerPlaylistItem.ReadXml(reader);
                    break;
                case "eventHandler2Item":
                    newPlaylistItem = EventHandlerPlaylistItem.ReadParameterizedXml(reader);
                    break;
                case "interactiveMenuItem":
                    newPlaylistItem = InteractiveMenuPlaylistItem.ReadXml(reader);
                    break;
                case "audioInItem":
                    newPlaylistItem = AudioInPlaylistItem.ReadXml(reader);
                    break;
                case "signChannelItem":
                    newPlaylistItem = SignChannelPlaylistItem.ReadXml(reader);
                    break;
                case "rssImageItem":
                    newPlaylistItem = MRSSDataFeedPlaylistItem.ReadRSSImagePlaylistItemXml(reader);
                    break;
                case "mrssDataFeedPlaylistItem":
                    newPlaylistItem = MRSSDataFeedPlaylistItem.ReadXml(reader);
                    break;
                case "mediaListItem":
                    newPlaylistItem = MediaListPlaylistItem.ReadXml(reader);
                    break;
                case "tripleUSBItem":
                    newPlaylistItem = TripleUSBPlaylistItem.ReadXml(reader);
                    break;
                case "streamItem":
                    newPlaylistItem = StreamPlaylistItem.ReadXml(reader);
                    break;
                case "videoStreamItem":
                    newPlaylistItem = VideoStreamPlaylistItem.ReadXml(reader);
                    break;
                case "audioStreamItem":
                    newPlaylistItem = AudioStreamPlaylistItem.ReadXml(reader);
                    break;
                case "mjpegItem":
                    newPlaylistItem = MjpegPlaylistItem.ReadXml(reader);
                    break;
                case "localPlaylistItem":
                    newPlaylistItem = LocalizedPlaylistItem.ReadXml(reader);
                    break;
                case "rfInputItem":
                    newPlaylistItem = RFInputPlaylistItem.ReadXml(reader);
                    break;
                case "rfScanItem":
                    newPlaylistItem = RFScanPlaylistItem.ReadXml(reader);
                    break;
                case "html5Item":
                    newPlaylistItem = HTML5PlaylistItem.ReadXml(reader);
                    break;
                case "xModemItem":
                    newPlaylistItem = XModemPlaylistItem.ReadXml(reader);
                    break;
                case "superStateItem":
                    newPlaylistItem = SuperStatePlaylistItem.ReadXml(reader);
                    break;
            }
            return newPlaylistItem;
        }

        public virtual ImageSource Thumbnail
        {
            get { return null; }
            set { }
        }

        public virtual string ThumbnailStretchMode
        {
            get { return "None"; }
            set { }
        }

        public virtual ImageSource Icon
        {
            get { return null; }
            set { }
        }

        public virtual string SlideDelayInterval
        {
            get { return ""; }
            set { }
        }

        public virtual string SlideTransition
        {
            get { return ""; }
            set { }
        }

        public virtual string Volume
        {
            get { return ""; }
            set { }
        }

        public virtual bool AutomaticallyLoop
        {
            get { return false; }
            set { }
        }

        public virtual string Parameters
        {
            get { return ""; }
            set { }
        }

        public virtual string Type
        {
            get { return ""; }
        }

        public virtual void Publish(List<PublishFile> publishFiles)
        {
        }

        public virtual void ReplaceMediaFiles(Dictionary<string, string> replacementFiles, bool preserveStateNames)
        {
        }

        public virtual void FindBrokenMediaLinks(List<Object> brokenLinks)
        {
        }

        public virtual void FindDuplicateMediaFiles(Dictionary<string, string> fileSpecs, DuplicateFileList duplicateFileList)
        {
        }

        public virtual bool FileExists(out string filePath)
        {
            filePath = "";
            return true;
        }

        public virtual List<string> GetLiveDataFeedsInUse()
        {
            return new List<string>();
        }

        public virtual bool DataFeedDataUsageLegal(LiveDataFeed liveDataFeed, Zone zone, LiveDataFeed.DataFeedUsage dataFeedUsage)
        {
            return true;
        }

        public virtual void UpdateLiveDataFeeds(LiveDataFeedSet liveDataFeedSet)
        {
        }

        public virtual string GetHTMLSiteInUse()
        {
            return null;
        }

        public virtual void UpdateHTMLSites(HTMLSiteSet htmlSiteSet)
        {
        }

        public virtual bool NamesEqual(string newName)
        {
            return false;
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
}
