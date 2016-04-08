using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
//using System.Windows.Media;

namespace BAModel
{
    abstract public class Playlist : INotifyPropertyChanged
    {
        protected string _name;
        protected ObservableCollection<PlaylistItem> _items = new ObservableCollection<PlaylistItem>();

        public Playlist(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.OnPropertyChanged("Name");
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        virtual public ObservableCollection<PlaylistItem> Items
        {
            get { return _items; }
            set { }
        }

        virtual public bool UsesAudio
        {
            get
            {
                foreach (PlaylistItem item in Items)
                {
                    if (item is AudioPlaylistItem) return true;
                }
                return false;
            }
        }

//        virtual public ImageSource Thumbnail
//        {
//            get
//            {
//                if (Items.Count == 0) return null;
//                PlaylistItem playlistItem = Items[0];
//                return playlistItem.Thumbnail;
//            }
//        }

        abstract public bool IsEqual(Object obj);

        virtual public bool IsEmpty()
        {
            return Items.Count == 0;
        }

        virtual public void Clear()
        {
            Items.Clear();
        }

        virtual public int IndexOfPlaylistItem(PlaylistItem item)
        {
            return Items.IndexOf(item);
        }

        virtual public void AddPlaylistItem(PlaylistItem item)
        {
            Items.Add(item);
        }

        virtual public void AddPlaylistItems(Collection<PlaylistItem> itemsToAdd)
        {
            foreach (PlaylistItem item in itemsToAdd)
            {
                AddPlaylistItem(item);
            }
        }

        virtual public void InsertPlaylistItem(PlaylistItem item, int insertionIndex)
        {
            // Insert or append the item.
            if ((insertionIndex != -1) && (insertionIndex < Items.Count))
            {
                Items.Insert(insertionIndex, item);
            }
            else
            {
                Items.Add(item);
            }
        }

        virtual public void InsertPlaylistItems(Collection<PlaylistItem> itemsToInsert, int insertionIndex)
        {
            foreach (PlaylistItem item in itemsToInsert)
            {
                InsertPlaylistItem(item, insertionIndex);
                insertionIndex++;
            }
        }

        virtual public void RemovePlaylistItem(PlaylistItem item)
        {
            Items.Remove(item);
        }

        virtual public void RemovePlaylistItems(Collection<PlaylistItem> itemsToRemove)
        {
            foreach (PlaylistItem playlistItem in itemsToRemove)
            {
                RemovePlaylistItem(playlistItem);
            }
        }

        virtual public void ReplaceMediaFiles(Dictionary<string, string> replacementFiles, bool preserveStateNames)
        {
            foreach (PlaylistItem playlistItem in Items)
            {
                playlistItem.ReplaceMediaFiles(replacementFiles, preserveStateNames);
            }
        }

        virtual public void FindBrokenMediaLinks(List<Object> brokenLinks)
        {
            foreach (PlaylistItem playlistItem in _items)
            {
                playlistItem.FindBrokenMediaLinks(brokenLinks);
            }
        }

        virtual public void FindDuplicateMediaFiles(Dictionary<string, string> fileSpecs, DuplicateFileList duplicateFileList)
        {
            foreach (PlaylistItem playlistItem in Items)
            {
                playlistItem.FindDuplicateMediaFiles(fileSpecs, duplicateFileList);
            }
        }

        virtual public void RegenerateThumbs()
        {
            foreach (PlaylistItem item in Items)
            {
                if (item is FilePlaylistItem)
                {
                    (item as FilePlaylistItem).RegenerateThumb();
                }
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
    }
}
