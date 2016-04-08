using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows.Controls;
//using System.Windows.Media;
using System.Collections.ObjectModel;

namespace BAModel
{
    public class MediaPlaylistItem : PlaylistItem
    {
        protected string _fileName;
//        protected Image _iconImage = null;

        public virtual string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                this.OnPropertyChanged("FileName");
                ItemLabel = _fileName;
            }
        }

        public virtual void CreateThumbnail()
        {
        }

        public override object Clone() // ICloneable implementation
        {
            return null;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            MediaPlaylistItem mediaPlaylistItem = (MediaPlaylistItem)obj;

            if (mediaPlaylistItem.FileName != this.FileName) return false;

            return base.IsEqual(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string Description()
        {
            return "Media";
        }

//        public override ImageSource Icon
//        {
//            get { return _iconImage.Source; }
//            set { }
//        }

//        public virtual Preview.PlaybackMediaPlaylistItem GetPlaybackMediaPlaylistItem(Preview.PlaybackZone playbackZone)
//        {
//            return new Preview.PlaybackUnsupportedPlaylistItem(this, playbackZone);
//        }

        public virtual void GetImageFiles(Dictionary<string, object> imagePaths)
        {
        }

        public virtual void GetUsedUserDefinedEvents(Dictionary<string, UserDefinedEvent> usedUserDefinedEvents)
        {
        }

        protected void AddUsedUserDefinedEvent(BSEvent bsEvent, Dictionary<string, UserDefinedEvent> usedUserDefinedEvents)
        {
            if (bsEvent != null && bsEvent is UserDefinedEvent)
            {
                UserDefinedEvent userDefinedEvent = bsEvent as UserDefinedEvent;
                usedUserDefinedEvents[userDefinedEvent.Name] = userDefinedEvent;
            }
        }

        public virtual void GetMissingUserDefinedEvents(Dictionary<string, UserDefinedEvent> missingUserDefinedEvents)
        {
        }

        protected void AddInvalidUserDefinedEvent(BSEvent bsEvent, Dictionary<string, UserDefinedEvent> missingUserDefinedEvents)
        {
            UserDefinedEvent invalidUserDefinedEvent = null;
            if ((invalidUserDefinedEvent = BSEventIsInvalidUserDefinedEvent(bsEvent)) != null)
            {
                missingUserDefinedEvents[invalidUserDefinedEvent.Name] = invalidUserDefinedEvent;
            }
        }

        private UserDefinedEvent BSEventIsInvalidUserDefinedEvent(BSEvent bsEvent)
        {
            if (bsEvent != null)
            {
                if (bsEvent is UserDefinedEvent)
                {
                    UserDefinedEvent userDefinedEvent = bsEvent as UserDefinedEvent;
                    if (!userDefinedEvent.IsValid()) return userDefinedEvent;
                }
            }

            return null;
        }

        public virtual List<UserVariable> GetUserVariablesInUse()
        {
            return new List<UserVariable>();
        }

        public virtual void UpdateUserVariables(UserVariableSet userVariableSet)
        {
        }

        public virtual void ConvertAudioCommands(Sign sign, Zone zone, List<BrightSignCmd> convertedBrightSignCmds)
        {
        }

        public virtual void UpdateUserDefinedEvents(ObservableCollection<UserDefinedEvent> userDefinedEvents)
        {
        }

        protected void UpdateUserDefinedEvent(BSEvent bsEvent, ObservableCollection<UserDefinedEvent> userDefinedEvents)
        {
            if (bsEvent != null && bsEvent is UserDefinedEvent)
            {
                UserDefinedEvent thisUserDefinedEvent = bsEvent as UserDefinedEvent;
                foreach (UserDefinedEvent userDefinedEvent in userDefinedEvents)
                {
                    if (userDefinedEvent.OriginalName != userDefinedEvent.Name && thisUserDefinedEvent.Name == userDefinedEvent.OriginalName)
                    {
                        thisUserDefinedEvent.Name = userDefinedEvent.Name;
                    }
                }
            }
        }

        public virtual SimpleUserEvent CreateDefaultTransitionEvent()
        {
            return null;
        }
    }
}
