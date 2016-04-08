using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using System.Diagnostics;

namespace BAModel
{
    // ZonePlaylist adds a state hierarchy for interactive playlists

    public class ZonePlaylist : Playlist
    {
        private ObservableCollection<MediaState> _allMediaStates = new ObservableCollection<MediaState>();

        public enum ZonePlaylistType
        {
            NonInteractive,
            Interactive
        }

        public string ZoneID { get; set; }
        public string ZoneType { get; set; }
        public ZonePlaylistType Type { get; set; }

        private double _mediaStateXStart = 107;
        private double _mediaStateYStart = 15;
        private double _mediaStateWidth = 116;
        private double _mediaStateHeight = 99;

        private Sign _sign = null;

        public ZonePlaylist(string name, string zoneID, string zoneType) :
            base(name)
        {
            Type = ZonePlaylistType.NonInteractive;
            ZoneID = zoneID;
            ZoneType = zoneType;
        }

        // This constructor used to specify the Sign object containing this playlist.
        // This will be used in maintaining interactive states for a non-interactive playlist.
        // If this is not specified with this constructor, Sign.CurrentSign will be used.
        public ZonePlaylist(string name, string zoneID, string zoneType, Sign sign) :
            base(name)
        {
            Type = ZonePlaylistType.NonInteractive;
            ZoneID = zoneID;
            ZoneType = zoneType;
            _sign = sign;
        }

        public Boolean IsInteractive
        {
            get { return Type == ZonePlaylistType.Interactive; }
        }

        public MediaState InitialMediaState { get; set; }

        void PlaylistItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MediaPlaylistItem playlistItem = sender as MediaPlaylistItem;
            if (playlistItem != null)
            {
                UpdatePlaylistItemProperty(playlistItem, e.PropertyName);
            }
        }

        // gets all the media states within a playlist, including all substates
        public ObservableCollection<MediaState> AllMediaStates
        {
            get
            {
                ObservableCollection<MediaState> allMediaStates = new ObservableCollection<MediaState>();

                foreach (MediaState mediaState in AllTopLevelMediaStates)
                {
                    GetMediaStates(mediaState, allMediaStates);
                }

                return allMediaStates;
            }
        }

        // represents all the top level states in a playlist - not the substates of superstates
        public ObservableCollection<MediaState> AllTopLevelMediaStates
        {
            get { return _allMediaStates; }
            set { _allMediaStates = value; }
        }

        public ZonePlaylist Copy(ZonePlaylist playlist)
        {
            playlist.Name = this.Name;
            playlist.Type = this.Type;
            playlist.ZoneID = this.ZoneID;
            playlist.ZoneType = this.ZoneType;

            foreach (PlaylistItem playlistItem in this.Items)
            {
                PlaylistItem newPlaylistItem = (PlaylistItem)playlistItem.Clone();
                // Do not call AddPlaylistItem here because that generates MediaStates
                // We copy media states separately below
                playlist.Items.Add(newPlaylistItem);
            }

            foreach (MediaState mediaState in this.AllTopLevelMediaStates)
            {
                playlist.AllTopLevelMediaStates.Add(mediaState.CloneForComparison());
            }

            playlist.InitialMediaState = null;
            if (this.InitialMediaState != null)
            {
                foreach (MediaState mediaState in playlist.AllTopLevelMediaStates)
                {
                    if (mediaState.Name == this.InitialMediaState.Name)
                    {
                        playlist.InitialMediaState = mediaState;
                    }
                }
            }

            // All mediaStates and transitions have been created, but the transitions only contain the
            // names of the media states (as not all of them existed during the copy). Now, match them up.
            foreach (MediaState mediaState in playlist.AllMediaStates)
            {
                foreach (Transition transition in mediaState.TransitionsOut)
                {
                    MatchTransition(playlist, transition, false, true);
                }
            }

            return playlist;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            ZonePlaylist playlist = (ZonePlaylist)obj;
            if ( (playlist.Items.Count != this.Items.Count) ||
                 (playlist.Name != this.Name) ) return false;

            if (playlist.Type != this.Type) return false;
            if (playlist.ZoneType != this.ZoneType) return false;

            for (int i = 0; i < this.Items.Count; i++)
            {
                if (!this.Items[i].IsEqual(playlist.Items[i])) return false;
            }

            // compare interactive data
            if (playlist.AllTopLevelMediaStates.Count != this.AllTopLevelMediaStates.Count) return false;

            if (playlist.InitialMediaState == null && this.InitialMediaState != null) return false;
            if (playlist.InitialMediaState != null && this.InitialMediaState == null) return false;
            if (playlist.InitialMediaState != null && this.InitialMediaState != null)
            {
                if (!playlist.InitialMediaState.IsEqual(this.InitialMediaState)) return false;
            }

            for (int i = 0; i < this.AllTopLevelMediaStates.Count; i++)
            {
                if (!this.AllTopLevelMediaStates[i].IsEqual(playlist.AllTopLevelMediaStates[i])) return false;
            }

            return true;
        }

        public override void Clear()
        {
            foreach (PlaylistItem playlistItem in Items)
            {
                playlistItem.PropertyChanged -= PlaylistItemPropertyChanged;
            }
            base.Clear();
            _allMediaStates.Clear();
        }

        public override void AddPlaylistItem(PlaylistItem item)
        {
            Debug.Assert(Type == ZonePlaylistType.NonInteractive);
            base.AddPlaylistItem(item);
            AddMediaStateForPlaylistItem(item as MediaPlaylistItem);
            item.PropertyChanged += PlaylistItemPropertyChanged;
        }

        public override void InsertPlaylistItem(PlaylistItem item, int insertionIndex)
        {
            Debug.Assert(Type == ZonePlaylistType.NonInteractive);
            base.InsertPlaylistItem(item, insertionIndex);
            InsertMediaStateForPlaylistItem(item as MediaPlaylistItem, insertionIndex);
            item.PropertyChanged += PlaylistItemPropertyChanged;
        }

        public override void RemovePlaylistItem(PlaylistItem item)
        {
            Debug.Assert(Type == ZonePlaylistType.NonInteractive);
            int removeAtIndex = Items.IndexOf(item);
            base.RemovePlaylistItem(item);
            RemoveMediaState(removeAtIndex);
            item.PropertyChanged -= PlaylistItemPropertyChanged;
        }

//        public override void RegenerateThumbs()
//        {
//            base.RegenerateThumbs();
//
//            if (Type == ZonePlaylistType.Interactive)
//            {
//                foreach (MediaState mediaState in AllMediaStates)
//                {
//                    MediaPlaylistItem mediaPlaylistItem = mediaState.MediaPlaylistItem;
//                    if (mediaPlaylistItem != null)
//                    {
//                        if (mediaPlaylistItem is FilePlaylistItem)
//                        {
//                            (mediaPlaylistItem as FilePlaylistItem).RegenerateThumb();
//                        }
//                    }
//                }
//            }
//        }

        private List<BrightSignCmd> CopyBrightSignCmdList(List<BrightSignCmd> brightSignCmds)
        {
            List<BrightSignCmd> newBrightSignCmds = new List<BrightSignCmd>();
            foreach (BrightSignCmd bsc in brightSignCmds)
            {
                BrightSignCmd newBSC = bsc.Clone();
                newBrightSignCmds.Add(newBSC);
            }
            return newBrightSignCmds;
        }

        private void AddCmdsToMediaState(List<BrightSignCmd> pendingBrightSignCmds, MediaState mediaState)
        {
            if (pendingBrightSignCmds.Count > 0)
            {
                List<BrightSignCmd> bscList = CopyBrightSignCmdList(pendingBrightSignCmds);
                mediaState.BrightSignEntryCmds = bscList;
                pendingBrightSignCmds.Clear();
            }
        }

        private void AddCmdsToTransition(List<BrightSignCmd> pendingBrightSignCmds, Transition transition)
        {
            if (pendingBrightSignCmds.Count > 0)
            {
                List<BrightSignCmd> bscList = CopyBrightSignCmdList(pendingBrightSignCmds);
                transition.BrightSignCmds = bscList;
                pendingBrightSignCmds.Clear();
            }
        }

        private string GenerateMediaStateName(bool useGUID, string fileName, int index)
        {
            if (useGUID)
            {
                return fileName + Guid.NewGuid().ToString();
            }
            else
            {
                return fileName + "-" + index.ToString();
            }
        }

        private void AddMediaStateForPlaylistItem(MediaPlaylistItem playlistItem)
        {
            if (ZoneType == "EnhancedAudio")
            {
                UpdateEnhancedAudioStates(false);
            }
            else
            {
                int stateCount = AllTopLevelMediaStates.Count;
                MediaState priorState = stateCount > 0 ? AllTopLevelMediaStates[stateCount - 1] : null;
                MediaState mediaState = CreateMediaState(playlistItem, priorState, stateCount, false);
                if (mediaState != null)
                {
                    AllTopLevelMediaStates.Add(mediaState);

                    MediaState initialMediaState = AllTopLevelMediaStates[0];
                    if (stateCount > 1 || !initialMediaState.MediaPlaylistItem.AutomaticallyLoop)
                    {
                        // Create transition to loop back to initial state
                        CreateTransitionOutEvent(mediaState, initialMediaState, false);
                    }
                }
                InitialMediaState = AllTopLevelMediaStates[0];
            }
        }

        private void InsertMediaStateForPlaylistItem(MediaPlaylistItem playlistItem, int insertionIndex)
        {
            if (ZoneType == "EnhancedAudio")
            {
                UpdateEnhancedAudioStates(false);
            }
            else
            {
                int stateCount = AllTopLevelMediaStates.Count;
                if (insertionIndex < 0 || insertionIndex >= stateCount)
                {
                    AddMediaStateForPlaylistItem(playlistItem);
                }
                else if (insertionIndex >= 0)
                {
                    MediaState priorState = insertionIndex > 0 ? AllTopLevelMediaStates[insertionIndex - 1] : null;
                    MediaState nextState = AllTopLevelMediaStates[insertionIndex];
                    MediaState mediaState = CreateMediaState(playlistItem, priorState, insertionIndex, false);
                    if (mediaState != null)
                    {
                        AllTopLevelMediaStates.Insert(insertionIndex, mediaState);
                        CreateTransitionOutEvent(mediaState, nextState, false);
                    }
                    InitialMediaState = AllTopLevelMediaStates[0];
                }
            }
        }

        private void RemoveMediaState(int removeAtIndex)
        {
            if (ZoneType == "EnhancedAudio")
            {
                UpdateEnhancedAudioStates(false);
            }
            else
            {
                if (removeAtIndex >= 0 && removeAtIndex < AllTopLevelMediaStates.Count)
                {
                    MediaState priorState = removeAtIndex > 0 ? AllTopLevelMediaStates[removeAtIndex - 1] : null;
                    MediaState nextState = removeAtIndex < AllTopLevelMediaStates.Count - 1 ? AllTopLevelMediaStates[removeAtIndex + 1] : null;
                    AllTopLevelMediaStates.RemoveAt(removeAtIndex);
                    if (nextState != null)
                    {
                        CreateTransitionOutEvent(priorState, nextState, false);
                    }
                    else if (AllTopLevelMediaStates.Count > 0)
                    {
                        MediaState initialMediaState = AllTopLevelMediaStates[0];
                        if (AllTopLevelMediaStates.Count > 1 || !initialMediaState.MediaPlaylistItem.AutomaticallyLoop)
                        {
                            // Create transition to loop back to initial state
                            CreateTransitionOutEvent(priorState, initialMediaState, false);
                        }
                    }
                }
                InitialMediaState = AllTopLevelMediaStates.Count > 0 ? AllTopLevelMediaStates[0] : null;
            }
        }

        private Sign GetSign()
        {
            return _sign != null ? _sign : Sign.CurrentSign;
        }

        private MediaState CreateMediaState(MediaPlaylistItem playlistItem, MediaState priorState, int index, bool publish)
        {
            MediaState mediaState = null;
            if ((playlistItem is VideoPlaylistItem) || (playlistItem is ImagePlaylistItem) ||
                (playlistItem is ClockPlaylistItem) || (playlistItem is RSSFeedPlaylistItem) || (playlistItem is RFInputPlaylistItem) ||
                (playlistItem is TextPlaylistItem) || (playlistItem is LiveVideoPlaylistItem) ||
                (playlistItem is AudioPlaylistItem) || (playlistItem is SignChannelPlaylistItem) || (playlistItem is StreamPlaylistItem) ||
                (playlistItem is MRSSDataFeedPlaylistItem) || (playlistItem is LocalizedPlaylistItem) || (playlistItem is BackgroundImagePlaylistItem) ||
                (playlistItem is HTML5PlaylistItem))
            {
                string mediaStateName = publish ? GenerateMediaStateName(true, playlistItem.FileName, 0) : playlistItem.FileName;
                Sign sign = GetSign();
                if (sign != null)
                {
                    int diffIndex = 1;
                    while (sign.MediaStateNameExists(mediaStateName))
                    {
                        diffIndex++;
                        mediaStateName = GenerateMediaStateName(publish, playlistItem.FileName, diffIndex);
                    }
                    // If this zone isn't in the sign yet, we have to check it separately
                    // This is the case when a sign is being read from a file
                    if (!sign.HasZoneID(this.ZoneID))
                    {
                        while (this.MediaStateNameExists(mediaStateName))
                        {
                            diffIndex++;
                            mediaStateName = GenerateMediaStateName(publish, playlistItem.FileName, diffIndex);
                        }
                    }
                }
                if (publish)
                {
                    mediaState = new MediaState(playlistItem, mediaStateName);
                }
                else
                {
                    mediaState = new MediaState(mediaStateName, playlistItem, new System.Windows.Rect(0, 0, _mediaStateWidth, _mediaStateHeight), index);
                }
                CreateTransitionOutEvent(priorState, mediaState, publish);
                SetVolumeCommand(mediaState);
            }
            return mediaState;
        }

        private SimpleUserEvent CreateTransitionOutEvent(MediaState mediaState, MediaState targetMediaState, bool publish)
        {
            if (mediaState == null || targetMediaState == null)
                return null;

            SimpleUserEvent transitionEvent = mediaState.MediaPlaylistItem.CreateDefaultTransitionEvent();
            if (transitionEvent != null)
            {
                Transition transition = null;
                if (publish)
                {
                    transition = new Transition(mediaState, transitionEvent, targetMediaState, new List<BrightSignCmd>());
                }
                else
                {
                    transition = new Transition(mediaState, transitionEvent, targetMediaState, new List<BrightSignCmd>(), "displayLabel", "bottom");
                }
                AddCmdsToTransition(new List<BrightSignCmd>(), transition);
                // Replace any existing transition
                mediaState.TransitionsOut.Clear();
                mediaState.TransitionsOut.Add(transition);
                targetMediaState.TransitionsIn.Clear();
                targetMediaState.TransitionsIn.Add(transition);
            }
            return transitionEvent;
        }

        private void SetVolumeCommand(MediaState mediaState)
        {
            MediaPlaylistItem playlistItem = mediaState.MediaPlaylistItem;
            string volume = playlistItem.Volume;
            if (volume != "")
            {
                BrightSignCmd brightSignCmd = null;

                Sign sign = GetSign();
                Boolean zoneVolumeSupported = false;
                if (sign != null)
                {
                    BrightSignModel model = BrightSignModelMgr.GetBrightSignModel(sign.Model);
                    zoneVolumeSupported = model.FeatureIsSupported(BrightSignModel.ModelFeature.AudioOutputControl);
                }
                if (zoneVolumeSupported)
                {
                    brightSignCmd = BrightSignCmdMgr.CommandSetByName["SetZoneVolume"].Clone();
                    ZoneVolume.PopulateBrightSignCmd(brightSignCmd, ZoneID, volume);
                }
                else
                {
                    BrightSignModel model = BrightSignModelMgr.GetBrightSignModel(Sign.CurrentSign.Model);
                    if (model.FeatureIsSupported(BrightSignModel.ModelFeature.AudioOutputControl))
                    {
                        brightSignCmd = BrightSignCmdMgr.CommandSetByName["SetZoneVolume"].Clone();
                        ZoneVolume.PopulateBrightSignCmd(brightSignCmd, ZoneID, volume);
                    }
                    else
                    {
                        BrightSignCommand bsc = null;
                        if (playlistItem is AudioPlaylistItem)
                        {
                            bsc = BrightSignCommandMgr.GetBrightSignCommand("setAudioVolume").Clone();
                        }
                        else
                        {
                            bsc = BrightSignCommandMgr.GetBrightSignCommand("setVideoVolume").Clone();
                        }
                        bsc.Parameters = volume;
                        brightSignCmd = BrightSignCmd.FromBrightSignCommand(bsc);
                    }
                }
                // Replace any existing command(s)
                mediaState.BrightSignEntryCmds.Clear();
                mediaState.BrightSignEntryCmds.Add(brightSignCmd);
            }
        }

        private void UpdatePlaylistItemProperty(MediaPlaylistItem playlistItem, string propertyName)
        {
            int index = Items.IndexOf(playlistItem);
            int stateCount = AllTopLevelMediaStates.Count;
            if (index >= 0 && index < stateCount)
            {
                MediaState mediaState = AllTopLevelMediaStates[index];
                if (propertyName == "DefaultTransition")
                {
                    Trace.WriteLine("ZonePlaylist: DefaultTransition Property changed: " + playlistItem.Description());
                    // Update transition
                    if (index < stateCount - 1)
                    {
                        CreateTransitionOutEvent(mediaState, AllTopLevelMediaStates[index + 1], false);
                    }
                    else
                    {
                        MediaState initialMediaState = AllTopLevelMediaStates[0];
                        if (stateCount > 1 || !initialMediaState.MediaPlaylistItem.AutomaticallyLoop)
                        {
                            // Create transition to loop back to initial state
                            CreateTransitionOutEvent(mediaState, initialMediaState, false);
                        }
                    }
                }
                else if (propertyName == "Volume")
                {
                    Trace.WriteLine("ZonePlaylist: Volume Property changed: " + playlistItem.Description());
                    SetVolumeCommand(mediaState);
                }
            }
        }

        private void UpdateEnhancedAudioStates(bool publish)
        {
            AllTopLevelMediaStates.Clear();

            MediaState mediaState = null;
            MediaState priorState = null;
            int index = 0;

            int playlistItemIndex = 0;
            while (playlistItemIndex < _items.Count)
            {
                MediaPlaylistItem mediaPlaylistItem = null;

                PlaylistItem playlistItem = _items[playlistItemIndex];

                if (playlistItem is AudioPlaylistItem)
                {
                    ObservableCollection<FilePlaylistItem> audioListPlaylistItems = new ObservableCollection<FilePlaylistItem>();

                    while (playlistItemIndex < _items.Count && (_items[playlistItemIndex] is AudioPlaylistItem))
                    {
                        playlistItem = _items[playlistItemIndex];
                        audioListPlaylistItems.Add(playlistItem as AudioPlaylistItem);
                        playlistItemIndex++;
                    }

                    MediaListPlaylistItem mediaListPlaylistItem = new MediaListPlaylistItem("audio", !publish);
                    mediaListPlaylistItem.FileName = "audioList";
                    mediaListPlaylistItem.FilePlaylistItems = audioListPlaylistItems;
                    mediaListPlaylistItem.AdvanceOnMediaEnd = true;
                    mediaListPlaylistItem.AdvanceOnImageTimeout = true;
                    mediaListPlaylistItem.PlayFromBeginning = true;
                    mediaListPlaylistItem.ImageTimeout = "0";
                    mediaListPlaylistItem.Shuffle = false;
                    mediaListPlaylistItem.SlideTransition = "No effect";
                    mediaListPlaylistItem.SendZoneMessage = false;
                    mediaListPlaylistItem.NextNavigation = null;
                    mediaListPlaylistItem.PreviousNavigation = null;

                    mediaPlaylistItem = mediaListPlaylistItem;
                }
                else
                {
                    mediaPlaylistItem = (playlistItem as MRSSDataFeedPlaylistItem);

                    playlistItemIndex++;
                }

                string mediaStateName = publish ? GenerateMediaStateName(true, mediaPlaylistItem.FileName, 0) : mediaPlaylistItem.FileName;
                Sign sign = GetSign();
                if (sign != null)
                {
                    while (sign.MediaStateNameExists(mediaStateName))
                    {
                        index++;
                        mediaStateName = GenerateMediaStateName(publish, mediaPlaylistItem.FileName, index);
                    }
                }
                if (publish)
                {
                    mediaState = new MediaState(mediaPlaylistItem, mediaStateName);
                }
                else
                {
                    mediaState = new MediaState(mediaStateName, mediaPlaylistItem, new System.Windows.Rect(0, 0, _mediaStateWidth, _mediaStateHeight), index);
                }

                CreateTransitionOutEvent(priorState, mediaState, false);
                AllTopLevelMediaStates.Add(mediaState);

                priorState = mediaState;
            }
            InitialMediaState = AllTopLevelMediaStates[0];
            if (AllTopLevelMediaStates.Count > 1)
            {
                // Create transition to loop back to initial state
                CreateTransitionOutEvent(mediaState, InitialMediaState, false);
            }
        }

        // Initialize coordinates for all current media states
        private void ArrangeInitialInteractiveViewData()
        {
            double x = _mediaStateXStart;
            double y = _mediaStateYStart;

            int count = AllTopLevelMediaStates.Count;
            foreach (MediaState mediaState in AllTopLevelMediaStates)
            {
                --count;
                mediaState.Rect = new System.Windows.Rect(x, y, _mediaStateWidth, _mediaStateHeight);
                y += 140;
                // Should be only one transition here, but just in case...
                foreach (Transition transitionOut in mediaState.TransitionsOut)
                {
                    // Last state Transition Display mode will not be a line
                    transitionOut.DisplayMode = count == 0 ? "displayLabel" : "displayConnectingLine";
                    transitionOut.LabelLocation = "bottom";
                }
            }
        }

        // Remove non-interactive support
        // Media states are already intact as they are maintained for non-interactive list
        public void ConvertToInteractive()
        {
            // Only do this if list is non-interactive
            if (Items.Count > 0)
            {
                // Set up graphic coordinates in media states
                ArrangeInitialInteractiveViewData();
                // Make sure InitialMediaState is set
                InitialMediaState = AllTopLevelMediaStates[0];
                // Clear non-interactive data (i.e., the Items list)
                foreach (PlaylistItem playlistItem in Items)
                {
                    playlistItem.PropertyChanged -= PlaylistItemPropertyChanged;
                }
                // Clear Items collection (base playlist)
                base.Clear();
                // Sign reference no longer needed
                _sign = null;
            }
            Type = ZonePlaylistType.Interactive;
        }

        // Convert from interactive to non-interactive
        // This will clear the playlist
        public void ConvertToNonInteractive()
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                Clear();
                Type = ZonePlaylistType.NonInteractive;
            }
        }

        private ZonePlaylist CreateInteractivePlaylistForPublishing()
        {
            if (Items.Count > 0)
            {
                ZonePlaylist publishPlaylist = new ZonePlaylist(this.Name, ZoneID, ZoneType);
                if (ZoneType == "EnhancedAudio")
                {
                    // Copy non-interactive items only
                    foreach (PlaylistItem playlistItem in Items)
                    {
                        PlaylistItem newPlaylistItem = (PlaylistItem)playlistItem.Clone();
                        publishPlaylist.Items.Add(newPlaylistItem);
                    }
                    publishPlaylist.UpdateEnhancedAudioStates(true);
                }
                else
                {
                    foreach (PlaylistItem playlistItem in Items)
                    {
                        PlaylistItem newPlaylistItem = (PlaylistItem)playlistItem.Clone();
                        publishPlaylist.Items.Add(newPlaylistItem);

                        int stateCount = publishPlaylist.AllTopLevelMediaStates.Count;
                        MediaState priorState = stateCount > 0 ? publishPlaylist.AllTopLevelMediaStates[stateCount - 1] : null;
                        MediaState mediaState = publishPlaylist.CreateMediaState(newPlaylistItem as MediaPlaylistItem, priorState, stateCount, true);
                        if (mediaState != null)
                        {
                            publishPlaylist.AllTopLevelMediaStates.Add(mediaState);

                            MediaState initialMediaState = publishPlaylist.AllTopLevelMediaStates[0];
                            if (stateCount > 0 || !initialMediaState.MediaPlaylistItem.AutomaticallyLoop)
                            {
                                // Create transition to loop back to initial state
                                publishPlaylist.CreateTransitionOutEvent(mediaState, initialMediaState, true);
                            }
                        }
                    }
                }
                publishPlaylist.InitialMediaState = publishPlaylist.AllTopLevelMediaStates[0];
                return publishPlaylist;
            }
            return this;
        }

        public void WriteToXml(XmlTextWriter writer, Sign sign, bool publish, string zoneType, string zoneId)
        {
            writer.WriteStartElement("playlist");

            writer.WriteElementString("name", _name);
            writer.WriteElementString("type", Type == ZonePlaylistType.Interactive ? "interactive" : "non-interactive");

            MediaState initialMediaState = null;
            ObservableCollection<MediaState> mediaStatesToPublish = new ObservableCollection<MediaState>();
            ObservableCollection<MediaState> transitionMediaStatesToPublish = new ObservableCollection<MediaState>();

            if (Type == ZonePlaylistType.NonInteractive)
            {
                // if saving the file (not publishing), save the playlist items as a simple playlist
                if (!publish)
                {
                    foreach (PlaylistItem playlistItem in _items)
                    {
                        playlistItem.WriteToXml(writer, publish, sign, null);
                    }
                }
                // if publishing, create an interactive playlist and publish
                else
                {
                    ZonePlaylist publishPlaylist = CreateInteractivePlaylistForPublishing();
                    mediaStatesToPublish = publishPlaylist.AllTopLevelMediaStates;
                    transitionMediaStatesToPublish = publishPlaylist.AllTopLevelMediaStates;
                    initialMediaState = publishPlaylist.InitialMediaState;
                }
            }
            else
            {
                mediaStatesToPublish = AllTopLevelMediaStates;
                transitionMediaStatesToPublish = AllMediaStates;
                initialMediaState = InitialMediaState;
            }

            // write out interactive data

            // states
            writer.WriteStartElement("states");

            // initial media state
            string initialMediaStateName = "";
            if (initialMediaState != null)
            {
                initialMediaStateName = initialMediaState.Name;
            }
            writer.WriteElementString("initialState", initialMediaStateName);

            foreach (MediaState mediaState in mediaStatesToPublish)
            {
                if (mediaState.MediaPlaylistItem != null)
                {
                    // state
                    writer.WriteStartElement("state");
                    mediaState.WriteToXml(writer, publish, sign);
                    writer.WriteEndElement(); // state
                }
            }

            foreach (MediaState mediaState in transitionMediaStatesToPublish)
            {
                foreach (Transition transition in mediaState.TransitionsOut)
                {
                    transition.WriteToXml(writer, publish);
                }
            }

            writer.WriteEndElement(); // states

            writer.WriteEndElement(); // playlist
        }

        public static ZonePlaylist ReadXml(XmlReader reader, Zone zone)
        {
            ZonePlaylist playlist = null;

            string name = "";

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        name = reader.ReadString();
                        playlist = new ZonePlaylist(name, zone.ZoneID, zone.ZoneType);
                        break;
                    case "type":
                        string type = reader.ReadString();
                        playlist.Type = type == "interactive" ? ZonePlaylistType.Interactive : ZonePlaylistType.NonInteractive;
                        break;
                    case "videoItem":
                    case "imageItem":
                    case "audioItem":
                    case "enhancedAudioItem":
                    case "rssItem":
                    case "twitterItem":
                    case "rssDataFeedPlaylistItem":
                    case "mrssDataFeedPlaylistItem":
                    case "textItem":
                    case "clockItem":
                    case "backgroundImageItem":
                    case "liveVideoItem":
                    case "audioInItem":
                    case "signChannelItem":
                    case "rssImageItem":
                    case "mediaListItem":
                    case "streamItem":
                    case "videoStreamItem":
                    case "audioStreamItem":
                    case "mjpegItem":
                    case "localPlaylistItem":
                    case "rfInputItem":
                    case "rfScanItem":
                    case "html5Item":
                    case "xModemItem":
                    case "superStateItem":
                        if (playlist == null) return null;
                        PlaylistItem newPlaylistItem = PlaylistItem.ReadXml(reader, zone);
                        playlist.AddPlaylistItem(newPlaylistItem);
                        break;
                    case "states":
                        string initialMediaStateName = "";

                        while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
                        {
                            if (playlist.Type == ZonePlaylistType.Interactive)
                            {
                                switch (reader.LocalName)
                                {
                                    case "initialState":
                                        initialMediaStateName = reader.ReadString();
                                        break;
                                    case "state":
                                        MediaState mediaState = MediaState.ReadXml(reader, -1);
                                        playlist.AllTopLevelMediaStates.Add(mediaState);
                                        break;
                                    case "transition":
                                        Transition transition = Transition.ReadXml(reader);
                                        MatchTransition(playlist, transition, true, true);
                                        MatchConditionalTargets(playlist, transition);
                                        break;
                                }
                            }
                        }
                        if (playlist.Type == ZonePlaylistType.Interactive)
                        {
                            MatchInitialState(playlist, initialMediaStateName);
                            MatchInteractiveMenuStates(playlist.AllMediaStates, playlist.AllMediaStates);
                        }
                        break;
                }
            }

            return playlist;
        }

        public void Publish(List<PublishFile> publishFiles)
        {
            foreach (PlaylistItem playlistItem in Items)
            {
                playlistItem.Publish(publishFiles);
            }

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllTopLevelMediaStates)
                {
                    if (mediaState.MediaPlaylistItem != null)
                    {
                        mediaState.MediaPlaylistItem.Publish(publishFiles);
                    }

                    foreach (Transition transition in mediaState.TransitionsOut)
                    {
                        transition.Publish(publishFiles);
                    }
                }
            }
        }

        override public void ReplaceMediaFiles(Dictionary<string, string> replacementFiles, bool preserveStateNames)
        {
            base.ReplaceMediaFiles(replacementFiles, preserveStateNames);

            foreach (MediaState mediaState in AllMediaStates)
            {
                if (mediaState.MediaPlaylistItem != null)
                {
                    string existingFileName = mediaState.MediaPlaylistItem.FileName;
                    mediaState.MediaPlaylistItem.ReplaceMediaFiles(replacementFiles, false);
                    if (existingFileName != mediaState.MediaPlaylistItem.FileName)
                    {
                        if (!preserveStateNames) mediaState.Name = GetUniqueMediaStateName(mediaState.MediaPlaylistItem.FileName);
                        mediaState.FileName = mediaState.MediaPlaylistItem.FileName;
                    }
                }

                foreach (Transition transition in mediaState.TransitionsOut)
                {
                    transition.ReplaceMediaFiles(replacementFiles);
                }
            }
        }

        public void MatchInteractiveMenuStates(List<MediaState> allMediaStates)
        {
            ObservableCollection<MediaState> mediaStates = new ObservableCollection<MediaState>(allMediaStates);
            MatchInteractiveMenuStates(mediaStates, this.AllMediaStates);
        }

        private static void MatchInteractiveMenuStates(ObservableCollection<MediaState> allMediaStates, ObservableCollection<MediaState> allPlaylistMediaStates)
        {
            Dictionary<string, MediaState> stateDictionary = new Dictionary<string,MediaState>();
            foreach (MediaState mediaState in allPlaylistMediaStates)
            {
                stateDictionary.Add(mediaState.Name, mediaState);
            }
            foreach (MediaState mediaState in allMediaStates)
            {
                MediaPlaylistItem mediaPlaylistItem = mediaState.MediaPlaylistItem;
                if (mediaPlaylistItem != null)
                {
                    if (mediaPlaylistItem is InteractiveMenuPlaylistItem)
                    {
                        InteractiveMenuPlaylistItem interactiveMenuPlaylistItem = mediaPlaylistItem as InteractiveMenuPlaylistItem;

                        if (interactiveMenuPlaylistItem.InteractiveMenuItems != null)
                        {
                            List<InteractiveMenuItem> interactiveMenuItemsToDelete = new List<InteractiveMenuItem>();

                            foreach (InteractiveMenuItem interactiveMenuItem in interactiveMenuPlaylistItem.InteractiveMenuItems)
                            {
                                if (!String.IsNullOrEmpty(interactiveMenuItem.TargetMediaStateName))
                                {
                                    if (stateDictionary.ContainsKey(interactiveMenuItem.TargetMediaStateName))
                                    {
                                        interactiveMenuItem.TargetMediaState = stateDictionary[interactiveMenuItem.TargetMediaStateName];
                                    }
                                    else
                                    {
                                        // if the target of an interactive menu item isn't found, delete the menu item
                                        interactiveMenuItemsToDelete.Add(interactiveMenuItem);
                                    }
                                }
                            }

                            foreach (InteractiveMenuItem interactiveMenuItemToDelete in interactiveMenuItemsToDelete)
                            {
                                InteractiveMenuPlaylistItem.UpdateNavigationReferencesAfterDelete(interactiveMenuPlaylistItem.InteractiveMenuItems, interactiveMenuItemToDelete.Index);
                                interactiveMenuPlaylistItem.InteractiveMenuItems.Remove(interactiveMenuItemToDelete);
                                InteractiveMenuPlaylistItem.UpdateMenuItemIndices(interactiveMenuPlaylistItem.InteractiveMenuItems);
                            }
                        }
                    }
                    else if (mediaPlaylistItem is SuperStatePlaylistItem)
                    {
                        SuperStatePlaylistItem superStatePlaylistItem = mediaPlaylistItem as SuperStatePlaylistItem;
                        MatchInteractiveMenuStates(superStatePlaylistItem.MediaStates, allPlaylistMediaStates); // or is it .AllMediaStates?
                    }
                }
            }
        }

        private static void MatchInitialState(ZonePlaylist playlist, string initialMediaStateName)
        {
            if (initialMediaStateName == "")
            {
                playlist.InitialMediaState = null;
            }
            else
            {
                foreach (MediaState mediaState in playlist.AllTopLevelMediaStates)
                {
                    if (mediaState.Name == initialMediaStateName)
                    {
                        playlist.InitialMediaState = mediaState;
                        break;
                    }
                }
            }
        }

        public void GetMediaStates(MediaState mediaState, ObservableCollection<MediaState> mediaStates)
        {
            mediaStates.Add(mediaState);

            if (mediaState.MediaPlaylistItem != null && mediaState.MediaPlaylistItem is SuperStatePlaylistItem)
            {
                foreach (MediaState subState in (mediaState.MediaPlaylistItem as SuperStatePlaylistItem).MediaStates)
                {
                    GetMediaStates(subState, mediaStates);
                }
            }
        }

        private static void MatchTransition(ZonePlaylist playlist, Transition transition, bool addToTransitionOutList, bool addToTransitionInList)
        {
            ObservableCollection<MediaState> allMediaStates = playlist.AllMediaStates;

            string sourceMediaStateName = transition.SourceMediaStateName;
            string targetMediaStateName = transition.TargetMediaStateName;
           
            foreach (MediaState mediaState in allMediaStates)
            {
                if (mediaState.Name == sourceMediaStateName)
                {
                    if (addToTransitionOutList) mediaState.TransitionsOut.Add(transition);
                    transition.SourceMediaState = mediaState;
                }
                if (mediaState.Name == targetMediaStateName)
                {
                    if (addToTransitionInList) mediaState.TransitionsIn.Add(transition);
                    transition.TargetMediaState = mediaState;
                }
            }
        }

        private static void MatchConditionalTargets(ZonePlaylist playlist, Transition transition)
        {
            foreach (ConditionalTarget conditionalTarget in transition.ConditionalTargets)
            {
                string targetMediaStateName = conditionalTarget.TargetMediaStateName;

                foreach (MediaState mediaState in playlist.AllMediaStates)
                {
                    if (mediaState.Name == targetMediaStateName)
                    {
                        conditionalTarget.TargetMediaState = mediaState;
                    }
                }
            }
        }

        public void CheckMRSSLiveDataFeeds(Sign sign)
        {
            foreach (MediaState mediaState in AllMediaStates)
            {
                if ((mediaState.MediaPlaylistItem != null) && (mediaState.MediaPlaylistItem is MRSSDataFeedPlaylistItem))
                {
                    MRSSDataFeedPlaylistItem mrssDataFeedPlaylistItem = mediaState.MediaPlaylistItem as MRSSDataFeedPlaylistItem;
                    mrssDataFeedPlaylistItem.CheckMRSSLiveDataFeed(sign);
                }
            }
        }

        override public void FindBrokenMediaLinks(List<Object> brokenLinks)
        {
            base.FindBrokenMediaLinks(brokenLinks);

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    if (mediaState.MediaPlaylistItem != null)
                    {
                        mediaState.MediaPlaylistItem.FindBrokenMediaLinks(brokenLinks);

                        foreach (Transition transition in mediaState.TransitionsOut)
                        {
                            transition.FindBrokenMediaLinks(brokenLinks);
                        }
                    }
                }
            }
        }

        override public bool IsEmpty()
        {
            if (Items.Count != 0) return false;
            return AllTopLevelMediaStates.Count == 0;
        }

        override public void FindDuplicateMediaFiles(Dictionary<string, string> fileSpecs, DuplicateFileList duplicateFileList)
        {
            base.FindDuplicateMediaFiles(fileSpecs, duplicateFileList);

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllTopLevelMediaStates)
                {
                    MediaPlaylistItem mediaPlaylistItem = mediaState.MediaPlaylistItem;
                    mediaPlaylistItem.FindDuplicateMediaFiles(fileSpecs, duplicateFileList);
                }
            }
        }

        public void GetUserVariablesInUse(Dictionary<string, UserVariableInUse> userVariablesInUse)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.GetUserVariablesInUse(userVariablesInUse);
                }
            }
        }

        public void UpdateUserDefinedEvents(ObservableCollection<UserDefinedEvent> userDefinedEvents)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.UpdateUserDefinedEvents(userDefinedEvents);
                }
            }
        }

        public void UpdateUserVariables(UserVariableSet userVariableSet)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.UpdateUserVariables(userVariableSet);
                }
            }
        }

        public void GetPresentationsInUse(Dictionary<string, PresentationInUse> presentationsInUse)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.GetPresentationsInUse(presentationsInUse);
                }
            }
        }

        public void UpdatePresentationIdentifiers(PresentationIdentifierSet presentationIdentifierSet)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.UpdatePresentationIdentifiers(presentationIdentifierSet);
                }
            }
        }

        public void GetScriptPluginsInUse(Dictionary<string, string> scriptPluginsInUse)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.GetScriptPluginsInUse(scriptPluginsInUse);
                }
            }
        }

        public void UpdateScriptPlugins(ScriptPluginSet scriptPluginSet)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.UpdateScriptPlugins(scriptPluginSet);
                }
            }
        }

        public void GetLiveDataFeedsInUse(Dictionary<string, string> liveDataFeedsInUse)
        {
            foreach (PlaylistItem playlistItem in Items)
            {
                List<string> liveDataFeedNamesInUse = playlistItem.GetLiveDataFeedsInUse();

                foreach (string liveDataFeedNameInUse in liveDataFeedNamesInUse)
                {
                    if (!liveDataFeedsInUse.ContainsKey(liveDataFeedNameInUse))
                    {
                        liveDataFeedsInUse.Add(liveDataFeedNameInUse, liveDataFeedNameInUse);
                    }
                }
            }

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.GetLiveDataFeedsInUse(liveDataFeedsInUse);
                }
            }
        }

        public bool DataFeedDataUsageLegal(LiveDataFeed liveDataFeed, Zone zone, LiveDataFeed.DataFeedUsage dataFeedUsage)
        {
            foreach (PlaylistItem playlistItem in Items)
            {
                if (!playlistItem.DataFeedDataUsageLegal(liveDataFeed, zone, dataFeedUsage)) return false;
            }

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    if (!mediaState.DataFeedDataUsageLegal(liveDataFeed, zone, dataFeedUsage)) return false;
                }
            }
            return true;
        }

        public void UpdateLiveDataFeeds(LiveDataFeedSet liveDataFeedSet)
        {
            foreach (PlaylistItem playlistItem in Items)
            {
                playlistItem.UpdateLiveDataFeeds(liveDataFeedSet);
            }

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.UpdateLiveDataFeeds(liveDataFeedSet);
                }
            }
        }

        public void GetHTMLSitesInUse(Dictionary<string, string> htmlSitesInUse)
        {
            foreach (PlaylistItem playlistItem in Items)
            {
                string htmlSiteNameInUse = playlistItem.GetHTMLSiteInUse();

                if (htmlSiteNameInUse != null)
                {
                    if (!htmlSitesInUse.ContainsKey(htmlSiteNameInUse))
                    {
                        htmlSitesInUse.Add(htmlSiteNameInUse, htmlSiteNameInUse);
                    }
                }
            }

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.GetHTMLSitesInUse(htmlSitesInUse);
                }
            }
        }

        public void UpdateHTMLSites(HTMLSiteSet htmlSiteSet)
        {
            foreach (PlaylistItem playlistItem in Items)
            {
                playlistItem.UpdateHTMLSites(htmlSiteSet);
            }

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.UpdateHTMLSites(htmlSiteSet);
                }
            }
        }

        public bool FilesExist(out string filePath)
        {
            filePath = "";

            foreach (PlaylistItem playlistItem in Items)
            {
                bool fileExists = playlistItem.FileExists(out filePath);
                if (!fileExists) return false;
            }

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllTopLevelMediaStates)
                {
                    MediaPlaylistItem mediaPlaylistItem = mediaState.MediaPlaylistItem;
                    bool fileExists = mediaPlaylistItem.FileExists(out filePath);
                    if (!fileExists) return false;
                }
            }
            return true;
        }

        public bool DebugPortUsed()
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    foreach (BrightSignCmd brightSignCmd in mediaState.BrightSignEntryCmds)
                    {
                        bool debugPortUsed = brightSignCmd.DebugPortUsed();
                        if (debugPortUsed) return true;
                    }

                    foreach (BrightSignCmd brightSignCmd in mediaState.BrightSignExitCmds)
                    {
                        bool debugPortUsed = brightSignCmd.DebugPortUsed();
                        if (debugPortUsed) return true;
                    }

                    foreach (Transition transition in mediaState.TransitionsOut)
                    {
                        if (transition.IsEventType("serial"))
                        {
                            return true;
                        }

                        List<BrightSignCmd> brightSignCmds = transition.BrightSignCmds;
                        foreach (BrightSignCmd brightSignCmd in brightSignCmds)
                        {
                            List<BSCommand> bsCommands = brightSignCmd.Commands;
                            foreach (BSCommand bsCommand in bsCommands)
                            {
                                bool debugPortUsed = brightSignCmd.DebugPortUsed();
                                if (debugPortUsed) return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void UsesUDP(out bool usesUDPSend, out bool usesUDPReceive)
        {
            usesUDPSend = false;
            usesUDPReceive = false;

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    List<BrightSignCmd> brightSignCmds = mediaState.BrightSignEntryCmds;
                    foreach (BrightSignCmd brightSignCmd in brightSignCmds)
                    {
                        if ((brightSignCmd.Name == "SendUDP") || (brightSignCmd.Name == "Synchronize"))
                        {
                            usesUDPSend = true;
                        }
                    }

                    brightSignCmds = mediaState.BrightSignExitCmds;
                    foreach (BrightSignCmd brightSignCmd in brightSignCmds)
                    {
                        if ((brightSignCmd.Name == "SendUDP") || (brightSignCmd.Name == "Synchronize"))
                        {
                            usesUDPSend = true;
                        }
                    }

                    ObservableCollection<Transition> transitionsOut = mediaState.TransitionsOut;
                    foreach (Transition transition in transitionsOut)
                    {
                        if (transition.IsEventType("udp") || transition.IsEventType("Synchronize"))
                        {
                            usesUDPReceive = true;
                        }

                        brightSignCmds = transition.BrightSignCmds;
                        foreach (BrightSignCmd brightSignCmd in brightSignCmds)
                        {
                            if ((brightSignCmd.Name == "SendUDP") || (brightSignCmd.Name == "Synchronize"))
                            {
                                usesUDPSend = true;
                            }
                        }
                    }
                }
            }
        }

        public void GetRemoteFiles(ref List<string> remoteFileUrls)
        {
            foreach (PlaylistItem playlistItem in Items)
            {
                if (playlistItem is FilePlaylistItem)
                {
                    FilePlaylistItem filePlaylistItem = playlistItem as FilePlaylistItem;
                    if (!filePlaylistItem.FileIsLocal)
                    {
                        remoteFileUrls.Add(filePlaylistItem.FilePath);
                    }
                }
            }

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    MediaPlaylistItem mediaPlaylistItem = mediaState.MediaPlaylistItem;
                    if (mediaPlaylistItem is FilePlaylistItem)
                    {
                        FilePlaylistItem filePlaylistItem = mediaPlaylistItem as FilePlaylistItem;
                        if (!filePlaylistItem.FileIsLocal)
                        {
                            remoteFileUrls.Add(filePlaylistItem.FilePath);
                        }
                    }
                }
            }
        }

        public void VersionUpdate(Sign sign, Zone zone, int oldVersion, int newVersion, List<BrightSignCmd> convertedBrightSignCmds)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                // upgrade from version 2 to version 3
                //      Volume in Interactive Playlists must be converted
                if (oldVersion == 2)
                {
                    foreach (MediaState mediaState in AllTopLevelMediaStates)
                    {
                        ConvertItem(mediaState);
                    }
                }

                // convert audio commands as necessary
                foreach (MediaState mediaState in AllTopLevelMediaStates)
                {
                    mediaState.ConvertAudioCommands(sign, zone, convertedBrightSignCmds);
                }
            }
        }

        private void ConvertItem(MediaState mediaState)
        {
            MediaPlaylistItem playlistItem = mediaState.MediaPlaylistItem;
            if ((playlistItem is VideoPlaylistItem) || (playlistItem is LiveVideoPlaylistItem) || (playlistItem is AudioPlaylistItem))
            {
                string volume = "";
                if (playlistItem is VideoPlaylistItem)
                {
                    VideoPlaylistItem vpi = playlistItem as VideoPlaylistItem;
                    volume = vpi.Volume;
                    vpi.Volume = "";
                }
                else if (playlistItem is LiveVideoPlaylistItem)
                {
                    LiveVideoPlaylistItem lvpi = playlistItem as LiveVideoPlaylistItem;
                    volume = lvpi.Volume;
                    lvpi.Volume = "";
                }
                else
                {
                    AudioPlaylistItem api = playlistItem as AudioPlaylistItem;
                    volume = api.Volume;
                    api.Volume = "";
                }
                if (volume != "")
                {
                    BrightSignCommand bsc = null;
                    if (playlistItem is AudioPlaylistItem)
                    {
                        bsc = BrightSignCommandMgr.GetBrightSignCommand("setAudioVolume").Clone();
                    }
                    else
                    {
                        bsc = BrightSignCommandMgr.GetBrightSignCommand("setVideoVolume").Clone();
                    }
                    bsc.Parameters = volume;
                    BrightSignCmd brightSignCmd = BrightSignCmd.FromBrightSignCommand(bsc);
                    mediaState.BrightSignEntryCmds.Add(brightSignCmd);
                }
            }
        }

        public void GetImageFiles(Dictionary<string, object> imagePaths)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    MediaPlaylistItem item = mediaState.MediaPlaylistItem;
                    if (item != null)
                    {
                        item.GetImageFiles(imagePaths);
                    }
                }
            }
        }

        public string GetUniqueMediaStateName(string mediaStateName, Dictionary<string, MediaState> clipboardMediaStates, Dictionary<string, MediaState> existingMediaStates)
        {
            string originalMediaStateName = mediaStateName;
            int counter = 1;

            // If there isn't a media state in the existing media states with the same name, then this name is okay
            if (!existingMediaStates.ContainsKey(mediaStateName)) return originalMediaStateName;

            // Find a name that is unique across both the existing media states names and the media states on the clipboard
            mediaStateName = originalMediaStateName + " " + counter.ToString();
            while (MediaStateNameExists(mediaStateName, clipboardMediaStates, existingMediaStates))
            {
                counter++;
                mediaStateName = originalMediaStateName + " " + counter.ToString();
            }

            return mediaStateName;
        }

        public bool MediaStateNameExists(string mediaStateName, Dictionary<string, MediaState> clipboardMediaStates, Dictionary<string, MediaState> existingMediaStates)
        {
            if (existingMediaStates.ContainsKey(mediaStateName)) return true;
            return (clipboardMediaStates.ContainsKey(mediaStateName));
        }

        public string GetUniqueMediaStateName(string mediaStateName)
        {
            string originalMediaStateName = mediaStateName;
            int counter = 0;

            while (MediaStateNameExists(mediaStateName))
            {
                counter++;
                mediaStateName = originalMediaStateName + " " + counter.ToString();
            }

            return mediaStateName;
        }

        public bool MediaStateNameExists(string mediaStateName)
        {
            // jsugg - 11/16/15 - bug 23306
            // Now that we are maintaining the mediaStates for non-interactive playlists, we just always check the media states
            //  directly for name collisions. No need to check the non-interactive Items list at all.
            ObservableCollection<MediaState> allMediaStates = AllMediaStates;

            foreach (MediaState mediaState in allMediaStates)
            {
                if (mediaState.Name.ToLower() == mediaStateName.ToLower()) return true;
            }
            return false;
        }

        public void GetUsedUserDefinedEvents(Dictionary<string, UserDefinedEvent> usedUserDefinedEvents)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.GetUsedUserDefinedEvents(usedUserDefinedEvents);
                }
            }
        }

        public void GetMissingUserDefinedEvents(Dictionary<string, UserDefinedEvent> missingUserDefinedEvents)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    mediaState.GetMissingUserDefinedEvents(missingUserDefinedEvents);
                }
            }
        }

        public bool GPSInUse()
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    if (mediaState.GPSInUse()) return true;
                }
            }
            return false;
        }

        public void GetSynchronizationUsage(out bool playlistUsesSyncCommands, out bool playlistHasSyncEvents)
        {
            playlistUsesSyncCommands = false;
            playlistHasSyncEvents = false;

            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (MediaState mediaState in AllMediaStates)
                {
                    bool usesSyncCommands;
                    bool hasSyncEvents;
                    mediaState.GetSynchronizationUsage(out usesSyncCommands, out hasSyncEvents);
                    playlistUsesSyncCommands |= usesSyncCommands;
                    playlistHasSyncEvents |= hasSyncEvents;
                }
            }
        }

        public void RebuildForUndo()
        {
            foreach (PlaylistItem playlistItem in Items)
            {
                if (playlistItem is MediaPlaylistItem)
                {
                    (playlistItem as MediaPlaylistItem).CreateThumbnail();
                }
            }

            // We need to maintain the media state hierarchy during Undo, even in non-interactive lists
            RelinkTransitionsIn();

            foreach (MediaState mediaState in AllMediaStates)
            {
                mediaState.RebuildForUndo();
            }
        }

        private void RelinkTransitionsIn()
        {
            foreach (MediaState mediaState in AllTopLevelMediaStates)
            {
                foreach (Transition transition in mediaState.TransitionsOut)
                {
                    string targetMediaStateName = transition.TargetMediaStateName;

                    foreach (MediaState targetMediaState in AllTopLevelMediaStates)
                    {
                        if (targetMediaState.Name == targetMediaStateName)
                        {
                            targetMediaState.TransitionsIn.Add(transition);
                        }
                    }
                }
            }
        }

        public bool ContainsAudioAsset()
        {
            foreach (MediaState mediaState in AllTopLevelMediaStates)
            {
                if (mediaState.MediaPlaylistItem != null)
                {
                    if (mediaState.IsAudioItem()) return true;
                }
            }

            return false;
        }

        public void MatchTransitions(List<Transition> transitions)
        {
            if (Type == ZonePlaylistType.Interactive)
            {
                foreach (Transition transition in transitions)
                {
                    MatchTransition(this, transition, true, true);
                    MatchConditionalTargets(this, transition);
                }
            }
        }

        public bool ImportStatesFromFile(XmlReader reader, out List<MediaState> states, out List<Transition> transitions, ObservableCollection<MediaState> existingMediaStates)
        {
            ObservableCollection<MediaState> cumulativeMediaStates = new ObservableCollection<MediaState>();
            foreach (MediaState ms in existingMediaStates) // existing media states
            {
                cumulativeMediaStates.Add(ms);
            }

            states = new List<MediaState>();
            transitions = new List<Transition>();

            try
            {
                while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
                {
                    switch (reader.LocalName)
                    {
                        case "state":
                            int newId = InteractivePlaylist.GetNextMediaStateID(cumulativeMediaStates);
                            MediaState state = MediaState.ReadXml(reader, newId);
                            states.Add(state);
                            cumulativeMediaStates.Add(state);
                            break;
                        case "transition":
                            Transition transition = Transition.ReadXml(reader);
                            transitions.Add(transition);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception in ImportStatesFromFile");
                Trace.WriteLine(ex.ToString());

                return false;
            }

            return true;
        }
    }
}
