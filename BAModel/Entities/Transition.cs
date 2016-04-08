using System;
using System.Collections.Generic;
using System.Xml;
//using System.Windows.Media;
using System.ComponentModel;
//using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;

namespace BAModel
{
    public class Transition : INotifyPropertyChanged
    {
        private MediaState _sourceMediaState = null;
        private BSEvent _bsEvent = null;
        private MediaState _targetMediaState = null;
        private bool _targetMediaStateIsPreviousState = false;
        private List<BrightSignCmd> _brightSignCmds = new List<BrightSignCmd>();
        private List<ConditionalTarget> _conditionalTargets = new List<ConditionalTarget>();
        private bool _assignInputToUserVariable = false;
        private string _variableToAssign = String.Empty;
        private bool _assignWildcardToUserVariable = false;
        private string _variableToAssignFromWildcard = String.Empty;

//        private Image _image = null;
//        private ArrowLine _line = null;
//        private StackPanel _label = null;

//        private Point _imageAnchor, _labelAnchor;
//        private Point _lineStartPointAnchor, _lineEndPointAnchor;

        private string _sourceMediaStateName = ""; // used only for reading xml
        private string _targetMediaStateName = ""; // used only for reading xml

        public string DisplayMode { get; set; }
        public string LabelLocation { get; set; }

        private double miniThumbWidth = 24;
        private double heightBetweenMediaStateAndMiniThumb = 45;
        private double widthBetweenMediaStateAndMiniThumb = 60;
        private double miniThumbHeight = 24;
        //private double widthOfEventLabel = 45;
        //private double heightOfEventLabel = 21;

        private double iconWidth = 24;

//        private Image _imgIcon = null;
//        private Image _imgIconSelected = null;

//        private Border _mediaStateID;
//        private TextBlock _mediaStateIDText;
//        private Image _imgHome = null;

        public Transition()
        {
            _bsEvent = new BSEvent();
            TargetMediaState = null;

            _imgIcon = new Image();
            _imgIcon.Width = 24;
            _imgIcon.Height = 24;

            _imgIconSelected = new Image();
            _imgIconSelected.Width = 24;
            _imgIconSelected.Height = 24;

            _image = new Image();
            _image.Width = 24;
            _image.Height = 24;
        }

        // used for publishing only - no graphics are created
        public Transition(MediaState sourceMediaState, BSEvent bsEvent, MediaState targetMediaState,
            List<BrightSignCmd> brightSignCmds)
        {
            _sourceMediaState = sourceMediaState;
            _bsEvent = bsEvent;
            _targetMediaState = targetMediaState;
            if (brightSignCmds != null) BrightSignCmds = brightSignCmds;
            _targetMediaStateIsPreviousState = false;
        }

        public Transition(MediaState sourceMediaState, BSEvent bsEvent, MediaState targetMediaState,
            List<BrightSignCmd> brightSignCmds, string displayMode, string labelLocation)
        {
            _sourceMediaState = sourceMediaState;
            _bsEvent = bsEvent;
            _targetMediaState = targetMediaState;
            if (brightSignCmds != null) BrightSignCmds = brightSignCmds;
            _targetMediaStateIsPreviousState = false;
            DisplayMode = displayMode;
            LabelLocation = labelLocation;

            _imgIcon = new Image();
            _imgIcon.Width = 24;
            _imgIcon.Height = 24;

            _imgIconSelected = new Image();
            _imgIconSelected.Width = 24;
            _imgIconSelected.Height = 24;

            _image = new Image();
            _image.Width = 24;
            _image.Height = 24;
        }

        private void SetImage()
        {
            string imageResourceName = "";
            string imageResourceSelectedName = "";
            _bsEvent.GetIconResourceNames(ref imageResourceName, ref imageResourceSelectedName);
            
            Image img = (Image)Application.Current.TryFindResource(imageResourceName);
            Image imgSelected = (Image)Application.Current.TryFindResource(imageResourceSelectedName);

            if (img != null)
            {
                _imgIcon.Source = img.Source;
                _imgIconSelected.Source = imgSelected.Source;
            }
        }

        private void CompleteClone(Transition newTransition, Transition existingTransition)
        {
            newTransition.TargetMediaStateIsPreviousState = existingTransition.TargetMediaStateIsPreviousState;
            newTransition.DisplayMode = existingTransition.DisplayMode;
            newTransition.LabelLocation = existingTransition.LabelLocation;

            foreach (BrightSignCmd brightSignCmd in existingTransition.BrightSignCmds)
            {
                newTransition.BrightSignCmds.Add(brightSignCmd.Clone());
            }

            foreach (ConditionalTarget conditionalTarget in existingTransition.ConditionalTargets)
            {
                newTransition.ConditionalTargets.Add(conditionalTarget.Clone());
            }

            newTransition.AssignInputToVariable = existingTransition.AssignInputToVariable;
            newTransition.VariableToAssign = existingTransition.VariableToAssign;

            newTransition.AssignWildcardToVariable = existingTransition.AssignWildcardToVariable;
            newTransition.VariableToAssignFromWildcard = existingTransition.VariableToAssignFromWildcard;

            newTransition.BSEvent = (BSEvent)existingTransition.BSEvent.Clone();
        }

        public Transition CloneSetSourceAndTarget()
        {
            Transition transition = new Transition();

            transition.SourceMediaState = this.SourceMediaState;
            transition.TargetMediaState = this.TargetMediaState;

            CompleteClone(transition, this);

            return transition;
        }

        public Transition CloneForComparison()
        {
            Transition transition = new Transition();

            // at this point, the source and target media states can't be set since they might not have been cloned yet.
            // a matching operation is done after all the media states and transitions are created.
            transition._sourceMediaStateName = this._sourceMediaState.Name;
            if (this._targetMediaState != null)
            {
                transition._targetMediaStateName = this._targetMediaState.Name;
            }
            else
            {
                transition._targetMediaStateName = "";
            }

            CompleteClone(transition, this);

            return transition;
        }

        public bool IsEqual(Transition transition)
        {
//            if (!transition.SourceMediaState.IsEqual(this.SourceMediaState)) return false;
            if (transition.SourceMediaState.Name != this.SourceMediaState.Name) return false;
            if (!transition.BSEvent.IsEqual(this.BSEvent)) return false;
//            if (!transition.TargetMediaState.IsEqual(this.TargetMediaState)) return false;
            if (transition.TargetMediaState != null && this.TargetMediaState != null)
            {
                if (transition.TargetMediaState.Name != this.TargetMediaState.Name) return false;
            }
            else if (transition.TargetMediaState != null || this.TargetMediaState != null)
            {
                return false;
            }
            if (transition.TargetMediaStateIsPreviousState != this.TargetMediaStateIsPreviousState) return false;
            if (transition.DisplayMode != this.DisplayMode) return false;
            if (transition.LabelLocation != this.LabelLocation) return false;

            List<BrightSignCmd> thisBrightSignCmds = this.BrightSignCmds;
            List<BrightSignCmd> transitionBrightSignCmds = transition.BrightSignCmds;

            if (thisBrightSignCmds.Count != transitionBrightSignCmds.Count) return false;

            int index = 0;
            foreach (BrightSignCmd bsc in thisBrightSignCmds)
            {
                if (!bsc.IsEqual(transitionBrightSignCmds[index])) return false;
                index++;
            }

            List<ConditionalTarget> thisConditionalTargets = this.ConditionalTargets;
            List<ConditionalTarget> transitionConditionalTargets = transition.ConditionalTargets;

            if (thisConditionalTargets.Count != transitionConditionalTargets.Count) return false;

            index = 0;
            foreach (ConditionalTarget conditionalTarget in thisConditionalTargets)
            {
                if (!conditionalTarget.IsEqual(transitionConditionalTargets[index++])) return false;
            }

            if (this.AssignInputToVariable != transition.AssignInputToVariable) return false;
            if (this.AssignInputToVariable)
            {
                if (this.VariableToAssign != transition.VariableToAssign) return false;
            }

            if (this.AssignWildcardToVariable != transition.AssignWildcardToVariable) return false;
            if (this.AssignWildcardToVariable)
            {
                if (this.VariableToAssignFromWildcard != transition.VariableToAssignFromWildcard) return false;
            }

            return true;
        }

        public void ShowAsSelected(bool selected)
        {
            if (DisplayMode == "displayLabel")
            {
                if (selected)
                {
                    // set image background
                    _line.Stroke = Brushes.Blue;
                    _line.Fill = Brushes.Blue;
                    _image.Source = _imgIconSelected.Source;
                }
                else
                {
                    // set image background
                    _line.Stroke = Brushes.Black;
                    _line.Fill = Brushes.Black;
                    _image.Source = _imgIcon.Source;
                }
            }
            if (DisplayMode == "displayConnectingLine")
            {
                if (selected)
                {
                    // set image background
                    _line.Stroke = Brushes.Blue;
                    _line.Fill = Brushes.Blue;
                    _image.Source = _imgIconSelected.Source;
                }
                else
                {
                    // set image background
                    _line.Stroke = Brushes.Black;
                    _line.Fill = Brushes.Black;
                    _image.Source = _imgIcon.Source;
                }
            }
        }

//        public void CreateGraphics(Canvas canvas)
//        {
//            StackPanel sp = null;
//
//            MediaState sourceMediaState = SourceMediaState;
//            MediaState targetMediaState = TargetMediaState;
//
//            System.Windows.Rect sourceRect = sourceMediaState.Rect;
//
//            double xSourceCenter, xSourceRight, ySourceTop, ySourceBottom, ySourceMiddle;
//            xSourceCenter = sourceRect.X + (sourceRect.Width / 2);
//            xSourceRight = sourceRect.X + sourceRect.Width;
//            ySourceTop = sourceRect.Y;
//            ySourceBottom = ySourceTop + sourceRect.Height;
//            ySourceMiddle = (ySourceTop + ySourceBottom) / 2;
//
//            if (DisplayMode == "displayLabel")
//            {
//                if (LabelLocation == "right")
//                {
//                    sp = CreateLabel(xSourceRight + widthBetweenMediaStateAndMiniThumb, ySourceMiddle, targetMediaState);
//                    _line = CreateLine(xSourceRight, xSourceRight + widthBetweenMediaStateAndMiniThumb, ySourceMiddle, ySourceMiddle);
//                    CreateEventImage(xSourceRight + (widthBetweenMediaStateAndMiniThumb / 2), ySourceMiddle);
//                }
//                else if (LabelLocation == "bottom")
//                {
//                    sp = CreateLabel(xSourceCenter, ySourceBottom + heightBetweenMediaStateAndMiniThumb, targetMediaState);
//                    _line = CreateLine(xSourceCenter, xSourceCenter, ySourceBottom, ySourceBottom + heightBetweenMediaStateAndMiniThumb);
//                    CreateEventImage(xSourceCenter, (_line.Y1 + _line.Y2) / 2);
//                }
//
//                _label = sp;
//
//                if (_label != null) canvas.Children.Add(_label);
//                canvas.Children.Add(_line);
//                try
//                {
//                    canvas.Children.Add(_image);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine("DisplayMode == displayLabel, exception is " + ex.Message);
//                }
//            }
//            else
//            {
//                if (TargetMediaState != null)
//                {
//                    System.Windows.Rect destinationRect = targetMediaState.Rect;
//                    double xDestinationCenter, yDestinationTop, yDestinationBottom, yDestinationMiddle;
//                    xDestinationCenter = destinationRect.X + (destinationRect.Width / 2);
//                    yDestinationTop = destinationRect.Y;
//                    yDestinationBottom = yDestinationTop + destinationRect.Height;
//                    yDestinationMiddle = (yDestinationTop + yDestinationBottom) / 2;
//
//                    _line = CreateLine(xSourceCenter, xDestinationCenter, ySourceBottom, yDestinationTop);
//                    canvas.Children.Add(_line);
//
//                    CreateEventImage((xSourceCenter + xDestinationCenter) / 2, (_line.Y1 + _line.Y2) / 2);
//                    try
//                    {
//                        canvas.Children.Add(_image);
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine("DisplayMode != displayLabel, exception is " + ex.Message);
//                    }
//                }
//            }
//        }

//        private StackPanel CreateLabel(double x, double y, MediaState mediaState)
//        {
//            if (mediaState != null)
//            {
//                StackPanel stackPanel = new StackPanel();
//                stackPanel.Orientation = Orientation.Vertical;
//                stackPanel.SetValue(Canvas.LeftProperty, x);
//                stackPanel.SetValue(Canvas.TopProperty, y);
//
//                _mediaStateIDText = new TextBlock();
//                _mediaStateIDText.Text = mediaState.ID.ToString();
//                _mediaStateIDText.Padding = new Thickness(5);
//                _mediaStateIDText.MinHeight = 24;
//                _mediaStateIDText.MinWidth = 24;
//                _mediaStateIDText.TextAlignment = TextAlignment.Center;
//                _mediaStateIDText.HorizontalAlignment = HorizontalAlignment.Center;
//
//                _mediaStateID = new Border();
//                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
//                mySolidColorBrush.Color = Color.FromArgb(255, 225, 226, 231);
//                _mediaStateID.Background = mySolidColorBrush;
//                _mediaStateID.BorderThickness = new Thickness(2);
//                _mediaStateID.CornerRadius = new CornerRadius(16);
//                _mediaStateID.Child = _mediaStateIDText;
//
//                stackPanel.Children.Add(_mediaStateID);
//
//                ToolTip tt = new ToolTip();
//                StackPanel spThumb = mediaState.CreateToolTip();
//                tt.Content = spThumb;
//                stackPanel.ToolTip = tt;
//
//                return stackPanel;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        public void RemoveGraphics(Canvas canvas)
//        {
//            if (_mediaStateID != null)
//            {
//                _mediaStateID.Child = null;
//            }
//
//            if (_imgIcon != null)
//            {
//                canvas.Children.Remove(_imgIcon);
//            }
//
//            canvas.Children.Remove(_label);
//            canvas.Children.Remove(_image);
//            canvas.Children.Remove(_line);
//
//        }

        public void MoveGraphicsVertically(double newY)
        {
            if (_label != null) _label.SetValue(Canvas.TopProperty, newY);
            _line.Y2 = newY + miniThumbHeight / 2;
            double y = CorrectYPosition((_line.Y1 + _line.Y2) / 2);
            _image.SetValue(Canvas.TopProperty, y);
        }

        public void MoveGraphicsHorizontally(double newX)
        {
            if (_label != null) _label.SetValue(Canvas.LeftProperty, newX);
            _line.X2 = newX + miniThumbWidth / 2;
            double x = CorrectXPosition((_line.X1 + _line.X2) / 2);
            _image.SetValue(Canvas.LeftProperty, x);
        }

        public void SetConnectingLineSourceX(double newX)
        {
            _line.X1 = newX;
            double x = CorrectXPosition((_line.X1 + _line.X2) / 2);
            _image.SetValue(Canvas.LeftProperty, x);
        }

        public void SetConnectingLineDestinationX(double newX)
        {
            _line.X2 = newX;
            double x = CorrectXPosition((_line.X1 + _line.X2) / 2);
            _image.SetValue(Canvas.LeftProperty, x);
        }

        public bool InitialState
        {
            set
            {
                if (_mediaStateID != null)
                {
                    if (value == true)
                    {
                        if (_imgHome == null)
                        {
                            _imgHome = new Image();
                            Image img = (Image)Application.Current.TryFindResource("home");
                            _imgHome.Source = img.Source;
                        }
                        _mediaStateID.Child = _imgHome;
                    }
                    else
                    {
                        _mediaStateID.Child = _mediaStateIDText;
                        //_mediaStateID.Child = _mediaStateWindow;
                    }
                }
            }
        }

//        private ArrowLine CreateLine(double x1, double x2, double y1, double y2)
//        {
//            ArrowLine line = new ArrowLine();
//            line.X1 = x1;
//            line.X2 = x2;
//            line.Y1 = y1;
//            line.Y2 = y2;
//            line.Stroke = Brushes.Black;
//            line.StrokeThickness = 1;
//            line.IsArrowClosed = true;
//            line.Fill = Brushes.Black;
//            line.ArrowLength = 8;
//            return line;
//        }

        private double CorrectXPosition(double x)
        {
            return x -= iconWidth / 2;
        }

        private double CorrectYPosition(double y)
        {
            return y -= iconWidth / 2;
        }

        private void CreateEventImage(double x, double y)
        {
            SetImage();
            _image.Source = _imgIcon.Source;

            x = CorrectXPosition(x);
            _image.SetValue(Canvas.LeftProperty, x);
            y = CorrectYPosition(y);
            _image.SetValue(Canvas.TopProperty, y);

            _image.ToolTip = _bsEvent.GetToolTip();

            ContextMenu cm = new ContextMenu();
            MenuItem mi = new MenuItem();
            mi.Header = BrightAuthorUtils.GetLocalizedString("Edit");
            mi.Name = "miEdit";
            mi.Command = ApplicationCommands.EditPlaylistItem;
            cm.Items.Add(mi);
            _image.ContextMenu = cm;
        }

//        private Point GetCurrentPosition(DependencyObject depObj)
//        {
//            Object o = null;
//
//            o = depObj.GetValue(Canvas.LeftProperty);
//            double left = (double)o;
//
//            o = depObj.GetValue(Canvas.TopProperty);
//            double top = (double)o;
//
//            return new Point(left, top);
//        }

        public void SetSourceAnchors()
        {
            Point p;

            if (DisplayMode == "displayLabel")
            {
                p = GetCurrentPosition(_image);
                _imageAnchor = p;

                _lineStartPointAnchor = new Point(_line.X1, _line.Y1);
                _lineEndPointAnchor = new Point(_line.X2, _line.Y2);

                if (_label != null)
                {
                    p = GetCurrentPosition(_label);
                    _labelAnchor = p;
                }
            }
            if (DisplayMode == "displayConnectingLine")
            {
                _lineStartPointAnchor = new Point(_line.X1, _line.Y1);
            }
        }

        public void SetDestinationAnchors()
        {
            if (_line != null)
            {
                _lineEndPointAnchor = new Point(_line.X2, _line.Y2);
            }
        }

//        private void UpdatePosition(DependencyObject depObj, Point anchorPosition, double deltaX, double deltaY)
//        {
//            depObj.SetValue(Canvas.LeftProperty, anchorPosition.X + deltaX);
//            depObj.SetValue(Canvas.TopProperty, anchorPosition.Y + deltaY);
//        }

        public void UpdateSourcePositions(double deltaX, double deltaY)
        {
            if (DisplayMode == "displayLabel")
            {
                UpdatePosition(_image, _imageAnchor, deltaX, deltaY);
                if (_label != null) UpdatePosition(_label, _labelAnchor, deltaX, deltaY);

                _line.X1 = _lineStartPointAnchor.X + deltaX;
                _line.Y1 = _lineStartPointAnchor.Y + deltaY;
                _line.X2 = _lineEndPointAnchor.X + deltaX;
                _line.Y2 = _lineEndPointAnchor.Y + deltaY;
            }
        }

        public void UpdateConnectingLineOut(double deltaX, double deltaY)
        {
            if (DisplayMode == "displayConnectingLine")
            {
                _line.X1 = _lineStartPointAnchor.X + deltaX;
                _line.Y1 = _lineStartPointAnchor.Y + deltaY;

                double x = CorrectXPosition((_line.X1 + _line.X2) / 2);
                _image.SetValue(Canvas.LeftProperty, x);
                double y = CorrectYPosition((_line.Y1 + _line.Y2) / 2);
                _image.SetValue(Canvas.TopProperty, y);
            }
        }

        public void UpdateConnectingLineIn(double deltaX, double deltaY)
        {
            if (DisplayMode == "displayConnectingLine")
            {
                _line.X2 = _lineEndPointAnchor.X + deltaX;
                _line.Y2 = _lineEndPointAnchor.Y + deltaY;

                double x = CorrectXPosition((_line.X1 + _line.X2) / 2);
                _image.SetValue(Canvas.LeftProperty, x);
                double y = CorrectYPosition((_line.Y1 + _line.Y2) / 2);
                _image.SetValue(Canvas.TopProperty, y);
            }
        }

        public string SourceMediaStateName
        {
            get { return _sourceMediaStateName; }
            set { _sourceMediaStateName = value; }
        }

        public string TargetMediaStateName
        {
            get { return _targetMediaStateName; }
            set { _targetMediaStateName = value; }
        }

        public BSEvent BSEvent
        {
            get { return _bsEvent; }
            set { _bsEvent = value; }
        }

        public MediaState SourceMediaState
        {
            get { return _sourceMediaState; }
            set
            {
                _sourceMediaState = value;
            }
        }

        public MediaState TargetMediaState
        {
            get { return _targetMediaState; }
            set
            {
                _targetMediaState = value;
            }
        }

        public bool TargetMediaStateIsPreviousState
        {
            get { return _targetMediaStateIsPreviousState; }
            set
            {
                _targetMediaStateIsPreviousState = value;
            }
        }

        public List<BrightSignCmd> BrightSignCmds
        {
            get { return _brightSignCmds; }
            set { _brightSignCmds = value; }
        }

        public List<ConditionalTarget> ConditionalTargets
        {
            get { return _conditionalTargets; }
            set { _conditionalTargets = value; }
        }

        public bool AssignInputToVariable
        {
            get { return _assignInputToUserVariable; }
            set { _assignInputToUserVariable = value; }
        }

        public string VariableToAssign
        {
            get { return _variableToAssign; }
            set { _variableToAssign = value; }
        }

        public bool AssignWildcardToVariable
        {
            get { return _assignWildcardToUserVariable; }
            set { _assignWildcardToUserVariable = value; }
        }

        public string VariableToAssignFromWildcard
        {
            get { return _variableToAssignFromWildcard; }
            set { _variableToAssignFromWildcard = value; }
        }

//        private System.Windows.Rect GetRectFromBorder(Image img)
//        {
//            Object o = img.GetValue(Canvas.LeftProperty);
//            double left = (double)o;
//
//            o = img.GetValue(Canvas.TopProperty);
//            double top = (double)o;
//
//            System.Windows.Rect rect = new System.Windows.Rect(left, top, img.ActualWidth, img.ActualHeight);
//
//            return rect;
//        }

//        public bool IsSelected(Point position)
//        {
//            if (DisplayMode == "displayConnectingLine")
//            {
//                if (_image != null)
//                {
//                    System.Windows.Rect rect = GetRectFromBorder(_image);
//                    if (rect.Contains(position)) return true;
//                }
//            }
//
//            else
//            {
//                if (DisplayMode == "displayLabel")
//                {
//                    if (_image != null)
//                    {
//                        System.Windows.Rect rect = GetRectFromBorder(_image);
//                        if (rect.Contains(position)) return true;
//                    }
//                }
//
//            }
//            
//            return false;
//        }

        public void WriteToXml(XmlTextWriter writer, bool publish)
        {
            if (publish && ((_bsEvent is VideoTimeCodeUserEvent) || (_bsEvent is AudioTimeCodeUserEvent)))
            {
                List<TimedBrightSignCmd> timedBrightSignCmds = null;
                if (_bsEvent is VideoTimeCodeUserEvent)
                {
                    timedBrightSignCmds = (_bsEvent as VideoTimeCodeUserEvent).TimedBrightSignCmds;
                }
                else
                {
                    timedBrightSignCmds = (_bsEvent as AudioTimeCodeUserEvent).TimedBrightSignCmds;
                }
                foreach (TimedBrightSignCmd timedBrightSignCmd in timedBrightSignCmds)
                {
                    writer.WriteStartElement("transition");

                    writer.WriteElementString("sourceMediaState", _sourceMediaState.Name);
                    writer.WriteElementString("targetMediaState", "");

                    // write out video time code item
                    BSEvent.WriteBaseInfoToXml(writer, publish);

                    writer.WriteStartElement("parameters");
                    writer.WriteElementString("parameter", timedBrightSignCmd.Timeout);
                    writer.WriteEndElement(); // parameters

                    // close out element that was written in base class - yuck!
                    writer.WriteEndElement();

                    // write out command associated with this video time code item
                    timedBrightSignCmd.PublishToXml(writer);

                    writer.WriteEndElement(); // transition
                }
            }
            else if (publish && (_bsEvent is UserDefinedEvent))
            {
                UserDefinedEvent userDefinedEvent = _bsEvent as UserDefinedEvent;
                foreach (UserEvent userEvent in userDefinedEvent.UserEvents)
                {
                    WriteTransitionToXml(writer, true, userEvent);
                }
            }
            else
            {
                WriteTransitionToXml(writer, publish, _bsEvent);
            }
        }

        private void WriteTransitionToXml(XmlTextWriter writer, bool publish, BSEvent bsEvent)
        {
            writer.WriteStartElement("transition");

            writer.WriteElementString("sourceMediaState", _sourceMediaState.Name);
            bsEvent.WriteToXml(writer, publish);
            if (TargetMediaState != null)
            {
                writer.WriteElementString("targetMediaState", _targetMediaState.Name);
            }
            else
            {
                writer.WriteElementString("targetMediaState", "");
            }
            if (TargetMediaStateIsPreviousState)
            {
                writer.WriteElementString("targetIsPreviousState", "yes");
            }

            if (!publish)
            {
                writer.WriteElementString("displayMode", DisplayMode);
                writer.WriteElementString("labelLocation", LabelLocation);
            }

            foreach (BrightSignCmd brightSignCmd in BrightSignCmds)
            {
                brightSignCmd.WriteToXml(writer);
            }

            foreach (ConditionalTarget conditionalTarget in ConditionalTargets)
            {
                conditionalTarget.WriteToXml(writer, publish);
            }

            writer.WriteElementString("assignInputToUserVariable", _assignInputToUserVariable.ToString());
            if (_assignInputToUserVariable)
            {
                writer.WriteElementString("variableToAssign", _variableToAssign);
            }

            writer.WriteElementString("assignWildcardToUserVariable", _assignWildcardToUserVariable.ToString());
            if (_assignWildcardToUserVariable)
            {
                writer.WriteElementString("variableToAssignFromWildcard", _variableToAssignFromWildcard);
            }

            writer.WriteEndElement(); // transition
        }

        public void Publish(List<PublishFile> publishFiles)
        {
            if (_bsEvent != null)
            {
                _bsEvent.Publish(publishFiles);
            }
        }

        public void ReplaceMediaFiles(Dictionary<string, string> replacementFiles)
        {
            if (_bsEvent != null)
            {
                _bsEvent.ReplaceMediaFiles(replacementFiles);
            }
        }

        public void FindBrokenMediaLinks(List<Object> brokenLinks)
        {
            if (_bsEvent != null)
            {
                _bsEvent.FindBrokenMediaLinks(brokenLinks);
            }
        }

        public static Transition ReadXml(XmlReader reader)
        {
            BrightSignCmd brightSignCmd = null;
            ConditionalTarget conditionalTarget = null;

            Transition transition = new Transition();
           
            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "sourceMediaState":
                        transition._sourceMediaStateName = reader.ReadString();
                        break;
                    case "userEvent":
                        transition.BSEvent = UserEvent.ReadXml(reader);
                        break;
                    case "userDefinedEvent":
                        transition.BSEvent = UserDefinedEvent.MatchUserDefinedEvent(reader);
                        break;
                    case "targetMediaState":
                        transition._targetMediaStateName = reader.ReadString();
                        break;
                    case "targetIsPreviousState":
                        string val = reader.ReadString();
                        if (val.ToLower() == "yes")
                        {
                            transition._targetMediaStateIsPreviousState = true;
                        }
                        break;
                    case "displayMode":
                        transition.DisplayMode = reader.ReadString();
                        break;
                    case "labelLocation":
                        transition.LabelLocation = reader.ReadString();
                        break;
                    case "brightSignCommand":
                        BrightSignCommand brightSignCommand = BrightSignCommand.ReadXml(reader);
                        brightSignCmd = BrightSignCmd.FromBrightSignCommand(brightSignCommand);
                        transition.BrightSignCmds.Add(brightSignCmd);
                        break;
                    case "brightSignCmd":
                        brightSignCmd = BrightSignCmd.ReadXml(reader);
                        transition.BrightSignCmds.Add(brightSignCmd);
                        break;
                    case "conditionalTarget":
                        conditionalTarget = ConditionalTarget.ReadXml(reader);
                        transition.ConditionalTargets.Add(conditionalTarget);
                        break;
                    case "assignInputToUserVariable":
                        transition.AssignInputToVariable = Convert.ToBoolean(reader.ReadString());
                        break;
                    case "variableToAssign":
                        transition.VariableToAssign = reader.ReadString();
                        break;
                    case "assignWildcardToUserVariable":
                        transition.AssignWildcardToVariable = Convert.ToBoolean(reader.ReadString());
                        break;
                    case "variableToAssignFromWildcard":
                        transition.VariableToAssignFromWildcard = reader.ReadString();
                        break;
                }
            }

            return transition;
        }

        public void GetUsedSerialPort(SortedList<int, SerialPortConfiguration> usedSerialPorts)
        {
            UserEvent serialUserEvent = null;

            if ((BSEvent is UserEvent) && (((UserEvent)BSEvent).UserEventName == "serial"))
            {
                serialUserEvent = BSEvent as UserEvent;
            }
            else if (BSEvent is UserDefinedEvent)
            {
                UserDefinedEvent userDefinedEvent = BSEvent as UserDefinedEvent;
                foreach (UserEvent userEvent in userDefinedEvent.UserEvents)
                {
                    if (userEvent.UserEventName == "serial")
                    {
                        serialUserEvent = userEvent;
                        break;
                    }
                }
            }

            if (serialUserEvent != null)
            {
                if (serialUserEvent is TwoParameterUserEvent)
                {
                    int port = Convert.ToInt32((serialUserEvent as TwoParameterUserEvent).Parameter);
                    if (!usedSerialPorts.ContainsKey(port))
                    {
                        usedSerialPorts.Add(port, null);
                    }
                }
            }
        }

        public bool IsEventType(string userEventName)
        {
            if ((BSEvent is UserEvent) && (((UserEvent)BSEvent).UserEventName == userEventName)) return true;

            if (BSEvent is UserDefinedEvent)
            {
                UserDefinedEvent userDefinedEvent = BSEvent as UserDefinedEvent;
                foreach (UserEvent userEvent in userDefinedEvent.UserEvents)
                {
                    if (userEvent.UserEventName == userEventName) return true;
                }
            }

            return false;
        }

        public bool IsDuplicateEvent(string userEventName, string parameter, string parameter2)
        {
            if (BSEvent is UserEvent)
            {
                return IsDuplicateUserEvent(BSEvent as UserEvent, userEventName, parameter, parameter2);
            }

            if (BSEvent is UserDefinedEvent)
            {
                UserDefinedEvent userDefinedEvent = BSEvent as UserDefinedEvent;
                foreach (UserEvent userEvent in userDefinedEvent.UserEvents)
                {
                    if (IsDuplicateUserEvent(userEvent, userEventName, parameter, parameter2)) return true;
                }
            }

            return false;
        }

        private bool IsDuplicateUserEvent(UserEvent userEvent, string userEventName, string parameter, string parameter2)
        {
            if (userEvent.UserEventName == userEventName)
            {
                if (userEvent is TwoParameterUserEvent)
                {
                    TwoParameterUserEvent twoParameterUserEvent = userEvent as TwoParameterUserEvent;
                    if (BrightAuthorUtils.StringsEqualCaseInsensitive(twoParameterUserEvent.Parameter, parameter) && BrightAuthorUtils.StringsEqualCaseInsensitive(twoParameterUserEvent.Parameter2, parameter2))
                    {
                        return true;
                    }
                }
                else if (userEvent is SimpleUserEvent)
                {
                    SimpleUserEvent simpleUserEvent = (SimpleUserEvent)userEvent;
                    if (BrightAuthorUtils.StringsEqualCaseInsensitive(simpleUserEvent.Parameter, parameter))
                    {
                        return true;
                    }
                }
                else if (userEvent is GPIOUserEvent)
                {
                    GPIOUserEvent gpioUserEvent = (GPIOUserEvent)userEvent;
                    if (gpioUserEvent.ButtonNumber == parameter && gpioUserEvent.ButtonDirection == parameter2)
                    {
                        return true;
                    }
                }
                else if (userEvent is BPUserEvent)
                {
                    BPUserEvent bpUserEvent = (BPUserEvent)userEvent;
                    if ((bpUserEvent.ButtonNumber == parameter) && (((int)bpUserEvent.BPIndex).ToString() == parameter2))
                    {
                        return true;
                    }
                }
                else if (userEvent is AuxConnectDisconnectUserEvent)
                {
                    string existingParameter = (userEvent as AuxConnectDisconnectUserEvent).AudioConnector.ToString();
                    if (userEvent.UserEventName == userEventName && parameter == existingParameter) return true;
                }
                else if (userEvent is UDPEvent)
                {
                    string existingParameter = (userEvent as UDPEvent).Parameter;
                    if (parameter == existingParameter) return true;
                }
            }
            return false;
        }

        public void GetUsedUserDefinedEvents(Dictionary<string, UserDefinedEvent> usedUserDefinedEvents)
        {
            if (BSEvent is UserDefinedEvent)
            {
                UserDefinedEvent userDefinedEvent = BSEvent as UserDefinedEvent;
                usedUserDefinedEvents[userDefinedEvent.Name] = userDefinedEvent;
            }
        }

        public void GetMissingUserDefinedEvents(Dictionary<string, UserDefinedEvent> missingUserDefinedEvents)
        {
            if (BSEvent is UserDefinedEvent)
            {
                UserDefinedEvent userDefinedEvent = BSEvent as UserDefinedEvent;
                if (!userDefinedEvent.IsValid())
                {
                    if (!missingUserDefinedEvents.ContainsKey(userDefinedEvent.Name))
                    {
                        missingUserDefinedEvents.Add(userDefinedEvent.Name, userDefinedEvent);
                    }
                }
            }
        }

        public List<UserVariable> GetUserVariablesInUse()
        {
            Dictionary<string, UserVariable> signUserVariables = Sign.CurrentSign.UserVariableSet.UserVariables;

            List<UserVariable> transitionUserVariables = new List<UserVariable>();

            if (BSEvent is TimeClockEvent)
            {
                TimeClockEvent timeClockEvent = BSEvent as TimeClockEvent;
                if (timeClockEvent.TimeClockEventSpec is TimeClockDateTimeByUserVariable)
                {
                    TimeClockDateTimeByUserVariable tcdtbuv = timeClockEvent.TimeClockEventSpec as TimeClockDateTimeByUserVariable;
                    if (signUserVariables.ContainsKey(tcdtbuv.UserVariableName))
                    {
                        UserVariable userVariable = signUserVariables[tcdtbuv.UserVariableName];
                        transitionUserVariables.Add(userVariable);
                    }
                }
            }

            foreach (ConditionalTarget conditionalTarget in _conditionalTargets)
            {
                if (conditionalTarget.VariableType == "userVariable")
                {
                    if (signUserVariables.ContainsKey(conditionalTarget.VariableName))
                    {
                        UserVariable userVariable = signUserVariables[conditionalTarget.VariableName];
                        if (!transitionUserVariables.Contains(userVariable))
                        {
                            transitionUserVariables.Add(userVariable);
                        }
                    }
                }

                List<UserVariable> userVariables = new List<UserVariable>();
                conditionalTarget.VariableValue.GetUserVariablesInUse(userVariables);
                conditionalTarget.VariableValue2.GetUserVariablesInUse(userVariables);
                foreach (UserVariable userVariable in userVariables)
                {
                    if (signUserVariables.ContainsKey(userVariable.Name))
                    {
                        transitionUserVariables.Add(userVariable);
                    }
                }
            }

            if (_assignInputToUserVariable)
            {
                if (signUserVariables.ContainsKey(_variableToAssign))
                {
                    UserVariable userVariable = signUserVariables[_variableToAssign];
                    if (!transitionUserVariables.Contains(userVariable))
                    {
                        transitionUserVariables.Add(userVariable);
                    }
                }
            }

            if (_assignWildcardToUserVariable)
            {
                if (signUserVariables.ContainsKey(_variableToAssignFromWildcard))
                {
                    UserVariable userVariable = signUserVariables[_variableToAssignFromWildcard];
                    if (!transitionUserVariables.Contains(userVariable))
                    {
                        transitionUserVariables.Add(userVariable);
                    }
                }
            }

            foreach (BrightSignCmd brightSignCmd in _brightSignCmds)
            {
                List<UserVariable> userVariables = brightSignCmd.GetUserVariablesInUse();
                foreach (UserVariable userVariable in userVariables)
                {
                    if (!transitionUserVariables.Contains(userVariable))
                    {
                        transitionUserVariables.Add(userVariable);
                    }
                }
            }

            return transitionUserVariables;
        }

        public void UpdateUserDefinedEvents(ObservableCollection<UserDefinedEvent> userDefinedEvents)
        {
            if (BSEvent != null && BSEvent is UserDefinedEvent)
            {
                UserDefinedEvent thisTransitionUserDefinedEvent = BSEvent as UserDefinedEvent;
                foreach (UserDefinedEvent userDefinedEvent in userDefinedEvents)
                {
                    if (userDefinedEvent.OriginalName != userDefinedEvent.Name && thisTransitionUserDefinedEvent.Name == userDefinedEvent.OriginalName)
                    {
                        thisTransitionUserDefinedEvent.Name = userDefinedEvent.Name;
                        _image.ToolTip = thisTransitionUserDefinedEvent.GetToolTip();
                    }
                }
            }
        }

        public void UpdateUserVariables(UserVariableSet userVariableSet)
        {
            if (BSEvent is TimeClockEvent)
            {
                TimeClockEvent timeClockEvent = BSEvent as TimeClockEvent;
                if (timeClockEvent.TimeClockEventSpec is TimeClockDateTimeByUserVariable)
                {
                    TimeClockDateTimeByUserVariable tcdtbuv = timeClockEvent.TimeClockEventSpec as TimeClockDateTimeByUserVariable;
                    tcdtbuv.UserVariableName = userVariableSet.UpdateUserVariableName(tcdtbuv.UserVariableName);
                }
            }

            foreach (ConditionalTarget conditionalTarget in _conditionalTargets)
            {
                if (conditionalTarget.VariableType == "userVariable")
                {
                    conditionalTarget.VariableName = userVariableSet.UpdateUserVariableName(conditionalTarget.VariableName);
                }
            }

            if (_assignInputToUserVariable)
            {
                if (_variableToAssign != String.Empty)
                {
                    _variableToAssign = userVariableSet.UpdateUserVariableName(_variableToAssign);
                }
            }

            if (_assignWildcardToUserVariable)
            {
                if (_variableToAssignFromWildcard != String.Empty)
                {
                    _variableToAssignFromWildcard = userVariableSet.UpdateUserVariableName(_variableToAssignFromWildcard);
                }
            }

            foreach (BrightSignCmd brightSignCmd in _brightSignCmds)
            {
                brightSignCmd.UpdateUserVariables(userVariableSet);
            }
        }

        private void AddPresentationsInUse(List<BrightSignCmd> brightSignCmds, List<PresentationIdentifier> transitionPresentationIdentifiers)
        {
            foreach (BrightSignCmd brightSignCmd in brightSignCmds)
            {
                List<PresentationIdentifier> presentationIdentifiers = brightSignCmd.GetPresentationsInUse();
                foreach (PresentationIdentifier presentationIdentifier in presentationIdentifiers)
                {
                    if (!transitionPresentationIdentifiers.Contains(presentationIdentifier))
                    {
                        transitionPresentationIdentifiers.Add(presentationIdentifier);
                    }
                }
            }
        }

        public List<PresentationIdentifier> GetPresentationsInUse()
        {
            List<PresentationIdentifier> transitionPresentationIdentifiers = new List<PresentationIdentifier>();

            AddPresentationsInUse(_brightSignCmds, transitionPresentationIdentifiers);

            foreach (ConditionalTarget conditionalTarget in _conditionalTargets)
            {
                AddPresentationsInUse(conditionalTarget.BrightSignCmds, transitionPresentationIdentifiers);
            }

            return transitionPresentationIdentifiers;
        }

        private void UpdatePresentationIdentifiers(List<BrightSignCmd> brightSignCmds, PresentationIdentifierSet presentationIdentifierSet)
        {
            foreach (BrightSignCmd brightSignCmd in brightSignCmds)
            {
                brightSignCmd.UpdatePresentationIdentifiers(presentationIdentifierSet);
            }
        }

        public void UpdatePresentationIdentifiers(PresentationIdentifierSet presentationIdentifierSet)
        {
            UpdatePresentationIdentifiers(_brightSignCmds, presentationIdentifierSet);

            foreach (ConditionalTarget conditionalTarget in _conditionalTargets)
            {
                UpdatePresentationIdentifiers(conditionalTarget.BrightSignCmds, presentationIdentifierSet);
            }
        }

        public string GetScriptPluginInUse()
        {
            if (BSEvent is PluginMessageEvent)
            {
                return (BSEvent as PluginMessageEvent).Name;
            }

            return String.Empty;
        }

        public void UpdateScriptPlugins(ScriptPluginSet scriptPluginSet)
        {
            if (BSEvent is PluginMessageEvent)
            {
                PluginMessageEvent pluginMessageEvent = BSEvent as PluginMessageEvent;
                pluginMessageEvent.Name = scriptPluginSet.UpdateScriptPluginName(pluginMessageEvent.Name);
            }
        }

        public bool GPSInUse()
        {
            if (BSEvent is GPSUserEvent) return true;

            if (BSEvent is UserDefinedEvent)
            {
                UserDefinedEvent userDefinedEvent = BSEvent as UserDefinedEvent;
                foreach (UserEvent userEvent in userDefinedEvent.UserEvents)
                {
                    if (userEvent is GPSUserEvent) return true;
                }
            }

            return false;
        }

        public void ConvertAudioCommands(Sign sign, Zone zone, List<BrightSignCmd> convertedBrightSignCmds)
        {
            foreach (BrightSignCmd brightSignCmd in BrightSignCmds)
            {
                brightSignCmd.ConvertAudioCommand(sign, zone, convertedBrightSignCmds);
            }

            foreach (ConditionalTarget conditionalTarget in ConditionalTargets)
            {
                conditionalTarget.ConvertAudioCommands(sign, zone, convertedBrightSignCmds);
            }

            if (BSEvent != null)
            {
                BSEvent.ConvertAudioCommand(sign, zone, convertedBrightSignCmds);
            }
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
