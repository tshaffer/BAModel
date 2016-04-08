using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;
//using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows;
//using System.Windows.Controls;

namespace BAModel
{
    public class Zone : INotifyPropertyChanged
    {
        private string _name;
        // TODO - store as percentages, in pixels, or both?
        private int _xStart;
        private int _yStart;
        private int _width;
        private int _height;
        private string _zoneType;
        private string _zoneID;
        private string _zoneTypeLabel;

        //private List<ZonePlaylist> _playlists = new List<ZonePlaylist>();

        ZonePlaylist _playlist = null;

        public double ZoomValue { get; set; }
        public double HorizontalOffset { get; set; }
        public double VerticalOffset { get; set; }

        public Zone(string name, int xStart, int yStart, int width, int height, string zoneType, string zoneID)
        {
            _name = name;
            _xStart = xStart;
            _yStart = yStart;
            _width = width;
            _height = height;
            _zoneType = zoneType;
            _zoneID = zoneID;

            _zoneTypeLabel = BrightAuthorUtils.GetLocalizedString(zoneType);

            ZoomValue = 1.0;
            HorizontalOffset = 0.0;
            VerticalOffset = 0.0;
        }

        public virtual object Clone() // ICloneable implementation
        {
            return null;
        }

        public object Copy(Object obj)
        {
            Zone zone = (Zone)obj;

            // copy all data members
            zone.Name = this.Name;
            zone.X = this.X;
            zone.Y = this.Y;
            zone.Width = this.Width;
            zone.Height = this.Height;
            zone.ZoneType = this.ZoneType;
            zone.ZoneID = this.ZoneID;

            zone.ZoomValue = this.ZoomValue;
            zone.HorizontalOffset = this.HorizontalOffset;
            zone.VerticalOffset = this.VerticalOffset;

            if (Playlist != null)
            {
                ZonePlaylist newPlaylist = new ZonePlaylist(Playlist.Name, _zoneID, _zoneType);
                zone.Playlist = this.Playlist.Copy(newPlaylist);
            }
            else
            {
                zone.Playlist = null;
            }

            zone._zoneTypeLabel = this._zoneTypeLabel;

            return zone;
        }

        public virtual bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            Zone zone = (Zone)obj;

            bool zonesEqual = (zone.Name == this.Name) && (zone.X == this.X) && (zone.Y == this.Y) &&
                 (zone.Width == this.Width) && (zone.Height == this.Height) &&
                 (zone.ZoneType == this.ZoneType) && (zone.ZoneID == this.ZoneID) &&
                 (zone.ZoomValue == this.ZoomValue) && (zone.HorizontalOffset == this.HorizontalOffset) && (zone.VerticalOffset == this.VerticalOffset)
                 ;
            if (!zonesEqual) return false;

            // compare playlists
            if ((this.Playlist == null) && (zone.Playlist == null)) return true;
            if ((this.Playlist == null) || (zone.Playlist == null)) return false;
            if (!this.Playlist.IsEqual(zone.Playlist)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.OnPropertyChanged("Name");
                this.OnPropertyChanged("FullName");
                this.OnPropertyChanged("FullNameAndType");
            }
        }

        public string FullName
        {
            get
            {
                string fullName = ZoneID;
                if (Name != "")
                {
                    fullName += ": ";
                    fullName += Name;
                }
                return fullName;
            }
        }

        public string FullNameAndType
        {
            get
            {
                return FullName + "\n" + ZoneType;
            }
        }

        public int X
        {
            get { return _xStart; }
            set { _xStart = value; }
        }

        public int Y
        {
            get { return _yStart; }
            set { _yStart = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public string ZoneType
        {
            get { return _zoneType; }
            set
            {
                _zoneType = value;
                if (_playlist != null)
                {
                    _playlist.ZoneType = value;
                }
            }
        }

        public string ZoneTypeLabel
        {
            get { return _zoneTypeLabel; }
            // set { _zoneType = value; }
        }

        public string ZoneID
        {
            get { return _zoneID; }
            set
            {
                _zoneID = value;
                if (_playlist != null)
                {
                    _playlist.ZoneID = value;
                }
            }
        }

        public ZonePlaylist Playlist
        {
            get { return _playlist; }
            set { _playlist = value; }
        }

        public ZonePlaylist AddNewPlaylist(string name)
        {
            _playlist = new ZonePlaylist(name, _zoneID, _zoneType);
            return _playlist;
        }

        public virtual int UsedAudioDecoders()
        {
            return 0;
        }

        public void WriteToXml(XmlTextWriter writer, bool publish, Sign sign)
        {
            writer.WriteStartElement("zone");

            writer.WriteElementString("name", _name);
            writer.WriteElementString("x", _xStart.ToString());
            writer.WriteElementString("y", _yStart.ToString());
            writer.WriteElementString("width", _width.ToString());
            writer.WriteElementString("height", _height.ToString());
            writer.WriteElementString("type", _zoneType);
            writer.WriteElementString("id", _zoneID);

            if (!publish)
            {
                writer.WriteElementString("zoomValue", ZoomValue.ToString());
                writer.WriteElementString("horizontalOffset", HorizontalOffset.ToString());
                writer.WriteElementString("verticalOffset", VerticalOffset.ToString());
            }

            writer.WriteStartElement("zoneSpecificParameters");
            WriteZoneSpecificDataToXml(writer, publish, sign);
            writer.WriteEndElement(); // ZoneSpecificParameters

            if (_playlist != null)
            {
                _playlist.WriteToXml(writer, sign, publish, _zoneType, _zoneID);
            }

            writer.WriteEndElement(); // zone
        }

        public virtual void WriteZoneSpecificDataToXml(XmlTextWriter writer, bool publish, Sign sign)
        {
        }

        public static void ReadZonesXml(XmlReader reader, Sign sign)
        {
            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "zone":
                        Zone zone = Zone.ReadZoneXml(reader, sign);
                        sign.ZoneList.Add(zone);

                        break;
                }
            }
        }

        public static Zone ReadZoneXml(XmlReader reader, Sign sign)
        {
            Zone zone = null;

            string name = "";
            int x = -1;
            int y = -1;
            int width = -1;
            int height = -1;
            string type = "";
            string id = "";

            double zoomValue = 1.0;
            double horizontalOffset = 0.0;
            double verticalOffset = 0.0;

            TextWidget tw = null;
            bool displayTime = true;
            Widget w = null;

            int viewMode = BrightAuthorUtils.GetViewModeValue(UserPreferences.ViewMode);
            AudioZone.AudioOutputSelection audioOutput = BrightAuthorUtils.GetAudioOutputEnum(UserPreferences.AudioOutput);
            AudioZone.AudioModeSelection audioMode = BrightAuthorUtils.GetAudioModeEnum(UserPreferences.AudioMode);
            AudioZone.AudioMappingSelection audioMapping = BrightAuthorUtils.GetAudioMappingEnum(UserPreferences.AudioMapping);
            AudioZone.AudioOutputType analogOutput = UserPreferences.AnalogOutput;
            AudioZone.AudioOutputType analog2Output = AudioZone.AudioOutputType.None;
            AudioZone.AudioOutputType analog3Output = AudioZone.AudioOutputType.None;
            AudioZone.AudioOutputType hdmiOutput = UserPreferences.HDMIOutput;
            AudioZone.AudioOutputType spdifOutput = UserPreferences.SPDIFOutput;
            AudioZone.AudioOutputType usbOutput = AudioZone.AudioOutputType.None;
            AudioZone.AudioOutputType usbOutputA = AudioZone.AudioOutputType.None;
            AudioZone.AudioOutputType usbOutputB = AudioZone.AudioOutputType.None;
            AudioZone.AudioOutputType usbOutputC = AudioZone.AudioOutputType.None;
            AudioZone.AudioOutputType usbOutputD = AudioZone.AudioOutputType.None;
            AudioZone.AudioMixMode audioMixMode = UserPreferences.AudioMixMode;
            int imageMode = BrightAuthorUtils.GetImageModeValue(UserPreferences.ImageMode);
            string videoVolume = UserPreferences.InitialVideoVolume;
            string audioVolume = UserPreferences.InitialAudioVolume;
            string minimumVolume = "0";
            string maximumVolume = "100";
            string liveVideoInput = UserPreferences.LiveVideoInput;
            string liveVideoStandard = UserPreferences.LiveVideoStandard;
            string brightness = UserPreferences.Brightness;
            string contrast = UserPreferences.Contrast;
            string saturation = UserPreferences.Saturation;
            string hue = UserPreferences.Hue;
            bool zOrderFront = true;
            bool mosaic = false;
            MosaicDecoder.MaxContentResolution maxResolution = MosaicDecoder.MaxContentResolution._NotApplicable;

            string clockRotation = UserPreferences.ClockRotation;

            int fadeLength = 0;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        name = reader.ReadString();
                        break;
                    case "x":
                        x = Convert.ToInt32(reader.ReadString());
                        break;
                    case "y":
                        y = Convert.ToInt32(reader.ReadString());
                        break;
                    case "width":
                        width = Convert.ToInt32(reader.ReadString());
                        break;
                    case "height":
                        height = Convert.ToInt32(reader.ReadString());
                        break;
                    case "type":
                        type = reader.ReadString();
                        break;
                    case "id":
                        id = reader.ReadString();
                        break;
                    case "zoomValue":
                        zoomValue = Convert.ToDouble(reader.ReadString());
                        break;
                    case "horizontalOffset":
                        horizontalOffset = Convert.ToDouble(reader.ReadString());
                        break;
                    case "verticalOffset":
                        verticalOffset = Convert.ToDouble(reader.ReadString());
                        break;
                    case "zoneSpecificParameters":
                        switch (type)
                        {
                            case "VideoOrImages":
                                VideoOrImagesZone.ReadZoneSpecificDataXml(reader, out viewMode, out audioOutput, out audioMode, out audioMapping,
                                    out analogOutput, out analog2Output, out analog3Output, out hdmiOutput, out spdifOutput,
                                    out usbOutput, out usbOutputA, out usbOutputB, out usbOutputC, out usbOutputD, 
                                    out audioMixMode,
                                    out imageMode,
                                    out videoVolume, out audioVolume, out minimumVolume, out maximumVolume,
                                    out liveVideoInput, out liveVideoStandard, out brightness, out contrast, out saturation, out hue, out zOrderFront, out mosaic, out maxResolution);
                                zone = new VideoOrImagesZone(name, x, y, width, height, type, id,
                                    viewMode, audioOutput, audioMode, audioMapping,
                                    analogOutput, analog2Output, analog3Output, hdmiOutput, spdifOutput,
                                    usbOutput, usbOutputA, usbOutputB, usbOutputC, usbOutputD, 
                                    audioMixMode,
                                    imageMode,
                                    videoVolume, audioVolume, minimumVolume, maximumVolume,
                                    liveVideoInput, liveVideoStandard, brightness, contrast, saturation, hue, zOrderFront, mosaic, maxResolution);
                                break;
                            case "VideoOnly":
                                VideoZone.ReadZoneSpecificDataXml(reader, out viewMode, out audioOutput, out audioMode, out audioMapping,
                                    out analogOutput, out analog2Output, out analog3Output, out hdmiOutput, out spdifOutput,
                                    out usbOutput, out usbOutputA, out usbOutputB, out usbOutputC, out usbOutputD,
                                    out audioMixMode,
                                    out videoVolume, out audioVolume, out minimumVolume, out maximumVolume,
                                    out liveVideoInput, out liveVideoStandard, out brightness, out contrast, out saturation, out hue, out zOrderFront,
                                    out mosaic, out maxResolution);
                                zone = new VideoZone(name, x, y, width, height, type, id,
                                    viewMode, audioOutput, audioMode, audioMapping,
                                    analogOutput, analog2Output, analog3Output, hdmiOutput, spdifOutput,
                                    usbOutput, usbOutputA, usbOutputB, usbOutputC, usbOutputD,
                                    audioMixMode,
                                    videoVolume, audioVolume, minimumVolume, maximumVolume,
                                    liveVideoInput, liveVideoStandard, brightness, contrast, saturation, hue,
                                    zOrderFront, mosaic, maxResolution);
                                break;
                            case "Images":
                                ImageZone.ReadZoneSpecificDataXml(reader, out imageMode);
                                zone = new ImageZone(name, x, y, width, height, type, id, imageMode);
                                break;
                            case "AudioOnly":
                                AudioZone.ReadZoneSpecificDataXml(reader, out audioOutput, out audioMode, out audioMapping,
                                    out analogOutput, out analog2Output, out analog3Output, out hdmiOutput, out spdifOutput,
                                    out usbOutput, out usbOutputA, out usbOutputB, out usbOutputC, out usbOutputD,
                                    out audioMixMode,
                                    out audioVolume, out minimumVolume, out maximumVolume);
                                zone = new AudioZone(name, x, y, width, height, type, id, audioOutput, audioMode, audioMapping,
                                    analogOutput, analog2Output, analog3Output, hdmiOutput, spdifOutput,
                                    usbOutput, usbOutputA, usbOutputB, usbOutputC, usbOutputD,
                                    audioMixMode,
                                    audioVolume, minimumVolume, maximumVolume);
                                break;
                            case "EnhancedAudio":
                                EnhancedAudioZone.ReadZoneSpecificDataXml(reader, out audioOutput, out audioMode, out audioMapping,
                                    out analogOutput, out analog2Output, out analog3Output, out hdmiOutput, out spdifOutput,
                                    out usbOutput, out usbOutputA, out usbOutputB, out usbOutputC, out usbOutputD,
                                    out audioMixMode,
                                    out audioVolume, out minimumVolume, out maximumVolume, out fadeLength);
                                zone = new EnhancedAudioZone(name, x, y, width, height, type, id, audioOutput, audioMode, audioMapping,
                                    analogOutput, analog2Output, analog3Output, hdmiOutput, spdifOutput,
                                    usbOutput, usbOutputA, usbOutputB, usbOutputC, usbOutputD,
                                    audioMixMode,
                                    audioVolume, minimumVolume, maximumVolume, fadeLength);
                                break;
                            case "Ticker":
                                int scrollSpeed = 100;
                                tw = TickerZone.ReadZoneSpecificDataXml(reader, out w, out scrollSpeed);
                                zone = new TickerZone(name, x, y, width, height, type, id, w);
                                TickerZone tz = (TickerZone)zone;
                                tz.TextWidget = tw;
                                tz.ScrollSpeed = scrollSpeed;
                                break;
                            case "Clock":
                                ClockZone.ReadZoneSpecificDataXml(reader, out displayTime, out w, out clockRotation);
                                zone = new ClockZone(name, x, y, width, height, type, id, displayTime, w, clockRotation);
                                break;
                            case "BackgroundImage":
                                zone = new BackgroundImageZone(name, x, y, width, height, type, id);
                                break;
                        }
                        break;
                    case "playlist":
                        zone.Playlist = ZonePlaylist.ReadXml(reader, zone);
                        break;
                }
            }

            zone.ZoomValue = zoomValue;
            zone.HorizontalOffset = horizontalOffset;
            zone.VerticalOffset = verticalOffset;

            return zone;
        }

//        public virtual void EditZoneParameters(Window1 parent)
//        {
//        }

        public virtual void Publish(List<PublishFile> publishFiles)
        {
            if (Playlist != null)
            {
                Playlist.Publish(publishFiles);
            }
        }

        public virtual void ReplaceMediaFiles(Dictionary<string, string> replacementFiles, bool preserveStateNames)
        {
            if (Playlist != null)
            {
                Playlist.ReplaceMediaFiles(replacementFiles, preserveStateNames);
            }
        }

        public void GetImageFiles(Dictionary<string, object> imagePaths)
        {
            if (Playlist != null)
            {
                Playlist.GetImageFiles(imagePaths);
            }
        }

        public void CheckMRSSLiveDataFeeds(Sign sign)
        {
            if (Playlist != null)
            {
                Playlist.CheckMRSSLiveDataFeeds(sign);
            }
        }

        public virtual void FindBrokenMediaLinks(List<Object> brokenLinks)
        {
            if (Playlist != null)
            {
                Playlist.FindBrokenMediaLinks(brokenLinks);
            }
        }

        public bool ZoneIsEmpty()
        {
            if (Playlist == null) return true;
            return Playlist.IsEmpty();
        }

        public void UsesUDP(out bool usesUDPSend, out bool usesUDPReceive)
        {
            usesUDPSend = false;
            usesUDPReceive = false;

            if (Playlist != null)
            {
                Playlist.UsesUDP(out usesUDPSend, out usesUDPReceive);
            }
        }

        public void FindDuplicateMediaFiles(Dictionary<string, string> fileSpecs, DuplicateFileList duplicateFileList)
        {
            if (this is TickerZone)
            {
                TickerZone tickerZone = this as TickerZone;

                tickerZone.Widget.FindDuplicateMediaFiles(fileSpecs, duplicateFileList);
            }
            else if (this is ClockZone)
            {
                ClockZone clockZone = this as ClockZone;

                clockZone.Widget.FindDuplicateMediaFiles(fileSpecs, duplicateFileList);
            }

            if (Playlist != null)
            {
                Playlist.FindDuplicateMediaFiles(fileSpecs, duplicateFileList);
            }
        }

        public void GetUserVariablesInUse(Dictionary<string, UserVariableInUse> userVariablesInUse)
        {
            if (Playlist != null)
            {
                Playlist.GetUserVariablesInUse(userVariablesInUse);
            }
        }

        public void UpdateUserDefinedEvents(ObservableCollection<UserDefinedEvent> userDefinedEvents)
        {
            if (Playlist != null)
            {
                Playlist.UpdateUserDefinedEvents(userDefinedEvents);
            }
        }

        public void UpdateUserVariables(UserVariableSet userVariableSet)
        {
            if (Playlist != null)
            {
                Playlist.UpdateUserVariables(userVariableSet);
            }
        }

        public void GetPresentationsInUse(Dictionary<string, PresentationInUse> presentationsInUse)
        {
            if (Playlist != null)
            {
                Playlist.GetPresentationsInUse(presentationsInUse);
            }
        }

        public void UpdatePresentationIdentifiers(PresentationIdentifierSet presentationIdentifierSet)
        {
            if (Playlist != null)
            {
                Playlist.UpdatePresentationIdentifiers(presentationIdentifierSet);
            }
        }

        public void UpdateScriptPlugins(ScriptPluginSet scriptPluginSet)
        {
            if (Playlist != null)
            {
                Playlist.UpdateScriptPlugins(scriptPluginSet);
            }
        }

        public void GetScriptPluginsInUse(Dictionary<string, string> scriptPluginsInUse)
        {
            if (Playlist != null)
            {
                Playlist.GetScriptPluginsInUse(scriptPluginsInUse);
            }
        }

        public void GetLiveDataFeedsInUse(Dictionary<string, string> liveDataFeedsInUse)
        {
            if (Playlist != null)
            {
                Playlist.GetLiveDataFeedsInUse(liveDataFeedsInUse);
            }
        }

        public bool DataFeedDataUsageLegal(LiveDataFeed liveDataFeed, LiveDataFeed.DataFeedUsage dataFeedUsage)
        {
            if (Playlist != null)
            {
                return Playlist.DataFeedDataUsageLegal(liveDataFeed, this, dataFeedUsage);
            }
            return true;
        }

        public void UpdateLiveDataFeeds(LiveDataFeedSet liveDataFeedSet)
        {
            if (Playlist != null)
            {
                Playlist.UpdateLiveDataFeeds(liveDataFeedSet);
            }
        }

        public void GetHTMLSitesInUse(Dictionary<string, string> htmlSitesInUse)
        {
            if (Playlist != null)
            {
                Playlist.GetHTMLSitesInUse(htmlSitesInUse);
            }
        }

        public void UpdateHTMLSites(HTMLSiteSet htmlSiteSet)
        {
            if (Playlist != null)
            {
                Playlist.UpdateHTMLSites(htmlSiteSet);
            }
        }

        public bool FilesExist(out string filePath)
        {
            bool fileExists = true;
            filePath = "";

            if (this is TickerZone)
            {
                TickerZone tickerZone = this as TickerZone;
                
                fileExists = tickerZone.Widget.FontExists(out filePath);
                if (!fileExists) return false;

                fileExists = tickerZone.Widget.BackgroundBitmapFileExists(out filePath);
                if (!fileExists) return false;
            }
            else if (this is ClockZone)
            {
                ClockZone clockZone = this as ClockZone;

                fileExists = clockZone.Widget.FontExists(out filePath);
                if (!fileExists) return false;

                fileExists = clockZone.Widget.BackgroundBitmapFileExists(out filePath);
                if (!fileExists) return false;
            }

            if (Playlist != null)
            {
                fileExists = Playlist.FilesExist(out filePath);
            }

            return fileExists;
        }

        public bool DebugPortUsed()
        {
            if ((this is VideoOrImagesZone) || (this is VideoZone) || (this is ImageZone) || (this is AudioZone))
            {
                if (Playlist != null)
                {
                    return Playlist.DebugPortUsed();
                }
            }
            return false;
        }

        public void GetRemoteFiles(ref List<string> remoteFileUrls)
        {
            if (Playlist != null)
            {
                Playlist.GetRemoteFiles(ref remoteFileUrls);
            }
        }

        public void VersionUpdate(Sign sign, int oldVersion, int newVersion, List<BrightSignCmd> convertedBrightSignCmds)
        {
            // upgrade from version 2 to version 3
            //      Volume in Interactive Playlists must be converted
            if (_playlist != null)
            {
                _playlist.VersionUpdate(sign, this, oldVersion, newVersion, convertedBrightSignCmds);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(
                    this, new PropertyChangedEventArgs(propName));
        }

        #endregion

        public void GetUsedUserDefinedEvents(Dictionary<string, UserDefinedEvent> usedUserDefinedEvents)
        {
            if (Playlist != null)
            {
                Playlist.GetUsedUserDefinedEvents(usedUserDefinedEvents);
            }
        }

        public void GetMissingUserDefinedEvents(Dictionary<string, UserDefinedEvent> missingUserDefinedEvents)
        {
            if (Playlist != null)
            {
                Playlist.GetMissingUserDefinedEvents(missingUserDefinedEvents);
            }
        }

        public bool MediaStateNameExists(string mediaStateName)
        {
            if (Playlist != null)
            {
                return Playlist.MediaStateNameExists(mediaStateName);
            }
            return false;
        }

        public bool GPSInUse()
        {
            if (Playlist != null)
            {
                return Playlist.GPSInUse();
            }
            
            return false;
        }

        public void GetSynchronizationUsage(out bool usesSyncCommands, out bool hasSyncEvents)
        {
            usesSyncCommands = false;
            hasSyncEvents = false;

            if (Playlist != null)
            {
                Playlist.GetSynchronizationUsage(out usesSyncCommands, out hasSyncEvents);
            }
        }

        public void RebuildForUndo()
        {
            if (Playlist != null)
            {
                Playlist.RebuildForUndo();
            }
        }

        public bool ContainsAudioAsset()
        {
            bool containsAudioAsset = false;

            if (Playlist != null)
            {
                return Playlist.ContainsAudioAsset();
            }

            return containsAudioAsset;
        }
    }

}
