using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Diagnostics;

namespace BAModel
{
    public class MediaState : INotifyPropertyChanged
    {
        private string _name = "";
        private MediaPlaylistItem _mediaPlaylistItem = null;
        private ObservableCollection<Transition> _transitionsOut = new ObservableCollection<Transition>();
        private ObservableCollection<Transition> _transitionsIn = new ObservableCollection<Transition>();
        private List<BrightSignCmd> _brightSignEntryCmds = new List<BrightSignCmd>();
        private List<BrightSignCmd> _brightSignExitCmds = new List<BrightSignCmd>();
        private string _notes = String.Empty;

        private StackPanel _stackPanel;
        private Border _mediaStateBorder;
        private Border _idBorder;
        private Line _idLine;
        private TextBlock _idTextBlock;
        private Image _imgHome = null;

        private Image _imgCurrentMediaState = null;

        private System.Windows.Rect _rect = new Rect(0, 0, 0, 0);

        string _fileName = "";

        public int ID { get; set; }

        private double thumbWidth = 116;
        private double thumbHeight = 99;

        private Point _anchorPosition;

        // the following constructor is only used for publishing - it does not deal with graphics
        // HACK HACK HACK - note the swapping of the parameters vs. one of the constructors below
        public MediaState(MediaPlaylistItem mediaPlaylistItem, string name)
        {
            _name = name;
            _mediaPlaylistItem = mediaPlaylistItem;
            FileName = mediaPlaylistItem.FileName;
        }


        public MediaState(string name, MediaPlaylistItem mediaPlaylistItem, Point position, int id)
        {
            _name = name;
            _mediaPlaylistItem = mediaPlaylistItem;
            FileName = mediaPlaylistItem.FileName;
            ID = id;

            CreateMediaStateGraphics(position.X - thumbWidth / 2, position.Y - thumbHeight / 2);
        }

        public MediaState(string name, MediaPlaylistItem mediaPlaylistItem, System.Windows.Rect rect, int id)
        {
            _name = name;
            _mediaPlaylistItem = mediaPlaylistItem;
            FileName = mediaPlaylistItem.FileName;
            ID = id;

            _rect = rect;

            CreateMediaStateGraphics(rect.X, rect.Y);
        }

        // this is really only used for determining if the MediaState has changed when closing the project
        private MediaState(string name, int id, MediaPlaylistItem mediaPlaylistItem, Rect rect)
        {
            _name = name;
            _mediaPlaylistItem = (MediaPlaylistItem)mediaPlaylistItem.Clone();
            _rect = rect;
            ID = id;
        }

        public MediaState CloneForComparison()
        {
            MediaState mediaState = new MediaState(this.Name, this.ID, this.MediaPlaylistItem, this.Rect);
            mediaState.Notes = this.Notes;

            foreach (Transition transition in this.TransitionsOut)
            {
                mediaState.TransitionsOut.Add(transition.CloneForComparison());
            }

            foreach (BrightSignCmd brightSignCmd in this.BrightSignEntryCmds)
            {
                mediaState.BrightSignEntryCmds.Add(brightSignCmd);
            }

            foreach (BrightSignCmd brightSignCmd in this.BrightSignExitCmds)
            {
                mediaState.BrightSignExitCmds.Add(brightSignCmd);
            }

            return mediaState;
        }

        public bool IsEqual(MediaState mediaState)
        {
            if (mediaState.Name != this.Name) return false;
            if (!mediaState.MediaPlaylistItem.IsEqual(this.MediaPlaylistItem)) return false;
            if (mediaState.Notes != this.Notes) return false;

            if (mediaState.TransitionsOut.Count != this.TransitionsOut.Count) return false;
            for (int i = 0; i < mediaState.TransitionsOut.Count; i++)
            {
                if (!this.TransitionsOut[i].IsEqual(mediaState.TransitionsOut[i])) return false;
            }

            List<BrightSignCmd> thisBrightSignEntryCmds = this.BrightSignEntryCmds;
            List<BrightSignCmd> mediaStateBrightSignEntryCmds = mediaState.BrightSignEntryCmds;

            if (thisBrightSignEntryCmds.Count != mediaStateBrightSignEntryCmds.Count) return false;

            int index = 0;
            foreach (BrightSignCmd bsc in thisBrightSignEntryCmds)
            {
                if (!bsc.IsEqual(mediaStateBrightSignEntryCmds[index])) return false;
                index++;
            }

            List<BrightSignCmd> thisBrightSignExitCmds = this.BrightSignExitCmds;
            List<BrightSignCmd> mediaStateBrightSignExitCmds = mediaState.BrightSignExitCmds;

            if (thisBrightSignExitCmds.Count != mediaStateBrightSignExitCmds.Count) return false;

            index = 0;
            foreach (BrightSignCmd bsc in thisBrightSignExitCmds)
            {
                if (!bsc.IsEqual(mediaStateBrightSignExitCmds[index])) return false;
                index++;
            }

            return true;
        }

        public void RebuildForUndo()
        {
            if (MediaPlaylistItem != null)
            {
                MediaPlaylistItem.CreateThumbnail();
                CreateMediaStateGraphics(_rect.X, _rect.Y);
                FileName = _mediaPlaylistItem.FileName;
            }
        }

        public void AddGraphicsToCanvas(Canvas canvas)
        {
            canvas.Children.Add(StackPanel);
            canvas.Children.Add(IDBorder);
            canvas.Children.Add(IDLine);

            UpdateCanvasSize(canvas);
        }

        private void UpdateCanvasSize(Canvas canvas)
        {
            double x = (double)StackPanel.GetValue(Canvas.LeftProperty);
            double y = (double)StackPanel.GetValue(Canvas.TopProperty);

            double minimumX = x + 200;
            if (minimumX > canvas.Width)
            {
                canvas.Width = minimumX;
            }

            double minimumY = y + 200;
            if (minimumY > canvas.Height)
            {
                canvas.Height = minimumY;
            }

        }

        public void RemoveAllGraphics(Canvas canvas)
        {
            if (StackPanel != null)
            {
                canvas.Children.Remove(StackPanel);
            }
            if (IDBorder != null)
            {
                canvas.Children.Remove(IDBorder);
            }
            if (IDLine != null)
            {
                canvas.Children.Remove(IDLine);
            }
        }

        public StackPanel CreateToolTip()
        {
            StackPanel spMediaState = new StackPanel();
            spMediaState.Orientation = Orientation.Vertical;

            // create button
            Button btn = new Button();

            // create border
            Border b = new Border();
            b = new Border();
            b.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xCB, 0xCB, 0xCB));
            b.BorderThickness = new Thickness(1);
            b.CornerRadius = new CornerRadius(2);
            b.MaxWidth = 110;
            b.MinWidth = 110;
            b.MinHeight = 93;
            b.Background = Brushes.White;

            // create image
            Image imgCurrentMediaState = new Image();
            imgCurrentMediaState.Height = 70;
            imgCurrentMediaState.Width = 100;
            imgCurrentMediaState.Stretch = Stretch.Uniform;
            //imgCurrentMediaState.Source = _mediaPlaylistItem.Thumbnail;
            Binding thumbnailBinding = new Binding("Thumbnail");
            thumbnailBinding.Source = _mediaPlaylistItem;
            imgCurrentMediaState.SetBinding(Image.SourceProperty, thumbnailBinding);

            // create label
            TextBlock lblMediaState = new TextBlock();
            lblMediaState.HorizontalAlignment = HorizontalAlignment.Center;
            lblMediaState.FontSize = 10;
            //lblMediaState.Text = _mediaPlaylistItem.FileName;
            Binding nameTextBinding = new Binding("Name");
            nameTextBinding.Source = this;
            lblMediaState.SetBinding(TextBlock.TextProperty, nameTextBinding);

            // create stack panel container
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;

            sp.Children.Add(imgCurrentMediaState);
            sp.Children.Add(lblMediaState);

            b.Child = sp;

            btn.Content = b;

            spMediaState.Children.Add(btn);

            return spMediaState;
        }

        public void SetMediaStateGraphicsBinding()
        {
            Binding thumbnailBinding = new Binding("Thumbnail");
            thumbnailBinding.Source = _mediaPlaylistItem;
            _imgCurrentMediaState.SetBinding(Image.SourceProperty, thumbnailBinding);
        }

        private void CreateMediaStateGraphics(double x, double y)
        {
            ToolTip tt = new ToolTip();
            Binding ttBinding = new Binding("NameAndNotes");
            ttBinding.Source = this;
            tt.SetBinding(ToolTip.ContentProperty, ttBinding);

            _stackPanel = new StackPanel();
            _stackPanel.Orientation = Orientation.Vertical;
            _stackPanel.SetValue(Canvas.LeftProperty, x);
            _stackPanel.SetValue(Canvas.TopProperty, y);

            _stackPanel.ToolTip = tt;

            // create button
            Button btn = new Button();

            // create border
            _mediaStateBorder = new Border();
            _mediaStateBorder.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xCB, 0xCB, 0xCB));
            _mediaStateBorder.BorderThickness = new Thickness(1);
            _mediaStateBorder.CornerRadius = new CornerRadius(2);
            _mediaStateBorder.MaxWidth = 110;
            _mediaStateBorder.MinWidth = 110;
            _mediaStateBorder.MinHeight = 93;
            _mediaStateBorder.Background = Brushes.White;

            // create image
            _imgCurrentMediaState = new Image();
            _imgCurrentMediaState.Height = 70;
            _imgCurrentMediaState.Width = 100;
            _imgCurrentMediaState.Stretch = Stretch.Uniform;
            SetMediaStateGraphicsBinding();

            // create label
            TextBlock lblMediaState = new TextBlock();
            lblMediaState.HorizontalAlignment = HorizontalAlignment.Center;
            lblMediaState.FontSize = 10;
            //lblMediaState.Text = _mediaPlaylistItem.FileName;
            Binding nameTextBinding = new Binding("Name");
            nameTextBinding.Source = this;
            lblMediaState.SetBinding(TextBlock.TextProperty, nameTextBinding);

            // create stack panel container
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;

            sp.Children.Add(_imgCurrentMediaState);
            sp.Children.Add(lblMediaState);

            ContextMenu cm = new ContextMenu();
            cm.Opened += MediaStateContextMenu_Opened;

            MenuItem miCopy = new MenuItem();
            miCopy.Header = "Copy";
            miCopy.Name = "miCopy";
            miCopy.Click += new RoutedEventHandler(miCopy_Click);
            cm.Items.Add(miCopy);

            MenuItem miPaste = new MenuItem();
            miPaste.Header = "Paste";
            miPaste.Name = "miPaste";
            miPaste.Click += new RoutedEventHandler(miPaste_Click);
            cm.Items.Add(miPaste);

            MenuItem mi = new MenuItem();
            mi.Header = BrightAuthorUtils.GetLocalizedString("Edit");
            mi.Name = "miEdit";
            mi.Command = ApplicationCommands.EditPlaylistItem;
            cm.Items.Add(mi);

            if (MediaPlaylistItem != null && MediaPlaylistItem is SuperStatePlaylistItem)
            {
                MenuItem miExpandSuperState = new MenuItem();
                miExpandSuperState.Header = "Expand";
                miExpandSuperState.Name = "miExpand";
                miExpandSuperState.Click += new RoutedEventHandler(miExpandSuperState_Click);
                cm.Items.Add(miExpandSuperState);
            }

            MenuItem miExport = new MenuItem();
            miExport.Header = "Export";
            miExport.Name = "miExport";
            miExport.Click += new RoutedEventHandler(miExport_Click);
            cm.Items.Add(miExport);

            _stackPanel.ContextMenu = cm;

            _mediaStateBorder.Child = sp;

            btn.Content = _mediaStateBorder;

            _stackPanel.Children.Add(btn);

            // label
            _idTextBlock = new TextBlock();
            _idTextBlock.Text = ID.ToString();
            _idTextBlock.Padding = new Thickness(5);
            _idTextBlock.MinHeight = 24;
            _idTextBlock.MinWidth = 24;
            _idTextBlock.TextAlignment = TextAlignment.Center;
            _idTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

            _idBorder = new Border();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 225, 226, 231);
            _idBorder.Background = mySolidColorBrush;
            _idBorder.BorderThickness = new Thickness(2);
            _idBorder.CornerRadius = new CornerRadius(16);
            _idBorder.Child = _idTextBlock;
            _idBorder.SetValue(Canvas.LeftProperty, x - 36);
            _idBorder.SetValue(Canvas.TopProperty, y + 10);
            /*
            ToolTip tt = new ToolTip();
            //tt.Content = Name;
            StackPanel spThumb = CreateMediaStateStackPanel();
            tt.Content = spThumb;
            _idBorder.ToolTip = tt;
            */

            _idLine = new Line();
            _idLine.X1 = x;
            _idLine.Y1 = y + 10 + 16;
            _idLine.X2 = x - 10;
            _idLine.Y2 = y + 10 + 16;
            _idLine.Stroke = Brushes.Black;
            _idLine.StrokeThickness = 1;
        }

        void miCopy_Click(object sender, RoutedEventArgs e)
        {
            Window1 mainWindow = Window1.GetInstance();
            mainWindow.CopySelectedStates();
        }

        void miPaste_Click(object sender, RoutedEventArgs e)
        {
            Window1 mainWindow = Window1.GetInstance();
            mainWindow.ExecutePaste();
        }

        void miExport_Click(object sender, RoutedEventArgs e)
        {
            Window1 mainWindow = Window1.GetInstance();
            mainWindow.ExportSelectedStates();
        }

        void miExpandSuperState_Click(object sender, RoutedEventArgs e)
        {
            Window1 mainWindow = Window1.GetInstance();
            mainWindow.DisplayPlaylist_ExpandSuperState(MediaPlaylistItem);
        }

        public void ShowIDGraphics()
        {
            _idBorder.Visibility = Visibility.Visible;
            _idLine.Visibility = Visibility.Visible;
        }

        public void HideIDGraphics()
        {
            _idBorder.Visibility = Visibility.Hidden;
            _idLine.Visibility = Visibility.Hidden;
        }

        public void MoveGraphics(Canvas canvas, double xNew, double yNew)
        {
            Object o = _stackPanel.GetValue(Canvas.LeftProperty);
            double xCurrent = (double)o;

            o = _stackPanel.GetValue(Canvas.TopProperty);
            double yCurrent = (double)o;

            double xDelta = xNew - xCurrent;
            double yDelta = yNew - yCurrent;

            this.StackPanel.SetValue(Canvas.LeftProperty, xNew);
            this.StackPanel.SetValue(Canvas.TopProperty, yNew);

            _idBorder.SetValue(Canvas.LeftProperty, xNew - 36);
            _idBorder.SetValue(Canvas.TopProperty, yNew + 10);

            _idLine.X1 = xNew;
            _idLine.Y1 = yNew + 10 + 16;
            _idLine.X2 = xNew - 10;
            _idLine.Y2 = yNew + 10 + 16;

            UpdateCanvasSize(canvas);
        }

        public bool InitialState
        {
            get { return _idBorder.Child != _idTextBlock; }
            set
            {
                if (value == true)
                {
                    //_idTextBlock.Visibility = Visibility.Hidden;
                    if (_imgHome == null)
                    {
                        _imgHome = new Image();
                        Image img = (Image)Application.Current.TryFindResource("home");
                        _imgHome.Source = img.Source;
                    }
                    _idBorder.Child = _imgHome;
                }
                else
                {
                    _idBorder.Child = _idTextBlock;
                }
            }
        }

        public StackPanel StackPanel
        {
            get { return _stackPanel; }
        }

        public Border IDBorder
        {
            get { return _idBorder; }
        }

        public Line IDLine
        {
            get { return _idLine; }
        }

        public TextBlock IDTextBlock
        {
            get { return _idTextBlock; }
        }

        public Point AnchorPosition
        {
            get { return _anchorPosition; }
            set { _anchorPosition = value; }
        }

        public void ClearRect()
        {
            _rect.Width = 0;
        }

        public System.Windows.Rect Rect
        {
            get
            {
                if (_rect.Width == 0)
                {
                    Object o = _stackPanel.GetValue(Canvas.LeftProperty);
                    double left = (double)o;

                    o = _stackPanel.GetValue(Canvas.TopProperty);
                    double top = (double)o;

                    System.Windows.Rect rect = new System.Windows.Rect(left, top, _stackPanel.ActualWidth, _stackPanel.ActualHeight);
                    return rect;
                }
                else
                {
                    return _rect;
                }
            }

            // Caution: use setter only in cases where layout is not active (such as initial layout of a converted non-interactive playlist)
            // TODO: layout and graphic info should be in a separate MediaStateInteractiveViewController class
            set
            {
                _rect = value;
                if (_stackPanel != null)
                {
                    _stackPanel.SetValue(Canvas.LeftProperty, _rect.X);
                    _stackPanel.SetValue(Canvas.TopProperty, _rect.Y);
                }
                if (_idBorder != null)
                {
                    _idBorder.SetValue(Canvas.LeftProperty, _rect.X - 36);
                    _idBorder.SetValue(Canvas.TopProperty, _rect.Y + 10);
                }
                if (_idLine != null)
                {
                    _idLine.X1 = _rect.X;
                    _idLine.Y1 = _rect.Y + 10 + 16;
                    _idLine.X2 = _rect.X - 10;
                    _idLine.Y2 = _rect.Y + 10 + 16;
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.OnPropertyChanged("Name");
                this.OnPropertyChanged("NameAndNotes");
            }
        }

        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                this.OnPropertyChanged("Notes");
                this.OnPropertyChanged("NameAndNotes");
            }
        }

        public string NameAndNotes
        {
            get
            {
                string retString = _name;
                if (!String.IsNullOrEmpty(_notes))
                {
                    retString += "\n" + _notes;
                }
                return retString;
            }
        }

        public MediaPlaylistItem MediaPlaylistItem
        {
            get { return _mediaPlaylistItem; }
            set
            {
                _mediaPlaylistItem = value;

                _fileName = _mediaPlaylistItem.FileName;
                this.OnPropertyChanged("FileName");
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                this.OnPropertyChanged("FileName");
            }
        }

        /*
        public string FileName
        {
            get { return _playlistItem.FileName; }
        }
        */

        public void InsertTransitionOut(Transition transition, int index)
        {
            if (index >= _transitionsOut.Count)
            {
                _transitionsOut.Add(transition);
            }
            else
            {
                _transitionsOut.Insert(index, transition);
            }
        }

        public void InsertTransitionIn(Transition transition, int index)
        {
            if (index >= _transitionsIn.Count)
            {
                _transitionsIn.Add(transition);
            }
            else
            {
                _transitionsIn.Insert(index, transition);
            }
        }

        public void AddTransitionOut(Transition transition)
        {
            _transitionsOut.Add(transition);
        }

        public void AddTransitionIn(Transition transition)
        {
            _transitionsIn.Add(transition);
        }

        public ObservableCollection<Transition> TransitionsOut
        {
            get { return _transitionsOut; }
        }

        public ObservableCollection<Transition> TransitionsIn
        {
            get { return _transitionsIn; }
        }

        public List<BrightSignCmd> BrightSignEntryCmds
        {
            get { return _brightSignEntryCmds; }
            set { _brightSignEntryCmds = value; }
        }

        public List<BrightSignCmd> BrightSignExitCmds
        {
            get { return _brightSignExitCmds; }
            set { _brightSignExitCmds = value; }
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

        public void WriteToXml(XmlTextWriter writer, bool publish, Sign sign)
        {
            writer.WriteElementString("name", _name);

            if (!publish)
            {
                System.Windows.Rect rect = Rect;

                writer.WriteElementString("x", Convert.ToInt32(rect.X).ToString());
                writer.WriteElementString("y", Convert.ToInt32(rect.Y).ToString());
                writer.WriteElementString("width", Convert.ToInt32(rect.Width).ToString());
                writer.WriteElementString("height", Convert.ToInt32(rect.Height).ToString());
                writer.WriteElementString("id", ID.ToString());
            }

            _mediaPlaylistItem.WriteToXml(writer, publish, sign, this);

            foreach (BrightSignCmd brightSignCmd in BrightSignEntryCmds)
            {
                brightSignCmd.WriteToXml(writer);
            }

            writer.WriteStartElement("brightSignExitCommands");
            foreach (BrightSignCmd brightSignCmd in BrightSignExitCmds)
            {
                brightSignCmd.WriteToXml(writer);
            }
            writer.WriteFullEndElement(); // brightSignExitCommands

            if (!publish)
            {
                writer.WriteElementString("notes", Notes);
            }
        }

        public static MediaState ReadXml(XmlReader reader, int overrideId)
        {
            BrightSignCmd brightSignCmd = null;

            string name = "";
            double x = 0, y = 0, width = 0, height = 0;
            MediaState mediaState = null;
            int readId = -1;
            string notes = String.Empty;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        name = reader.ReadString();
                        break;
                    case "x":
                        string xStr = reader.ReadString();
                        x = ReadDouble(xStr);
                        break;
                    case "y":
                        string yStr = reader.ReadString();
                        y = ReadDouble(yStr);
                        break;
                    case "width":
                        string widthStr = reader.ReadString();
                        width = ReadDouble(widthStr);
                        break;
                    case "height":
                        string heightStr = reader.ReadString();
                        height = ReadDouble(heightStr);
                        break;
                    case "id":
                        string idStr = reader.ReadString();
                        readId = Convert.ToInt32(idStr);
                        break;
                    case "notes":
                        mediaState.Notes = reader.ReadString();
                        break;
                    case "videoItem":
                    case "imageItem":
                    case "audioItem":
                    case "enhancedAudioItem":
                    case "liveVideoItem":
                    case "rssImageItem":
                    case "mrssDataFeedPlaylistItem":
                    case "signChannelItem":
                    case "mediaListItem":
                    case "audioInItem":
                    case "tripleUSBItem":
                    case "templatePlaylistItem":
                    case "liveTextItem":
                    case "playFileItem":
                    case "eventHandlerItem":
                    case "eventHandler2Item":
                    case "interactiveMenuItem":
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
                        PlaylistItem newPlaylistItem = PlaylistItem.ReadXml(reader, null);
                        MediaPlaylistItem mediaPlaylistItem = (MediaPlaylistItem)newPlaylistItem;
                        int idToUse = overrideId > 0 ? overrideId : readId;
                        mediaState = new MediaState(name, mediaPlaylistItem, new System.Windows.Rect(x, y, width, height), idToUse);
                        break;
                    case "brightSignCommand":
                        BrightSignCommand brightSignCommand = BrightSignCommand.ReadXml(reader);
                        brightSignCmd = BrightSignCmd.FromBrightSignCommand(brightSignCommand);
                        mediaState.BrightSignEntryCmds.Add(brightSignCmd);
                        break;
                    case "brightSignCmd":
                        brightSignCmd = BrightSignCmd.ReadXml(reader);
                        mediaState.BrightSignEntryCmds.Add(brightSignCmd);
                        break;
                    case "brightSignExitCommands":
                        ReadBrightSignExitCmdsXml(reader, mediaState);
                        break;
                }
            }

            return mediaState;
        }

        public static void ReadBrightSignExitCmdsXml(XmlReader reader, MediaState mediaState)
        {
            BrightSignCmd brightSignCmd = null;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "brightSignCmd":
                        brightSignCmd = BrightSignCmd.ReadXml(reader);
                        mediaState.BrightSignExitCmds.Add(brightSignCmd);
                        break;
                }
            }
        }

        // x, y, width, and height values were occasionally written on European systems with commas
        // this code deals with that situation. In the future, these values are properly converted to integers before written
        private static double ReadDouble(string valStr)
        {
            int index = valStr.IndexOf(',');
            if (index >= 0)
            {
                valStr = valStr.Substring(0, index);
            }
            return Convert.ToDouble(valStr);
        }

        public void ShowAsSelected(bool selected)
        {
            if (selected)
            {
                _mediaStateBorder.Background = Brushes.Blue;
            }
            else
            {
                _mediaStateBorder.Background = Brushes.White;
            }
        }

        public void DeleteInternalTransition(Transition transition)
        {
            if (MediaPlaylistItem != null && MediaPlaylistItem is InteractiveMenuPlaylistItem)
            {
                InteractiveMenuPlaylistItem interactiveMenuPlaylistItem = MediaPlaylistItem as InteractiveMenuPlaylistItem;
                interactiveMenuPlaylistItem.RemoveInternalTransition(transition);
            }
        }

        public void GetUsedUserDefinedEvents(Dictionary<string, UserDefinedEvent> usedUserDefinedEvents)
        {
            foreach (Transition transition in TransitionsOut)
            {
                transition.GetUsedUserDefinedEvents(usedUserDefinedEvents);
            }

            if (MediaPlaylistItem != null)
            {
                MediaPlaylistItem.GetUsedUserDefinedEvents(usedUserDefinedEvents);
            }
        }

        public void GetMissingUserDefinedEvents(Dictionary<string, UserDefinedEvent> missingUserDefinedEvents)
        {
            foreach (Transition transition in TransitionsOut)
            {
                transition.GetMissingUserDefinedEvents(missingUserDefinedEvents);
            }

            if (MediaPlaylistItem != null)
            {
                MediaPlaylistItem.GetMissingUserDefinedEvents(missingUserDefinedEvents);
            }

        }

        private void AddInUseUserVariables(Dictionary<string, UserVariableInUse> userVariablesInUse, List<UserVariable> userVariables)
        {
            foreach (UserVariable userVariable in userVariables)
            {
                string userVariableName = userVariable.Name;
                if (userVariablesInUse.ContainsKey(userVariableName))
                {
                    UserVariableInUse userVariableInUse = userVariablesInUse[userVariableName];
                    if (!userVariableInUse.StateNames.Contains(Name))
                    {
                        userVariableInUse.StateNames.Add(Name);
                    }
                }
                else
                {
                    List<string> stateNames = new List<string>();
                    stateNames.Add(Name);

                    UserVariableInUse userVariableInUse = new UserVariableInUse()
                    {
                        UserVariable = userVariable,
                        StateNames = stateNames
                    };

                    userVariablesInUse.Add(userVariableName, userVariableInUse);
                }
            }
        }

        public void GetGPIOSInUse(string[] gpioConfiguration)
        {
            foreach (Transition transition in TransitionsOut)
            {
                if (transition.BSEvent is UserEvent)
                {
                    UserEvent userEvent = transition.BSEvent as UserEvent;
                    if (userEvent.UserEventName == "gpioUserEvent")
                    {
                        SimpleUserEvent simpleUserEvent = (SimpleUserEvent)userEvent;
                        string gpio = simpleUserEvent.Parameter;
                        int gpioIndex = Convert.ToInt32(gpio);
                        gpioConfiguration[gpioIndex] = "input";
                    }
                }

                GetGPIOCommandsInUse(gpioConfiguration, transition.BrightSignCmds);
            }

            GetGPIOCommandsInUse(gpioConfiguration, BrightSignEntryCmds);
            GetGPIOCommandsInUse(gpioConfiguration, BrightSignExitCmds);
        }

        private void GetGPIOCommandsInUse(string[] gpioConfiguration, List<BrightSignCmd> brightSignCmds)
        {
            foreach (BrightSignCmd bsc in brightSignCmds)
            {
                foreach (BSCommand bsCommand in bsc.Commands)
                {
                    if ((bsCommand.Name == "gpioOnCommand") || (bsCommand.Name == "gpioOffCommand"))
                    {
                        Dictionary<string, BSParameter> parameters = bsCommand.Parameters;
                        if (parameters.ContainsKey("gpioNumber"))
                        {
                            BSParameter parameter = parameters["gpioNumber"];
                            string parameterValue;
                            if (parameter.ParameterValue.IsTextOnlyParameter(out parameterValue))
                            {
                                int gpioIndex = Convert.ToInt32(parameterValue);
                                gpioConfiguration[gpioIndex] = "output";
                            }
                        }
                    }

                    if (bsCommand.Name == "gpioSetStateCommand")
                    {
                        if (bsCommand.Parameters.ContainsKey("stateValue"))
                        {
                            BSParameter bsParameter = bsCommand.Parameters["stateValue"];
                            BSParameterValue parameterValue = bsParameter.ParameterValue;
                            string currentValue = parameterValue.GetCurrentValue();
                            int gpioStateValue;
                            bool success = Int32.TryParse(currentValue, out gpioStateValue);

                            for (int i = 0; i < 8; i++)
                            {
                                int bitValue = (int)Math.Pow(2, i);
                                if ((bitValue & gpioStateValue) == bitValue)
                                {
                                    gpioConfiguration[i] = "output";
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetUserVariablesInUse(Dictionary<string, UserVariableInUse> userVariablesInUse)
        {
            if (_mediaPlaylistItem != null)
            {
                List<UserVariable> userVariables = _mediaPlaylistItem.GetUserVariablesInUse();
                AddInUseUserVariables(userVariablesInUse, userVariables);
            }

            foreach (Transition transition in _transitionsOut)
            {
                List<UserVariable> userVariables = transition.GetUserVariablesInUse();
                AddInUseUserVariables(userVariablesInUse, userVariables);
            }

            foreach (BrightSignCmd brightSignCmd in _brightSignEntryCmds)
            {
                List<UserVariable> userVariables = brightSignCmd.GetUserVariablesInUse();
                AddInUseUserVariables(userVariablesInUse, userVariables);
            }

            foreach (BrightSignCmd brightSignCmd in _brightSignExitCmds)
            {
                List<UserVariable> userVariables = brightSignCmd.GetUserVariablesInUse();
                AddInUseUserVariables(userVariablesInUse, userVariables);
            }
        }

        public void UpdateUserDefinedEvents(ObservableCollection<UserDefinedEvent> userDefinedEvents)
        {
            if (_mediaPlaylistItem != null)
            {
                _mediaPlaylistItem.UpdateUserDefinedEvents(userDefinedEvents);
            }

            foreach (Transition transition in _transitionsOut)
            {
                transition.UpdateUserDefinedEvents(userDefinedEvents);
            }
        }

        public void UpdateUserVariables(UserVariableSet userVariableSet)
        {
            if (_mediaPlaylistItem != null)
            {
                _mediaPlaylistItem.UpdateUserVariables(userVariableSet);
            }

            foreach (Transition transition in _transitionsOut)
            {
                transition.UpdateUserVariables(userVariableSet);
            }

            foreach (BrightSignCmd brightSignCmd in _brightSignEntryCmds)
            {
                brightSignCmd.UpdateUserVariables(userVariableSet);
            }

            foreach (BrightSignCmd brightSignCmd in _brightSignExitCmds)
            {
                brightSignCmd.UpdateUserVariables(userVariableSet);
            }
        }

        public void GetSynchronizationUsage(out bool usesSyncCommands, out bool hasSyncEvents)
        {
            usesSyncCommands = false;
            hasSyncEvents = false;

            foreach (BrightSignCmd brightSignCmd in _brightSignEntryCmds)
            {
                if (brightSignCmd.Name == "Synchronize")
                {
                    usesSyncCommands = true;
                    break;
                }
            }

            if (!usesSyncCommands)
            {
                foreach (BrightSignCmd brightSignCmd in _brightSignExitCmds)
                {
                    if (brightSignCmd.Name == "Synchronize")
                    {
                        usesSyncCommands = true;
                        break;
                    }
                }
            }

            foreach (Transition transition in _transitionsOut)
            {
                BSEvent bsEvent = transition.BSEvent;
                if (bsEvent is UserDefinedEvent)
                {
                    List<UserEvent> userEvents = (bsEvent as UserDefinedEvent).UserEvents;
                    foreach (UserEvent userEvent in userEvents)
                    {
                        if (userEvent.UserEventName == "synchronize")
                        {
                            hasSyncEvents = true;
                            break;
                        }
                    }
                }
                else if (bsEvent is UserEvent)
                {
                    if ((bsEvent as UserEvent).UserEventName == "synchronize")
                    {
                        hasSyncEvents = true;
                    }
                }

                foreach (BrightSignCmd brightSignCmd in transition.BrightSignCmds)
                {
                    if (brightSignCmd.Name == "Synchronize")
                    {
                        usesSyncCommands = true;
                        break;
                    }
                }
            }
        }

        private void AddInUsePresentations(Dictionary<string, PresentationInUse> presentationsInUse, List<PresentationIdentifier> presentationIdentifiers)
        {
            foreach (PresentationIdentifier presentationIdentifier in presentationIdentifiers)
            {
                string presentationName = presentationIdentifier.Name;
                if (presentationsInUse.ContainsKey(presentationName))
                {
                    PresentationInUse presentationInUse = presentationsInUse[presentationName];
                    if (!presentationInUse.StateNames.Contains(Name))
                    {
                        presentationInUse.StateNames.Add(Name);
                    }
                }
                else
                {
                    List<string> stateNames = new List<string>();
                    stateNames.Add(Name);

                    PresentationInUse presentationInUse = new PresentationInUse()
                    {
                        Presentation = presentationIdentifier,
                        StateNames = stateNames
                    };

                    presentationsInUse.Add(presentationName, presentationInUse);
                }
            }
        }

        public void GetPresentationsInUse(Dictionary<string, PresentationInUse> presentationsInUse)
        {
            foreach (Transition transition in _transitionsOut)
            {
                List<PresentationIdentifier> presentationIdentifiers = transition.GetPresentationsInUse();
                AddInUsePresentations(presentationsInUse, presentationIdentifiers);
            }
        }

        public void UpdatePresentationIdentifiers(PresentationIdentifierSet presentationIdentifierSet)
        {
            foreach (Transition transition in _transitionsOut)
            {
                transition.UpdatePresentationIdentifiers(presentationIdentifierSet);
            }
        }

        private void AddScriptPluginInUse(string scriptPluginInUse, Dictionary<string, string> scriptPluginsInUse)
        {
            if (scriptPluginInUse != String.Empty)
            {
                if (!scriptPluginsInUse.ContainsKey(scriptPluginInUse))
                {
                    scriptPluginsInUse.Add(scriptPluginInUse, scriptPluginInUse);
                }
            }
        }

        public void GetScriptPluginsInUse(Dictionary<string, string> scriptPluginsInUse)
        {
            foreach (Transition transition in _transitionsOut)
            {
                AddScriptPluginInUse(transition.GetScriptPluginInUse(), scriptPluginsInUse);
            }

            foreach (BrightSignCmd brightSignCmd in _brightSignEntryCmds)
            {
                AddScriptPluginInUse(brightSignCmd.GetScriptPluginsInUse(), scriptPluginsInUse);
            }

            foreach (BrightSignCmd brightSignCmd in _brightSignExitCmds)
            {
                AddScriptPluginInUse(brightSignCmd.GetScriptPluginsInUse(), scriptPluginsInUse);
            }
        }

        public void UpdateScriptPlugins(ScriptPluginSet scriptPluginSet)
        {
            foreach (Transition transition in _transitionsOut)
            {
                transition.UpdateScriptPlugins(scriptPluginSet);
            }

            foreach (BrightSignCmd brightSignCmd in _brightSignEntryCmds)
            {
                brightSignCmd.UpdateScriptPlugins(scriptPluginSet);
            }

            foreach (BrightSignCmd brightSignCmd in _brightSignExitCmds)
            {
                brightSignCmd.UpdateScriptPlugins(scriptPluginSet);
            }
        }

        public void GetLiveDataFeedsInUse(Dictionary<string, string> liveDataFeedsInUse)
        {
            if (_mediaPlaylistItem != null)
            {
                List<string> liveDataFeedNamesInUse = _mediaPlaylistItem.GetLiveDataFeedsInUse();

                foreach (string liveDataFeedNameInUse in liveDataFeedNamesInUse)
                {
                    if (!liveDataFeedsInUse.ContainsKey(liveDataFeedNameInUse))
                    {
                        liveDataFeedsInUse.Add(liveDataFeedNameInUse, liveDataFeedNameInUse);
                    }
                }
            }
        }

        public bool DataFeedDataUsageLegal(LiveDataFeed liveDataFeed, Zone zone, LiveDataFeed.DataFeedUsage dataFeedUsage)
        {
            if (_mediaPlaylistItem != null)
            {
                return _mediaPlaylistItem.DataFeedDataUsageLegal(liveDataFeed, zone, dataFeedUsage);
            }
            return true;
        }

        public void UpdateLiveDataFeeds(LiveDataFeedSet liveDataFeedSet)
        {
            if (_mediaPlaylistItem != null)
            {
                _mediaPlaylistItem.UpdateLiveDataFeeds(liveDataFeedSet);
            }
        }

        public void GetHTMLSitesInUse(Dictionary<string, string> htmlSitesInUse)
        {
            if (_mediaPlaylistItem != null)
            {
                string htmlSiteNameInUse =  _mediaPlaylistItem.GetHTMLSiteInUse();

                if (htmlSiteNameInUse != null)
                {
                    if (!htmlSitesInUse.ContainsKey(htmlSiteNameInUse))
                    {
                        htmlSitesInUse.Add(htmlSiteNameInUse, htmlSiteNameInUse);
                    }
                }
            }
        }

        public void UpdateHTMLSites(HTMLSiteSet htmlSiteSet)
        {
            if (_mediaPlaylistItem != null)
            {
                _mediaPlaylistItem.UpdateHTMLSites(htmlSiteSet);
            }
        }

        public bool GPSInUse()
        {
            foreach (Transition transition in TransitionsOut)
            {
                if (transition.GPSInUse()) return true;
            }

            return false;
        }

        public void ConvertAudioCommands(Sign sign, Zone zone, List<BrightSignCmd> convertedBrightSignCmds)
        {
            if (MediaPlaylistItem != null)
            {
                MediaPlaylistItem.ConvertAudioCommands(sign, zone, convertedBrightSignCmds);
            }

            foreach (BrightSignCmd brightSignCmd in BrightSignEntryCmds)
            {
                brightSignCmd.ConvertAudioCommand(sign, zone, convertedBrightSignCmds);
            }

            foreach (Transition transition in TransitionsOut)
            {
                transition.ConvertAudioCommands(sign, zone, convertedBrightSignCmds);
            }
        }

        public bool IsAudioItem()
        {
            if (MediaPlaylistItem != null)
            {
                if (MediaPlaylistItem is AudioPlaylistItem)
                {
                    return true;
                }

                if ((MediaPlaylistItem is MediaListPlaylistItem) && ((MediaPlaylistItem as MediaListPlaylistItem).MediaType == "audio"))
                {
                    return true;
                }

                if ((MediaPlaylistItem is PlayFilePlaylistItem) && ((MediaPlaylistItem as PlayFilePlaylistItem).MediaType == "audio"))
                {
                    return true;
                }
            }

            return false;
        }

        private void MediaStateContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            try
            {
                Window1 mainWindow = Window1.GetInstance();
                mainWindow.SelectMediaStateViaRightClick(this);

                int numSelectedStates = mainWindow.GetNumberOfSelectedInteractiveStates();

                ContextMenu cm = (ContextMenu)sender;

                foreach (MenuItem mi in cm.Items)
                {
                    if ((mi.Name == "miExport") || (mi.Name == "miCopy"))
                    {
                        mi.IsEnabled = numSelectedStates > 0;
                    }
                    if (mi.Name == "miPaste")
                    {
                        mi.IsEnabled = mainWindow.EnablePaste();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception in MediaStateContextMenu_Opened");
                App.myTraceListener.Assert(false, ex.ToString());
            }
        }
    }
}
