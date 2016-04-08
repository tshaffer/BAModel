using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
//using System.Windows.Media;
using System.Windows;

namespace BAModel
{
    class TickerZone : Zone, ICloneable
    {
        private TextWidget _textWidget;

        private Widget _widget;

        public int ScrollSpeed { get; set; }

        public TickerZone(string name, int xStart, int yStart, int width, int height, string zoneType, string id, Widget w) :
            base(name, xStart, yStart, width, height, zoneType, id)
        {
            _textWidget = new TextWidget(
                UserPreferences.TickerNumberOfLines,
                UserPreferences.TickerDisplayTime,
                UserPreferences.TickerRotation,
                UserPreferences.TickerAlignment,
                UserPreferences.TickerTextAppearance);

            _widget = w;

            ScrollSpeed = UserPreferences.TickerScrollSpeed;
        }

        public override object Clone() // ICloneable implementation
        {
            TickerZone tickerZone = new TickerZone(this.Name, this.X, this.Y, this.Width, this.Height, this.ZoneType, this.ZoneID, this.Widget);
            if (this._textWidget != null)
            {
                tickerZone._textWidget = (TextWidget)this._textWidget.Copy();
            }
            if (this._widget != null)
            {
                tickerZone._widget = this._widget.Copy();
            }

            tickerZone.ScrollSpeed = this.ScrollSpeed;

            return base.Copy(tickerZone);
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            TickerZone zone = (TickerZone)obj;

            if (!this._textWidget.IsEqual(zone._textWidget)) return false;

            if (!this._widget.IsEqual(zone._widget)) return false;

            if (this.ScrollSpeed != zone.ScrollSpeed) return false;

            return base.IsEqual(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public TextWidget TextWidget
        {
            get { return _textWidget; }
            set { _textWidget = value; }
        }

        public Widget Widget
        {
            get { return _widget; }
            set { _widget = value; }
        }

        public override void WriteZoneSpecificDataToXml(XmlTextWriter writer, bool publish, Sign sign)
        {
            _textWidget.WriteToXml(writer, publish, sign);
            _widget.WriteToXml(writer, publish);
            writer.WriteElementString("scrollSpeed", ScrollSpeed.ToString());
        }

        public static TextWidget ReadZoneSpecificDataXml(XmlReader reader, out Widget w, out int scrollSpeed)
        {
            TextWidget tw = null;
            w = null;
            scrollSpeed = 100;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "textWidget":
                        tw = TextWidget.ReadFromXml(reader);
                        break;
                    case "widget":
                        w = Widget.ReadFromXml(reader);
                        break;
                    case "scrollSpeed":
                        scrollSpeed = Convert.ToInt32(reader.ReadString());
                        break;
                }
            }

            return tw;
        }


//        public override void EditZoneParameters(Window1 parent)
//        {
//            EditTickerZoneDlg editTickerZoneDlg = new EditTickerZoneDlg();
//            editTickerZoneDlg.Owner = parent;
//
//            editTickerZoneDlg.NumberOfLines = _textWidget.NumberOfLines;
//            editTickerZoneDlg.DisplayTime = _textWidget.Delay;
//            editTickerZoneDlg.Rotation = _textWidget.Rotation;
//            editTickerZoneDlg.Alignment = _textWidget.Alignment;
//            editTickerZoneDlg.TextAppearance = _textWidget.ScrollingMethod;
//            editTickerZoneDlg.ScrollSpeed = ScrollSpeed;
//
//            editTickerZoneDlg.ForegroundTextColor = _widget.ForegroundTextColor;
//            editTickerZoneDlg.BackgroundTextColor = _widget.BackgroundTextColor;
//            editTickerZoneDlg.FontPath = _widget.Font;
//            editTickerZoneDlg.BackgroundBitmapFile = _widget.BackroundBitmapFile;
//            editTickerZoneDlg.StretchBackgroundBitmap = _widget.StretchBitmapFile;
//            editTickerZoneDlg.SafeTextRegionX = _widget.SafeTextRegionX;
//            editTickerZoneDlg.SafeTextRegionY = _widget.SafeTextRegionY;
//            editTickerZoneDlg.SafeTextRegionWidth = _widget.SafeTextRegionWidth;
//            editTickerZoneDlg.SafeTextRegionHeight = _widget.SafeTextRegionHeight;
//
//            if (editTickerZoneDlg.ShowDialog() == true)
//            {
//                _textWidget.NumberOfLines = editTickerZoneDlg.NumberOfLines;
//                _textWidget.Delay = editTickerZoneDlg.DisplayTime;
//                _textWidget.Rotation = editTickerZoneDlg.Rotation;
//                _textWidget.Alignment = editTickerZoneDlg.Alignment;
//                _textWidget.ScrollingMethod = editTickerZoneDlg.TextAppearance;
//                ScrollSpeed = editTickerZoneDlg.ScrollSpeed;
//
//                _widget.ForegroundTextColor = editTickerZoneDlg.ForegroundTextColor;
//                _widget.BackgroundTextColor = editTickerZoneDlg.BackgroundTextColor;
//                _widget.Font = editTickerZoneDlg.FontPath;
//
//                _widget.BackroundBitmapFile = editTickerZoneDlg.BackgroundBitmapFile;
//                _widget.StretchBitmapFile = editTickerZoneDlg.StretchBackgroundBitmap;
//                _widget.SafeTextRegionX = editTickerZoneDlg.SafeTextRegionX;
//                _widget.SafeTextRegionY = editTickerZoneDlg.SafeTextRegionY;
//                _widget.SafeTextRegionWidth = editTickerZoneDlg.SafeTextRegionWidth;
//                _widget.SafeTextRegionHeight = editTickerZoneDlg.SafeTextRegionHeight;
//
//                if (UserPreferences.SavePropertiesForAllFuture)
//                {
//                    UserPreferences.TickerNumberOfLines = editTickerZoneDlg.NumberOfLines;
//                    UserPreferences.TickerDisplayTime = editTickerZoneDlg.DisplayTime;
//                    UserPreferences.TickerRotation = editTickerZoneDlg.Rotation;
//                    UserPreferences.TickerAlignment = editTickerZoneDlg.Alignment;
//                    UserPreferences.TickerTextAppearance = editTickerZoneDlg.TextAppearance;
//                    UserPreferences.TickerScrollSpeed = editTickerZoneDlg.ScrollSpeed;
//
//                    UserPreferences.TickerFont = editTickerZoneDlg.FontPath;
//                    UserPreferences.TickerForegroundTextColor = editTickerZoneDlg.ForegroundTextColor;
//                    UserPreferences.TickerBackgroundTextColor = editTickerZoneDlg.BackgroundTextColor;
//
//                    UserPreferences.TickerBackgroundBitmapFile = editTickerZoneDlg.BackgroundBitmapFile;
//                    UserPreferences.TickerStretchBackgroundBitmap = editTickerZoneDlg.StretchBackgroundBitmap == 1 ? "true" : "false";
//                    UserPreferences.TickerSafeTextRegionX = editTickerZoneDlg.SafeTextRegionX;
//                    UserPreferences.TickerSafeTextRegionY = editTickerZoneDlg.SafeTextRegionY;
//                    UserPreferences.TickerSafeTextRegionWidth = editTickerZoneDlg.SafeTextRegionWidth;
//                    UserPreferences.TickerSafeTextRegionHeight = editTickerZoneDlg.SafeTextRegionHeight;
//                }
//            }
//        }

        public override void Publish(List<PublishFile> publishFiles)
        {
            Widget.Publish(publishFiles);

            if (Playlist != null)
            {
                Playlist.Publish(publishFiles);
            }
        }

//        public override void ReplaceMediaFiles(Dictionary<string, string> replacementFiles, bool preserveStateNames)
//        {
//            Widget.ReplaceMediaFiles(replacementFiles);
//
//            if (Playlist != null)
//            {
//                Playlist.ReplaceMediaFiles(replacementFiles, preserveStateNames);
//            }
//        }

//        public override void FindBrokenMediaLinks(List<Object> brokenLinks)
//        {
//            if (Playlist != null)
//            {
//                Playlist.FindBrokenMediaLinks(brokenLinks);
//            }
//            Widget.FindBrokenMediaLinks(brokenLinks);
//        }
    }
}
