using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
//using System.Windows.Media;
using System.Windows;

namespace BAModel
{
    class ClockZone : Zone
    {
        private bool _displayTime;

//        private Widget _widget;
        private string _rotation;

        public ClockZone(string name, int xStart, int yStart, int width, int height, string zoneType, string id,
            bool displayTime, Widget w, string rotation) :
            base (name, xStart, yStart, width, height, zoneType, id)
        {
            _displayTime = displayTime;
            _widget = w;
            _rotation = rotation;
        }

        public override object Clone() // ICloneable implementation
        {
            Widget newWidget = this.Widget.Copy();

            ClockZone clockZone = new ClockZone(this.Name, this.X, this.Y, this.Width, this.Height, this.ZoneType, this.ZoneID, 
                this.DisplayTime, newWidget, this.Rotation);

            return base.Copy(clockZone);
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            ClockZone zone = (ClockZone)obj;

            if ((zone.DisplayTime != this.DisplayTime) ||
                (!this.Widget.IsEqual(zone.Widget)) ||
                (zone.Rotation != this.Rotation) )
            {
                return false;
            }

            return base.IsEqual(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool DisplayTime
        {
            get { return _displayTime; }
            set { _displayTime = value; }
        }

        public Widget Widget
        {
            get { return _widget; }
            set { _widget = value; }
        }

        public string Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        public override void WriteZoneSpecificDataToXml(XmlTextWriter writer, bool publish, Sign sign)
        {
            writer.WriteElementString("displayTime", _displayTime.ToString());

            string rotation = _rotation;
            if (publish && sign != null)
            {
                // device does not know about monitor orientation; do translation here
                if (sign.MonitorOrientation == MonitorOrientation.Portrait)
                {
                    switch (_rotation)
                    {
                        case "90":
                            rotation = "0";
                            break;
                        case "180":
                            rotation = "90";
                            break;
                        case "270":
                            rotation = "180";
                            break;
                        case "0":
                        default:
                            rotation = "270";
                            break;
                    }
                }
                else if (sign.MonitorOrientation == MonitorOrientation.PortraitBottomOnRight)
                {
                    switch (_rotation)
                    {
                        case "90":
                            rotation = "180";
                            break;
                        case "180":
                            rotation = "270";
                            break;
                        case "270":
                            rotation = "0";
                            break;
                        case "0":
                        default:
                            rotation = "90";
                            break;
                    }
                }
            }

            writer.WriteElementString("rotation", rotation);

            _widget.WriteToXml(writer, publish);
        }

        public static void ReadZoneSpecificDataXml(XmlReader reader, out bool displayTime, out Widget w, out string rotation)
        {
            displayTime = true;
            w = null;
            rotation = "0";

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "displayTime":
                        string displayTimeStr = reader.ReadString();
                        if (displayTimeStr != "True")
                        {
                            displayTime = false;
                        }
                        break;
                    case "rotation":
                        rotation = reader.ReadString();
                        break;
                    case "widget":
                        w = Widget.ReadFromXml(reader);
                        break;
                }
            }
        }

//        public override void EditZoneParameters(Window1 parent)
//        {
//            EditClockZoneDlg editClockZoneDlg = new EditClockZoneDlg();
//            editClockZoneDlg.Owner = parent;
//
//            editClockZoneDlg.ForegroundTextColor = _widget.ForegroundTextColor;
//            editClockZoneDlg.BackgroundTextColor = _widget.BackgroundTextColor;
//            editClockZoneDlg.FontPath = _widget.Font;
//            editClockZoneDlg.BackgroundBitmapFile = _widget.BackroundBitmapFile;
//            editClockZoneDlg.StretchBackgroundBitmap = _widget.StretchBitmapFile;
//            editClockZoneDlg.SafeTextRegionX = _widget.SafeTextRegionX;
//            editClockZoneDlg.SafeTextRegionY = _widget.SafeTextRegionY;
//            editClockZoneDlg.SafeTextRegionWidth = _widget.SafeTextRegionWidth;
//            editClockZoneDlg.SafeTextRegionHeight = _widget.SafeTextRegionHeight;
//            editClockZoneDlg.Rotation = this.Rotation;
//
//            if (editClockZoneDlg.ShowDialog() == true)
//            {
//                _widget.ForegroundTextColor = editClockZoneDlg.ForegroundTextColor;
//                _widget.BackgroundTextColor = editClockZoneDlg.BackgroundTextColor;
//                _widget.Font = editClockZoneDlg.FontPath;
//
//                _widget.BackroundBitmapFile = editClockZoneDlg.BackgroundBitmapFile;
//                _widget.StretchBitmapFile = editClockZoneDlg.StretchBackgroundBitmap;
//                _widget.SafeTextRegionX = editClockZoneDlg.SafeTextRegionX;
//                _widget.SafeTextRegionY = editClockZoneDlg.SafeTextRegionY;
//                _widget.SafeTextRegionWidth = editClockZoneDlg.SafeTextRegionWidth;
//                _widget.SafeTextRegionHeight = editClockZoneDlg.SafeTextRegionHeight;
//                this.Rotation = editClockZoneDlg.Rotation;
//
//                if (UserPreferences.SavePropertiesForAllFuture)
//                {
//                    UserPreferences.ClockForegroundTextColor = editClockZoneDlg.ForegroundTextColor;
//                    UserPreferences.ClockBackgroundTextColor = editClockZoneDlg.BackgroundTextColor;
//                    UserPreferences.ClockFont = editClockZoneDlg.FontPath;
//                    UserPreferences.ClockBackgroundBitmapFile = editClockZoneDlg.BackgroundBitmapFile;
//                    UserPreferences.ClockStretchBackgroundBitmap = editClockZoneDlg.StretchBackgroundBitmap == 1 ? "true" : "false";
//                    UserPreferences.ClockSafeTextRegionX = editClockZoneDlg.SafeTextRegionX;
//                    UserPreferences.ClockSafeTextRegionY = editClockZoneDlg.SafeTextRegionY;
//                    UserPreferences.ClockSafeTextRegionWidth = editClockZoneDlg.SafeTextRegionWidth;
//                    UserPreferences.ClockSafeTextRegionHeight = editClockZoneDlg.SafeTextRegionHeight;
//                    UserPreferences.ClockRotation = editClockZoneDlg.Rotation;
//                }
//            }
//        }

        public override void Publish(List<PublishFile> publishFiles)
        {
            Widget.Publish(publishFiles);
        }

        public override void ReplaceMediaFiles(Dictionary<string, string> replacementFiles, bool preserveStateNames)
        {
            Widget.ReplaceMediaFiles(replacementFiles);
        }

        public override void FindBrokenMediaLinks(List<Object> brokenLinks)
        {
            Widget.FindBrokenMediaLinks(brokenLinks);
        }
    }
}
