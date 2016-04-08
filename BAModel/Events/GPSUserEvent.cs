using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
    public class GPSUserEvent : UserEvent
    {
        // a GPSUserEvent can represent either entering the region or exiting the region
        public bool EnterRegion { get; set; }
        public GPSRegion GPSRegion { get; set; }

        public GPSUserEvent()
        {
            EnterRegion = true;
            GPSRegion = new BrightAuthor.GPSRegion();

            UserEventName = "gpsEvent";
            Value = "GPS";
            DialogTitle = "";
            ImageResourceName = "iconGPSEvent";
            ImageResourceNameLarge = "iconGPSEventLarge";
            ImageResourceSelectedName = "iconGPSEventSelected";
            ValueI18NResource = "GPS";
        }

        public override string GetToolTip()
        {
            return "GPS";
        }

        public new static GPSUserEvent ReadXml(XmlReader reader)
        {
            GPSUserEvent gpsUserEvent = new GPSUserEvent();
            gpsUserEvent.EnterRegion = true;
            gpsUserEvent.GPSRegion = new GPSRegion();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "enterRegion":
                        gpsUserEvent.EnterRegion = UserPreferences.ConvertStringToBool(reader.ReadString());
                        break;
                    case "gpsRegion":
                        gpsUserEvent.GPSRegion = GPSRegion.ReadXml(reader);
                        break;
                }
            }

            return gpsUserEvent;
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            writer.WriteStartElement("parameters");

            writer.WriteElementString("enterRegion", EnterRegion.ToString());
            GPSRegion.WriteToXml(writer, publish);

            writer.WriteEndElement(); // parameters

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            GPSUserEvent gpsUserEvent = new GPSUserEvent();
            gpsUserEvent.EnterRegion = this.EnterRegion;
            gpsUserEvent.GPSRegion = this.GPSRegion.Clone();
            return gpsUserEvent;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is GPSUserEvent)) return false;

            GPSUserEvent gpsUserEvent = (GPSUserEvent)bsEvent;

            if ((gpsUserEvent.EnterRegion != this.EnterRegion) ||
                (!gpsUserEvent.GPSRegion.IsEqual(this.GPSRegion)))
            {
                return false;
            }

            return true;
        }
    }
}
