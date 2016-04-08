using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
    public class TextWidget
    {
        protected string _numberOfLines;
        protected string _delay;
        protected string _scrollingMethod;
        protected string _rotation;
        protected string _alignment;

        public TextWidget(string numberOfLines, string delay, string rotation, string alignment, string scrollingMethod)
        {
            _numberOfLines = numberOfLines;
            _delay = delay;
            _rotation = rotation;
            _alignment = alignment;
            _scrollingMethod = scrollingMethod;
        }

        public object Copy() // ICloneable implementation
        {
            TextWidget textWidget = new TextWidget(this._numberOfLines,
                this._delay, this._rotation, this._alignment, this._scrollingMethod);
            return textWidget;
        }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            TextWidget tw = (TextWidget)obj;

            if (
                 (this._numberOfLines != tw._numberOfLines) ||
                 (this._delay != tw._delay) ||
                 (this._rotation != tw._rotation) ||
                 (this._alignment != tw._alignment) ||
                 (this._scrollingMethod != tw._scrollingMethod))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void WriteToXml(XmlTextWriter writer, bool publish, Sign sign)
        {
            writer.WriteStartElement("textWidget");

            writer.WriteElementString("numberOfLines", _numberOfLines);
            writer.WriteElementString("delay", _delay);

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
            
            writer.WriteElementString("alignment", _alignment);
            writer.WriteElementString("scrollingMethod", _scrollingMethod);

            writer.WriteEndElement(); // TextWidget
        }

        public string NumberOfLines
        {
            get
            {
                return _numberOfLines;
            }
            set
            {
                _numberOfLines = value;
            }
        }

        public string Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay = value;
            }
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

        public string Alignment
        {
            get { return _alignment; }
            set { _alignment = value; }
        }

        public string ScrollingMethod
        {
            get
            {
                return _scrollingMethod;
            }
            set
            {
                _scrollingMethod = value;
            }
        }

        public static TextWidget ReadFromXml(XmlReader reader)
        {
            string numberOfLines = "";
            string delay = "";
            string rotation = "";
            string alignment = "";
            string scrollingMethod = "";

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "numberOfLines":
                        numberOfLines = reader.ReadString();
                        break;
                    case "delay":
                        delay = reader.ReadString();
                        break;
                    case "rotation":
                        rotation = reader.ReadString();
                        break;
                    case "alignment":
                        alignment = reader.ReadString();
                        break;
                    case "scrollingMethod":
                        scrollingMethod = reader.ReadString();
                        break;
                }
            }

            if ((numberOfLines == "") || (delay == "") || (scrollingMethod == "")) return null;

            TextWidget tw = new TextWidget(numberOfLines, delay, 
                rotation, alignment, scrollingMethod);
            return tw;
        }
    }
}
