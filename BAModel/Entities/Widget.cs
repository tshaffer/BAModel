using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
using System.IO;

namespace BAModel
{
    public class Widget
    {
        protected string _foregroundTextColor;
        protected string _backgroundTextColor;
        protected string _font;
        protected int _fontSize;
        protected string _backgroundBitmapFile;
        protected int _stretchBitmapFile;
        protected string _safeTextRegionX;
        protected string _safeTextRegionY;
        protected string _safeTextRegionWidth;
        protected string _safeTextRegionHeight;

        public Widget(string foregroundTextColor, string backgroundTextColor, string font)
        {
            _foregroundTextColor = foregroundTextColor;
            _backgroundTextColor = backgroundTextColor;
            _font = font;
            _fontSize = 0;

            _backgroundBitmapFile = "";
            _stretchBitmapFile = 1;
            _safeTextRegionX = "";
            _safeTextRegionY = "";
            _safeTextRegionWidth = "";
            _safeTextRegionHeight = "";
        }

        public Widget Copy() // ICloneable implementation
        {
            Widget widget = new Widget(this.ForegroundTextColor, this.BackgroundTextColor, this.Font);
            widget.FontSize = this.FontSize;
            widget.BackroundBitmapFile = this.BackroundBitmapFile;
            widget.StretchBitmapFile = this.StretchBitmapFile;
            widget.SafeTextRegionX = this.SafeTextRegionX;
            widget.SafeTextRegionY = this.SafeTextRegionY;
            widget.SafeTextRegionWidth = this.SafeTextRegionWidth;
            widget.SafeTextRegionHeight = this.SafeTextRegionHeight;

            return widget;
        }

        public virtual bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            Widget widget = (Widget)obj;

            if ((this._foregroundTextColor != widget._foregroundTextColor) ||
                 (this._backgroundTextColor != widget._backgroundTextColor) ||
                 (this._font != widget._font) ||
                 (this._fontSize != widget._fontSize) ||
                 (this._backgroundBitmapFile != widget._backgroundBitmapFile) ||
                 (this._stretchBitmapFile != widget._stretchBitmapFile) ||
                 (this._safeTextRegionX != widget._safeTextRegionX) ||
                 (this._safeTextRegionY != widget._safeTextRegionY) ||
                 (this._safeTextRegionWidth != widget._safeTextRegionWidth) ||
                 (this._safeTextRegionHeight != widget._safeTextRegionHeight))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string ForegroundTextColor
        {
            get
            {
                return _foregroundTextColor;
            }
            set
            {
                _foregroundTextColor = value;
            }
        }

        public string BackgroundTextColor
        {
            get
            {
                return _backgroundTextColor;
            }
            set
            {
                _backgroundTextColor = value;
            }
        }

        public string Font
        {
            get { return _font; }
            set { _font = value; }
        }

        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        public string BackroundBitmapFile
        {
            get { return _backgroundBitmapFile; }
            set { _backgroundBitmapFile = value; }
        }

        public int StretchBitmapFile
        {
            get { return _stretchBitmapFile; }
            set { _stretchBitmapFile = value; }
        }

        public string SafeTextRegionX
        {
            get { return _safeTextRegionX; }
            set { _safeTextRegionX = value; }
        }

        public string SafeTextRegionY
        {
            get { return _safeTextRegionY; }
            set { _safeTextRegionY = value; }
        }

        public string SafeTextRegionWidth
        {
            get { return _safeTextRegionWidth; }
            set { _safeTextRegionWidth = value; }
        }

        public string SafeTextRegionHeight
        {
            get { return _safeTextRegionHeight; }
            set { _safeTextRegionHeight = value; }
        }

        private static void GetColorValues(string colorSpec, ref byte a, ref byte r, ref byte g, ref byte b)
        {
            int colorSpecLength = colorSpec.Length;
            int alphaSpecLength = colorSpecLength - 6;

            string alpha = colorSpec.Substring(0, alphaSpecLength);
            a = Byte.Parse(alpha, System.Globalization.NumberStyles.AllowHexSpecifier);
            string red = colorSpec.Substring(alphaSpecLength, 2);
            r = Byte.Parse(red, System.Globalization.NumberStyles.AllowHexSpecifier);
            string green = colorSpec.Substring(alphaSpecLength + 2, 2);
            g = Byte.Parse(green, System.Globalization.NumberStyles.AllowHexSpecifier);
            string blue = colorSpec.Substring(alphaSpecLength + 4, 2);
            b = Byte.Parse(blue, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private static string GetColor(string alpha, string red, string green, string blue)
        {
            byte a = Convert.ToByte(alpha);
            byte r = Convert.ToByte(red);
            byte g = Convert.ToByte(green);
            byte b = Convert.ToByte(blue);
            return a.ToString("X2") + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }

        public static void WriteColorToXml(XmlTextWriter writer, string elementValue, string elementName)
        {
            byte a = 0, r = 0, g = 0, b = 0;
            GetColorValues(elementValue, ref a, ref r, ref g, ref b);
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("a", a.ToString());
            writer.WriteAttributeString("r", r.ToString());
            writer.WriteAttributeString("g", g.ToString());
            writer.WriteAttributeString("b", b.ToString());
            writer.WriteEndElement(); // elementName
        }

        public void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("widget");

            if (_foregroundTextColor != "")
            {
                WriteColorToXml(writer, _foregroundTextColor, "foregroundTextColor");
            }
            if (_backgroundTextColor != "")
            {
                WriteColorToXml(writer, _backgroundTextColor, "backgroundTextColor");
            }
            if (_font != "")
            {
                if (publish && _font != "System")
                {
                    FileInfo fi = new FileInfo(_font);
                    string fileName = fi.Name;
                    writer.WriteElementString("font", fileName);
                }
                else
                {
                    writer.WriteElementString("font", _font);
                }
            }
            writer.WriteElementString("fontSize", _fontSize.ToString());
            if (_backgroundBitmapFile != "")
            {
                writer.WriteStartElement("backgroundBitmap");
                if (publish)
                {
                    FileInfo fi = new FileInfo(_backgroundBitmapFile);
                    string fileName = fi.Name;
                    writer.WriteAttributeString("file", fileName);
                }
                else
                {
                    writer.WriteAttributeString("file", _backgroundBitmapFile);
                }
                string stretch = "true";
                if (_stretchBitmapFile == 0)
                {
                    stretch = "false";
                }
                writer.WriteAttributeString("stretch", stretch);
                writer.WriteEndElement(); //backGroundBitmap
            }
            if (_safeTextRegionX != "")
            {
                writer.WriteStartElement("safeTextRegion");

                writer.WriteElementString("safeTextRegionX", _safeTextRegionX);
                writer.WriteElementString("safeTextRegionY", _safeTextRegionY);
                writer.WriteElementString("safeTextRegionWidth", _safeTextRegionWidth);
                writer.WriteElementString("safeTextRegionHeight", _safeTextRegionHeight);

                writer.WriteEndElement(); // safeTextRegion
            }

            writer.WriteEndElement(); // Widget

        }

        public void Publish(List<PublishFile> publishFiles)
        {
            if (Font != "" && Font != "System")
            {
                FileInfo fi = new FileInfo(_font);
                publishFiles.Add(
                    new PublishFile
                    {
                        FileName = fi.Name,
                        FilePath = _font,
                        FileIsLocal = true
                    }
                );
            }

            if (_backgroundBitmapFile != "")
            {
                FileInfo fi = new FileInfo(_backgroundBitmapFile);
                publishFiles.Add(
                    new PublishFile
                    {
                        FileName = fi.Name,
                        FilePath = _backgroundBitmapFile,
                        FileIsLocal = true
                    }
                );
            }
        }

        public void ReplaceMediaFiles(Dictionary<string, string> replacementFiles)
        {
            if (_backgroundBitmapFile != "")
            {
                if (replacementFiles.ContainsKey(_backgroundBitmapFile))
                {
                    string filePath = replacementFiles[_backgroundBitmapFile];
                    _backgroundBitmapFile = filePath;
                }
            }
        }

        public static string ReadColor(XmlReader reader)
        {
            string alpha = reader.GetAttribute("a");
            string red = reader.GetAttribute("r");
            string green = reader.GetAttribute("g");
            string blue = reader.GetAttribute("b");
            return Widget.GetColor(alpha, red, green, blue);
        }

        public static Widget ReadFromXml(XmlReader reader)
        {
            string foregroundTextColor = "";
            string backgroundTextColor = "";
            string fontPath = "";
            int fontSize = 0;

            string safeTextRegionX = "";
            string safeTextRegionY = "";
            string safeTextRegionWidth = "";
            string safeTextRegionHeight = "";

            string backgroundBitmapFile = "";
            int stretch = 0;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "foregroundTextColor":
                        foregroundTextColor = Widget.ReadColor(reader);
                        break;
                    case "backgroundTextColor":
                        backgroundTextColor = Widget.ReadColor(reader);
                        break;
                    case "font":
                        fontPath = reader.ReadString();
                        break;
                    case "fontSize":
                        fontSize = Convert.ToInt16(reader.ReadString());
                        break;
                    case "safeTextRegion":
                        ReadSafeTextRegion(reader, out safeTextRegionX, out safeTextRegionY,
                            out safeTextRegionWidth, out safeTextRegionHeight);
                        break;
                    case "backgroundBitmap":
                        backgroundBitmapFile = reader.GetAttribute("file");
                        string stretchSpec = reader.GetAttribute("stretch");
                        if (stretchSpec == "true")
                        {
                            stretch = 1;
                        }
                        else
                        {
                            stretch = 0;
                        }
                        break;

                }
            }

            Widget w = new Widget(foregroundTextColor, backgroundTextColor, fontPath);

            w.FontSize = fontSize;
            w.BackroundBitmapFile = backgroundBitmapFile;
            w.StretchBitmapFile = stretch;

            w.SafeTextRegionX = safeTextRegionX;
            w.SafeTextRegionY = safeTextRegionY;
            w.SafeTextRegionWidth = safeTextRegionWidth;
            w.SafeTextRegionHeight = safeTextRegionHeight;

            return w;
        }

        public static void ReadSafeTextRegion(XmlReader reader,
            out string safeTextRegionX, out string safeTextRegionY,
            out string safeTextRegionWidth, out string safeTextRegionHeight)
        {
            safeTextRegionX = "";
            safeTextRegionY = "";
            safeTextRegionWidth = "";
            safeTextRegionHeight = "";

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "safeTextRegionX":
                        safeTextRegionX = reader.ReadString();
                        break;
                    case "safeTextRegionY":
                        safeTextRegionY = reader.ReadString();
                        break;
                    case "safeTextRegionWidth":
                        safeTextRegionWidth = reader.ReadString();
                        break;
                    case "safeTextRegionHeight":
                        safeTextRegionHeight = reader.ReadString();
                        break;
                }
            }
        }

        public void FindDuplicateMediaFiles(Dictionary<string, string> fileSpecs, DuplicateFileList duplicateFileList)
        {
            if ((Font != "") && (Font != "System"))
            {
                FileInfo fi = new FileInfo(_font);

                BrightAuthorUtils.FindDuplicateMediaFiles(fi.Name, Font, fileSpecs, duplicateFileList);
            }

            if (_backgroundBitmapFile != "")
            {
                FileInfo fi = new FileInfo(_backgroundBitmapFile);

                BrightAuthorUtils.FindDuplicateMediaFiles(fi.Name, _backgroundBitmapFile, fileSpecs, duplicateFileList);
            }
        }

        public bool FontExists(out string fontPath)
        {
            fontPath = "";

            if ((Font != "") && (Font != "System"))
            {
                fontPath = Font;
                return File.Exists(fontPath);
            }

            return true;
        }

        public bool BackgroundBitmapFileExists(out string backgroundBitmapFile)
        {
            backgroundBitmapFile = "";

            if (BackroundBitmapFile != "")
            {
                backgroundBitmapFile = BackroundBitmapFile;
                return File.Exists(backgroundBitmapFile);
            }

            return true;
        }

        public void FindBrokenMediaLinks(List<Object> brokenLinks)
        {
            AddBrokenLink(brokenLinks, BackroundBitmapFile, false);
            if (Font == "System") return;
            AddBrokenLink(brokenLinks, Font, true);
        }

        private void AddBrokenLink(List<Object> brokenLinks, string filePath, bool isFont)
        {
            if (!String.IsNullOrEmpty(filePath))
            {
                if (!File.Exists(filePath))
                {
                    WidgetBrokenLink imibl = new WidgetBrokenLink
                    {
                        Widget = this,
                        FilePath = filePath,
                        IsFont = isFont
                    };
                    brokenLinks.Add(imibl);
                }
            }
        }
    }

    public class WidgetBrokenLink : IBrokenLinkFixer
    {
        public Widget Widget { get; set; }
        public string FilePath { get; set; }
        public bool IsFont { get; set; }

        public string ItemFilePath
        {
            get { return FilePath; }
            set
            {
                if (IsFont)
                {
                    Widget.Font = value;
                }
                else
                {
                    Widget.BackroundBitmapFile = value;
                }
            }
        }
    }
}
