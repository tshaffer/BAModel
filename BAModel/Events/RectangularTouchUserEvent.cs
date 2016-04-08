using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace BAModel
{
    public class RectangularTouchUserEvent : UserEvent
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public RectangularTouchUserEvent()
        {
            UserEventName = "rectangularTouchEvent";
            Value = "Rectangular Touch";
            DialogTitle = "";
            ImageResourceName = "iconTouch";
            ImageResourceNameLarge = "iconTouchLarge";
            ImageResourceSelectedName = "iconTouchSelected";
            ValueI18NResource = "RectangularTouchEvent";
        }

        public override string GetToolTip()
        {
            return BrightAuthorUtils.GetLocalizedString("RectangularTouchEventTT");
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            writer.WriteStartElement("parameters");
            writer.WriteElementString("x", ((int)X).ToString());
            writer.WriteElementString("y", ((int)Y).ToString());
            writer.WriteElementString("width", ((int)Width).ToString());
            writer.WriteElementString("height", ((int)Height).ToString());

            writer.WriteEndElement(); // parameters

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }

        public override object Clone() // ICloneable implementation
        {
            RectangularTouchUserEvent userEvent = new RectangularTouchUserEvent();
            userEvent.X = this.X;
            userEvent.Y = this.Y;
            userEvent.Width = this.Width;
            userEvent.Height = this.Height;

            return userEvent;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is RectangularTouchUserEvent)) return false;

            RectangularTouchUserEvent rtue = (RectangularTouchUserEvent)bsEvent;
            if ((rtue.X != this.X) || (rtue.Y != this.Y) || (rtue.Width != this.Width) || (rtue.Height != this.Height)) return false;

            return true;
        }

        public override string Description()
        {
            return String.Format("{0}: {1},{2},{3},{4}",Value,X,Y,Width,Height);
        }

//        public void UpdateValues(RectangularTouchRegion rtr)
//        {
//            X = rtr.X;
//            Y = rtr.Y;
//            Width = rtr.Width;
//            Height = rtr.Height;
//        }


        public new static RectangularTouchUserEvent ReadXml(XmlReader reader)
        {
            RectangularTouchUserEvent rectangularTouchUserEvent = null;
            string xValue = "", yValue = "", widthValue = "", heightValue = "";

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "x":
                        xValue = reader.ReadString();
                        break;
                    case "y":
                        yValue = reader.ReadString();
                        break;
                    case "width":
                        widthValue = reader.ReadString();
                        break;
                    case "height":
                        heightValue = reader.ReadString();
                        break;
                    case "rollover":
                        break;
                }
            }

            rectangularTouchUserEvent = new RectangularTouchUserEvent();
            rectangularTouchUserEvent.X = Convert.ToInt32(xValue);
            rectangularTouchUserEvent.Y = Convert.ToInt32(yValue);
            rectangularTouchUserEvent.Width = Convert.ToInt32(widthValue);
            rectangularTouchUserEvent.Height = Convert.ToInt32(heightValue);

            return rectangularTouchUserEvent;
        }
    }
}
