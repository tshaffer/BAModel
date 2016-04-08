using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
    public class AuxDisconnectUserEvent : AuxConnectDisconnectUserEvent
    {
        public AuxDisconnectUserEvent()
        {
            UserEventName = "auxDisconnectUserEvent";
            Value = "Aux Disconnect";
            DialogTitle = "Aux Disconnect";
            ImageResourceName = "iconAuxDisconnect";
            ImageResourceNameLarge = "iconAuxDisconnectLarge";
            ImageResourceSelectedName = "iconAuxDisconnectSelected";
            ValueI18NResource = "AuxDisconnectEvent";
        }

        public override string GetToolTip()
        {
            return "Aux Disconnect";
        }

        public new static AuxDisconnectUserEvent ReadXml(XmlReader reader)
        {
            AuxDisconnectUserEvent auxDisconnectUserEvent = new AuxDisconnectUserEvent();
            auxDisconnectUserEvent.AudioConnector = AudioConnectorType.Aux300Audio1;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "audioConnector":
                        string audioConnectorStr = reader.ReadString();
                        auxDisconnectUserEvent.AudioConnector = (AudioConnectorType)Enum.Parse(typeof(AudioConnectorType), audioConnectorStr);
                        break;
                }
            }

            return auxDisconnectUserEvent;
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            writer.WriteStartElement("parameters");

            writer.WriteElementString("audioConnector", AudioConnector.ToString());

            writer.WriteEndElement(); // parameters

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            AuxDisconnectUserEvent auxDisconnectUserEvent = new AuxDisconnectUserEvent();
            auxDisconnectUserEvent.AudioConnector = this.AudioConnector;
            return auxDisconnectUserEvent;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is AuxDisconnectUserEvent)) return false;

            return base.IsEqual(bsEvent);
        }
    }
}
